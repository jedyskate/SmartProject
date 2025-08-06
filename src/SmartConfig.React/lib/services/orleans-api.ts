export interface HelloResponse {
    message: string;
}

export async function sayHello(name: string): Promise<HelloResponse> {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;

    const response = await fetch(`${baseUrl}/api/Orleans/HelloWorld`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({name})
    })

    if (!response.ok)
        throw new Error(response.statusText || "Failed to say hello");

    return await response.json();
}