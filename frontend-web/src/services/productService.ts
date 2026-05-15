import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import type { CreateProductRequest, Product } from '../types/catalog';

const client = createApiClient(`${API.catalog}/api`);

export const productService = {
  getAll: async (name?: string) => {
    const { data } = await client.get('/products', { params: name ? { name } : {} });
    return mapList<Product>(Array.isArray(data) ? data : []);
  },
  getById: async (id: number) => {
    const { data } = await client.get(`/products/${id}`);
    return data as Product;
  },
  create: (body: CreateProductRequest) => client.post('/products', body),
  update: (id: number, body: CreateProductRequest) => client.put(`/products/${id}`, body),
  delete: (id: number) => client.delete(`/products/${id}`),
};
