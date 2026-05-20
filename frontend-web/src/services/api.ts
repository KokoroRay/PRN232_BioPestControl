import axios from 'axios';
import type { ApiResponse, DashboardStats, RevenueStat, StatsFilterRequest } from '../types/statistics';

const API_BASE_URL = 'http://localhost:5112/api'; // Ordering Service

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add interceptor to add token if needed
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export const statisticsService = {
  getSummary: async (filter?: StatsFilterRequest) => {
    const response = await api.get<ApiResponse<DashboardStats>>('/statistics/summary', { params: filter });
    return response.data;
  },
  getRevenueChart: async (filter?: StatsFilterRequest) => {
    const response = await api.get<ApiResponse<RevenueStat[]>>('/statistics/revenue-chart', { params: filter });
    return response.data;
  },
  getTotalRevenue: async () => {
    const response = await api.get<ApiResponse<number>>('/statistics/total-revenue');
    return response.data;
  },
  getTotalSold: async () => {
    const response = await api.get<ApiResponse<number>>('/statistics/total-sold');
    return response.data;
  },
};
