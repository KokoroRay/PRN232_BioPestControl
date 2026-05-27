import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import type { ApiResponse } from '../types/api';

const client = createApiClient(`${API.identity}/api`);

export interface LoginPayload {
  email: string;
  password: string;
}

export interface AuthData {
  token: string;
  email: string;
  fullName?: string;
  role: string;
  avatarUrl?: string;
}

export async function login(payload: LoginPayload): Promise<AuthData> {
  const { data } = await client.post<ApiResponse<AuthData>>('/auth/login', payload);
  const body = data as unknown as Record<string, unknown>;
  const inner = (body.data ?? body.Data) as Record<string, unknown>;
  return {
    token: String(inner.token ?? inner.Token),
    email: String(inner.email ?? inner.Email),
    fullName: (inner.fullName ?? inner.FullName) as string | undefined,
    role: String(inner.role ?? inner.Role),
    avatarUrl: (inner.avatarUrl ?? inner.AvatarUrl) as string | undefined,
  };
}

export interface RegisterPayload {
  email: string;
  password: string;
  fullName?: string;
}

export async function register(payload: RegisterPayload): Promise<void> {
  await client.post('/auth/register', payload);
}

export async function googleLogin(idToken: string): Promise<AuthData> {
  const { data } = await client.post<ApiResponse<AuthData>>('/auth/google-login', { idToken });
  const body = data as unknown as Record<string, unknown>;
  const inner = (body.data ?? body.Data) as Record<string, unknown>;
  return {
    token: String(inner.token ?? inner.Token),
    email: String(inner.email ?? inner.Email),
    fullName: (inner.fullName ?? inner.FullName) as string | undefined,
    role: String(inner.role ?? inner.Role),
    avatarUrl: (inner.avatarUrl ?? inner.AvatarUrl) as string | undefined,
  };
}

