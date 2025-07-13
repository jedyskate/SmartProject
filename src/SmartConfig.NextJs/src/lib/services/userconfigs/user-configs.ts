// lib/api/userConfigs.ts

export type SearchUserConfigsParams = {
    identifiers?: string[];
    name?: string;
    status?: string[];
    createdFrom?: string;
    createdTo?: string;
    page?: number;
    pageSize?: number;
    returnPreferences?: boolean;
    returnSettings?: boolean;
};

export enum UserConfigStatus {
    Inactive = 'Inactive',
    Active = 'Active',
    Deleted = 'Deleted'
}

export type UserConfig = {
    identifier: string;
    name: string;
    userPreferences?: {
        language: string;
        notificationType: {
            email: boolean;
            sms: boolean;
        };
        userNotifications: {
            newsLetter: boolean;
            billings: boolean;
        };
    };
    userSettings?: { key: string; value: string }[];
    status: UserConfigStatus;
    createdUtc: string;
    updatedUtc: string;
};

export type UpsertUserConfigPayload = {
    identifier: string;
    name: string;
    userPreferences?: {
        language: string;
        notificationType: {
            email: boolean;
            sms: boolean;
        };
        userNotifications: {
            newsLetter: boolean;
            billings: boolean;
        };
    };
    userSettings?: { key: string; value: string }[];
    status: UserConfigStatus;
    options: {
        upsertPreferences: boolean;
        upsertSettings: boolean;
        returnPreferences: boolean;
        returnSettings: boolean;
    };
};

export async function fetchUserConfigs(params: SearchUserConfigsParams): Promise<UserConfig[]> {
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    const today = new Date();
    const oneYearAgo = new Date(today);
    oneYearAgo.setFullYear(today.getFullYear() - 1);

    const body = {
        identifiers: params.identifiers ?? [],
        name: params.name ?? '',
        status: params.status ?? [],
        createdFrom: params.createdFrom ?? oneYearAgo.toISOString(),
        createdTo: params.createdTo ?? today.toISOString(),
        configPagination: {
            page: params.page ?? 1,
            pageSize: params.pageSize ?? 10,
        },
        options: {
            returnPreferences: params.returnPreferences ?? true,
            returnSettings: params.returnSettings ?? true,
        },
    };

    const response = await fetch(`${baseUrl}/api/UserConfig/SearchUserConfigs`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
        },
        body: JSON.stringify(body),
    });

    if (!response.ok) {
        throw new Error(`API error: ${response.status}`);
    }

    const data = await response.json();
    return data.response?.results || [];
}

export async function getUserConfigById(identifier: string): Promise<UserConfig> {
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    const res = await fetch(`${baseUrl}/api/UserConfig/GetUserConfig`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
        },
        body: JSON.stringify({
            identifier,
            options: {
                returnPreferences: true,
                returnSettings: true,
            },
        }),
    });

    if (!res.ok) throw new Error('Failed to fetch user config');

    const data = await res.json();
    return data.response as UserConfig;
}

export async function upsertUserConfig(data: UpsertUserConfigPayload): Promise<UserConfig> {
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    const res = await fetch(`${baseUrl}/api/UserConfig/UpsertUserConfig`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!res.ok) {
        const errorDetail = await res.text().catch(() => 'No additional error detail available');
        throw new Error(`Failed to upsert user config: ${res.status} ${res.statusText}. Detail: ${errorDetail}`);
    }

    return res.json() as Promise<UserConfig>;
}
