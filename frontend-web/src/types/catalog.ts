export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface Product {
  id: number;
  sku: string;
  name: string;
  description?: string;
  unit?: string;
  unitPrice: number;
  imageUrl?: string;
  categoryId: number;
  categoryName?: string;
  chemicalProfileId?: number;
  chemicalName?: string;
  isActive: boolean;
  createdAt?: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
}

export interface CreateProductRequest {
  sku: string;
  name: string;
  description?: string;
  unit?: string;
  unitPrice: number;
  imageUrl?: string;
  categoryId: number;
  chemicalProfileId?: number;
  isActive?: boolean;
}
