import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys } from '../lib/normalize';
import { unwrap } from '../types/api';

const client = createApiClient(`${API.identity}/api`);

export interface Profile {
  id: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  avatarUrl?: string;
  role: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface UpdateProfilePayload {
  fullName?: string;
  avatarUrl?: string;
  phoneNumber?: string;
}

export interface ChangePasswordPayload {
  oldPassword: string;
  newPassword: string;
}

export const profileService = {
  getProfile: async () => {
    const { data } = await client.get('/profile');
    return mapKeys<Profile>(unwrap(data) as Record<string, unknown>);
  },

  updateProfile: async (payload: UpdateProfilePayload) => {
    const { data } = await client.put('/profile', payload);
    return mapKeys<Profile>(unwrap(data) as Record<string, unknown>);
  },

  changePassword: async (payload: ChangePasswordPayload) => {
    const { data } = await client.put('/profile/change-password', payload);
    return data;
  },

  uploadAvatar: async (file: File) => {
    const form = new FormData();
    form.append('file', file);
    const { data } = await client.post('/profile/avatar', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data as { success: boolean; url?: string; message?: string };
  },
};
