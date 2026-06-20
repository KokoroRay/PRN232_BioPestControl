import React, { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { Package, Truck, CheckCircle, Clock, XCircle, Search, Loader2 } from 'lucide-react';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { AccountSidebar } from '../../components/public/AccountSidebar';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { orderStatusLabel } from '../../lib/checkoutUtils';
import { orderService } from '../../services/orderService';
import type { Order } from '../../types/ordering';

const STATUS_TABS = [
  { label: 'All', value: '' },
  { label: 'Pending', value: 'WaitingConfirmation', icon: Clock },
  { label: 'Confirmed', value: 'Confirmed', icon: Package },
  { label: 'Processed', value: 'Processing', icon: Package },
  { label: 'Shipped', value: 'Shipping', icon: Truck },
  { label: 'Delivered', value: 'Delivered', icon: CheckCircle },
  { label: 'Cancelled', value: 'Cancelled', icon: XCircle },
];

const STATUS_COLORS: Record<string, string> = {
  Delivered: 'text-teal-600',
  WaitingConfirmation: 'text-amber-600',
  Confirmed: 'text-blue-600',
  Processing: 'text-blue-600',
  Shipping: 'text-indigo-600',
  Cancelled: 'text-red-600',
};

const BORDER_COLORS: Record<string, string> = {
  Delivered: 'border-teal-500',
  WaitingConfirmation: 'border-amber-500',
  Confirmed: 'border-blue-500',
  Processing: 'border-blue-500',
  Shipping: 'border-indigo-500',
  Cancelled: 'border-red-500',
};

const formatPrice = (value: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

interface Counts {
  all: number;
  [key: string]: number;
}

const CustomerOrdersPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const { isAuthenticated } = useAuth();
  const { showToast } = useToast();

  const [orders, setOrders] = useState<Order[]>([]);
  const [counts, setCounts] = useState<Counts>({ all: 0 });
  const [loading, setLoading] = useState(true);
  const [cancelId, setCancelId] = useState<string | null>(null);

  const status = searchParams.get('status') ?? '';
  const search = searchParams.get('search') ?? '';
  const page = Number(searchParams.get('page') ?? '1');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [result, allResult] = await Promise.all([
        orderService.getMyOrders({ status: status || undefined, search: search || undefined, page, pageSize: 10 }),
        orderService.getMyOrders({ pageSize: 1 }),
      ]);
      setOrders(result.items);
      setCounts((prev) => ({ ...prev, all: allResult.totalCount }));
    } catch {
      showToast('Failed to load orders', 'error');
    } finally {
      setLoading(false);
    }
  }, [status, search, page, showToast]);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/orders' } });
      return;
    }
    load();
  }, [isAuthenticated, navigate, load]);

  const handleCancel = async () => {
    if (!cancelId) return;
    try {
      await orderService.cancelMyOrder(cancelId);
      showToast('Order cancelled');
      setCancelId(null);
      load();
    } catch {
      showToast('Could not cancel order', 'error');
    }
  };

  const isPending = (s: string) => s === 'WaitingConfirmation';
  const isDelivered = (s: string) => s === 'Delivered';
  const isCancelled = (s: string) => s === 'Cancelled';
  const isFinished = (s: string) => isDelivered(s) || isCancelled(s);

  return (
    <div className="orders-page">
      <div className="orders-container">
        <aside className="orders-sidebar-col">
          <AccountSidebar active="orders" />
        </aside>

        <main className="orders-main">
          {/* Tab bar */}
          <div className="orders-tab-bar">
            {STATUS_TABS.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.value}
                  type="button"
                  className={`orders-tab-btn ${status === tab.value ? 'active' : ''}`}
                  onClick={() => {
                    const params = new URLSearchParams(searchParams);
                    if (tab.value) params.set('status', tab.value);
                    else params.delete('status');
                    params.delete('page');
                    setSearchParams(params);
                  }}
                >
                  {Icon && <Icon size={13} />}
                  {tab.label}
                  {counts.all > 0 && (
                    <span className={`orders-tab-badge ${status === tab.value ? 'active-badge' : ''}`}>
                      {tab.value === '' ? counts.all : 0}
                    </span>
                  )}
                </button>
              );
            })}
          </div>

          {/* Search */}
          <form
            className="orders-search-bar"
            onSubmit={(e) => {
              e.preventDefault();
              const data = new FormData(e.currentTarget);
              const params = new URLSearchParams(searchParams);
              const q = String(data.get('search') ?? '').trim();
              if (q) params.set('search', q);
              else params.delete('search');
              params.delete('page');
              setSearchParams(params);
            }}
          >
            <Search size={16} className="orders-search-icon" />
            <input
              name="search"
              defaultValue={search}
              placeholder="Search by order ID or product name"
            />
          </form>

          {/* Order list */}
          {loading ? (
            <div className="orders-loading">
              <Loader2 size={32} className="spin" />
              <p>Loading orders…</p>
            </div>
          ) : orders.length === 0 ? (
            <div className="orders-empty-card">
              <p className="orders-empty-title">No orders found</p>
              <Link to="/products" className="public-cta">
                Browse Products
              </Link>
            </div>
          ) : (
            <div className="orders-cards">
              {orders.map((order) => {
                const borderColor = BORDER_COLORS[order.status] ?? 'border-primary';
                const statusColor = STATUS_COLORS[order.status] ?? 'text-primary';
                const Icon = STATUS_TABS.find((t) => t.value === order.status)?.icon ?? Package;

                return (
                  <div
                    key={order.id}
                    className={`orders-card ${borderColor}`}
                  >
                    <div className="orders-card-header">
                      <span className="orders-card-id">Order #{order.id.slice(0, 8)}…</span>
                      <span className={`orders-card-status ${statusColor}`}>
                        <Icon size={13} />
                        {orderStatusLabel(order.status)}
                      </span>
                    </div>

                    <div className="orders-card-body">
                      <div className="orders-card-product">
                        <div className="orders-card-thumb">
                          <span className="material-symbols-outlined">inventory_2</span>
                        </div>
                        <div className="orders-card-meta">
                          <p className={`orders-card-items ${isCancelled(order.status) ? 'orders-card-cancelled' : ''}`}>
                            {order.items.length} item{order.items.length !== 1 ? 's' : ''}
                          </p>
                          <p className="orders-card-date">
                            {new Date(order.orderDate).toLocaleDateString('en-US', {
                              month: 'short', day: 'numeric', year: 'numeric',
                            })}
                          </p>
                        </div>
                        <Link to={`/orders/${order.id}`} className="orders-card-details-link">
                          View details <span className="material-symbols-outlined">keyboard_arrow_right</span>
                        </Link>
                      </div>
                    </div>

                    <div className="orders-card-footer">
                      <div className="orders-card-total">
                        <span>Order Total:</span>
                        <strong className={isCancelled(order.status) ? 'text-gray-400' : ''}>
                          {formatPrice(order.totalAmount)}
                        </strong>
                      </div>
                      <div className="orders-card-actions">
                        {isFinished(order.status) ? (
                          <>
                            <Link to="/products" className="orders-action-secondary">Buy Again</Link>
                            <Link to={`/orders/${order.id}`} className="orders-action-secondary">Details</Link>
                            {isDelivered(order.status) && (
                              <Link to={`/orders/${order.id}`} className="orders-action-feedback">Feedback</Link>
                            )}
                          </>
                        ) : (
                          <>
                            <Link to={`/orders/${order.id}`} className="orders-action-secondary">Details</Link>
                            {isPending(order.status) && (
                              <button
                                type="button"
                                className="orders-action-cancel"
                                onClick={() => setCancelId(order.id)}
                              >
                                Cancel Order
                              </button>
                            )}
                          </>
                        )}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {/* Pagination */}
          {orders.length > 0 && page > 1 && (
            <div className="orders-pagination">
              <button
                type="button"
                className="orders-page-btn"
                onClick={() => {
                  const params = new URLSearchParams(searchParams);
                  params.set('page', String(page - 1));
                  setSearchParams(params);
                }}
              >
                <span className="material-symbols-outlined">chevron_left</span>
              </button>
              <span className="orders-page-current">Page {page}</span>
              <button
                type="button"
                className="orders-page-btn"
                onClick={() => {
                  const params = new URLSearchParams(searchParams);
                  params.set('page', String(page + 1));
                  setSearchParams(params);
                }}
              >
                <span className="material-symbols-outlined">chevron_right</span>
              </button>
            </div>
          )}
        </main>
      </div>

      <ConfirmModal
        open={cancelId != null}
        title="Cancel Order?"
        message="Are you sure you want to cancel this order? This action cannot be undone."
        confirmLabel="Yes, Cancel Order"
        danger
        onConfirm={handleCancel}
        onCancel={() => setCancelId(null)}
      />
    </div>
  );
};

export default CustomerOrdersPage;
