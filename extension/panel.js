let transactions = [];
let logs = [];
let inventory = {};

function updateLogDisplay() {
    const logContainer = document.getElementById('logContainer');
    if (logContainer) {
        logContainer.innerHTML = logs.map(log =>
            `<div class="log-entry ${log.type}">
                <span class="timestamp">[${log.timestamp}]</span> ${log.message}
            </div>`
        ).join('');
        logContainer.scrollTop = logContainer.scrollHeight;
    }
}

// Update display function to format dates nicely
function updateDisplay() {
    const transactionCount = document.getElementById('transactionCount');
    if (transactionCount) {
        transactionCount.textContent = `Transactions: ${transactions.length}`;
    }

    const transactionsElement = document.getElementById('transactions');
    if (transactionsElement) {
        // Format transactions for display with formatted dates
        const formattedTransactions = transactions.map(t => ({
            ...t,
            createdAt: new Date(t.createdAt).toLocaleString()
        }));
        transactionsElement.textContent = JSON.stringify(formattedTransactions, null, 2);
    }

    const inventoryElement = document.getElementById('inventory');
    if (inventoryElement) {
        inventoryElement.textContent = JSON.stringify(inventory, null, 2);
    }
}
function addLog(message, type = 'info') {
    const timestamp = new Date().toISOString().replace('T', ' ').substr(0, 19);
    logs.push({message, type, timestamp});
    if (logs.length > 1000) {
        logs.shift(); // Keep last 1000 logs
    }
    updateLogDisplay();
}

