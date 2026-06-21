import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { CheckoutSummary } from '../../components/public/CheckoutSummary';
import { LoadingState } from '../../components/admin/LoadingState';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { useToast } from '../../context/ToastContext';
import { checkoutStorage } from '../../lib/checkoutStorage';
import { buildShippingAddress, filterSelectedCartItems } from '../../lib/checkoutUtils';
import { orderService } from '../../services/orderService';

type PaymentMethod = 'COD' | 'PayOS';

const PaymentPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const selectedItems = searchParams.get('selectedItems') ?? '';
  const { isAuthenticated } = useAuth();
  const { cart, loading, refreshCart, removeItem } = useCart();
  const { showToast } = useToast();

  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('COD');
  const [submitting, setSubmitting] = useState(false);
  const shipping = checkoutStorage.load();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/checkout/payment' } });
      return;
    }
    if (!shipping) {
      navigate(`/checkout?selectedItems=${encodeURIComponent(selectedItems)}`, { replace: true });
    }
  }, [isAuthenticated, navigate, shipping, selectedItems]);

  useEffect(() => {
    refreshCart();
  }, [refreshCart]);

  const checkoutItems = useMemo(
    () => filterSelectedCartItems(cart?.items ?? [], selectedItems),
    [cart?.items, selectedItems],
  );

  const prepareCartForOrder = async () => {
    if (!cart?.items.length) return;
    const selectedSet = new Set(checkoutItems.map((item) => item.id));
    const toRemove = cart.items.filter((item) => !selectedSet.has(item.id));
    await Promise.all(toRemove.map((item) => removeItem(item.id)));
  };

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!shipping) return;

    setSubmitting(true);
    try {
      await prepareCartForOrder();
      const order = await orderService.placeOrder({
        shippingAddress: buildShippingAddress(shipping.fullName, shipping.phone, shipping.address),
        paymentMethod,
      });

      checkoutStorage.clear();
      await refreshCart();

      if (paymentMethod === 'PayOS') {
        showToast('PayOS online payment is not configured yet. Order was created as unpaid.', 'error');
      } else {
        showToast('Order placed successfully!');
      }

      navigate(`/orders/${order.id}`, { replace: true });
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Could not place order. Please try again.';
      showToast(message, 'error');
    } finally {
      setSubmitting(false);
    }
  };

  if (!isAuthenticated || !shipping) return null;

  return (
    <div className="checkout-page">
      <div className="public-container">
        <div className="checkout-stepper">
          <Link to={`/checkout?selectedItems=${encodeURIComponent(selectedItems)}`} className="checkout-step done">
            1 Shipping
          </Link>
          <span className="checkout-step active">2 Payment</span>
        </div>

        {loading ? (
          <LoadingState message="Loading payment..." />
        ) : (
          <div className="checkout-layout">
            <div className="checkout-main">
              <h1>Secure Checkout</h1>
              <p className="checkout-subtitle">Step 2 of 2: choose payment method</p>

              <div className="checkout-shipping-recap">
                <strong>{shipping.fullName}</strong>
                <span>{shipping.phone}</span>
                <span>{shipping.address}</span>
              </div>

              <form className="checkout-form" onSubmit={handlePlaceOrder}>
                <div className="payment-options">
                  <label className={`payment-option ${paymentMethod === 'COD' ? 'selected' : ''}`}>
                    <input
                      type="radio"
                      name="paymentMethod"
                      value="COD"
                      checked={paymentMethod === 'COD'}
                      onChange={() => setPaymentMethod('COD')}
                    />
                    <div>
                      <strong>Cash on Delivery (COD)</strong>
                      <p>Pay in cash when you receive your order.</p>
                    </div>
                  </label>
                  <label className={`payment-option ${paymentMethod === 'PayOS' ? 'selected' : ''}`}>
                    <input
                      type="radio"
                      name="paymentMethod"
                      value="PayOS"
                      checked={paymentMethod === 'PayOS'}
                      onChange={() => setPaymentMethod('PayOS')}
                    />
                    <div>
                      <strong>PayOS (Online)</strong>
                      <p>Online payment via PayOS — redirect requires payment-service setup.</p>
                    </div>
                  </label>
                </div>

                <div className="checkout-form-actions">
                  <Link
                    to={`/checkout?selectedItems=${encodeURIComponent(selectedItems)}`}
                    className="checkout-back-link"
                  >
                    Back to Shipping
                  </Link>
                  <button type="submit" className="public-cta" disabled={submitting}>
                    {submitting ? 'Placing order...' : 'Place Order'}
                  </button>
                </div>
              </form>
            </div>

            <aside className="checkout-aside">
              <CheckoutSummary items={checkoutItems} selectedItems={selectedItems} />
            </aside>
          </div>
        )}
      </div>
    </div>
  );
};

export default PaymentPage;
