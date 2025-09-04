function scrollToBottom(element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}

window.streamChat = async (url, body, dotNetRef) => {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(body)
        });

        if (!response.ok) {
            const errorText = await response.text();
            dotNetRef.invokeMethodAsync('StreamFailed', `HTTP error! status: ${response.status} - ${errorText}`);
            return;
        }

        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = '';

        while (true) {
            const { done, value } = await reader.read();
            if (done) {
                break;
            }

            buffer += decoder.decode(value, { stream: true });
            const lines = buffer.split('\n');
            buffer = lines.pop(); // Keep the last partial line in the buffer

            for (const line of lines) {
                if (line.trim()) {
                    // Use invokeMethodAsync without await to avoid blocking the JS loop
                    dotNetRef.invokeMethodAsync('ProcessStreamChunk', line);
                }
            }
        }
        
        if (buffer.trim()) {
             dotNetRef.invokeMethodAsync('ProcessStreamChunk', buffer);
        }

        dotNetRef.invokeMethodAsync('StreamCompleted');

    } catch (error) {
        dotNetRef.invokeMethodAsync('StreamFailed', error.toString());
    }
};