// Extract inventory from response
function extractInventory(responseText) {
    try {
        addLog(`Processing inventory response data`, 'info');
        addLog(`Raw inventory response length: ${responseText.length} characters`, 'info');

        let jsonObjects = [];
        let currentObject = '';
        let braceCount = 0;

        // Iterate through each character to properly split JSON objects
        for (let char of responseText) {
            currentObject += char;
            if (char === '{') braceCount++;
            if (char === '}') braceCount--;

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

        addLog(`Successfully split inventory response into ${jsonObjects.length} valid JSON objects`, 'info');

        // Find the object that contains the inventory items
        for (const obj of jsonObjects) {
            // The inventory items are in json[2][0][0]
            if (Array.isArray(obj.json?.[2]?.[0]?.[0])) {
                const items = obj.json[2][0][0];
                // Verify it's an inventory array by checking first item structure
                if (items[0] && 'itemId' in items[0] && 'count' in items[0]) {
                    addLog(`Found inventory data with ${items.length} items`, 'success');
                    return items;
                }
            }
        }

        addLog('No inventory data found in any of the JSON objects', 'warning');
    } catch (error) {
        addLog(`Error parsing inventory response: ${error.message}`, 'error');
        console.error('Full error:', error);
    }
    return null;
}

async function exportData() {
    if (transactions.length === 0 && Object.keys(inventory).length === 0) {
        addLog('No data to export', 'error');
        return;
    }

    try {
        const zip = new JSZip();
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-');

        // Add metadata file
        const metadata = {
            exportDate: new Date().toISOString(),
            transactionCount: transactions.length,
            inventoryItemCount: Array.isArray(inventory) ? inventory.length : Object.keys(inventory).length
        };
        zip.file('metadata.json', JSON.stringify(metadata, null, 2));

        // Add transactions if we have any
        if (transactions.length > 0) {
            zip.file('transactions.json', JSON.stringify(transactions, null, 2));
            addLog(`Added ${transactions.length} transactions to export`, 'info');
        }

        // Add inventory if we have any
        if (Object.keys(inventory).length > 0) {
            zip.file('inventory.json', JSON.stringify(inventory, null, 2));
            addLog(`Added inventory data to export`, 'info');
        }

        // Generate the ZIP file
        const blob = await zip.generateAsync({
            type: 'blob',
            compression: 'DEFLATE',
            compressionOptions: {
                level: 9
            }
        });

        // Trigger download
        const url = URL.createObjectURL(blob);
        chrome.downloads.download({
            url: url,
            filename: `mistwood-data-${timestamp}.zip`
        }, () => {
            URL.revokeObjectURL(url);
            addLog('Export completed successfully', 'success');
        });
    } catch (error) {
        addLog(`Export failed: ${error.message}`, 'error');
        console.error('Export error:', error);
    }
}

function clearAll() {
    transactions = [];
    inventory = {};
    logs = [];
    addLog('Cleared all data', 'info');
    updateDisplay();
    updateLogDisplay();
}

// Update toggleSection to use proper Unicode characters
function toggleSection(targetId) {
    const section = document.getElementById(targetId);
    const button = document.querySelector(`[data-target="${targetId}"]`);
    if (section && button) {
        section.classList.toggle('collapsed');
        button.textContent = section.classList.contains('collapsed') ? '►' : '▼';
    }
}

// Add this function to parse multiple JSON objects
function parseMultipleJsonObjects(responseText) {
    try {
        let jsonObjects = [];
        let currentObject = '';
        let braceCount = 0;

        // Iterate through each character to properly split JSON objects
        for (let char of responseText) {
            currentObject += char;
            if (char === '{') braceCount++;
            if (char === '}') braceCount--;

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

        return jsonObjects;
    } catch (error) {
        addLog(`Error parsing response: ${error.message}`, 'error');
        console.error('Full error:', error);
        return [];
    }
}

// Update the processTransactions function
function processTransactions(jsonObjects) {
    for (const obj of jsonObjects) {
        // The transactions are in json[2][0][0].items
        if (Array.isArray(obj.json?.[2]?.[0]?.[0]?.items)) {
            const transactionData = obj.json[2][0][0].items;
            addLog(`Found ${transactionData.length} transactions`, 'success');
            return transactionData;
        }
    }
    addLog('No transaction data found in response', 'warning');
    return null;
}

document.addEventListener('DOMContentLoaded', () => {
    addLog('Panel initialized', 'info');

    // Set up buttons
    document.getElementById('exportBtn').addEventListener('click', exportData);
    document.getElementById('clearBtn').addEventListener('click', clearAll);

    // Set up collapsible sections
    document.querySelectorAll('.collapse-btn').forEach(btn => {
        btn.addEventListener('click', () => toggleSection(btn.dataset.target));
    });

    // Listen for custom transaction events
// Update the transaction event listener
    window.addEventListener('newTransactionData', function(event) {
        if (event.detail && event.detail.data) {
            addLog(`Received new transaction data`, 'info');

            try {
                const jsonObjects = parseMultipleJsonObjects(event.detail.data);
                const newTransactions = processTransactions(jsonObjects);

                if (newTransactions && Array.isArray(newTransactions)) {
                    // Ensure we don't add duplicates by checking transaction IDs
                    const existingIds = new Set(transactions.map(t => t.id));
                    const uniqueNewTransactions = newTransactions.filter(t => !existingIds.has(t.id));

                    if (uniqueNewTransactions.length > 0) {
                        // The transactions already have timestamps (createdAt)
                        transactions = [...transactions, ...uniqueNewTransactions];
                        addLog(`Added ${uniqueNewTransactions.length} new unique transactions`, 'success');
                        updateDisplay();
                    } else {
                        addLog('No new unique transactions found', 'info');
                    }
                }
            } catch (error) {
                addLog(`Failed to process transaction data: ${error.message}`, 'error');
                console.error('Transaction processing error:', error);
            }
        }
    });

    // Listen for custom inventory events
    window.addEventListener('newInventoryData', function (event) {
        console.log('Received inventory data event:', event);
        if (event.detail && event.detail.data) {
            addLog(`Received new inventory response data`, 'info');
            const newInventory = extractInventory(event.detail.data);
            if (newInventory && typeof newInventory === 'object') {
                addLog(`Processing new inventory data`, 'info');
                inventory = newInventory;
                addLog(`Updated inventory data`, 'success');
                updateDisplay();
            } else {
                addLog('No valid inventory data found in response', 'error');
            }
        }
    });

    // Initial display update
    updateDisplay();
});

// Handle messages from background script
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.type === 'NEW_TRANSACTION') {
        transactions.push(message.transaction);
        addLog('New transaction received from background script', 'info');
        updateDisplay();
    } else if (message.type === 'NEW_INVENTORY') {
        inventory = message.inventory;
        addLog('New inventory received from background script', 'info');
        updateDisplay();
    }
});