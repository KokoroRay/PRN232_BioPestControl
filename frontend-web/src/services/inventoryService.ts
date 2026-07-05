import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapList } from '../lib/normalize';
import { unwrap } from '../types/api';
import type { ImportProductItem, ProductDetail, ProductStock } from '../types/inventory';

const client = createApiClient(`${API.inventory}/api`);

export const inventoryService = {
  getStock: async (search?: string, page: number = 1, pageSize: number = 10) => {
    const { data } = await client.get('/inventory/stock', { params: { search, page, pageSize } });
    const inner = unwrap<any>(data);
    if (inner && inner.items) {
      return {
        items: mapList<ProductStock>(inner.items),
        totalCount: inner.totalCount,
        page: inner.page,
        pageSize: inner.pageSize
      };
    }
    const items = mapList<ProductStock>(Array.isArray(inner) ? inner : []);
    return { items, totalCount: items.length, page: 1, pageSize: items.length };
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
