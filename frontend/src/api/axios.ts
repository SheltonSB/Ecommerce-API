import axios from 'axios';
import toast from 'react-hot-toast';

const baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5154/api';

// In a production environment, if the API URL is not set, log a critical warning.
if (import.meta.env.PROD && !import.meta.env.VITE_API_BASE_URL) {
  console.warn('%cCRITICAL WARNING: VITE_API_BASE_URL is not set. The application will not connect to the backend.', 'color: red; font-weight: bold; font-size: 16px;');
}

const api = axios.create({ baseURL });

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
api.interceptors.response.use(
  (response) => response,
  (error) => {
    const message =
      error?.response?.data?.message ||
      error?.response?.data?.error ||
      (Array.isArray(error?.response?.data) ? error.response.data.join(', ') : null) ||
      error?.message ||
      'Something went wrong';
    if (message) toast.error(message);
    return Promise.reject(error);
  }
);

export default api;
