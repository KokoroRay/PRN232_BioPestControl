export interface CartItem {
  id: string;
  productId: number;
  productName: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
  subTotal: number;
  addedAt?: string;
  updatedAt?: string;
}

export interface Cart {
  id?: string;
  customerId?: string;
  items: CartItem[];
  totalQuantity: number;
  totalPrice: number;
}

export interface AddToCartRequest {
  productId: number;
  productName: string;
  unitPrice: number;
  productImageUrl?: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}

export interface Discount {
  id: number;
  name: string;
  discountPercent: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  productId: number;
  isCurrentlyRunning?: boolean;
}

export interface CreateDiscountRequest {
  name: string;
  discountPercent: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  productId: number;
}
