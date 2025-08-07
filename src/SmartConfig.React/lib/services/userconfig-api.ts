import { API_URL } from '../../src/config';

export async function search(name: string): Promise<string> {
    const result = await fetch(`${API_URL}/api/Orleans/HelloWorld`, {
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