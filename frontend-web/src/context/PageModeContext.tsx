import React, { createContext, useContext, useMemo } from 'react';

export type PageMode = 'admin' | 'staff';

interface PageModeContextValue {
  mode: PageMode;
  isStaff: boolean;
  isAdmin: boolean;
  /** CRUD sản phẩm, danh mục, giảm giá, hóa chất (admin only) */
  canManageCatalog: boolean;
  canImportWarehouse: boolean;
  canManageStaff: boolean;
}

const PageModeContext = createContext<PageModeContextValue | null>(null);

export const PageModeProvider: React.FC<{ mode: PageMode; children: React.ReactNode }> = ({
  mode,
  children,
}) => {
  const value = useMemo<PageModeContextValue>(() => {
    const isStaff = mode === 'staff';
    return {
      mode,
      isStaff,
      isAdmin: mode === 'admin',
      canManageCatalog: !isStaff,
      canImportWarehouse: !isStaff,
      canManageStaff: !isStaff,
    };
  }, [mode]);

  return <PageModeContext.Provider value={value}>{children}</PageModeContext.Provider>;
};

export function usePageMode() {
  const ctx = useContext(PageModeContext);
  if (!ctx) {
    return {
      mode: 'admin' as PageMode,
      isStaff: false,
      isAdmin: true,
      canManageCatalog: true,
      canImportWarehouse: true,
      canManageStaff: true,
    };
  }
  return ctx;
}
