import React from 'react';
import { Link } from 'react-router-dom';
import type { CartItem } from '../../types/trading';
import bgImage from '../../assets/Backgroud_1.2.png';

const formatPrice = (value: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

interface CheckoutSummaryProps {
  items: CartItem[];
  editCartHref?: string;
  selectedItems?: string;
}

export const CheckoutSummary: React.FC<CheckoutSummaryProps> = ({
  items,
  editCartHref,
  selectedItems,
}) => {
  const total = items.reduce((sum, item) => sum + item.subTotal, 0);
  const defaultHref = selectedItems
    ? `/cart?selected=${selectedItems}`
    : '/cart';

  return (
    <div className="checkout-summary-card">
      <div className="checkout-summary-header">
        <h3>Order Summary</h3>
        <Link to={editCartHref ?? defaultHref} className="checkout-summary-edit">
          Edit Cart
        </Link>
      </div>
      <ul className="checkout-summary-list">        {items.map((item) => (
          <li key={item.id}>
            <div
              className="checkout-summary-thumb"
              style={{ backgroundImage: `url(${item.productImageUrl || bgImage})` }}
            />
            <div className="checkout-summary-meta">
              <strong>{item.productName}</strong>
              <span>Qty {item.quantity}</span>
              <span>{formatPrice(item.unitPrice)} each</span>
            </div>
            <strong>{formatPrice(item.subTotal)}</strong>
          </li>
        ))}
      </ul>
      <div className="checkout-summary-rows">
        <div>
          <span>Subtotal</span>
          <strong>{formatPrice(total)}</strong>
        </div>
        <div>
          <span>Shipping</span>
          <strong>Calculated at review</strong>
        </div>
        <div>
          <span>Tax</span>
          <strong>{formatPrice(0)}</strong>
        </div>
      </div>
      <div className="checkout-summary-total">
        <span>Total</span>
        <strong>{formatPrice(total)}</strong>
      </div>
    </div>
  );
};
