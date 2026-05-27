import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrap } from '../types/api';
import type { ImportProductItem, ProductDetail, ProductStock } from '../types/inventory';

const client = createApiClient(`${API.inventory}/api`);

export const inventoryService = {
  getStock: async (search?: string) => {
    const { data } = await client.get('/inventory/stock', { params: { search } });
    const inner = unwrap<ProductStock[]>(data);
    return mapList<ProductStock>(Array.isArray(inner) ? inner : []);
  },
  importProducts: (items: ImportProductItem[], note?: string, supplierName?: string) =>
    client.post('/inventory/import', { items, note, supplierName }),
  getById: async (id: number) => {
    const { data } = await client.get(`/inventory/${id}`);
    const inner = unwrap<Record<string, unknown>>(data);
    const detail = mapList<ProductDetail>([inner])[0];
    const history = inner.importHistory ?? inner.ImportHistory;
    detail.importHistory = mapList(
      Array.isArray(history) ? history : [],
    ) as ProductDetail['importHistory'];
    return detail;
  },
};
