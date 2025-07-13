'use client';

import { useState } from 'react';
import { sayHello } from '@/lib/services/smartproject/orleansapi';

export default function HelloPage() {
    const [name, setName] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const result = await sayHello(name);
            setMessage(result.response); // Already correct
        } catch (err) {
            setError(err.message || 'Something went wrong');
        }
    };

    return (
        <main style={{ padding: 20 }}>
            <h1>Say Hello to the Backend</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Your name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    style={{ padding: 8, marginRight: 10 }}
                    required
                />
                <button type="submit">Send</button>
            </form>

            {message && <p style={{ marginTop: 20 }}>{message}</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}
        </main>
    );
}
