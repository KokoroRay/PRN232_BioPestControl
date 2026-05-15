export interface Staff {
  id: string;
  userId: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  isActive: boolean;
  isFullAccess: boolean;
  permissionCount: number;
  permissions?: Permission[];
}

export interface Permission {
  id: number;
  code: string;
  displayName: string;
  description?: string;
  groupCode: string;
  groupName: string;
  displayOrder: number;
  isActive: boolean;
}

export interface PermissionGroup {
  groupCode: string;
  groupName: string;
  permissions: Permission[];
}

export interface Customer {
  id: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateStaffRequest {
  email: string;
  password: string;
  fullName?: string;
  phoneNumber?: string;
  isFullAccess: boolean;
  permissionIds: number[];
}

export interface UpdateStaffRequest {
  fullName?: string;
  phoneNumber?: string;
  newPassword?: string;
  isFullAccess: boolean;
  permissionIds: number[];
}

export interface UpdateCustomerRequest {
  fullName?: string;
  phoneNumber?: string;
}
