/** Map API JSON keys to camelCase for frontend types */
export function mapKeys<T>(obj: Record<string, unknown>): T {
  const out: Record<string, unknown> = {};
  for (const [k, v] of Object.entries(obj)) {
    const camel = k.charAt(0).toLowerCase() + k.slice(1);
    out[camel] = v;
  }
  return out as T;
}

export function mapList<T>(list: unknown[]): T[] {
  if (!Array.isArray(list)) return [];
  return list.map((item) => mapKeys<T>(item as Record<string, unknown>));
}
