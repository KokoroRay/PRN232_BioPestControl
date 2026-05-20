export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface DashboardStats {
  totalRevenue: number;
  totalSoldQuantity: number;
  totalLinkedProducts: number;
}

export interface RevenueStat {
  date: string;
  revenue: number;
}

export interface OrderSummary {
  id: string;
  customerId: string;
  orderDate: string;
  totalAmount: number;
  status: 'Pending' | 'Paid' | 'Shipped' | 'Delivered' | 'Cancelled';
}

export interface StatsFilterRequest {
  fromDate?: string;
  toDate?: string;
}
