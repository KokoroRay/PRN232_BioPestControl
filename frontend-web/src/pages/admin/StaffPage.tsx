import React, { useCallback, useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { StaffPermissionsPicker } from '../../components/admin/StaffPermissionsPicker';
import { useToast } from '../../context/ToastContext';
import { permissionService } from '../../services/permissionService';
import { staffService } from '../../services/staffService';
import type {
  CreateStaffRequest,
  PermissionGroup,
  Staff,
  UpdateStaffRequest,
} from '../../types/identity';

const emptyCreate: CreateStaffRequest = {
  email: '',
  password: '',
  fullName: '',
  phoneNumber: '',
  isFullAccess: true,
  permissionIds: [],
};

const StaffPage: React.FC = () => {
  const { showToast } = useToast();
  const [list, setList] = useState<Staff[]>([]);
  const [permissionGroups, setPermissionGroups] = useState<PermissionGroup[]>([]);
  const [loading, setLoading] = useState(true);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [createForm, setCreateForm] = useState<CreateStaffRequest>(emptyCreate);
  const [editForm, setEditForm] = useState<UpdateStaffRequest>({
    fullName: '',
    phoneNumber: '',
    newPassword: '',
    isFullAccess: true,
    permissionIds: [],
  });
  const [editEmail, setEditEmail] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setList(await staffService.getAll());
    } catch {
      showToast('Failed to load staff', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    load();
    permissionService.getGrouped().then(setPermissionGroups).catch(() => {
      showToast('Failed to load permissions', 'error');
    });
  }, [load, showToast]);

  const validatePermissions = (isFullAccess: boolean, permissionIds: number[]) => {
    if (!isFullAccess && permissionIds.length === 0) {
      showToast('Select at least one permission or enable Full access', 'error');
      return false;
    }
    return true;
  };

  const openCreate = () => {
    setEditId(null);
    setCreateForm(emptyCreate);
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    try {
      const staff = await staffService.getById(id);
      setEditId(id);
      setEditEmail(staff.email);
      setEditForm({
        fullName: staff.fullName ?? '',
        phoneNumber: staff.phoneNumber ?? '',
        newPassword: '',
        isFullAccess: staff.isFullAccess,
        permissionIds: (staff.permissions ?? []).map((p) => p.id),
      });
      setDrawerOpen(true);
    } catch {
      showToast('Failed to load staff details', 'error');
    }
  };

  const closeDrawer = () => {
    setDrawerOpen(false);
    setEditId(null);
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validatePermissions(createForm.isFullAccess, createForm.permissionIds)) return;
    setSaving(true);
    try {
      const body: CreateStaffRequest = {
        ...createForm,
        fullName: createForm.fullName || undefined,
        phoneNumber: createForm.phoneNumber || undefined,
      };
      await staffService.create(body);
      showToast('Staff created');
      closeDrawer();
      load();
    } catch {
      showToast('Create failed', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editId) return;
    if (!validatePermissions(editForm.isFullAccess, editForm.permissionIds)) return;
    setSaving(true);
    try {
      const body: UpdateStaffRequest = {
        fullName: editForm.fullName || undefined,
        phoneNumber: editForm.phoneNumber || undefined,
        isFullAccess: editForm.isFullAccess,
        permissionIds: editForm.permissionIds,
      };
      if (editForm.newPassword?.trim()) {
        body.newPassword = editForm.newPassword.trim();
      }
      await staffService.update(editId, body);
      showToast('Staff updated');
      closeDrawer();
      load();
    } catch {
      showToast('Update failed', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await staffService.delete(deleteId);
      showToast('Staff removed');
      setDeleteId(null);
      load();
    } catch {
      showToast('Delete failed', 'error');
    }
  };

  const isEdit = !!editId;

  return (
    <div className="admin-page">
      <PageHeader title="Staff Management" actionLabel="Add Staff" onAction={openCreate} />
      {loading ? (
        <LoadingState />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Member</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Access</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {list.map((s) => (
                <tr key={s.id}>
                  <td>
                    <strong>{s.fullName ?? '—'}</strong>
                    <div className="text-muted">{s.id.slice(0, 8)}</div>
                  </td>
                  <td>{s.email}</td>
                  <td>{s.phoneNumber ?? '—'}</td>
                  <td>
                    {s.isFullAccess ? 'Full access' : `${s.permissionCount} permissions`}
                  </td>
                  <td className="actions-cell">
                    <button
                      type="button"
                      className="btn-icon"
                      onClick={() => openEdit(s.id)}
                      aria-label="Edit staff"
                    >
                      <Pencil size={18} />
                    </button>
                    <button
                      type="button"
                      className="btn-icon danger"
                      onClick={() => setDeleteId(s.id)}
                      aria-label="Delete staff"
                    >
                      <Trash2 size={18} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <Drawer
        open={drawerOpen}
        title={isEdit ? 'Edit Staff' : 'Add Staff'}
        wide
        onClose={closeDrawer}
      >
        {isEdit ? (
          <form onSubmit={handleUpdate} className="form-stack">
            <label>
              Email
              <input type="email" value={editEmail} disabled />
            </label>
            <label>
              Full name
              <input
                value={editForm.fullName}
                onChange={(e) => setEditForm({ ...editForm, fullName: e.target.value })}
              />
            </label>
            <label>
              Phone
              <input
                value={editForm.phoneNumber}
                onChange={(e) => setEditForm({ ...editForm, phoneNumber: e.target.value })}
              />
            </label>
            <label>
              New password
              <input
                type="password"
                placeholder="Leave blank to keep current"
                value={editForm.newPassword}
                onChange={(e) => setEditForm({ ...editForm, newPassword: e.target.value })}
                minLength={8}
                pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"
                title="Password must contain at least 1 uppercase letter, 1 lowercase letter, and 1 number"
              />
            </label>
            <label className="checkbox-row">
              <input
                type="checkbox"
                checked={editForm.isFullAccess}
                onChange={(e) =>
                  setEditForm({
                    ...editForm,
                    isFullAccess: e.target.checked,
                    permissionIds: e.target.checked ? [] : editForm.permissionIds,
                  })
                }
              />
              Full access (manager)
            </label>
            {!editForm.isFullAccess && (
              <StaffPermissionsPicker
                groups={permissionGroups}
                selectedIds={editForm.permissionIds}
                onChange={(permissionIds) => setEditForm({ ...editForm, permissionIds })}
              />
            )}
            <div className="form-actions">
              <button type="button" className="btn-secondary" onClick={closeDrawer}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" disabled={saving}>
                {saving ? 'Saving...' : 'Save changes'}
              </button>
            </div>
          </form>
        ) : (
          <form onSubmit={handleCreate} className="form-stack">
            <label>
              Email *
              <input
                type="email"
                value={createForm.email}
                onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })}
                required
              />
            </label>
            <label>
              Password *
              <input
                type="password"
                value={createForm.password}
                onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })}
                required
                minLength={8}
                pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"
                title="Password must contain at least 1 uppercase letter, 1 lowercase letter, and 1 number"
              />
            </label>
            <label>
              Full name
              <input
                value={createForm.fullName}
                onChange={(e) => setCreateForm({ ...createForm, fullName: e.target.value })}
              />
            </label>
            <label>
              Phone
              <input
                value={createForm.phoneNumber}
                onChange={(e) => setCreateForm({ ...createForm, phoneNumber: e.target.value })}
              />
            </label>
            <label className="checkbox-row">
              <input
                type="checkbox"
                checked={createForm.isFullAccess}
                onChange={(e) =>
                  setCreateForm({
                    ...createForm,
                    isFullAccess: e.target.checked,
                    permissionIds: e.target.checked ? [] : createForm.permissionIds,
                  })
                }
              />
              Full access (manager)
            </label>
            {!createForm.isFullAccess && (
              <StaffPermissionsPicker
                groups={permissionGroups}
                selectedIds={createForm.permissionIds}
                onChange={(permissionIds) => setCreateForm({ ...createForm, permissionIds })}
              />
            )}
            <div className="form-actions">
              <button type="button" className="btn-secondary" onClick={closeDrawer}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" disabled={saving}>
                {saving ? 'Creating...' : 'Create'}
              </button>
            </div>
          </form>
        )}
      </Drawer>

      <ConfirmModal
        open={!!deleteId}
        title="Delete Staff"
        message="Remove this staff member?"
        danger
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  );
};

export default StaffPage;
