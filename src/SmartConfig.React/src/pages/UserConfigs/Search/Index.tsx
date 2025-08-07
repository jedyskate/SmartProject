// src/pages/UserConfigs/Search/Index.tsx
import React, { useEffect, useState, useCallback } from 'react';
import { fetchUserConfigs, type UserConfig } from '../../../../lib/services/userconfig-api';
import './Index.css';

const UserPreferencesDisplay: React.FC<{ preferences: UserConfig['userPreferences'] }> = ({ preferences }) => {
    if (!preferences) {
        return <span className="no-preferences">N/A</span>;
    }

    const { language, notificationType, userNotifications } = preferences;

    return (
        <ul className="preferences-list">
            <li><strong>Language:</strong> {language}</li>
            <li>
                <strong>Notifications:</strong>
                <ul>
                    <li>Email: {notificationType?.email ? 'Yes' : 'No'}</li>
                    <li>SMS: {notificationType?.sms ? 'Yes' : 'No'}</li>
                </ul>
            </li>
            <li>
                <strong>User Notifications:</strong>
                <ul>
                    <li>Newsletter: {userNotifications?.newsLetter ? 'Yes' : 'No'}</li>
                    <li>Billings: {userNotifications?.billings ? 'Yes' : 'No'}</li>
                </ul>
            </li>
        </ul>
    );
};

const SearchUserConfigs: React.FC = () => {
    const [configs, setConfigs] = useState<UserConfig[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [name, setName] = useState<string>('');
    const [page, setPage] = useState<number>(1);
    const [pageSize] = useState<number>(10);

    const getConfigs = useCallback(async () => {
        try {
            setLoading(true);
            const data = await fetchUserConfigs({ name, page, pageSize });
            setConfigs(data);
            setError(null);
        } catch (err) {
            setError('Failed to fetch user configs');
            console.error(err);
        } finally {
            setLoading(false);
        }
    }, [name, page, pageSize]);

    useEffect(() => {
        getConfigs();
    }, [getConfigs]);

    const handleSearch = () => {
        setPage(1);
        getConfigs();
    };
    
    const renderContent = () => {
        if (loading) {
            return <div className="loading-indicator">Loading...</div>;
        }

        if (error) {
            return <div className="error-message">{error}</div>;
        }

        if (configs.length === 0) {
            return <div className="no-results">No user configurations found.</div>;
        }

        return (
            <>
                <table className="configs-table">
                    <thead>
                        <tr>
                            <th>Identifier</th>
                            <th>Name</th>
                            <th>Status</th>
                            <th>User Preferences</th>
                            <th>Created At</th>
                        </tr>
                    </thead>
                    <tbody>
                        {configs.map((config) => (
                            <tr key={config.identifier}>
                                <td>{config.identifier}</td>
                                <td>{config.name}</td>
                                <td><span className={`status status-${config.status?.toLowerCase()}`}>{config.status}</span></td>
                                <td><UserPreferencesDisplay preferences={config.userPreferences} /></td>
                                <td>{new Date(config.createdUtc).toLocaleString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                <div className="pagination-container">
                    <button onClick={() => setPage(page - 1)} disabled={page <= 1}>
                        Previous
                    </button>
                    <span>Page {page}</span>
                    <button onClick={() => setPage(page + 1)} disabled={configs.length < pageSize}>
                        Next
                    </button>
                </div>
            </>
        );
    };

    return (
        <div className="search-container">
            <header className="page-header">
                <h1>User Configurations</h1>
            </header>
            <div className="filter-container">
                <input
                    type="text"
                    placeholder="Filter by name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                />
                <button onClick={handleSearch}>Search</button>
            </div>
            <main className="results-container">
                {renderContent()}
            </main>
        </div>
    );
};

export default SearchUserConfigs;
