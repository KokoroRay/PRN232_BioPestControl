import { API } from '../config/api';
import { createApiClient } from '../lib/http';
import { mapKeys, mapList } from '../lib/normalize';
import { unwrap } from '../types/api';
import type { Permission, PermissionGroup } from '../types/identity';

const client = createApiClient(`${API.identity}/api`);

export const permissionService = {
  getGrouped: async (): Promise<PermissionGroup[]> => {
    const { data } = await client.get('/admin/permissions/grouped');
    const raw = unwrap<unknown>(data);
    if (!Array.isArray(raw)) return [];
    return raw.map((g) => {
      const row = g as Record<string, unknown>;
      const group = mapKeys<PermissionGroup>(row);
      const perms = row.permissions ?? row.Permissions;
      group.permissions = mapList<Permission>(Array.isArray(perms) ? perms : []);
      return group;
    });
  },
};
