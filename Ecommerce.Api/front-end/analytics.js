/**
 * Analytics Instrumentation Module
 * Handles event batching, identity management, and reliable delivery.
 */

import { API_URL } from './config';

const FLUSH_INTERVAL = 5000; // Send events every 5 seconds
const BATCH_SIZE = 10; // Or when we have 10 events

let eventQueue = [];
let flushTimer = null;

// --- Helpers ---

const generateUUID = () => {
    return crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).substring(2) + Date.now().toString(36);
};

const getStorageItem = (key) => localStorage.getItem(key);
const setStorageItem = (key, val) => localStorage.setItem(key, val);

// 1. Identity Management
const getAnonymousId = () => {
    let id = getStorageItem('analytics_anonymous_id');
    if (!id) {
        id = generateUUID();
        setStorageItem('analytics_anonymous_id', id);
    }
    return id;
};

const getSessionId = () => {
    let id = getStorageItem('analytics_session_id');
    // Simple session logic: persist until cleared or browser close (sessionStorage could also work)
    if (!id) {
        id = generateUUID();
        setStorageItem('analytics_session_id', id);
    }
    return id;
};

const getUserId = () => {
    // We try to get the ID from the auth context storage
    const userStr = getStorageItem('user');
    return userStr ? JSON.parse(userStr).id : null;
};

// 2. Transport (Collection Layer)
const flush = () => {
    if (eventQueue.length === 0) return;

    const payload = [...eventQueue];
    eventQueue = []; // Clear queue immediately

    const blob = new Blob([JSON.stringify({ events: payload })], { type: 'application/json' });
    const endpoint = `${API_URL}/analytics/batch`;

    // Use sendBeacon for reliability during navigation/unload
    if (navigator.sendBeacon) {
        const success = navigator.sendBeacon(endpoint, blob);
        if (!success) console.warn('Analytics: sendBeacon failed, falling back to fetch');
    }

    // Fallback for older browsers or if sendBeacon fails/is full
    if (!navigator.sendBeacon) {
        fetch(endpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ events: payload }),
            keepalive: true // Critical for background sending
        }).catch(err => console.warn('Analytics: Flush failed', err));
    }
};

// 3. Event Emitter
export const track = (eventName, properties = {}) => {
    const event = {
        event_id: generateUUID(),
        event: eventName,
        timestamp: new Date().toISOString(),
        anonymous_id: getAnonymousId(),
        session_id: getSessionId(),
        user_id: getUserId(),
        context: {
            page_url: window.location.href,
            referrer: document.referrer,
            user_agent: navigator.userAgent,
            screen_width: window.screen.width,
        },
        properties: properties
    };

    eventQueue.push(event);
    console.log(`📊 [Analytics] Queued: ${eventName}`, event);

    if (eventQueue.length >= BATCH_SIZE) {
        flush();
    }
};

export const pageView = (path) => {
    track('page_view', { path });
};

// Auto-flush setup
flushTimer = setInterval(flush, FLUSH_INTERVAL);
document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'hidden') flush();
});