export interface HelloResponse {
    message: string;
}

export async function sayHello(name: string): Promise<HelloResponse> {
    const baseUrl = process.env.API_BASE_URL;
    const baseUrlCode = process.env.API_BASE_URL;

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