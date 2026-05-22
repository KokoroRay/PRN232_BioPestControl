import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { productService } from '../../services/productService';
import { categoryService } from '../../services/categoryService';
import { chemicalService } from '../../services/chemicalService';
import { getApiErrorMessage } from '../../lib/apiError';
import type { Product, CreateProductRequest, UpdateProductRequest } from '../../types/catalog';
import type { Category } from '../../types/catalog';
import type { Chemical } from '../../types/agri';

const emptyCreate: CreateProductRequest = {
  sku: '',
  name: '',
  description: '',
  unit: '',
  unitPrice: 0,
  imageUrl: '',
  categoryId: 0,
  chemicalProfileId: undefined,
  isActive: true,
};

type ProductFormState = CreateProductRequest & { managedByStaffId?: number };

const ProductsPage: React.FC = () => {
  const { canManageCatalog } = usePageMode();
  const { showToast } = useToast();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [chemicals, setChemicals] = useState<Chemical[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [categoryFilter, setCategoryFilter] = useState(0);
  const [statusFilter, setStatusFilter] = useState('');
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [form, setForm] = useState<ProductFormState>(emptyCreate);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const loadMeta = useCallback(async () => {
    const [c, ch] = await Promise.all([categoryService.getAll(), chemicalService.getAll()]);
    setCategories(c);
    setChemicals(ch);
  }, []);

  const loadProducts = useCallback(
    async (nameQuery?: string) => {
      setLoading(true);
      try {
        const p = await productService.getAll(nameQuery);
        setProducts(p);
      } catch {
        showToast('Failed to load products', 'error');
      } finally {
        setLoading(false);
      }
    },
    [showToast],
  );

  useEffect(() => {
    loadMeta().catch(() => showToast('Failed to load filters', 'error'));
  }, [loadMeta, showToast]);

  useEffect(() => {
    const q = search.trim();
    const t = setTimeout(() => {
      loadProducts(q || undefined);
    }, 300);
    return () => clearTimeout(t);
  }, [search, loadProducts]);

  const displayed = useMemo(() => {
    let list = products;
    if (categoryFilter > 0) list = list.filter((p) => p.categoryId === categoryFilter);
    if (statusFilter === 'active') list = list.filter((p) => p.isActive);
    if (statusFilter === 'inactive') list = list.filter((p) => !p.isActive);
    return list;
  }, [products, categoryFilter, statusFilter]);

  const openCreate = () => {
    setEditId(null);
    setForm({ ...emptyCreate, categoryId: categories[0]?.id ?? 0 });
    setDrawerOpen(true);
  };

  const openEdit = (p: Product) => {
    setEditId(p.id);
    setForm({
      sku: p.sku,
      name: p.name,
      description: p.description ?? '',
      unit: p.unit ?? '',
      unitPrice: p.unitPrice,
      imageUrl: p.imageUrl ?? '',
      categoryId: p.categoryId,
      chemicalProfileId: p.chemicalProfileId,
      isActive: p.isActive,
      managedByStaffId: p.managedByStaffId,
    });
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editId) {
        const body: UpdateProductRequest = {
          sku: form.sku,
          name: form.name,
          description: form.description || undefined,
          unit: form.unit || undefined,
          unitPrice: form.unitPrice,
          imageUrl: form.imageUrl || undefined,
          categoryId: form.categoryId,
          chemicalProfileId: form.chemicalProfileId,
          isActive: form.isActive ?? true,
          managedByStaffId: form.managedByStaffId,
        };
        await productService.update(editId, body);
      } else {
        const body: CreateProductRequest = {
          sku: form.sku,
          name: form.name,
          description: form.description || undefined,
          unit: form.unit || undefined,
          unitPrice: form.unitPrice,
          imageUrl: form.imageUrl || undefined,
          categoryId: form.categoryId,
          chemicalProfileId: form.chemicalProfileId,
          isActive: form.isActive ?? true,
        };
        await productService.create(body);
      }
      showToast(editId ? 'Product updated' : 'Product created');
      setDrawerOpen(false);
      loadProducts(search.trim() || undefined);
    } catch (err) {
      showToast(getApiErrorMessage(err, 'Save failed'), 'error');
    }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try {
      await productService.delete(deleteId);
      showToast('Product deleted');
      setDeleteId(null);
      loadProducts(search.trim() || undefined);
    } catch (err) {
      showToast(getApiErrorMessage(err, 'Delete failed'), 'error');
    }
  };

  const formatPrice = (v: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);

  return (
    <div className="admin-page">
      <PageHeader
        title="Products Management"
        subtitle="Manage your bio-pesticide product catalog."
        actionLabel={canManageCatalog ? 'Add Product' : undefined}
        onAction={canManageCatalog ? openCreate : undefined}
      />
      <div className="filter-bar grid-4">
        <input
          type="search"
          placeholder="Search name, SKU (API)..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select value={categoryFilter} onChange={(e) => setCategoryFilter(Number(e.target.value))}>
          <option value={0}>All Categories</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Statuses</option>
          <option value="active">Active</option>
          <option value="inactive">Inactive</option>
        </select>
      </div>
      {loading ? (
        <LoadingState />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Category</th>
                <th>Unit</th>
                <th>Chemical</th>
                <th>Price</th>
                <th>Managed by</th>
                <th>Status</th>
                {canManageCatalog && <th className="text-center">Actions</th>}
              </tr>
            </thead>
            <tbody>
              {displayed.length === 0 ? (
                <tr>
                  <td colSpan={canManageCatalog ? 8 : 7} className="empty-cell">
                    No products found
                  </td>
                </tr>
              ) : (
                displayed.map((p) => (
                  <tr key={p.id}>
                    <td>
                      <strong>{p.name}</strong>
                      <div className="text-muted">{p.sku}</div>
                      {p.description ? (
                        <div className="text-muted product-desc-preview">{p.description}</div>
                      ) : null}
                    </td>
                    <td>{p.categoryName ?? p.categoryId}</td>
                    <td>{p.unit ?? '—'}</td>
                    <td>{p.chemicalName ?? (p.chemicalProfileId ? `#${p.chemicalProfileId}` : '—')}</td>
                    <td>{formatPrice(p.unitPrice)}</td>
                    <td>{p.managedByStaffName ?? '—'}</td>
                    <td>
                      <span className={`pill ${p.isActive ? 'pill-green' : 'pill-red'}`}>
                        {p.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    {canManageCatalog && (
                      <td className="actions-cell">
                        <button type="button" className="btn-icon" onClick={() => openEdit(p)}>
                          <Pencil size={18} />
                        </button>
                        <button
                          type="button"
                          className="btn-icon danger"
                          onClick={() => setDeleteId(p.id)}
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
        title={editId ? 'Edit Product' : 'Add Product'}
        onClose={() => setDrawerOpen(false)}
        wide
      >
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            SKU *
            <input value={form.sku} onChange={(e) => setForm({ ...form, sku: e.target.value })} required />
          </label>
          <label>
            Name *
            <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          </label>
          <label>
            Unit
            <input
              value={form.unit}
              onChange={(e) => setForm({ ...form, unit: e.target.value })}
              placeholder="kg, lít, gói..."
            />
          </label>
          <label>
            Unit Price *
            <input
              type="number"
              min={0.01}
              step={1000}
              value={form.unitPrice}
              onChange={(e) => setForm({ ...form, unitPrice: Number(e.target.value) })}
              required
            />
          </label>
          <label>
            Category *
            <select
              value={form.categoryId}
              onChange={(e) => setForm({ ...form, categoryId: Number(e.target.value) })}
              required
            >
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
          <label>
            Chemical Profile
            <select
              value={form.chemicalProfileId ?? ''}
              onChange={(e) =>
                setForm({
                  ...form,
                  chemicalProfileId: e.target.value ? Number(e.target.value) : undefined,
                })
              }
            >
              <option value="">None</option>
              {chemicals.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
          {canManageCatalog && editId && (
            <label>
              Managed by Staff ID
              <input
                type="number"
                min={0}
                value={form.managedByStaffId ?? ''}
                onChange={(e) =>
                  setForm({
                    ...form,
                    managedByStaffId: e.target.value ? Number(e.target.value) : undefined,
                  })
                }
                placeholder="Optional — catalog staff id"
              />
            </label>
          )}
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
            Active (selling)
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
        title="Delete Product"
        message="Delete this product?"
        danger
        confirmLabel="Delete"
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  );
};

export default ProductsPage;
