import axios from 'axios';

// IMPORTANT: Check your .NET terminal for the "https" port.
// It is usually 7110 or 7071. Update this URL if needed.
const BASE_URL = 'https://localhost:7110/api';

const api = axios.create({
    baseURL: BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor: Automatically adds the JWT Token to every request
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

// Interceptor: Handle 401 Errors (Token expired)
api.interceptors.response.use((response) => response, (error) => {
    if (error.response && error.response.status === 401) {
        // Optional: Auto-logout if token is invalid
        // localStorage.removeItem('token');
        // window.location.href = '/login';
    }
    return Promise.reject(error);
});

export default api;