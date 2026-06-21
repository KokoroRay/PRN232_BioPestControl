import type { CartItem } from '../types/trading';

export function filterSelectedCartItems(items: CartItem[], selectedItems?: string | null) {
  if (!selectedItems?.trim()) return items;
  const ids = new Set(
    selectedItems
      .split(',')
      .map((id) => id.trim())
      .filter(Boolean),
  );
  if (ids.size === 0) return items;
  return items.filter((item) => ids.has(item.id));
}

export function buildShippingAddress(fullName: string, phone: string, address: string) {
  return `${address.trim()} | Receiver: ${fullName.trim()} | Phone: ${phone.trim()}`;
}

export function parseShippingAddress(shippingAddress?: string) {
  if (!shippingAddress) {
    return { fullName: '', phone: '', address: '' };
  }
  const parts = shippingAddress.split('|').map((part) => part.trim());
  if (parts.length >= 3) {
    const phone = parts[parts.length - 1].replace(/^Phone:\s*/i, '') ?? '';
    const fullName = parts[parts.length - 2].replace(/^Receiver:\s*/i, '') ?? '';
    const address = parts.slice(0, -2).join(' | ');
    return { address, fullName, phone };
  }
  return { fullName: '', phone: '', address: shippingAddress };
}

export function orderStatusLabel(status: string) {
  const map: Record<string, string> = {
    WaitingConfirmation: 'Pending',
    Confirmed: 'Confirmed',
    Processing: 'Processed',
    Shipping: 'Shipped',
    Delivered: 'Delivered',
    Cancelled: 'Cancelled',
  };
  return map[status] ?? status;
}
