import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys } from '../lib/normalize';
import { unwrap } from '../types/api';
import type { AddToCartRequest, Cart, CartItem, UpdateCartItemRequest } from '../types/trading';

const client = createApiClient(`${API.ordering}/api`);

function mapCartItem(raw: unknown): CartItem {
  const item = mapKeys<Record<string, unknown>>(raw as Record<string, unknown>);
  const unitPrice = Number(item.unitPrice ?? 0);
  const quantity = Number(item.quantity ?? 0);
  const subTotal = Number(item.subTotal ?? unitPrice * quantity);
  return {
    id: String(item.id ?? ''),
    productId: Number(item.productId ?? 0),
    productName: String(item.productName ?? ''),
    productImageUrl: item.productImageUrl as string | undefined,
    unitPrice,
    quantity,
    subTotal,
    addedAt: item.addedAt as string | undefined,
    updatedAt: item.updatedAt as string | undefined,
  };
}

function mapCart(raw: unknown): Cart {
  const data = mapKeys<Record<string, unknown>>(raw as Record<string, unknown>);
  const items = Array.isArray(data.items)
    ? data.items.map(mapCartItem)
    : [];
  const totalQuantity = Number(data.totalQuantity ?? items.reduce((s, i) => s + i.quantity, 0));
  const totalPrice = Number(
    data.totalPrice ?? items.reduce((s, i) => s + i.subTotal, 0),
  );
  return {
    id: data.id ? String(data.id) : undefined,
    customerId: data.customerId ? String(data.customerId) : undefined,
    items,
    totalQuantity,
    totalPrice,
  };
}

export const cartService = {
  getCart: async () => {
    const { data } = await client.get('/cart');
    return mapCart(unwrap(data));
  },

  addItem: async (body: AddToCartRequest) => {
    const { data } = await client.post('/cart/items', body);
    return mapCart(unwrap(data));
  },

  updateItem: async (itemId: string, body: UpdateCartItemRequest) => {
    const { data } = await client.put(`/cart/items/${itemId}`, body);
    return mapCartItem(unwrap(data));
  },

  removeItem: async (itemId: string) => {
    await client.delete(`/cart/items/${itemId}`);
  },
};
