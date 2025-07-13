'use client';

import { useEffect } from 'react';
import { initOpenTelemetry } from './init';

export default function OpenTelemetryInit() {
    useEffect(() => {
        initOpenTelemetry();
    }, []);

    return null;
}
