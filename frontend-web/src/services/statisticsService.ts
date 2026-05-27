import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import type { ApiResponse, StatsFilterRequest, DashboardStats, RevenueStat } from '../types/statistics';

const client = createApiClient(`${API.ordering}/api`);

function pickData<T>(res: { data: unknown }): T {
  const body = res.data as Record<string, unknown>;
  return (body.data ?? body.Data) as T;
}

export const statisticsService = {
  getSummary: async (filter?: StatsFilterRequest) => {
    const res = await client.get<ApiResponse<DashboardStats>>('/statistics/summary', { params: filter });
    return { success: true, data: pickData<DashboardStats>(res) };
  },
  getRevenueChart: async (filter?: StatsFilterRequest) => {
    const res = await client.get<ApiResponse<RevenueStat[]>>('/statistics/revenue-chart', { params: filter });
    const raw = pickData<RevenueStat[]>(res);
    return {
      success: true,
      data: (raw ?? []).map((r) => ({
        date: String((r as RevenueStat & { Date?: string }).date ?? (r as { Date?: string }).Date ?? ''),
        revenue: Number((r as RevenueStat).revenue ?? (r as { Revenue?: number }).Revenue ?? 0),
      })),
    };
  },
  getTotalRevenue: async () => {
    const res = await client.get<ApiResponse<number>>('/statistics/total-revenue');
    return pickData<number>(res);
  },
  getTotalSold: async () => {
    const res = await client.get<ApiResponse<number>>('/statistics/total-sold');
    return pickData<number>(res);
  },
};
