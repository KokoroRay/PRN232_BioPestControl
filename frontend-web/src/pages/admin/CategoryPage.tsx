import React, { useCallback, useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { categoryService } from '../../services/categoryService';
import type { Category } from '../../types/catalog';

const CategoryPage: React.FC = () => {
  const { canManageCatalog } = usePageMode();
  const { showToast } = useToast();
  const [items, setItems] = useState<Category[]>([]);
  const [filtered, setFiltered] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await categoryService.getAll();
      setItems(data);
      setFiltered(data);
    } catch {
      showToast('Failed to load categories', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    const q = search.trim().toLowerCase();
    if (!q) {
      setFiltered(items);
      return;
    }
    setFiltered(
      items.filter(
        (c) =>
          c.name.toLowerCase().includes(q) ||
          (c.description?.toLowerCase().includes(q) ?? false),
      ),
    );
  }, [search, items]);

  const openCreate = () => {
    setEditId(null);
    setName('');
    setDescription('');
    setDrawerOpen(true);
  };

  const openEdit = (c: Category) => {
    setEditId(c.id);
    setName(c.name);
    setDescription(c.description ?? '');
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const body = { name, description: description || undefined };
      if (editId) await categoryService.update(editId, body);
      else await categoryService.create(body);
      showToast(editId ? 'Category updated' : 'Category created');
      setDrawerOpen(false);
      load();
    } catch {
      showToast('Save failed', 'error');
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await categoryService.delete(deleteTarget.id);
      showToast('Category deleted');
      setDeleteTarget(null);
      load();
    } catch {
      showToast('Delete failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader
        title="Category Management"
        subtitle="Manage biological control product categories."
        actionLabel={canManageCatalog ? 'Add Category' : undefined}
        onAction={canManageCatalog ? openCreate : undefined}
      />
      <div className="filter-bar">
        <input
          type="search"
          placeholder="Search categories..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>
      {loading ? (
        <LoadingState message="Loading categories..." />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Category</th>
                <th>Description</th>
                {canManageCatalog && <th className="text-center">Actions</th>}
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr>
                  <td colSpan={3} className="empty-cell">
                    No categories found
                  </td>
                </tr>
              ) : (
                filtered.map((c) => (
                  <tr key={c.id}>
                    <td>
                      <strong>{c.name}</strong>
                      <div className="text-muted">CAT-{String(c.id).padStart(3, '0')}</div>
                    </td>
                    <td>{c.description || '—'}</td>
                    {canManageCatalog && (
                      <td className="actions-cell">
                        <button type="button" className="btn-icon" onClick={() => openEdit(c)} title="Edit">
                          <Pencil size={18} />
                        </button>
                        <button
                          type="button"
                          className="btn-icon danger"
                          onClick={() => setDeleteTarget({ id: c.id, name: c.name })}
                          title="Delete"
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
      <Drawer open={drawerOpen} title={editId ? 'Edit Category' : 'Add Category'} onClose={() => setDrawerOpen(false)}>
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            Name *
            <input value={name} onChange={(e) => setName(e.target.value)} required />
          </label>
          <label>
            Description
            <textarea value={description} onChange={(e) => setDescription(e.target.value)} rows={3} />
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
        open={!!deleteTarget}
        title="Delete Category"
        message={`Delete "${deleteTarget?.name}"?`}
        confirmLabel="Delete"
        danger
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
      />
    </div>
  );
};

export default CategoryPage;
