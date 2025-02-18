let transactions = [];
let debuggeeId = null;

// Function to extract transactions from response
function extractTransactions(response) {
    try {
        if (typeof response === 'string') {
            // Split the response by '}' and process each part
            const parts = response.split('}{').map((part, index) => {
                if (index > 0) part = '{' + part;
                if (index < response.split('}{').length - 1) part = part + '}';
                return JSON.parse(part);
            });

            // Find the part that contains the items
            for (const part of parts) {
                if (part.json?.[2]?.[0]?.[0]?.items) {
                    return part.json[2][0][0].items;
                }
            }
        }
    } catch (error) {
        console.error('Error parsing response:', error);
    }
    return null;
}

// Attach debugger when DevTools is opened
chrome.runtime.onConnect.addListener(function(port) {
    if (port.name === 'devtools-page') {
        port.onDisconnect.addListener(function() {
            if (debuggeeId) {
                chrome.debugger.detach(debuggeeId);
                debuggeeId = null;
            }
        });
    }
});

// Handle Network events
chrome.debugger.onEvent.addListener((source, method, params) => {
    if (method === 'Network.responseReceived') {
        const { requestId, response } = params;
        if (response.url.includes('mistwood.pl/api/trpc/exchange.transactionHistory')) {
            chrome.debugger.sendCommand(
                source,
                'Network.getResponseBody',
                { requestId },
                function(response) {
                    if (response && response.body) {
                        const newTransactions = extractTransactions(response.body);
                        if (newTransactions) {
                            transactions.push(...newTransactions);
                            chrome.runtime.sendMessage({
                                type: 'NEW_TRANSACTIONS',
                                transactions: newTransactions
                            });
                        }
                    }
                }
            );
        }
    }
});

// Attach debugger when a tab is activated
chrome.tabs.onActivated.addListener(function(activeInfo) {
    if (!debuggeeId) {
        debuggeeId = { tabId: activeInfo.tabId };
        chrome.debugger.attach(debuggeeId, '1.2', function() {
            chrome.debugger.sendCommand(debuggeeId, 'Network.enable');
        });
    }
});

// Listen for messages from the DevTools panel
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.type === 'GET_ALL_TRANSACTIONS') {
        sendResponse(transactions);
    } else if (message.type === 'CLEAR_TRANSACTIONS') {
        transactions = [];
        sendResponse({ success: true });
    }
    return true;
});