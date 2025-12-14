'use client';

import { useEffect } from 'react';

export default function OpenTelemetryInit() {
    useEffect(() => {
        // Dynamically import and initialize OpenTelemetry only in the browser
        import('./init').then(({ initOpenTelemetry }) => {
            initOpenTelemetry();
        });
    }, []);

    return null;
}
