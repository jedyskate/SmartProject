// src/pages/Orleans/HelloWorld.tsx

import React, { useState } from 'react';
import { sayHello} from '../../../lib/services/orleans-api';

export default function HelloWorld() {
    const [name, setName] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError('');
        setMessage('');

        try {
            const message = await sayHello(name);
            setMessage(message);
        } catch (err: unknown) {
            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError('Something went wrong');
            }
        }
    };

    return (
        <div>
            <h1>Say hello to the Backend</h1>

            <form onSubmit={handleSubmit} className="form-container">
                <label>Name:</label>
                <input type="text" value={name} onChange={(e) => setName(e.target.value)}/>
                <button type="submit">Say Hello</button>
            </form>

            {message && <h2>{message}</h2>}
            {error && <p style={{color: 'red'}}>{error}</p>}
        </div>
    );
}
