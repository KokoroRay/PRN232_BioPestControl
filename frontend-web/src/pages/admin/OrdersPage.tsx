import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Eye } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { orderService } from '../../services/orderService';
import type { Order } from '../../types/ordering';

const STATUSES = [
  'WaitingConfirmation',
  'Confirmed',
  'Processing',
  'Shipping',
  'Delivered',
  'Cancelled',
];

const OrdersPage: React.FC = () => {
  const { isStaff } = usePageMode();
  const { showToast } = useToast();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [detail, setDetail] = useState<Order | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const fetch = isStaff ? orderService.getStaffOrders : orderService.getAdminOrders;
      const res = await fetch({
        search: search || undefined,
        status: statusFilter || undefined,
        pageSize: 50,
      });
      setOrders(res.items);
    } catch {
      showToast('Failed to load orders', 'error');
    } finally {
      setLoading(false);
    }
  }, [search, statusFilter, showToast, isStaff]);

  useEffect(() => {
    const t = setTimeout(load, 300);
    return () => clearTimeout(t);
  }, [load]);

  const counts = useMemo(() => {
    const c: Record<string, number> = {};
    for (const s of STATUSES) c[s] = 0;
    for (const o of orders) c[o.status] = (c[o.status] ?? 0) + 1;
    return c;
  }, [orders]);

  const formatMoney = (v: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);

  const updateStatus = async (id: string, newStatus: string) => {
    try {
      await orderService.updateStatus(id, newStatus, isStaff);
      showToast('Order status updated');
      load();
      if (detail?.id === id) {
        const getOne = isStaff ? orderService.getStaffOrder : orderService.getAdminOrder;
        const updated = await getOne(id);
        setDetail(updated);
      }
    } catch {
      showToast('Update failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader title="Orders Management" subtitle="Track and manage customer orders." />
      <div className="stats-row">
        <div className="mini-stat"><span>Pending</span><strong>{counts.WaitingConfirmation ?? 0}</strong></div>
        <div className="mini-stat"><span>Processing</span><strong>{(counts.Confirmed ?? 0) + (counts.Processing ?? 0)}</strong></div>
        <div className="mini-stat"><span>Shipping</span><strong>{counts.Shipping ?? 0}</strong></div>
        <div className="mini-stat"><span>Delivered</span><strong>{counts.Delivered ?? 0}</strong></div>
      </div>
      <div className="filter-bar">
        <input type="search" placeholder="Search orders..." value={search} onChange={(e) => setSearch(e.target.value)} />
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Status</option>
          {STATUSES.map((s) => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>
      </div>
      {loading ? (
        <LoadingState />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Order</th>
                <th>Date</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Payment</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {orders.map((o) => (
                <tr key={o.id}>
                  <td><code>#{String(o.id).slice(0, 8)}</code></td>
                  <td>{new Date(o.orderDate).toLocaleString('vi-VN')}</td>
                  <td>{formatMoney(o.totalAmount)}</td>
                  <td><span className="pill">{o.status}</span></td>
                  <td>{o.paymentStatus}</td>
                  <td>
                    <button type="button" className="btn-icon" onClick={() => setDetail(o)}><Eye size={18} /></button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <Drawer open={!!detail} title="Order Detail" onClose={() => setDetail(null)} wide>
        {detail && (
          <div className="form-stack">
            <p><strong>ID:</strong> {detail.id}</p>
            <p><strong>Status:</strong> {detail.status}</p>
            <p><strong>Total:</strong> {formatMoney(detail.totalAmount)}</p>
            <label>Update status
              <select defaultValue={detail.status} onChange={(e) => updateStatus(detail.id, e.target.value)}>
                {STATUSES.map((s) => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
            </label>
            <h4>Items</h4>
            <ul className="order-items-list">
              {detail.items?.map((i) => (
                <li key={i.id}>{i.productName} × {i.quantity} — {formatMoney(i.unitPrice * i.quantity)}</li>
              ))}
            </ul>
          </div>
        )}
      </Drawer>
    </div>
  );
};

export default OrdersPage;
