// src/pages/UserConfigs/Search.tsx
import React, { useEffect, useState } from 'react';
import { fetchUserConfigs, type UserConfig } from '../../../../lib/services/userconfig-api';
import './Index.css';

const SearchUserConfigs: React.FC = () => {
    const [configs, setConfigs] = useState<UserConfig[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const getConfigs = async () => {
            try {
                setLoading(true);
                const data = await fetchUserConfigs({});
                setConfigs(data);
                setError(null);
            } catch (err) {
                setError('Failed to fetch user configs');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        getConfigs();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <div className="search-container">
            <h2>User Configurations</h2>
            <table className="configs-table">
                <thead>
                    <tr>
                        <th>Identifier</th>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Created At</th>
                        <th>Updated At</th>
                    </tr>
                </thead>
                <tbody>
                    {configs.map((config) => (
                        <tr key={config.identifier}>
                            <td>{config.identifier}</td>
                            <td>{config.name}</td>
                            <td>{config.status}</td>
                            <td>{new Date(config.createdUtc).toLocaleString()}</td>
                            <td>{new Date(config.updatedUtc).toLocaleString()}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default SearchUserConfigs;