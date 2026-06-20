import type { CheckoutShippingInfo } from '../types/ordering';

const SHIPPING_KEY = 'checkout_shipping';

export const checkoutStorage = {
  save(info: CheckoutShippingInfo) {
    sessionStorage.setItem(SHIPPING_KEY, JSON.stringify(info));
  },
  load(): CheckoutShippingInfo | null {
    const raw = sessionStorage.getItem(SHIPPING_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as CheckoutShippingInfo;
    } catch {
      return null;
    }
  },
  clear() {
    sessionStorage.removeItem(SHIPPING_KEY);
  },
};
