// src/pages/UserConfigs/Search/Index.tsx
import React, { useEffect, useState, useCallback } from 'react';
import { fetchUserConfigs, type UserConfig, type UserConfigStatus } from '../../../../lib/services/userconfig-api';
import './Index.css';

const availableStatuses: UserConfigStatus[] = ['Active', 'Inactive', 'Deleted'];

const UserPreferencesDisplay: React.FC<{ preferences: UserConfig['userPreferences'] }> = ({ preferences }) => {
    if (!preferences) {
        return <span className="no-preferences">N/A</span>;
    }

    const { language, notificationType, userNotifications } = preferences;

    const items: string[] = [];
    if (language) items.push(`Lang: ${language}`);
    if (notificationType?.email || notificationType?.sms) {
        const types = [];
        if (notificationType?.email) types.push("Email");
        if (notificationType?.sms) types.push("Sms");

        items.push(`Type: ${types.join(", ")}`);
    }
    if (userNotifications?.newsLetter || userNotifications?.billings) {
        const types = [];
        if (userNotifications?.newsLetter) types.push("Newsletter");
        if (userNotifications?.billings) types.push("Billings");

        items.push(`Notification: ${types.join(", ")}`);
    }
    if (items.length === 0) {
        return <span className="no-preferences">None</span>;
    }

    return (
        <div className="preferences-compact">
            {items.map((item, index) => (
                <span key={index} className="preference-item">{item}</span>
            ))}
        </div>
    );
};

const UserSettingsDisplay: React.FC<{ settings: UserConfig['userSettings'] }> = ({ settings }) => {
    if (!settings || settings.length === 0) {
        return <span className="no-settings">N/A</span>;
    }

    return (
        <div className="settings-compact">
            {settings.map((setting, index) => (
                <span key={index} className="setting-item">
                    <span className="setting-key">{setting.key}:</span>
                    <span className="setting-value">{String(setting.value)}</span>
                </span>
            ))}
        </div>
    );
};

const SearchUserConfigs: React.FC = () => {
    const [configs, setConfigs] = useState<UserConfig[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [name, setName] = useState<string>('');
    const [statuses, setStatuses] = useState<UserConfigStatus[]>([]);
    const [page, setPage] = useState<number>(1);
    const [pageSize] = useState<number>(10);

    const getConfigs = useCallback(async () => {
        try {
            setLoading(true);
            const data = await fetchUserConfigs({ name, status: statuses, page, pageSize });
            setConfigs(data);
            setError(null);
        } catch (err) {
            setError('Failed to fetch user configs');
            console.error(err);
        } finally {
            setLoading(false);
        }
    }, [name, statuses, page, pageSize]);

    useEffect(() => {
        getConfigs();
    }, [getConfigs]);

    const handleSearch = () => {
        setPage(1);
        getConfigs();
    };

    const handleStatusChange = (status: UserConfigStatus) => {
        setStatuses(prevStatuses =>
            prevStatuses.includes(status)
                ? prevStatuses.filter(s => s !== status)
                : [...prevStatuses, status]
        );
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
                            <th>Name</th>
                            <th>Status</th>
                            <th>User Preferences</th>
                            <th>User Settings</th>
                            <th>Created At</th>
                        </tr>
                    </thead>
                    <tbody>
                        {configs.map((config) => (
                            <tr key={config.identifier}>
                                <td>{config.name}</td>
                                <td><span className={`status status-${config.status?.toLowerCase()}`}>{config.status}</span></td>
                                <td><UserPreferencesDisplay preferences={config.userPreferences} /></td>
                                <td><UserSettingsDisplay settings={config.userSettings} /></td>
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
                <div className="status-filter">
                    <span className="filter-label">Status:</span>
                    {availableStatuses.map(status => (
                        <label key={status} className="status-checkbox">
                            <input
                                type="checkbox"
                                checked={statuses.includes(status)}
                                onChange={() => handleStatusChange(status)}
                            />
                            {status}
                        </label>
                    ))}
                </div>
                <button onClick={handleSearch}>Search</button>
            </div>
            <main className="results-container">
                {renderContent()}
            </main>
        </div>
    );
};

export default SearchUserConfigs;
