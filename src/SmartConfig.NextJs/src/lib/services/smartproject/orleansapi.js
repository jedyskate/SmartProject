export async function sayHello(name) {
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    const res = await fetch(`${baseUrl}/api/Orleans/HelloWorld`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name }),
    });

    if (!res.ok) {
        throw new Error('Failed to say hello');
    }

    const data = await res.json();
    return data; // e.g. { message: "Hello, Javier!" }
}
