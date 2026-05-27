import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys, mapList } from '../lib/normalize';
import { unwrap, unwrapPaged } from '../types/api';
import type { CreateStaffRequest, Staff, UpdateStaffRequest } from '../types/identity';

const client = createApiClient(`${API.identity}/api`);

export const staffService = {
  getAll: async (params?: { keyword?: string; page?: number; pageSize?: number }) => {
    const { data } = await client.get('/admin/staffs', { params: { page: 1, pageSize: 50, ...params } });
    const paged = unwrapPaged<Staff>(data);
    return mapList<Staff>(paged.items as unknown[]);
  },
  getById: async (id: string): Promise<Staff> => {
    const { data } = await client.get(`/admin/staffs/${id}`);
    const inner = unwrap<Record<string, unknown>>(data);
    const staff = mapKeys<Staff>(inner);
    const perms = inner.permissions ?? inner.Permissions;
    if (Array.isArray(perms)) {
      staff.permissions = mapList(perms);
    }
    return staff;
  },
  create: (body: CreateStaffRequest) => client.post('/admin/staffs', body),
  update: (id: string, body: UpdateStaffRequest) => client.put(`/admin/staffs/${id}`, body),
  delete: (id: string) => client.delete(`/admin/staffs/${id}`),
};
