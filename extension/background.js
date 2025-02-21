let transactions = [];
let inventory = [];
let profile = null;  // Add profile state
let debuggeeId = null;


// Add profile extraction function
function extractProfile(response) {
    try {
        if (typeof response === 'string') {
            // Split the response by '}' and process each part
            const parts = response.split('}{').map((part, index) => {
                if (index > 0) part = '{' + part;
                if (index < response.split('}{').length - 1) part = part + '}';
                return JSON.parse(part);
            });

            // Recursively search for an object with the unique profile properties
            function findProfileInObject(obj) {
                if (!obj || typeof obj !== 'object') return null;

                // Check if current object is the profile
                if (obj.totalXp !== undefined &&
                    obj.remainingBattles !== undefined &&
                    obj.level !== undefined &&
                    obj.silver !== undefined) {
                    return obj;
                }

                // Search in arrays
                if (Array.isArray(obj)) {
                    for (const item of obj) {
                        const result = findProfileInObject(item);
                        if (result) return result;
                    }
                }

                // Search in object properties
                for (const key in obj) {
                    if (obj[key] && typeof obj[key] === 'object') {
                        const result = findProfileInObject(obj[key]);
                        if (result) return result;
                    }
                }

                return null;
            }

            // Search through each part
            for (const part of parts) {
                const profile = findProfileInObject(part);
                if (profile) {
                    return profile;
                }
            }
        }
    } catch (error) {
        console.error('Error parsing profile response:', error);
    }
    return null;
}

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

// Function to extract inventory from response
function extractInventory(response) {
    try {
        if (typeof response === 'string') {
            // Split the response by '}' and process each part
            const parts = response.split('}{').map((part, index) => {
                if (index > 0) part = '{' + part;
                if (index < response.split('}{').length - 1) part = part + '}';
                return JSON.parse(part);
            });

            // Find the part that contains the inventory items
            for (const part of parts) {
                // The inventory items are in json[2][0][0] in the last object
                if (Array.isArray(part.json?.[2]?.[0]?.[0])) {
                    const items = part.json[2][0][0];
                    // Verify it's an inventory array by checking first item structure
                    if (items[0] && 'itemId' in items[0] && 'count' in items[0]) {
                        return items;
                    }
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
        if (response.url.includes('mistwood.pl/api/trpc/breeders.me')) {
            chrome.debugger.sendCommand(
                source,
                'Network.getResponseBody',
                { requestId },
                function(response) {
                    if (response && response.body) {
                        const newProfile = extractProfile(response.body);
                        if (newProfile) {
                            profile = newProfile;
                            // Send to any listening DevTools panels
                            chrome.runtime.sendMessage({
                                type: 'newProfileData',
                                profile: newProfile
                            });
                        }
                    }
                }
            );
        } else if (response.url.includes('mistwood.pl/api/trpc/exchange.transactionHistory')) {
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
        } else if (response.url.includes('mistwood.pl/api/trpc/inventory')) {
            chrome.debugger.sendCommand(
                source,
                'Network.getResponseBody',
                { requestId },
                function(response) {
                    if (response && response.body) {
                        const newInventory = extractInventory(response.body);
                        if (newInventory) {
                            inventory = newInventory;
                            chrome.runtime.sendMessage({
                                type: 'NEW_INVENTORY',
                                inventory: newInventory
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
    } else if (message.type === 'GET_INVENTORY') {
        sendResponse(inventory);
    } else if (message.type === 'GET_PROFILE') {
        sendResponse(profile);
    } else if (message.type === 'CLEAR_ALL') {
        transactions = [];
        inventory = [];
        profile = null;
        sendResponse({ success: true });
    }
    return true;
});