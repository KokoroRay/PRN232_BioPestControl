import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrapPaged } from '../types/api';
import type { Customer, UpdateCustomerRequest } from '../types/identity';

const client = createApiClient(`${API.identity}/api`);

async function fetchCustomers(path: string, params?: { keyword?: string; page?: number; pageSize?: number }) {
  const { data } = await client.get(path, { params: { page: 1, pageSize: 50, ...params } });
  const paged = unwrapPaged<Customer>(data);
  return mapList<Customer>(paged.items as unknown[]);
}

export const customerService = {
  getAll: (params?: { keyword?: string; page?: number; pageSize?: number }) =>
    fetchCustomers('/admin/customers', params),
  getAllStaff: (params?: { keyword?: string; page?: number; pageSize?: number }) =>
    fetchCustomers('/staff/customers', params),
  update: (id: string, body: UpdateCustomerRequest, asStaff = false) =>
    client.put(`${asStaff ? '/staff' : '/admin'}/customers/${id}`, body),
  updateStatus: (id: string, isActive: boolean, asStaff = false) =>
    client.patch(`${asStaff ? '/staff' : '/admin'}/customers/${id}/status`, { isActive }),
};
