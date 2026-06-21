import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { cartService } from '../services/cartService';
import { getApiErrorMessage } from '../lib/apiError';
import type { Product } from '../types/catalog';
import type { Cart } from '../types/trading';
import { useAuth } from './AuthContext';

interface AddToCartResult {
  success: boolean;
  needLogin?: boolean;
  message?: string;
}

interface CartContextValue {
  cart: Cart | null;
  itemCount: number;
  loading: boolean;
  refreshCart: () => Promise<void>;
  addToCart: (product: Pick<Product, 'id' | 'name' | 'unitPrice' | 'imageUrl'>, quantity?: number) => Promise<AddToCartResult>;
  updateQuantity: (itemId: string, quantity: number) => Promise<boolean>;
  removeItem: (itemId: string) => Promise<boolean>;
}

const emptyCart: Cart = { items: [], totalQuantity: 0, totalPrice: 0 };

const CartContext = createContext<CartContextValue | null>(null);

export const CartProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(false);

  const refreshCart = useCallback(async () => {
    if (!isAuthenticated) {
      setCart(null);
      return;
    }
    setLoading(true);
    try {
      const data = await cartService.getCart();
      setCart(data);
    } catch {
      setCart(emptyCart);
    } finally {
      setLoading(false);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    refreshCart();
  }, [isAuthenticated]);

  const addToCart = useCallback(
    async (
      product: Pick<Product, 'id' | 'name' | 'unitPrice' | 'imageUrl'>,
      quantity = 1,
    ): Promise<AddToCartResult> => {
      if (!isAuthenticated) {
        return {
          success: false,
          needLogin: true,
          message: 'Please login to add items to cart',
        };
      }
      try {
        const updated = await cartService.addItem({
          productId: product.id,
          productName: product.name,
          unitPrice: product.unitPrice,
          productImageUrl: product.imageUrl,
          quantity,
        });
        setCart(updated);
        return { success: true, message: 'Item added to cart successfully' };
      } catch (err) {
        return {
          success: false,
          message: getApiErrorMessage(err, 'Could not add to cart'),
        };
      }
    },
    [isAuthenticated],
  );

  const updateQuantity = useCallback(async (itemId: string, quantity: number) => {
    try {
      const updatedItem = await cartService.updateItem(itemId, { quantity });
      setCart((prev) => {
        if (!prev) return prev;
        const items = prev.items.map((item) =>
          item.id === itemId
            ? {
                ...item,
                quantity: updatedItem.quantity,
                subTotal: updatedItem.subTotal,
              }
            : item,
        );
        return {
          ...prev,
          items,
          totalQuantity: items.reduce((s, i) => s + i.quantity, 0),
          totalPrice: items.reduce((s, i) => s + i.subTotal, 0),
        };
      });
      return true;
    } catch {
      return false;
    }
  }, []);

  const removeItem = useCallback(async (itemId: string) => {
    try {
      await cartService.removeItem(itemId);
      setCart((prev) => {
        if (!prev) return prev;
        const items = prev.items.filter((item) => item.id !== itemId);
        return {
          ...prev,
          items,
          totalQuantity: items.reduce((s, i) => s + i.quantity, 0),
          totalPrice: items.reduce((s, i) => s + i.subTotal, 0),
        };
      });
      return true;
    } catch {
      return false;
    }
  }, []);

  const itemCount = cart?.items.reduce((s, i) => s + i.quantity, 0) ?? 0;

  const value = useMemo(
    () => ({
      cart,
      itemCount,
      loading,
      refreshCart,
      addToCart,
      updateQuantity,
      removeItem,
    }),
    [cart, itemCount, loading, refreshCart, addToCart, updateQuantity, removeItem],
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
};

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
