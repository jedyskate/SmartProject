'use client';

import React, { useEffect, useState } from 'react';
import { fetchUserConfigs, UserConfig } from '@/lib/services/userconfigs/user-configs';

export default function UserConfigsPage() {
    const [configs, setConfigs] = useState<UserConfig[]>([]);
    const [loading, setLoading] = useState(false);

    // Search parameters
    const [name, setName] = useState('');
    const [status, setStatus] = useState('active');
    const [dateFrom, setDateFrom] = useState('');
    const [dateTo, setDateTo] = useState('');

    const handleSearch = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        try {
            const params = {
                name,
                status: status ? [status] : [],
                createdFrom: dateFrom ? new Date(dateFrom).toISOString() : undefined,
                createdTo: dateTo ? new Date(dateTo).toISOString() : undefined,
            };
            const results = await fetchUserConfigs(params);
            setConfigs(results);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        handleSearch(new Event('load') as any); // initial fetch
    }, []);

    return (
        <div className="p-6 space-y-6">
            <h1 className="text-2xl font-bold   ">User Configs</h1>

            {/* Search Form */}
            <form onSubmit={handleSearch} className="space-y-4 bg-gray-50 p-4 rounded border">
                <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
                    <input
                        type="text"
                        placeholder="Name"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        className="p-2 border rounded"
                    />
                    <select
                        value={status}
                        onChange={(e) => setStatus(e.target.value)}
                        className="p-2 border rounded"
                    >
                        <option value="">All Statuses</option>
                        <option value="active">Active</option>
                        <option value="inactive">Inactive</option>
                    </select>
                    <input
                        type="date"
                        value={dateFrom}
                        onChange={(e) => setDateFrom(e.target.value)}
                        className="p-2 border rounded"
                    />
                    <input
                        type="date"
                        value={dateTo}
                        onChange={(e) => setDateTo(e.target.value)}
                        className="p-2 border rounded"
                    />
                </div>
                <div className="flex justify-between items-center">
                    <button
                        type="submit"
                        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                    >
                        Search
                    </button>
                    <a
                        href="/user-configs/create"
                        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                    >
                        + New Config
                    </a>
                </div>
            </form>

            {/* Results */}
            {loading ? (
                <p>Loading...</p>
            ) : (
                <div className="overflow-auto rounded-xl shadow border">
                    <table className="min-w-full bg-white text-sm text-left">
                        <thead className="bg-gray-100 text-xs uppercase text-gray-600">
                        <tr>
                            <th className="px-4 py-2">Name</th>
                            <th className="px-4 py-2">Identifier</th>
                            <th className="px-4 py-2">Status</th>
                            <th className="px-4 py-2">Created</th>
                            <th className="px-4 py-2">Language</th>
                            <th className="px-4 py-2">Notifications</th>
                            <th className="px-4 py-2">Settings</th>
                            <th className="px-4 py-2">Actions</th>
                        </tr>
                        </thead>
                        <tbody>
                        {configs.map((config) => (
                            <tr key={config.identifier} className="border-t hover:bg-gray-50">
                                <td className="px-4 py-2">{config.name}</td>
                                <td className="px-4 py-2">{config.identifier}</td>
                                <td className="px-4 py-2">{config.status}</td>
                                <td className="px-4 py-2">
                                    {new Date(config.createdUtc).toLocaleDateString()}
                                </td>
                                <td className="px-4 py-2">
                                    {config.userPreferences?.language ?? '—'}
                                </td>
                                <td className="px-4 py-2">
                                        <span>
                                            Email: {config.userPreferences?.notificationType.email ? '✔️' : '❌'}
                                            &nbsp;&nbsp;&nbsp;
                                        </span>
                                    <span>
                                            SMS: {config.userPreferences?.notificationType.sms ? '✔️' : '❌'}
                                        &nbsp;&nbsp;&nbsp;
                                        </span>
                                    <span>
                                            NL: {config.userPreferences?.userNotifications.newsLetter ? '✔️' : '❌'}
                                        &nbsp;&nbsp;&nbsp;
                                        </span>
                                    <span>
                                            Billing: {config.userPreferences?.userNotifications.billings ? '✔️' : '❌'}
                                        </span>
                                </td>
                                <td className="px-4 py-2">
                                    {config.userSettings?.length ? (
                                        <ul className="list-disc ml-4">
                                            {config.userSettings.map((s, i) => (
                                                <li key={i}>{s.key}: {s.value}</li>
                                            ))}
                                        </ul>
                                    ) : '—'}
                                </td>
                                <td className="px-4 py-2">
                                    <a
                                        href={`/user-configs/upsert/${encodeURIComponent(config.identifier)}`}
                                        className="text-blue-600 hover:underline"
                                    >
                                        Edit
                                    </a>
                                </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}
