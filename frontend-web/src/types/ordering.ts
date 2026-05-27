export interface Order {
  id: string;
  customerId: string;
  orderDate: string;
  status: string;
  paymentStatus: string;
  paymentMethod: string;
  totalAmount: number;
  shippingAddress?: string;
  items: OrderItem[];
}

export interface OrderItem {
  id: string;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
}

export interface OrderFilter {
  status?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}
