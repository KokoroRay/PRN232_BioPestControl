import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrapPaged } from '../types/api';
import type { Chemical, CreateChemicalRequest } from '../types/agri';

const client = createApiClient(`${API.agriExpert}/api`);

export const chemicalService = {
  getAll: async (params?: { keyword?: string; page?: number; pageSize?: number }) => {
    const { data } = await client.get('/admin/chemicals', { params: { page: 1, pageSize: 100, ...params } });
    const paged = unwrapPaged<Chemical>(data);
    return mapList<Chemical>(paged.items as unknown[]);
  },
  getAllStaff: async (params?: { keyword?: string; page?: number; pageSize?: number }) => {
    const { data } = await client.get('/staff/chemicals', { params: { page: 1, pageSize: 100, ...params } });
    const paged = unwrapPaged<Chemical>(data);
    return mapList<Chemical>(paged.items as unknown[]);
  },
  create: (body: CreateChemicalRequest) => client.post('/admin/chemicals', body),
  update: (id: number, body: CreateChemicalRequest) => client.put(`/admin/chemicals/${id}`, body),
  delete: (id: number) => client.delete(`/admin/chemicals/${id}`),
};
