// Create the DevTools panel
chrome.devtools.panels.create(
    "Mistwood Exchange",
    null,
    "panel.html",
    function(panel) {
        console.log("[Mistwood Extension] DevTools panel created");

        // Get reference to the panel window when it's shown
        panel.onShown.addListener(function(windowObject) {
            // When the panel is shown, attach the message handler
            windowObject.addEventListener('message', function(event) {
                console.log("[Mistwood Extension] Panel received message:", event.data);
            });

            // Store the panel window reference
            panelWindow = windowObject;
            console.log("[Mistwood Extension] Panel window reference obtained");
        });

        panel.onHidden.addListener(function() {
            panelWindow = null;
        });
    }
);

// Listen to network requests in the inspected window
chrome.devtools.network.onRequestFinished.addListener(
    async (request) => {
        try {
            if (request.request.url.includes('mistwood.pl/api/trpc/exchange.transactionHistory')) {
                console.log('[Mistwood Extension] Captured exchange history request:', request.request.url);

                // Get the response body
                request.getContent((content, encoding) => {
                    if (content) {
                        console.log('[Mistwood Extension] Got response content, sending to panel');
                        // Create a custom event to send to the panel
                        const event = new CustomEvent('newTransactionData', {
                            detail: {
                                type: 'NEW_RESPONSE',
                                data: content,
                                timestamp: new Date().toISOString(),
                                url: request.request.url
                            }
                        });

                        // Send to panel window if available
                        if (panelWindow) {
                            panelWindow.dispatchEvent(event);
                        }
                    }
                });
            }
        } catch (error) {
            console.error('[Mistwood Extension] Error processing request:', error);
        }
    }
);