export interface ProductStock {
  id: number;
  sku: string;
  name: string;
  stockQuantity: number;
  lowStockThreshold: number;
  isLowStock: boolean;
  isActive: boolean;
}

export interface ImportProductItem {
  sku: string;
  quantity: number;
  importPrice: number;
  expirationDate?: string;
}

export interface WarehouseImport {
  id: number;
  batchCode: string;
  productId: number;
  productSku: string;
  productName: string;
  quantityImported: number;
  importPrice: number;
  supplierName?: string;
  note?: string;
  expirationDate?: string;
  importedByUserName?: string;
  importedAt: string;
}

export interface ProductDetail extends ProductStock {
  description?: string;
  unit?: string;
  createdAt?: string;
  updatedAt?: string;
  importHistory: WarehouseImport[];
}
