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
  const [provinces, setProvinces] = useState<{ Code: string; FullName: string; Wards: { Code: string; FullName: string }[] }[]>([]);
  const [wards, setWards] = useState<{ Code: string; FullName: string }[]>([]);

  const [selectedProvince, setSelectedProvince] = useState<{ Code: string; FullName: string } | null>(null);
  const [selectedWard, setSelectedWard] = useState<{ Code: string; FullName: string } | null>(null);
  const [streetAddress, setStreetAddress] = useState('');
  
  const [useProfileAddress, setUseProfileAddress] = useState(false);
  const [profileAddress, setProfileAddress] = useState('');
  const [streetAddress, setStreetAddress] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch('/data/vietnam_provinces.json')
      .then((res) => res.json())
      .then((data) => setProvinces(data))
      .catch(() => setError('Failed to load provinces.'));
  }, []);

  const handleProvinceChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const prov = provinces.find((p) => p.Code === code) || null;
    setSelectedProvince(prov);
    setSelectedWard(null);
    setWards(prov ? prov.Wards : []);
  };

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
        if (profile.address) {
          setProfileAddress(profile.address);
          setUseProfileAddress(true);
        }
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

    if (!useProfileAddress && (!streetAddress.trim() || !selectedProvince || !selectedWard)) {
      setError('Please fill in all required fields.');
      return;
    }

    if (!/^0\d{9}$/.test(phone.trim())) {
      setError('Phone number must be 10 digits and start with 0.');
      return;
    }

    const fullAddress = useProfileAddress 
      ? profileAddress 
      : `${streetAddress.trim()}, ${selectedWard!.FullName}, ${selectedProvince!.FullName}`;

    setSubmitting(true);
    checkoutStorage.save({
      fullName: fullName.trim(),
      phone: phone.trim(),
      address: fullAddress,
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
                {profileAddress && (
                  <div className="checkout-address-toggle" style={{ marginBottom: '1rem' }}>
                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                      <input 
                        type="checkbox" 
                        checked={useProfileAddress} 
                        onChange={(e) => setUseProfileAddress(e.target.checked)} 
                      />
                      Sử dụng địa chỉ mặc định từ Profile: <strong>{profileAddress}</strong>
                    </label>
                  </div>
                )}
                
                {!useProfileAddress && (
                  <>
                    <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                      <label style={{ flex: 1, marginBottom: 0 }}>
                        Province/City *
                        <select required value={selectedProvince?.Code || ''} onChange={handleProvinceChange}>
                          <option value="">Select Province</option>
                          {provinces.map((p) => (
                            <option key={p.Code} value={p.Code}>{p.FullName}</option>
                          ))}
                        </select>
                      </label>
                      <label style={{ flex: 1, marginBottom: 0 }}>
                        Ward/Commune *
                        <select required value={selectedWard?.Code || ''} onChange={(e) => setSelectedWard(wards.find((w) => w.Code === e.target.value) || null)} disabled={!selectedProvince}>
                          <option value="">Select Ward</option>
                          {wards.map((w) => (
                            <option key={w.Code} value={w.Code}>{w.FullName}</option>
                          ))}
                        </select>
                      </label>
                    </div>
                    <label>
                      Street Address *
                      <input
                        value={streetAddress}
                        onChange={(e) => setStreetAddress(e.target.value)}
                        placeholder="123 Green Way"
                        required
                      />
                    </label>
                  </>
                )}
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
