import React, { useCallback, useEffect, useState } from 'react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { customerService } from '../../services/customerService';
import type { Customer } from '../../types/identity';

const CustomersPage: React.FC = () => {
  const { isStaff } = usePageMode();
  const { showToast } = useToast();
  const [list, setList] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = isStaff
        ? await customerService.getAllStaff({ keyword: search || undefined })
        : await customerService.getAll({ keyword: search || undefined });
      setList(data);
    } catch {
      showToast('Failed to load customers', 'error');
    } finally {
      setLoading(false);
    }
  }, [search, showToast, isStaff]);

  useEffect(() => {
    const t = setTimeout(load, 300);
    return () => clearTimeout(t);
  }, [load]);

  const toggleStatus = async (c: Customer) => {
    try {
      await customerService.updateStatus(c.id, !c.isActive, isStaff);
      showToast('Customer status updated');
      load();
    } catch {
      showToast('Update failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader title="Customer Management" subtitle="View and manage customer accounts." />
      <div className="filter-bar">
        <input placeholder="Search customers..." value={search} onChange={(e) => setSearch(e.target.value)} />
      </div>
      {loading ? <LoadingState /> : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead><tr><th>Customer</th><th>Email</th><th>Phone</th><th>Status</th><th></th></tr></thead>
            <tbody>
              {list.map((c) => (
                <tr key={c.id}>
                  <td><strong>{c.fullName ?? '—'}</strong></td>
                  <td>{c.email}</td>
                  <td>{c.phoneNumber ?? '—'}</td>
                  <td><span className={`pill ${c.isActive ? 'pill-green' : 'pill-red'}`}>{c.isActive ? 'Active' : 'Locked'}</span></td>
                  <td>
                    <button type="button" className="btn-secondary btn-sm" onClick={() => toggleStatus(c)}>
                      {c.isActive ? 'Lock' : 'Unlock'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default CustomersPage;
