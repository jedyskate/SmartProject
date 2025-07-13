'use client';

import { useRouter } from 'next/navigation';
import React, { useState } from 'react';
import {
    upsertUserConfig,
    UserConfig,
    UserConfigStatus,
    UpsertUserConfigPayload,
} from '@/lib/services/userconfigs/user-configs';

export default function CreateUserConfigPage() {
    const router = useRouter();

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

    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
    ) => {
        const { name, value, type } = e.target;
        const checked = (e.target as HTMLInputElement).checked;

        setForm((prev) => {
            const newForm: Partial<UserConfig> = JSON.parse(JSON.stringify(prev));
            const nameParts = name.split('.');
            let currentLevel: any = newForm;

            for (let i = 0; i < nameParts.length - 1; i++) {
                const part = nameParts[i];
                if (!currentLevel[part]) {
                    currentLevel[part] = {};
                }
                currentLevel = currentLevel[part];
            }

            const finalKey = nameParts[nameParts.length - 1];
            currentLevel[finalKey] = type === 'checkbox' ? checked : value;
            return newForm;
        });
    };

    const handleSettingChange = (index: number, field: 'key' | 'value', value: string) => {
        setForm((prev) => {
            const newSettings = [...(prev.userSettings || [])];
            if (newSettings[index]) {
                newSettings[index] = { ...newSettings[index], [field]: value };
            }
            return { ...prev, userSettings: newSettings };
        });
    };

    const addSetting = () => {
        setForm((prev) => ({
            ...prev,
            userSettings: [...(prev.userSettings || []), { key: '', value: '' }],
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);

        if (!form.identifier || !form.name || !form.status) {
            setError('Identifier, name, and status are required.');
            setSaving(false);
            return;
        }

        const payload: UpsertUserConfigPayload = {
            identifier: form.identifier,
            name: form.name,
            status: form.status,
            userPreferences: form.userPreferences,
            userSettings: form.userSettings,
            options: {
                upsertPreferences: true,
                upsertSettings: true,
                returnPreferences: true,
                returnSettings: true,
            },
        };

        try {
            await upsertUserConfig(payload);
            router.push('/user-configs/search');
        } catch (err) {
            console.error('Failed to create config:', err);
            setError(err instanceof Error ? err.message : 'Failed to create config.');
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="max-w-2xl mx-auto p-6 space-y-6 bg-white shadow-lg rounded-lg mt-10">
            <h1 className="text-3xl font-bold text-gray-800 text-center mb-6">Create User Configuration</h1>

            {error && <p className="text-red-600 text-sm text-center">{error}</p>}

            <form onSubmit={handleSubmit} className="space-y-6">
                <div>
                    <label htmlFor="identifier" className="block text-sm font-medium text-gray-700 mb-1">
                        Identifier
                    </label>
                    <input
                        type="text"
                        id="identifier"
                        name="identifier"
                        placeholder="Unique Identifier"
                        value={form.identifier}
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md"
                        required
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
                        placeholder="Config Name"
                        value={form.name}
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md"
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
                        className="w-full p-3 border border-gray-300 rounded-md bg-white"
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
                        value={form.userPreferences?.language || ''}
                        onChange={handleChange}
                        className="w-full p-3 border border-gray-300 rounded-md"
                    />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50">
                        <input
                            type="checkbox"
                            name="userPreferences.notificationType.email"
                            checked={form.userPreferences?.notificationType?.email ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        Email Notifications
                    </label>

                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50">
                        <input
                            type="checkbox"
                            name="userPreferences.notificationType.sms"
                            checked={form.userPreferences?.notificationType?.sms ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        SMS Notifications
                    </label>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50">
                        <input
                            type="checkbox"
                            name="userPreferences.userNotifications.newsLetter"
                            checked={form.userPreferences?.userNotifications?.newsLetter ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        Newsletter
                    </label>

                    <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-md bg-gray-50">
                        <input
                            type="checkbox"
                            name="userPreferences.userNotifications.billings"
                            checked={form.userPreferences?.userNotifications?.billings ?? false}
                            onChange={handleChange}
                            className="form-checkbox h-5 w-5 text-blue-600 rounded"
                        />
                        Billings
                    </label>
                </div>

                <hr className="border-gray-200" />
                <h2 className="text-xl font-semibold text-gray-700">User Settings</h2>

                {form.userSettings?.map((setting, index) => (
                    <div key={index} className="grid grid-cols-2 gap-4">
                        <input
                            type="text"
                            placeholder="Key"
                            value={setting.key}
                            onChange={(e) => handleSettingChange(index, 'key', e.target.value)}
                            className="p-3 border border-gray-300 rounded-md"
                        />
                        <input
                            type="text"
                            placeholder="Value"
                            value={setting.value}
                            onChange={(e) => handleSettingChange(index, 'value', e.target.value)}
                            className="p-3 border border-gray-300 rounded-md"
                        />
                    </div>
                ))}

                <button
                    type="button"
                    onClick={addSetting}
                    className="text-sm text-blue-600 hover:underline"
                >
                    + Add setting
                </button>

                <button
                    type="submit"
                    disabled={saving}
                    className="w-full bg-blue-600 text-white py-3 rounded-md hover:bg-blue-700 transition"
                >
                    {saving ? 'Saving...' : 'Create Configuration'}
                </button>
            </form>
        </div>
    );
}
