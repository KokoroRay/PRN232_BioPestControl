import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import type { Category, CreateCategoryRequest } from '../types/catalog';

const client = createApiClient(`${API.catalog}/api`);

export const categoryService = {
  getAll: async (name?: string) => {
    const { data } = await client.get('/categories', { params: name ? { name } : {} });
    return mapList<Category>(Array.isArray(data) ? data : []);
  },
  getById: async (id: number) => {
    const { data } = await client.get(`/categories/${id}`);
    return data as Category;
  },
  create: (body: CreateCategoryRequest) => client.post('/categories', body),
  update: (id: number, body: CreateCategoryRequest) => client.put(`/categories/${id}`, body),
  delete: (id: number) => client.delete(`/categories/${id}`),
};
