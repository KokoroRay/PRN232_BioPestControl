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
