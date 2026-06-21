import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Minus, Plus, Trash2 } from 'lucide-react';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { LoadingState } from '../../components/admin/LoadingState';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { useToast } from '../../context/ToastContext';
import type { CartItem } from '../../types/trading';
import bgImage from '../../assets/Backgroud_1.2.png';

const SELECTED_KEY = 'cartSelectedItems';
const LAST_BUY_KEY = 'lastBuyProductId';

const formatPrice = (value: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const CartPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { cart, loading, updateQuantity, removeItem } = useCart();
  const { showToast } = useToast();

  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [removeTarget, setRemoveTarget] = useState<string | null>(null);
  const [busyItemId, setBusyItemId] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/cart' } });
    }
  }, [isAuthenticated, navigate]);

  useEffect(() => {
    if (!cart?.items.length) {
      setSelectedIds(new Set());
      return;
    }

    let initial = new Set(cart.items.map((item) => item.id));
    try {
      const stored = localStorage.getItem(SELECTED_KEY);
      if (stored !== null) {
        initial = new Set(
          stored
            .split(',')
            .map((id) => id.trim())
            .filter(Boolean),
        );
      }
      const lastBuy = localStorage.getItem(LAST_BUY_KEY);
      if (lastBuy) {
        const match = cart.items.find((item) => String(item.productId) === lastBuy);
        if (match) initial.add(match.id);
        localStorage.removeItem(LAST_BUY_KEY);
      }
    } catch {
      // ignore storage errors
    }
    setSelectedIds(initial);
  }, [cart?.items]);

  const items = cart?.items ?? [];

  const selectedTotal = useMemo(
    () =>
      items
        .filter((item) => selectedIds.has(item.id))
        .reduce((sum, item) => sum + item.subTotal, 0),
    [items, selectedIds],
  );

  const toggleSelect = (itemId: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(itemId)) next.delete(itemId);
      else next.add(itemId);
      try {
        localStorage.setItem(SELECTED_KEY, Array.from(next).join(','));
      } catch {
        // ignore
      }
      return next;
    });
  };

  const handleQuantityChange = async (item: CartItem, nextQty: number) => {
    if (nextQty < 1) return;
    setBusyItemId(item.id);
    const ok = await updateQuantity(item.id, nextQty);
    setBusyItemId(null);
    if (ok) {
      showToast('Cart updated');
    } else {
      showToast('Could not update quantity', 'error');
    }
  };

  const handleRemove = async () => {
    if (!removeTarget) return;
    setBusyItemId(removeTarget);
    const ok = await removeItem(removeTarget);
    setBusyItemId(null);
    setRemoveTarget(null);
    if (ok) {
      showToast('Item removed from cart');
      setSelectedIds((prev) => {
        const next = new Set(prev);
        next.delete(removeTarget);
        try {
          localStorage.setItem(SELECTED_KEY, Array.from(next).join(','));
        } catch {
          // ignore
        }
        return next;
      });
    } else {
      showToast('Could not remove item', 'error');
    }
  };

  if (!isAuthenticated) return null;

  return (
    <div className="cart-page">
      <div className="public-container">
        <nav className="cart-breadcrumbs">
          <Link to="/">Home</Link>
          <span>/</span>
          <span>Shopping Cart</span>
        </nav>

        <div className="cart-page-header">
          <div>
            <h1>Your Cart</h1>
            <p>
              {items.length === 0
                ? 'No items in your cart yet'
                : `${items.length} product${items.length > 1 ? 's' : ''} in your cart`}
            </p>
          </div>
          <Link to="/products" className="cart-continue-link">
            Continue Shopping
          </Link>
        </div>

        {loading ? (
          <LoadingState message="Loading cart..." />
        ) : (
          <div className="cart-layout">
            <div className="cart-items-column">
              {items.length === 0 ? (
                <div className="cart-empty">
                  <p className="cart-empty-title">Your cart is empty</p>
                  <p>Browse products and add items to your cart.</p>
                  <Link to="/products" className="public-cta">
                    Browse Products
                  </Link>
                </div>
              ) : (
                items.map((item) => {
                  const selected = selectedIds.has(item.id);
                  const busy = busyItemId === item.id;
                  return (
                    <article key={item.id} className="cart-item-card">
                      <div className="cart-item-main">
                        <button
                          type="button"
                          className={`cart-select-btn ${selected ? 'selected' : ''}`}
                          onClick={() => toggleSelect(item.id)}
                          aria-pressed={selected}
                        >
                          {selected ? '✓' : ''}
                        </button>
                        <div
                          className="cart-item-image"
                          style={{
                            backgroundImage: `url(${item.productImageUrl || bgImage})`,
                          }}
                        />
                        <div className="cart-item-info">
                          <Link to={`/products/${item.productId}`} className="cart-item-name">
                            {item.productName}
                          </Link>
                          <p>Unit price: {formatPrice(item.unitPrice)}</p>
                        </div>
                      </div>

                      <div className="cart-item-actions">
                        <div className="cart-qty-controls">
                          <button
                            type="button"
                            disabled={busy || item.quantity <= 1}
                            onClick={() => handleQuantityChange(item, item.quantity - 1)}
                          >
                            <Minus size={14} />
                          </button>
                          <span>{item.quantity}</span>
                          <button
                            type="button"
                            disabled={busy}
                            onClick={() => handleQuantityChange(item, item.quantity + 1)}
                          >
                            <Plus size={14} />
                          </button>
                        </div>
                        <strong className="cart-item-total">{formatPrice(item.subTotal)}</strong>
                        <button
                          type="button"
                          className="cart-remove-btn"
                          disabled={busy}
                          onClick={() => setRemoveTarget(item.id)}
                          aria-label="Remove item"
                        >
                          <Trash2 size={18} />
                        </button>
                      </div>
                    </article>
                  );
                })
              )}
            </div>

            <aside className="cart-summary">
              <div className="cart-summary-card">
                <h3>Order Summary</h3>
                <div className="cart-summary-rows">
                  <div>
                    <span>Subtotal</span>
                    <strong>{formatPrice(selectedTotal)}</strong>
                  </div>
                  <div>
                    <span>Shipping</span>
                    <strong>Calculated at checkout</strong>
                  </div>
                  <div>
                    <span>Tax</span>
                    <strong>{formatPrice(0)}</strong>
                  </div>
                </div>
                <div className="cart-summary-total">
                  <span>Total</span>
                  <strong>{formatPrice(selectedTotal)}</strong>
                </div>
                <button
                  type="button"
                  className="public-cta cart-checkout-btn"
                  disabled={selectedIds.size === 0}
                  onClick={() => {
                    const selected = Array.from(selectedIds).join(',');
                    navigate(`/checkout?selectedItems=${encodeURIComponent(selected)}`);
                  }}
                >
                  Proceed to Checkout
                </button>
              </div>
              <div className="cart-trust-box">
                <h4>Guaranteed Live Delivery</h4>
                <p>
                  Biological products are handled with care. Report any issues within 24 hours of
                  delivery.
                </p>
              </div>
            </aside>
          </div>
        )}
      </div>

      <ConfirmModal
        open={removeTarget != null}
        title="Remove item"
        message="Are you sure you want to remove this item from your cart?"
        confirmLabel="Remove"
        danger
        onConfirm={handleRemove}
        onCancel={() => setRemoveTarget(null)}
      />
    </div>
  );
};

export default CartPage;
