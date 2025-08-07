export async function sayHello(name: string): Promise<string> {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;

    const result = await fetch(`${baseUrl}/api/Orleans/HelloWorld`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({name})
    })

    if (!result.ok)
        throw new Error(result.statusText || "Failed to say hello");
    
    const data = await result.json()
    return data.response;
}