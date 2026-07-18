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
  const [allOrders, setAllOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [paymentFilter, setPaymentFilter] = useState('');
  const [detail, setDetail] = useState<Order | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const fetch = isStaff ? orderService.getStaffOrders : orderService.getAdminOrders;
      // Lấy tổng thể trang (fetch all)
      const res = await fetch({ pageSize: 500 });
      setAllOrders(res.items);
    } catch {
      showToast('Failed to load orders', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast, isStaff]);

  useEffect(() => {
    load();
  }, [load]);

  const displayed = useMemo(() => {
    let list = allOrders;
    if (statusFilter) list = list.filter(o => o.status === statusFilter);
    if (paymentFilter) list = list.filter(o => o.paymentStatus === paymentFilter);
    if (search.trim()) {
      const q = search.toLowerCase().trim();
      list = list.filter(o => 
        String(o.id).toLowerCase().includes(q) || 
        (o.shippingAddress && o.shippingAddress.toLowerCase().includes(q)) ||
        (o.items && o.items.some(i => i.productName.toLowerCase().includes(q)))
      );
    }
    return list;
  }, [allOrders, statusFilter, paymentFilter, search]);

  const counts = allOrders.reduce((acc, o) => {
    acc[o.status] = (acc[o.status] ?? 0) + 1;
    return acc;
  }, {} as Record<string, number>);

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

  const parseShipping = (addr?: string) => {
    if (!addr) return { fullName: '', phone: '', address: '' };
    const parts = addr.split('|').map(p => p.trim());
    if (parts.length >= 3) {
      const phone = parts[parts.length - 1].replace(/^Phone:\s*/i, '') ?? '';
      const fullName = parts[parts.length - 2].replace(/^Receiver:\s*/i, '') ?? '';
      const address = parts.slice(0, -2).join(' | ');
      return { address, fullName, phone };
    }
    return { fullName: '', phone: '', address: addr };
  };

  const getNextStatus = (current: string) => {
    const map: Record<string, string> = {
      WaitingConfirmation: 'Confirmed',
      Confirmed: 'Processing',
      Processing: 'Shipping',
      Shipping: 'Delivered',
    };
    return map[current];
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
        <input type="search" placeholder="Search orders or customers..." value={search} onChange={(e) => setSearch(e.target.value)} />
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Status</option>
          {STATUSES.map((s) => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>
        <select value={paymentFilter} onChange={(e) => setPaymentFilter(e.target.value)}>
          <option value="">All Payments</option>
          <option value="Paid">Paid</option>
          <option value="Unpaid">Unpaid</option>
          <option value="Refunded">Refunded</option>
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
                <th>Customer</th>
                <th>Date</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Payment</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {displayed.length === 0 ? (
                <tr>
                  <td colSpan={7} className="text-center py-4">No orders found</td>
                </tr>
              ) : (
                displayed.map((o) => (
                  <tr key={o.id}>
                    <td><code>#{String(o.id).slice(0, 8)}</code></td>
                    <td>{parseShipping(o.shippingAddress).fullName || 'N/A'}</td>
                    <td>{new Date(o.orderDate).toLocaleString('vi-VN')}</td>
                    <td>{formatMoney(o.totalAmount)}</td>
                    <td><span className={`pill ${o.status.toLowerCase()}`}>{o.status}</span></td>
                    <td><span className={`pill ${o.paymentStatus.toLowerCase()}`}>{o.paymentStatus}</span></td>
                    <td>
                      <button type="button" className="btn-icon" onClick={() => setDetail(o)}><Eye size={18} /></button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
      <Drawer open={!!detail} title="Order Detail" onClose={() => setDetail(null)} wide>
        {detail && (
          <div className="form-stack">
            <p><strong>ID:</strong> {detail.id}</p>
            <p><strong>Customer Name:</strong> {parseShipping(detail.shippingAddress).fullName}</p>
            <p><strong>Phone:</strong> {parseShipping(detail.shippingAddress).phone}</p>
            <p><strong>Shipping Address:</strong> {parseShipping(detail.shippingAddress).address}</p>
            <hr style={{ margin: '1rem 0', borderColor: '#eee' }} />
            <p><strong>Status:</strong> <span className={`pill ${detail.status.toLowerCase()}`}>{detail.status}</span></p>
            <p><strong>Payment Status:</strong> <span className={`pill ${detail.paymentStatus.toLowerCase()}`}>{detail.paymentStatus}</span> {detail.paymentStatus === 'Refunded' ? '(Refund is processing)' : ''}</p>
            <p><strong>Total:</strong> {formatMoney(detail.totalAmount)}</p>
            <hr style={{ margin: '1rem 0', borderColor: '#eee' }} />
            
            {getNextStatus(detail.status) && (
              <div style={{ marginBottom: '1rem' }}>
                <p style={{ marginBottom: '0.5rem', fontWeight: 600 }}>Next Action:</p>
                <button 
                  className="btn btn-primary" 
                  onClick={() => updateStatus(detail.id, getNextStatus(detail.status))}
                >
                  Mark as {getNextStatus(detail.status)}
                </button>
              </div>
            )}
            
            {detail.status === 'WaitingConfirmation' && (
              <div style={{ marginBottom: '1rem' }}>
                <button 
                  className="btn" style={{ backgroundColor: '#fff0f0', color: '#d93025', border: '1px solid #fce8e6' }}
                  onClick={() => {
                    const reason = prompt("Enter cancellation reason:");
                    if (reason) orderService.cancel(detail.id, reason, isStaff).then(() => { showToast('Cancelled'); load(); setDetail(null); }).catch(() => showToast('Failed to cancel', 'error'));
                  }}
                >
                  Cancel Order
                </button>
              </div>
            )}
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
