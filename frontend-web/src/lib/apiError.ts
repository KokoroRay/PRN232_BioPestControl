/** Extract message from ASP.NET / axios error responses */
export function getApiErrorMessage(err: unknown, fallback = 'Request failed'): string {
  const ax = err as {
    response?: { data?: { message?: string; title?: string } | string };
    message?: string;
  };
  const data = ax.response?.data;
  if (typeof data === 'string' && data.trim()) return data;
  if (data && typeof data === 'object') {
    if (data.message) return String(data.message);
    if (data.title) return String(data.title);
  }
  if (err instanceof Error && err.message) return err.message;
  return fallback;
}
