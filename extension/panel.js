let transactions = [];
let logs = [];

function updateLogDisplay() {
    const logContainer = document.getElementById('logContainer');
    if (logContainer) {
        logContainer.innerHTML = logs
            .map(log => `<div class="log-entry ${log.type}">[${log.timestamp}] ${log.message}</div>`)
            .join('');
    }
}

function updateDisplay() {
    const countElement = document.getElementById('transactionCount');
    if (countElement) {
        countElement.textContent = `Transactions: ${transactions.length}`;
    }

    const transactionsElement = document.getElementById('transactions');
    if (transactionsElement) {
        transactionsElement.textContent = JSON.stringify(transactions, null, 2);
    }
}

function addLog(message, type = 'info') {
    console.log(`[${type}] ${message}`); // Add console logging for debugging
    const timestamp = new Date().toISOString();
    const logEntry = {
        timestamp,
        message,
        type
    };
    logs.unshift(logEntry);
    if (logs.length > 100) logs.pop();
    updateLogDisplay();
}

// Extract transactions from response
function extractTransactions(responseText) {
    try {
        addLog(`Processing response data`, 'info');

        // First, let's log the raw response for debugging
        addLog(`Raw response length: ${responseText.length} characters`, 'info');

        // Split the response into separate JSON objects
        let jsonObjects = [];
        let currentObject = '';
        let braceCount = 0;

        // Iterate through each character to properly split JSON objects
        for (let char of responseText) {
            currentObject += char;

            if (char === '{') braceCount++;
            if (char === '}') braceCount--;

            // When we reach the end of a complete JSON object
            if (braceCount === 0 && currentObject.trim()) {
                try {
                    const parsed = JSON.parse(currentObject);
                    jsonObjects.push(parsed);
                    currentObject = '';
                } catch (e) {
                    addLog(`Failed to parse object: ${e.message}`, 'error');
                }
            }
        }

        addLog(`Successfully split response into ${jsonObjects.length} valid JSON objects`, 'info');

        // Find the object that contains the items
        for (const obj of jsonObjects) {
            if (obj.json?.[2]?.[0]?.[0]?.items) {
                const items = obj.json[2][0][0].items;
                addLog(`Found ${items.length} transactions in response`, 'success');
                return items;
            }
        }

        addLog('No transactions array found in any of the JSON objects', 'warning');
    } catch (error) {
        addLog(`Error parsing response: ${error.message}`, 'error');
        console.error('Full error:', error);
    }
    return null;
}

function downloadTransactions() {
    if (transactions.length === 0) {
        addLog('No transactions to download', 'error');
        return;
    }

    const data = JSON.stringify(transactions, null, 2);
    const blob = new Blob([data], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');

    chrome.downloads.download({
        url: url,
        filename: `mistwood-transactions-${timestamp}.json`
    }, () => {
        addLog(`Downloaded ${transactions.length} transactions`, 'success');
    });
}

function clearAll() {
    transactions = [];
    addLog('Cleared all transactions', 'info');
    updateDisplay();
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    addLog('Panel initialized', 'info');

    // Set up buttons
    document.getElementById('downloadBtn').addEventListener('click', downloadTransactions);
    document.getElementById('clearBtn').addEventListener('click', clearAll);

    // Listen for custom transaction events
    window.addEventListener('newTransactionData', function(event) {
        console.log('Received transaction data event:', event);
        if (event.detail && event.detail.data) {
            addLog(`Received new response data`, 'info');
            const newTransactions = extractTransactions(event.detail.data);
            if (newTransactions && Array.isArray(newTransactions)) {
                addLog(`Processing ${newTransactions.length} new transactions`, 'info');
                transactions.push(...newTransactions);
                addLog(`Added ${newTransactions.length} new transactions (total: ${transactions.length})`, 'success');
                updateDisplay();
            } else {
                addLog('No valid transactions found in response', 'error');
            }
        }
    });

    // Initial display update
    updateDisplay();
});