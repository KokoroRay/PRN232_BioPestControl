import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { CheckoutSummary } from '../../components/public/CheckoutSummary';
import { LoadingState } from '../../components/admin/LoadingState';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { checkoutStorage } from '../../lib/checkoutStorage';
import { filterSelectedCartItems } from '../../lib/checkoutUtils';
import { profileService } from '../../services/profileService';

const PurchasePage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const selectedItems = searchParams.get('selectedItems') ?? '';
  const { isAuthenticated } = useAuth();
  const { cart, loading, refreshCart } = useCart();

  const [fullName, setFullName] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/checkout' } });
    }
  }, [isAuthenticated, navigate]);

  useEffect(() => {
    refreshCart();
  }, [refreshCart]);

  useEffect(() => {
    profileService
      .getProfile()
      .then((profile) => {
        setFullName(profile.fullName ?? '');
        setPhone(profile.phoneNumber ?? '');
      })
      .catch(() => {
        // optional prefill
      });
  }, []);

  const checkoutItems = useMemo(
    () => filterSelectedCartItems(cart?.items ?? [], selectedItems),
    [cart?.items, selectedItems],
  );

  useEffect(() => {
    if (loading) return;
    if (checkoutItems.length === 0) {
      navigate('/cart', { replace: true });
    }
  }, [loading, checkoutItems.length, navigate]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!fullName.trim() || !phone.trim() || !address.trim()) {
      setError('Please fill in all required fields.');
      return;
    }

    if (!/^0\d{9}$/.test(phone.trim())) {
      setError('Phone number must be 10 digits and start with 0.');
      return;
    }

    if (address.trim().length < 5) {
      setError('Please enter a full shipping address.');
      return;
    }

    setSubmitting(true);
    checkoutStorage.save({
      fullName: fullName.trim(),
      phone: phone.trim(),
      address: address.trim(),
      selectedItems,
    });
    navigate(`/checkout/payment?selectedItems=${encodeURIComponent(selectedItems)}`);
    setSubmitting(false);
  };

  if (!isAuthenticated) return null;

  return (
    <div className="checkout-page">
      <div className="public-container">
        <div className="checkout-stepper">
          <span className="checkout-step active">1 Shipping</span>
          <span className="checkout-step">2 Payment</span>
        </div>

        {loading ? (
          <LoadingState message="Loading checkout..." />
        ) : (
          <div className="checkout-layout">
            <div className="checkout-main">
              <h1>Shipping Address</h1>
              <p className="checkout-subtitle">Where should we send your order?</p>

              <form className="checkout-form" onSubmit={handleSubmit}>
                {error && <div className="checkout-error">{error}</div>}
                <label>
                  Full Name *
                  <input value={fullName} onChange={(e) => setFullName(e.target.value)} required />
                </label>
                <label>
                  Phone Number *
                  <input
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="0901234567"
                    required
                  />
                </label>
                <label>
                  Street Address *
                  <input
                    value={address}
                    onChange={(e) => setAddress(e.target.value)}
                    placeholder="123 Green Way, District 1, HCMC"
                    required
                  />
                </label>
                <div className="checkout-form-actions">
                  <Link to="/cart" className="checkout-back-link">
                    Back to Cart
                  </Link>
                  <button type="submit" className="public-cta" disabled={submitting}>
                    Continue to Payment
                  </button>
                </div>
              </form>
            </div>

            <aside className="checkout-aside">
              <CheckoutSummary
                items={checkoutItems}
                editCartHref={`/cart${selectedItems ? `?selected=${selectedItems}` : ''}`}
              />
            </aside>
          </div>
        )}
      </div>
    </div>
  );
};

export default PurchasePage;
