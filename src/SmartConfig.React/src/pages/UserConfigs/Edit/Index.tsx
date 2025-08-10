// src/pages/UserConfigs/Edit/Index.tsx
import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    getUserConfigById,
    upsertUserConfig,
    type UserConfig,
    type UserConfigStatus,
    type UpsertUserConfigPayload,
} from '../../../../lib/services/userconfig-api';
import './Index.css';

const availableStatuses: UserConfigStatus[] = ['Active', 'Inactive', 'Deleted'];

const EditUserConfig: React.FC = () => {
    const { identifier } = useParams<{ identifier: string }>();
    const navigate = useNavigate();
    const [config, setConfig] = useState<UserConfig | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [name, setName] = useState<string>('');
    const [status, setStatus] = useState<UserConfigStatus>('Inactive');
    const [userPreferences, setUserPreferences] = useState<UserConfig['userPreferences']>({
        language: '',
        notificationType: { email: false, sms: false },
        userNotifications: { newsLetter: false, billings: false },
    });
    const [userSettings, setUserSettings] = useState<{ key: string; value: string }[]>([]);

    const fetchConfig = useCallback(async () => {
        if (!identifier) return;
        try {
            setLoading(true);
            const data = await getUserConfigById(identifier);
            setConfig(data);
            setName(data.name);
            setStatus(data.status);
            if (data.userPreferences) {
                setUserPreferences(data.userPreferences);
            }
            if (data.userSettings) {
                setUserSettings(data.userSettings);
            }
            setError(null);
        } catch (err) {
            setError('Failed to fetch user config');
            console.error(err);
        } finally {
            setLoading(false);
        }
    }, [identifier]);

    useEffect(() => {
        fetchConfig();
    }, [fetchConfig]);

    const handlePreferenceChange = (field: string, value: any) => {
        const keys = field.split('.');
        setUserPreferences(prev => {
            const newPrefs = prev ? { ...prev } : {
                language: '',
                notificationType: { email: false, sms: false },
                userNotifications: { newsLetter: false, billings: false }
            };

            let current: any = newPrefs;
            for (let i = 0; i < keys.length - 1; i++) {
                const key = keys[i];
                if (current[key] === undefined || current[key] === null) {
                    current[key] = {};
                }
                current[key] = { ...current[key] };
                current = current[key];
            }

            current[keys[keys.length - 1]] = value;

            return newPrefs;
        });
    };

    const handleSettingChange = (index: number, field: 'key' | 'value', value: string) => {
        const newSettings = [...userSettings];
        newSettings[index][field] = value;
        setUserSettings(newSettings);
    };

    const addSetting = () => {
        setUserSettings([...userSettings, { key: '', value: '' }]);
    };

    const removeSetting = (index: number) => {
        const newSettings = userSettings.filter((_, i) => i !== index);
        setUserSettings(newSettings);
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        if (!config) return;

        const payload: UpsertUserConfigPayload = {
            ...config,
            name,
            status,
            userPreferences,
            userSettings,
            options: {
                upsertPreferences: true,
                upsertSettings: true,
                returnPreferences: true,
                returnSettings: true,
            },
        };

        try {
            await upsertUserConfig(payload);
            navigate('/userconfigs/search/index');
        } catch (err) {
            setError('Failed to save user config');
            console.error(err);
        }
    };

    if (loading) {
        return <div className="loading-indicator">Loading...</div>;
    }

    if (error) {
        return <div className="error-message">{error}</div>;
    }

    if (!config) {
        return <div className="no-results">User configuration not found.</div>;
    }

    return (
        <div className="edit-container">
            <header className="page-header">
                <h1>Edit User Configuration</h1>
            </header>
            <form onSubmit={handleSubmit} className="edit-form">
                <div className="form-group">
                    <label htmlFor="name">Name</label>
                    <input
                        id="name"
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="status">Status</label>
                    <select
                        id="status"
                        value={status}
                        onChange={(e) => setStatus(e.target.value as UserConfigStatus)}
                    >
                        {availableStatuses.map((s) => (
                            <option key={s} value={s}>
                                {s}
                            </option>
                        ))}
                    </select>
                </div>

                <fieldset className="form-group">
                    <legend>User Preferences</legend>
                    <div className="form-group">
                        <label htmlFor="language">Language</label>
                        <input
                            id="language"
                            type="text"
                            value={userPreferences?.language || ''}
                            onChange={(e) => handlePreferenceChange('language', e.target.value)}
                        />
                    </div>
                    <div className="form-group">
                        <label>Notification Type</label>
                        <div className="checkbox-group">
                            <label>
                                <input
                                    type="checkbox"
                                    checked={userPreferences?.notificationType?.email || false}
                                    onChange={(e) => handlePreferenceChange('notificationType.email', e.target.checked)}
                                />
                                Email
                            </label>
                            <label>
                                <input
                                    type="checkbox"
                                    checked={userPreferences?.notificationType?.sms || false}
                                    onChange={(e) => handlePreferenceChange('notificationType.sms', e.target.checked)}
                                />
                                SMS
                            </label>
                        </div>
                    </div>
                    <div className="form-group">
                        <label>User Notifications</label>
                        <div className="checkbox-group">
                            <label>
                                <input
                                    type="checkbox"
                                    checked={userPreferences?.userNotifications?.newsLetter || false}
                                    onChange={(e) => handlePreferenceChange('userNotifications.newsLetter', e.target.checked)}
                                />
                                Newsletter
                            </label>
                            <label>
                                <input
                                    type="checkbox"
                                    checked={userPreferences?.userNotifications?.billings || false}
                                    onChange={(e) => handlePreferenceChange('userNotifications.billings', e.target.checked)}
                                />
                                Billings
                            </label>
                        </div>
                    </div>
                </fieldset>

                <fieldset className="form-group">
                    <legend>User Settings</legend>
                    {userSettings.map((setting, index) => (
                        <div key={index} className="setting-item">
                            <input
                                type="text"
                                placeholder="Key"
                                value={setting.key}
                                onChange={(e) => handleSettingChange(index, 'key', e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Value"
                                value={setting.value}
                                onChange={(e) => handleSettingChange(index, 'value', e.target.value)}
                            />
                            <button type="button" onClick={() => removeSetting(index)}>Remove</button>
                        </div>
                    ))}
                    <button type="button" onClick={addSetting}>Add Setting</button>
                </fieldset>

                <div className="form-actions">
                    <button type="submit" className="save-button">Save</button>
                    <button type="button" onClick={() => navigate('/userconfigs/search/index')} className="cancel-button">
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
};

export default EditUserConfig;