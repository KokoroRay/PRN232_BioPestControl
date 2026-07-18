import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys, mapList } from '../lib/normalize';
import { unwrap, unwrapPaged } from '../types/api';
import type { Order, OrderFilter, PlaceOrderRequest } from '../types/ordering';

const client = createApiClient(`${API.ordering}`);

const staffPrefix = '/api/staff/orders';
const adminPrefix = '/api/admin/orders';
const customerPrefix = '/api/orders';

function mapOrder(raw: unknown): Order {
  const data = mapKeys<Record<string, unknown>>(raw as Record<string, unknown>);
  const items = Array.isArray(data.items)
    ? mapList<Order['items'][number]>(data.items as unknown[])
    : [];
  return {
    id: String(data.id ?? ''),
    customerId: String(data.customerId ?? ''),
    orderDate: String(data.orderDate ?? ''),
    updatedAt: data.updatedAt as string | undefined,
    status: String(data.status ?? ''),
    statusCode: Number(data.statusCode ?? 0) || undefined,
    paymentStatus: String(data.paymentStatus ?? ''),
    paymentMethod: String(data.paymentMethod ?? ''),
    totalAmount: Number(data.totalAmount ?? 0),
    shippingAddress: data.shippingAddress as string | undefined,
    cancelledAt: data.cancelledAt as string | undefined,
    items,
  };
}

export const orderService = {
  getMyOrders: async (filter?: OrderFilter) => {
    const { data } = await client.get(customerPrefix, { params: filter });
    const paged = unwrapPaged<Order>(data);
    return {
      ...paged,
      items: (paged.items as unknown[]).map(mapOrder),
    };
  },

  getMyOrder: async (id: string) => {
    const { data } = await client.get(`${customerPrefix}/${id}`);
    return mapOrder(unwrap(data));
  },

  placeOrder: async (body: PlaceOrderRequest) => {
    const { data } = await client.post(customerPrefix, body);
    return mapOrder(unwrap(data));
  },

  cancelMyOrder: async (id: string) => {
    const { data } = await client.delete(`${customerPrefix}/${id}/cancel`);
    return mapOrder(unwrap(data));
  },

  getAdminOrders: async (filter?: OrderFilter) => {
    const { data } = await client.get(adminPrefix, { params: filter });
    const paged = unwrapPaged<Order>(data);
    return { ...paged, items: (paged.items as unknown[]).map(mapOrder) };
  },

  getStaffOrders: async (filter?: OrderFilter) => {
    const { data } = await client.get(staffPrefix, { params: filter });
    const paged = unwrapPaged<Order>(data);
    return { ...paged, items: (paged.items as unknown[]).map(mapOrder) };
  },

  getAdminOrder: async (id: string) => {
    const { data } = await client.get(`${adminPrefix}/${id}`);
    return mapOrder(unwrap(data));
  },

  getStaffOrder: async (id: string) => {
    const { data } = await client.get(`${staffPrefix}/${id}`);
    return mapOrder(unwrap(data));
  },

  updateStatus: (id: string, newStatus: string, asStaff = false) =>
    client.put(`${asStaff ? staffPrefix : adminPrefix}/${id}/status`, { newStatus }),

  cancel: (id: string, reason: string, asStaff = false) =>
    client.delete(`${asStaff ? staffPrefix : adminPrefix}/${id}/cancel`, { data: { reason } }),

  markAsPaid: async (id: string) => {
    const { data } = await client.put(`${customerPrefix}/${id}/mark-paid`);
    return mapOrder(unwrap(data));
  },
};
