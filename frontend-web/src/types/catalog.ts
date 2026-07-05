export interface Category {
  id: number;
  name: string;
  description?: string;
  createdByAdminId?: number;
  createdByAdminName?: string;
  managedByStaffId?: number;
  managedByStaffName?: string;
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
  updatedAt?: string;
  createdByAdminId?: number;
  createdByAdminName?: string;
  managedByStaffId?: number;
  managedByStaffName?: string;
  cropIds?: number[];
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  createdByAdminId?: number;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  managedByStaffId?: number;
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
  createdByAdminId?: number;
  cropIds?: number[];
}

export interface UpdateProductRequest {
  sku: string;
  name: string;
  description?: string;
  unit?: string;
  unitPrice: number;
  imageUrl?: string;
  categoryId: number;
  chemicalProfileId?: number;
  isActive?: boolean;
  managedByStaffId?: number;
  cropIds?: number[];
}
