import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys, mapList } from '../lib/normalize';
import type { CreateProductRequest, Product, UpdateProductRequest } from '../types/catalog';

const client = createApiClient(`${API.catalog}/api`);

function mapProduct(raw: unknown): Product {
  return mapKeys<Product>(raw as Record<string, unknown>);
}

export const productService = {
  getAll: async (filter?: any) => {
    const { data } = await client.get('/products', { params: filter || {} });
    return mapList<Product>(Array.isArray(data) ? data : []);
  },
  getById: async (id: number) => {
    const { data } = await client.get(`/products/${id}`);
    return mapProduct(data);
  },
  create: (body: CreateProductRequest) => client.post('/products', body),
  update: (id: number, body: UpdateProductRequest) => client.put(`/products/${id}`, body),
  delete: (id: number) => client.delete(`/products/${id}`),
};
