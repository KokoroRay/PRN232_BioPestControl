export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages?: number;
}

/** Normalize ASP.NET camelCase / PascalCase responses */
export function unwrap<T>(payload: unknown): T {
  if (payload && typeof payload === 'object' && 'data' in payload) {
    return (payload as ApiResponse<T>).data;
  }
  return payload as T;
}

export function unwrapPaged<T>(payload: unknown): PagedResult<T> {
  const raw = unwrap<PagedResult<T> | Record<string, unknown>>(payload);
  if (!raw || typeof raw !== 'object') {
    return { items: [], totalCount: 0, page: 1, pageSize: 20 };
  }
  const r = raw as Record<string, unknown>;
  return {
    items: (r.items ?? r.Items ?? []) as T[],
    totalCount: Number(r.totalCount ?? r.TotalCount ?? 0),
    page: Number(r.page ?? r.Page ?? 1),
    pageSize: Number(r.pageSize ?? r.PageSize ?? 20),
    totalPages: Number(r.totalPages ?? r.TotalPages ?? 0) || undefined,
  };
}
