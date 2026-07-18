import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Loader2 } from 'lucide-react';
import { AccountSidebar } from '../../components/public/AccountSidebar';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { orderStatusLabel, parseShippingAddress } from '../../lib/checkoutUtils';
import { orderService } from '../../services/orderService';
import type { Order } from '../../types/ordering';
import { useCart } from '../../context/CartContext';

const formatPrice = (value: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const CustomerOrderDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { showToast } = useToast();
  const { addToCart } = useCart();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: `/orders/${id}` } });
      return;
    }
    if (!id) return;

    orderService
      .getMyOrder(id)
      .then(setOrder)
      .catch(() => {
        showToast('Failed to load order', 'error');
        setLoading(false);
      })
      .finally(() => setLoading(false));
  }, [id, isAuthenticated, navigate, showToast]);

  if (!isAuthenticated) return null;

  const shipping = order ? parseShippingAddress(order.shippingAddress) : null;

  return (
    <div className="orders-page">
      <div className="orders-container">
        <aside className="orders-sidebar-col">
          <AccountSidebar active="orders" />
        </aside>

        <main className="orders-main">
          <Link to="/orders" className="order-detail-back">
            <ArrowLeft size={18} />
            Back to Orders
          </Link>

          {loading ? (
            <div className="orders-loading">
              <Loader2 size={32} className="spin" />
              <p>Loading order…</p>
            </div>
          ) : !order ? (
            <div className="orders-empty-card">
              <p className="orders-empty-title">Order not found</p>
            </div>
          ) : (
            <div className="order-detail-card">
              {/* Header */}
              <div className="order-detail-header">
                <h1>Order #{order.id.slice(0, 8)}…</h1>
                <p className="order-detail-date">
                  {new Date(order.orderDate).toLocaleDateString('en-US', {
                    month: 'long', day: 'numeric', year: 'numeric',
                  })}
                  {' · '}
                  {orderStatusLabel(order.status)}
                </p>
                <div className="order-detail-info-grid">
                  {shipping && (
                    <>
                      <p>
                        <span className="order-detail-label">Receiver:</span> {shipping.fullName || '—'}
                      </p>
                      <p>
                        <span className="order-detail-label">Phone:</span> {shipping.phone || '—'}
                      </p>
                      <p className="order-detail-address">
                        <span className="order-detail-label">Address:</span> {shipping.address || order.shippingAddress}
                      </p>
                    </>
                  )}
                </div>
              </div>

              {/* Items */}
              <div className="order-detail-items-section">
                <h2 className="order-detail-section-title">Items</h2>
                <ul className="order-detail-items">
                  {order.items.map((item) => (
                    <li key={item.id} className="order-detail-item">
                      <div className="order-detail-item-thumb">
                        {item.productImageUrl ? (
                          <img src={item.productImageUrl} alt={item.productName} />
                        ) : (
                          <span className="material-symbols-outlined">inventory_2</span>
                        )}
                      </div>
                      <div className="order-detail-item-info">
                        <p className="order-detail-item-name">{item.productName}</p>
                        <p className="order-detail-item-price">
                          Qty {item.quantity} × {formatPrice(item.unitPrice)}
                        </p>
                      </div>
                      <strong className="order-detail-item-total">
                        {formatPrice(item.quantity * item.unitPrice)}
                      </strong>
                    </li>
                  ))}
                </ul>
              </div>

              {/* Footer */}
              <div className="order-detail-footer">
                <div className="order-detail-total-block">
                  <p className="order-detail-label">Order Total</p>
                  <p className="order-detail-grand-total">{formatPrice(order.totalAmount)}</p>
                </div>
                <button 
                  type="button"
                  className="public-cta"
                  onClick={async () => {
                    if (!order) return;
                    try {
                      await Promise.all(order.items.map(item => 
                        addToCart({
                          id: item.productId,
                          name: item.productName,
                          unitPrice: item.unitPrice,
                          imageUrl: item.productImageUrl || ''
                        }, item.quantity)
                      ));
                      navigate('/cart');
                    } catch {
                      showToast('Could not add items to cart', 'error');
                    }
                  }}
                >
                  Buy Again
                </button>
              </div>
            </div>
          )}
        </main>
      </div>
    </div>
  );
};

export default CustomerOrderDetailsPage;
