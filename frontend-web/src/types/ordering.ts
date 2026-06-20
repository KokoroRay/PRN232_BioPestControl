export interface OrderItem {
  id: string;
  productId: number;
  productName: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
  subTotal?: number;
}

export interface Order {
  id: string;
  customerId: string;
  orderDate: string;
  updatedAt?: string;
  status: string;
  statusCode?: number;
  paymentStatus: string;
  paymentMethod: string;
  totalAmount: number;
  shippingAddress?: string;
  cancelledAt?: string;
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  shippingAddress: string;
  paymentMethod: 'COD' | 'PayOS';
}

export interface OrderFilter {
  status?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CheckoutShippingInfo {
  fullName: string;
  phone: string;
  address: string;
  selectedItems: string;
}
