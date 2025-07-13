'use client';

import { useParams, useRouter } from 'next/navigation';
import React, { useEffect, useState } from 'react';
import {
    getUserConfigById,
    upsertUserConfig,
    UserConfig,
    UserConfigStatus,
    UpsertUserConfigPayload
} from '@/lib/services/userconfigs/user-configs'; // Adjust path if UpsertUserConfigPayload is elsewhere

export default function EditUserConfigPage() {
    const router = useRouter();
    const params = useParams();
    const identifier = params?.id as string | undefined;

    const [form, setForm] = useState<Partial<UserConfig>>({
        identifier: '',
        name: '',
        status: UserConfigStatus.Active,
        userPreferences: {
            language: '',
            notificationType: { email: false, sms: false },
            userNotifications: { newsLetter: false, billings: false },
        },
        userSettings: [{ key: '', value: '' }],
    });

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!identifier) {
            setError('No user configuration ID provided for editing.');
            setLoading(false);
            return;
        }

        const loadUserConfig = async () => {
            try {
                const data = await getUserConfigById(identifier);
                if (!data) {
                    setError('User configuration not found.');
                    return; // Don't setForm if no data
                }

                // Populate form with fetched data, providing defaults for missing optional fields
                setForm({
                    identifier: data.identifier,
                    name: data.name,
                    status: data.status,
                    userPreferences: {
                        language: data.userPreferences?.language ?? '',
                        notificationType: {
                            email: data.userPreferences?.notificationType?.email ?? false,
                            sms: data.userPreferences?.notificationType?.sms ?? false,
                        },
                        userNotifications: {
                            newsLetter: data.userPreferences?.userNotifications?.newsLetter ?? false,
                            billings: data.userPreferences?.userNotifications?.billings ?? false,
                        },
                    },
                    userSettings: data.userSettings?.length
                        ? data.userSettings
                        : [{ key: '', value: '' }], // Ensure at least one empty setting row
                });
            } catch (err) {
                console.error('Failed to load user configuration:', err);
                setError('Failed to load user configuration. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        loadUserConfig();
    }, [identifier]); // Dependency array: re-run effect if identifier changes

    /**
     * Handles changes for all input types (text, select, checkbox),
     * including nested properties using dot notation in the name attribute.
     */
    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
    ) => {
        const { name, value, type } = e.target;
        const checked = (e.target as HTMLInputElement).checked; // Only relevant for checkboxes

        setForm((prev) => {
            // Deep clone the previous state to ensure immutability with nested objects
            // For more complex state, consider a utility library like 'lodash.clonedeep'
            const newForm: Partial<UserConfig> = JSON.parse(JSON.stringify(prev));

            const nameParts = name.split('.');
            let currentLevel: any = newForm; // Use 'any' for traversal due to dynamic keys

            // Traverse the object path specified by `name` (e.g., 'userPreferences.language')
            for (let i = 0; i < nameParts.length - 1; i++) {
                const part = nameParts[i];
                if (!currentLevel[part]) {
                    // Initialize intermediate objects if they don't exist
                    currentLevel[part] = {};
                }
                currentLevel = currentLevel[part];
            }

            // Set the final value based on input type
            const finalKey = nameParts[nameParts.length - 1];
            if (type === 'checkbox') {
                currentLevel[finalKey] = checked;
            } else {
                currentLevel[finalKey] = value;
            }

            return newForm;
        });
    };

    /**
     * Handles changes specifically for the dynamic userSettings array.
     */
    const handleSettingChange = (index: number, field: 'key' | 'value', value: string) => {
        setForm((prev) => {
            const newSettings = [...(prev.userSettings || [])]; // Ensure it's an array
            if (newSettings[index]) {
                newSettings[index] = { ...newSettings[index], [field]: value };
            }
            return { ...prev, userSettings: newSettings };
        });
    };

    /**
     * Adds a new empty row to the userSettings array.
     */
    const addSetting = () => {
        setForm((prev) => ({
            ...prev,
            userSettings: [...(prev.userSettings || []), { key: '', value: '' }],
        }));
    };

    /**
     * Handles form submission to upsert (update/insert) the user configuration.
     */
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);

        // Ensure identifier is present
        if (!identifier) {
            setError('Cannot update: User configuration ID is missing.');
            setSaving(false);
            return;
        }

        // Runtime check for required fields from the form state,
        // especially for those that are not optional in UpsertUserConfigPayload.
        // In this case, 'name' and 'status' are explicitly required in the payload.
        if (!form.name) {
            setError('Configuration name is required.');
            setSaving(false);
            return;
        }

        if (!form.status) {
            setError('Configuration status is required.');
            setSaving(false);
            return;
        }

        try {
            // Construct the payload based on the UpsertUserConfigPayload type
            const payload: UpsertUserConfigPayload = {
                // Identifier, name, and status are directly from the form/params
                identifier: identifier,
                name: form.name,
                status: form.status, // This is now guaranteed by the check above

                // User preferences and settings are optional in the form,
                // but we want to ensure they are passed if they exist.
                userPreferences: form.userPreferences,
                userSettings: form.userSettings,

                options: {
                    upsertPreferences: true,
                    upsertSettings: true,
                    returnPreferences: true,
                    returnSettings: true,
                },
            };

            await upsertUserConfig(payload);
            router.push('/user-configs/search'); // Navigate on success
        } catch (err) {
            console.error('Failed to save config:', err);
            // Provide a more user-friendly error message if the error object contains one
            const errorMessage = (err instanceof Error) ? err.message : 'Failed to save config. Please try again.';
            setError(errorMessage);
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <p className="p-4 text-center">Loading user configuration...</p>;
    if (error) return <p className="p-4 text-red-500 text-center">{error}</p>;

    return (
        <div className="max-w-2xl mx-auto p-6 space-y-6 bg-white shadow-lg rounded-lg mt-10">
            <h1 className="text-3xl font-bold text-gray-800 text-center mb-6">Edit User Configuration</h1>

            <form onSubmit={handleSubmit} className="space-y-6">
                <div>
                    <label htmlFor="identifier" className="block text-sm font-medium text-gray-700 mb-1">
                        Identifier
                    </label>
                    <input
                        type="text"
                        id="identifier"
                        name="identifier"
                        placeholder="Identifier"
                        value={form.identifier}
                        className="w-full p-3 border border-gray-300 rounded-md bg-gray-100 focus:outline-none"
                        readOnly
                        aria-readonly="true"
                    />
                </div>

                <div>
                    <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                        Name
                    </label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        placeholder="Name"
                        value={form.name}
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                    />
                </div>

                <div>
                    <label htmlFor="status" className="block text-sm font-medium text-gray-700 mb-1">
                        Status
                    </label>
                    <select
                        id="status"
                        name="status"
                        value={form.status}
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 bg-white"
                    >
                        {Object.values(UserConfigStatus).map((status) => (
                            <option key={status} value={status}>
                                {status}
                            </option>
                        ))}
                    </select>
                </div>

                <hr className="border-gray-200" />

                <h2 className="text-xl font-semibold text-gray-700">User Preferences</h2>
                <div>
                    <label htmlFor="userPreferences.language" className="block text-sm font-medium text-gray-700 mb-1">
                        Language
                    </label>
                    <input
                        type="text"
                        id="userPreferences.language"
                        name="userPreferences.language"
                        value={form.userPreferences?.language || ''} // Ensure default for display if somehow undefined
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                    />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors">
                        <input
                            type="checkbox"
                            name="userPreferences.notificationType.email"
                            checked={form.userPreferences?.notificationType?.email ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        <span className="text-gray-700">Email Notifications</span>
                    </label>
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors">
                        <input
                            type="checkbox"
                            name="userPreferences.notificationType.sms"
                            checked={form.userPreferences?.notificationType?.sms ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        <span className="text-gray-700">SMS Notifications</span>
                    </label>
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors">
                        <input
                            type="checkbox"
                            name="userPreferences.userNotifications.newsLetter"
                            checked={form.userPreferences?.userNotifications?.newsLetter ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        <span className="text-gray-700">Newsletter</span>
                    </label>
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors">
                        <input
                            type="checkbox"
                            name="userPreferences.userNotifications.billings"
                            checked={form.userPreferences?.userNotifications?.billings ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        <span className="text-gray-700">Billing Updates</span>
                    </label>
                </div>

                <hr className="border-gray-200" />

                <div className="space-y-4">
                    <h2 className="text-xl font-semibold text-gray-700">Settings</h2>
                    {form.userSettings?.map((setting, index) => (
                        <div key={index} className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                            <input
                                type="text"
                                placeholder="Key"
                                value={setting.key}
                                onChange={(e) => handleSettingChange(index, 'key', e.target.value)}
                                className="p-3 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                            />
                            <input
                                type="text"
                                placeholder="Value"
                                value={setting.value}
                                onChange={(e) => handleSettingChange(index, 'value', e.target.value)}
                                className="p-3 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                            />
                        </div>
                    ))}
                    <button
                        type="button"
                        onClick={addSetting}
                        className="text-sm text-blue-600 hover:underline px-4 py-2 rounded-md transition-colors duration-200"
                    >
                        + Add Setting
                    </button>
                </div>

                <button
                    type="submit"
                    disabled={saving}
                    className="w-full bg-blue-600 text-white font-semibold py-3 px-6 rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                >
                    {saving ? 'Updating...' : 'Update Configuration'}
                </button>
            </form>
        </div>
    );
}