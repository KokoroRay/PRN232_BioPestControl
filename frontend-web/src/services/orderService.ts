import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrap, unwrapPaged } from '../types/api';
import type { Order, OrderFilter } from '../types/ordering';

const client = createApiClient(`${API.ordering}`);

const staffPrefix = '/api/staff/orders';
const adminPrefix = '/api/admin/orders';

export const orderService = {
  getAdminOrders: async (filter?: OrderFilter) => {
    const { data } = await client.get(adminPrefix, { params: filter });
    const paged = unwrapPaged<Order>(data);
    return { ...paged, items: mapList<Order>(paged.items as unknown[]) };
  },
  getStaffOrders: async (filter?: OrderFilter) => {
    const { data } = await client.get(staffPrefix, { params: filter });
    const paged = unwrapPaged<Order>(data);
    return { ...paged, items: mapList<Order>(paged.items as unknown[]) };
  },
  getAdminOrder: async (id: string) => {
    const { data } = await client.get(`${adminPrefix}/${id}`);
    return unwrap<Order>(data);
  },
  getStaffOrder: async (id: string) => {
    const { data } = await client.get(`${staffPrefix}/${id}`);
    return unwrap<Order>(data);
  },
  updateStatus: (id: string, newStatus: string, asStaff = false) =>
    client.put(`${asStaff ? staffPrefix : adminPrefix}/${id}/status`, { newStatus }),
  cancel: (id: string, reason: string, asStaff = false) =>
    client.delete(`${asStaff ? staffPrefix : adminPrefix}/${id}/cancel`, { data: { reason } }),
};
