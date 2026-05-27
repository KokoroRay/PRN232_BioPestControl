import React, { useCallback, useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { usePageMode } from '../../context/PageModeContext';
import { useToast } from '../../context/ToastContext';
import { discountService } from '../../services/discountService';
import { productService } from '../../services/productService';
import type { Discount, CreateDiscountRequest } from '../../types/trading';
import type { Product } from '../../types/catalog';

const DiscountsPage: React.FC = () => {
  const { canManageCatalog } = usePageMode();
  const { showToast } = useToast();
  const [list, setList] = useState<Discount[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [activeOnly, setActiveOnly] = useState('');
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [form, setForm] = useState<CreateDiscountRequest>({
    name: '',
    discountPercent: 10,
    startDate: new Date().toISOString().slice(0, 16),
    endDate: new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 16),
    isActive: true,
    productId: 1,
  });

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [d, p] = await Promise.all([
        discountService.getAll({
          search: search || undefined,
          isActive: activeOnly === 'true' ? true : activeOnly === 'false' ? false : undefined,
        }),
        productService.getAll(),
      ]);
      setList(d);
      setProducts(p);
      if (p[0] && !editId) setForm((f) => ({ ...f, productId: p[0].id }));
    } catch {
      showToast('Failed to load discounts', 'error');
    } finally {
      setLoading(false);
    }
  }, [search, activeOnly, showToast, editId]);

  useEffect(() => {
    const t = setTimeout(load, 300);
    return () => clearTimeout(t);
  }, [load]);

  const openCreate = () => {
    setEditId(null);
    setDrawerOpen(true);
  };

  const openEdit = (d: Discount) => {
    setEditId(d.id);
    setForm({
      name: d.name,
      discountPercent: d.discountPercent,
      startDate: d.startDate.slice(0, 16),
      endDate: d.endDate.slice(0, 16),
      isActive: d.isActive,
      productId: d.productId,
    });
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const body = {
      ...form,
      startDate: new Date(form.startDate).toISOString(),
      endDate: new Date(form.endDate).toISOString(),
    };
    try {
      if (editId) await discountService.update(editId, body);
      else await discountService.create(body);
      showToast(editId ? 'Discount updated' : 'Discount created');
      setDrawerOpen(false);
      load();
    } catch {
      showToast('Save failed', 'error');
    }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try {
      await discountService.delete(deleteId);
      showToast('Discount deleted');
      setDeleteId(null);
      load();
    } catch {
      showToast('Delete failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader
        title="Discounts & Promotions"
        subtitle={canManageCatalog ? 'Manage coupons and sales.' : 'View promotions (read-only).'}
        actionLabel={canManageCatalog ? 'Create Discount' : undefined}
        onAction={canManageCatalog ? openCreate : undefined}
      />
      <div className="filter-bar">
        <input placeholder="Search by name..." value={search} onChange={(e) => setSearch(e.target.value)} />
        <select value={activeOnly} onChange={(e) => setActiveOnly(e.target.value)}>
          <option value="">All Statuses</option>
          <option value="true">Active Only</option>
          <option value="false">Expired Only</option>
        </select>
      </div>
      {loading ? <LoadingState /> : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead><tr><th>Name</th><th>Value</th><th>Product</th><th>Duration</th><th>Status</th>{canManageCatalog && <th></th>}</tr></thead>
            <tbody>
              {list.map((d) => (
                <tr key={d.id}>
                  <td><strong>{d.name}</strong></td>
                  <td><span className="pill pill-red">-{d.discountPercent}%</span></td>
                  <td>{products.find((p) => p.id === d.productId)?.name ?? d.productId}</td>
                  <td>{new Date(d.startDate).toLocaleDateString('vi-VN')} – {new Date(d.endDate).toLocaleDateString('vi-VN')}</td>
                  <td>{d.isCurrentlyRunning || d.isActive ? <span className="pill pill-green">Active</span> : <span className="pill">Inactive</span>}</td>
                  {canManageCatalog && (
                    <td className="actions-cell">
                      <button type="button" className="btn-icon" onClick={() => openEdit(d)}><Pencil size={18} /></button>
                      <button type="button" className="btn-icon danger" onClick={() => setDeleteId(d.id)}><Trash2 size={18} /></button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <Drawer open={drawerOpen} title={editId ? 'Edit Discount' : 'Create Discount'} onClose={() => setDrawerOpen(false)}>
        <form onSubmit={handleSubmit} className="form-stack">
          <label>Name *<input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required /></label>
          <label>Percent *<input type="number" min={0} max={100} value={form.discountPercent} onChange={(e) => setForm({ ...form, discountPercent: Number(e.target.value) })} /></label>
          <label>Product<select value={form.productId} onChange={(e) => setForm({ ...form, productId: Number(e.target.value) })}>{products.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}</select></label>
          <label>Start<input type="datetime-local" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} /></label>
          <label>End<input type="datetime-local" value={form.endDate} onChange={(e) => setForm({ ...form, endDate: e.target.value })} /></label>
          <label className="checkbox-row"><input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} /> Active</label>
          <div className="form-actions">
            <button type="button" className="btn-secondary" onClick={() => setDrawerOpen(false)}>Cancel</button>
            <button type="submit" className="btn-primary">Save</button>
          </div>
        </form>
      </Drawer>
      <ConfirmModal open={deleteId != null} title="Delete Discount" message="Delete this discount?" danger onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </div>
  );
};

export default DiscountsPage;
