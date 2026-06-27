import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys, mapList } from '../lib/normalize';
import type { Article, CreateArticleRequest } from '../types/article';

const client = createApiClient(`${API.article}/api`);

function mapArticle(raw: unknown): Article {
  return mapKeys<Article>(raw as Record<string, unknown>);
}

export const articleService = {
  getAll: async (params?: { title?: string; status?: string; tags?: string }) => {
    const { data } = await client.get('/articles', { params });
    return mapList<Article>(Array.isArray(data) ? data : []);
  },
  getById: async (id: string) => {
    const { data } = await client.get(`/articles/${id}`);
    return mapArticle(data);
  },
  create: (body: CreateArticleRequest) => client.post('/articles', body),
  update: (id: string, body: CreateArticleRequest) => client.put(`/articles/${id}`, body),
  delete: (id: string) => client.delete(`/articles/${id}`),
};
