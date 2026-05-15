import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrap } from '../types/api';
import type { CreateDiscountRequest, Discount } from '../types/trading';

const client = createApiClient(`${API.trading}/api`);

export const discountService = {
  getAll: async (params?: { search?: string; isActive?: boolean }) => {
    const { data } = await client.get('/discounts', { params });
    const list = unwrap<Discount[]>(data);
    return mapList<Discount>(Array.isArray(list) ? list : Array.isArray(data) ? (data as Discount[]) : []);
  },
  create: (body: CreateDiscountRequest) => client.post('/discounts', body),
  update: (id: number, body: CreateDiscountRequest) => client.put(`/discounts/${id}`, body),
  delete: (id: number) => client.delete(`/discounts/${id}`),
};
