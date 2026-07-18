import React, { useCallback, useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { cropService } from '../../services/cropService';
import type { CropResponse } from '../../services/cropService';
import { getApiErrorMessage } from '../../lib/apiError';
import axios from 'axios';
import { API } from '../../config/api';

const API_URL = `${API.catalog}/api`;

type CropFormState = {
  name: string;
  description?: string;
  imageUrl?: string;
  isActive: boolean;
};

const emptyForm: CropFormState = {
  name: '',
  description: '',
  imageUrl: '',
  isActive: true,
};

const AdminCropsPage: React.FC = () => {
  const { canManageCatalog } = usePageMode();
  const { showToast } = useToast();
  const [crops, setCrops] = useState<CropResponse[]>([]);
  const [loading, setLoading] = useState(true);
  
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [form, setForm] = useState<CropFormState>(emptyForm);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const loadCrops = useCallback(async () => {
    setLoading(true);
    try {
      const data = await cropService.getAllCrops();
      setCrops(data);
    } catch {
      showToast('Failed to load crops', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    loadCrops();
  }, [loadCrops]);

  const openCreate = () => {
    setEditId(null);
    setForm(emptyForm);
    setDrawerOpen(true);
  };

  const openEdit = (c: CropResponse) => {
    setEditId(c.id);
    setForm({
      name: c.name,
      description: c.description ?? '',
      imageUrl: c.imageUrl ?? '',
      isActive: c.isActive,
    });
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('token');
      const headers = { Authorization: `Bearer ${token}` };

      if (editId) {
        await axios.put(`${API_URL}/crops/${editId}`, form, { headers });
      } else {
        await axios.post(`${API_URL}/crops`, form, { headers });
      }
      showToast(editId ? 'Crop updated' : 'Crop created');
      setDrawerOpen(false);
      loadCrops();
    } catch (err) {
      showToast(getApiErrorMessage(err, 'Save failed'), 'error');
    }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try {
      const token = localStorage.getItem('token');
      await axios.delete(`${API_URL}/crops/${deleteId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      showToast('Crop deleted');
      setDeleteId(null);
      loadCrops();
    } catch (err) {
      showToast(getApiErrorMessage(err, 'Delete failed'), 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader
        title="Crops Management"
        subtitle="Manage crop categories and their information."
        actionLabel={canManageCatalog ? 'Add Crop' : undefined}
        onAction={canManageCatalog ? openCreate : undefined}
      />
      
      {loading ? (
        <LoadingState />
      ) : (
        <div className="data-table-wrap mt-4">
          <table className="data-table">
            <thead>
              <tr>
                <th>Image</th>
                <th>Name</th>
                <th>Slug</th>
                <th>Status</th>
                {canManageCatalog && <th className="text-center">Actions</th>}
              </tr>
            </thead>
            <tbody>
              {crops.length === 0 ? (
                <tr>
                  <td colSpan={canManageCatalog ? 5 : 4} className="empty-cell">
                    No crops found
                  </td>
                </tr>
              ) : (
                crops.map((c) => (
                  <tr key={c.id}>
                    <td>
                      {c.imageUrl ? (
                        <img src={c.imageUrl} alt={c.name} style={{ width: 40, height: 40, objectFit: 'cover', borderRadius: 4 }} />
                      ) : (
                        <div style={{ width: 40, height: 40, background: '#eee', borderRadius: 4 }} />
                      )}
                    </td>
                    <td>
                      <strong>{c.name}</strong>
                      {c.description ? (
                        <div className="text-muted product-desc-preview" style={{ maxWidth: '300px' }}>{c.description}</div>
                      ) : null}
                    </td>
                    <td>{c.slug}</td>
                    <td>
                      <span className={`pill ${c.isActive ? 'pill-green' : 'pill-red'}`}>
                        {c.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    {canManageCatalog && (
                      <td className="actions-cell">
                        <button type="button" className="btn-icon" onClick={() => openEdit(c)}>
                          <Pencil size={18} />
                        </button>
                        <button
                          type="button"
                          className="btn-icon danger"
                          onClick={() => setDeleteId(c.id)}
                        >
                          <Trash2 size={18} />
                        </button>
                      </td>
                    )}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      <Drawer
        open={drawerOpen}
        title={editId ? 'Edit Crop' : 'Add Crop'}
        onClose={() => setDrawerOpen(false)}
      >
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            Name *
            <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          </label>
          <label>
            Image URL
            <input value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} />
          </label>
          {form.imageUrl ? (
            <img src={form.imageUrl} alt="" className="product-form-thumb" />
          ) : null}
          <label>
            Description
            <textarea
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              rows={4}
            />
          </label>
          <label className="checkbox-row">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
            />
            Active (visible to users)
          </label>
          <div className="form-actions">
            <button type="button" className="btn-secondary" onClick={() => setDrawerOpen(false)}>
              Cancel
            </button>
            <button type="submit" className="btn-primary">
              {editId ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </Drawer>

      <ConfirmModal
        open={deleteId != null}
        title="Delete Crop"
        message="Delete this crop? This will remove it from all associated products."
        danger
        confirmLabel="Delete"
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  );
};

export default AdminCropsPage;
