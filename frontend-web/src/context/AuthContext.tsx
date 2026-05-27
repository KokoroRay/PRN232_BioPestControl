import React, { createContext, useCallback, useContext, useMemo, useState } from 'react';
import { clearAuth, getStoredUser, getToken, setAuth, type AuthUser } from '../lib/http';
import { login as apiLogin, googleLogin as apiGoogleLogin, type LoginPayload } from '../services/authService';

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (payload: LoginPayload, options?: { allowedRoles?: string[] }) => Promise<string>;
  googleLogin: (idToken: string, options?: { allowedRoles?: string[] }) => Promise<string>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(() => (getToken() ? getStoredUser() : null));

  const login = useCallback(async (payload: LoginPayload, options?: { allowedRoles?: string[] }) => {
    const data = await apiLogin(payload);
    const allowed = options?.allowedRoles;
    if (allowed && !allowed.includes(data.role)) {
      throw new Error('You do not have permission to access this portal.');
    }
    if (!allowed && data.role !== 'Admin' && data.role !== 'Staff') {
      throw new Error('You do not have permission to access the admin portal.');
    }
    const authUser: AuthUser = {
      email: data.email,
      fullName: data.fullName,
      role: data.role,
      avatarUrl: data.avatarUrl,
    };
    setAuth(data.token, authUser);
    setUser(authUser);
    return data.role;
  }, []);

  const googleLogin = useCallback(async (idToken: string, options?: { allowedRoles?: string[] }) => {
    const data = await apiGoogleLogin(idToken);
    const allowed = options?.allowedRoles;
    if (allowed && !allowed.includes(data.role)) {
      throw new Error('You do not have permission to access this portal.');
    }
    const authUser: AuthUser = {
      email: data.email,
      fullName: data.fullName,
      role: data.role,
      avatarUrl: data.avatarUrl,
    };
    setAuth(data.token, authUser);
    setUser(authUser);
    return data.role;
  }, []);

  const logout = useCallback(() => {
    clearAuth();
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      user,
      isAuthenticated: !!user && !!getToken(),
      login,
      googleLogin,
      logout,
    }),
    [user, login, googleLogin, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
