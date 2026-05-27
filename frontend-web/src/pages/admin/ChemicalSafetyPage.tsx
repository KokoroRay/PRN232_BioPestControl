import React, { useCallback, useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { chemicalService } from '../../services/chemicalService';
import type { Chemical, CreateChemicalRequest } from '../../types/agri';

const empty: CreateChemicalRequest = {
  name: '',
  vietnameseName: '',
  casNumber: '',
  chemicalGroup: '',
  toxicityLevel: '',
  safetyNotes: '',
  isActive: true,
};

const ChemicalSafetyPage: React.FC = () => {
  const { isStaff, canManageCatalog } = usePageMode();
  const { showToast } = useToast();
  const [list, setList] = useState<Chemical[]>([]);
  const [loading, setLoading] = useState(true);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [form, setForm] = useState<CreateChemicalRequest>(empty);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setList(isStaff ? await chemicalService.getAllStaff() : await chemicalService.getAll());
    } catch {
      showToast('Failed to load chemicals', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast, isStaff]);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = () => {
    setEditId(null);
    setForm(empty);
    setDrawerOpen(true);
  };

  const openEdit = (c: Chemical) => {
    setEditId(c.id);
    setForm({
      name: c.name,
      vietnameseName: c.vietnameseName,
      casNumber: c.casNumber,
      chemicalGroup: c.chemicalGroup,
      chemicalFormula: c.chemicalFormula,
      description: c.description,
      toxicityLevel: c.toxicityLevel,
      usageMethod: c.usageMethod,
      safetyNotes: c.safetyNotes,
      targetCrops: c.targetCrops,
      targetPests: c.targetPests,
      isActive: c.isActive,
    });
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editId) await chemicalService.update(editId, form);
      else await chemicalService.create(form);
      showToast(editId ? 'Chemical updated' : 'Chemical created');
      setDrawerOpen(false);
      load();
    } catch {
      showToast('Save failed', 'error');
    }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try {
      await chemicalService.delete(deleteId);
      showToast('Chemical deleted');
      setDeleteId(null);
      load();
    } catch {
      showToast('Delete failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader
        title="Chemical Safety"
        subtitle={isStaff ? 'View chemical safety profiles (read-only).' : 'Manage chemical profiles and safety data.'}
        actionLabel={canManageCatalog ? 'Add Chemical' : undefined}
        onAction={canManageCatalog ? openCreate : undefined}
      />
      {loading ? <LoadingState /> : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead><tr><th>Name</th><th>Group</th><th>Toxicity</th><th>Status</th>{canManageCatalog && <th></th>}</tr></thead>
            <tbody>
              {list.map((c) => (
                <tr key={c.id}>
                  <td><strong>{c.name}</strong>{c.vietnameseName && <div className="text-muted">{c.vietnameseName}</div>}</td>
                  <td>{c.chemicalGroup ?? '—'}</td>
                  <td>{c.toxicityLevel ?? '—'}</td>
                  <td><span className={`pill ${c.isActive ? 'pill-green' : 'pill-red'}`}>{c.isActive ? 'Active' : 'Inactive'}</span></td>
                  {canManageCatalog && (
                    <td className="actions-cell">
                      <button type="button" className="btn-icon" onClick={() => openEdit(c)}><Pencil size={18} /></button>
                      <button type="button" className="btn-icon danger" onClick={() => setDeleteId(c.id)}><Trash2 size={18} /></button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <Drawer open={drawerOpen} title={editId ? 'Edit Chemical' : 'Add Chemical'} onClose={() => setDrawerOpen(false)} wide>
        <form onSubmit={handleSubmit} className="form-stack">
          <label>Name *<input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required /></label>
          <label>Vietnamese name<input value={form.vietnameseName} onChange={(e) => setForm({ ...form, vietnameseName: e.target.value })} /></label>
          <label>CAS number<input value={form.casNumber} onChange={(e) => setForm({ ...form, casNumber: e.target.value })} /></label>
          <label>Group<input value={form.chemicalGroup} onChange={(e) => setForm({ ...form, chemicalGroup: e.target.value })} /></label>
          <label>Toxicity<input value={form.toxicityLevel} onChange={(e) => setForm({ ...form, toxicityLevel: e.target.value })} /></label>
          <label>Safety notes<textarea value={form.safetyNotes} onChange={(e) => setForm({ ...form, safetyNotes: e.target.value })} rows={4} /></label>
          <label className="checkbox-row"><input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} /> Active</label>
          <div className="form-actions">
            <button type="button" className="btn-secondary" onClick={() => setDrawerOpen(false)}>Cancel</button>
            <button type="submit" className="btn-primary">Save</button>
          </div>
        </form>
      </Drawer>
      <ConfirmModal open={deleteId != null} title="Delete Chemical" message="Delete this chemical profile?" danger onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </div>
  );
};

export default ChemicalSafetyPage;
