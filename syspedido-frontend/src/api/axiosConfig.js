import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5128/api', // Puerto configurado en launchSettings.json - http://localhost:5128
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
