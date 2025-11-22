import axios from 'axios';
import toast from 'react-hot-toast';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5154/api'
});

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
