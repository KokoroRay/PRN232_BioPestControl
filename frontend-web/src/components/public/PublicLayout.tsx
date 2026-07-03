import React, { useMemo } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { Leaf, LogOut, ShoppingCart, UserCircle2 } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';

export const PublicLayout: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const { itemCount, loading } = useCart();
  const navigate = useNavigate();
  const displayName = useMemo(() => user?.fullName || user?.email?.split('@')[0] || 'User', [user]);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="public-layout">
      <header className="public-header">
        <div className="public-container public-header-inner">
          <Link to="/" className="public-brand">
            <span className="public-brand-icon">
              <Leaf size={18} />
            </span>
            <span>BioPestControl</span>
          </Link>

          <nav className="public-nav">
            <Link to="/">Home</Link>
            <Link to="/products">Products</Link>
            <Link to="/about">About</Link>
            <Link to="/contact">Contact</Link>
            <Link to="/articles">News/Article</Link>
            <Link to="/agri-calculations">Agri Calculators</Link>
          </nav>

          <div className="public-auth-actions">
            <Link to="/cart" className="public-cart-link" aria-label="Cart">
              <ShoppingCart size={20} />
              {!loading && itemCount > 0 && <span className="public-cart-badge">{itemCount}</span>}
            </Link>
            {isAuthenticated ? (
              <div className="public-user-dropdown-container">
                <div className="public-user-avatar">
                  {user?.avatarUrl ? (
                    <img src={user.avatarUrl} alt={displayName} />
                  ) : (
                    <UserCircle2 size={24} />
                  )}
                </div>
                <div className="public-user-dropdown-menu">
                  <div className="public-dropdown-header">
                    <span className="public-dropdown-name">{displayName}</span>
                    <span className="public-dropdown-email">{user?.email}</span>
                  </div>
                  <hr className="public-dropdown-divider" />
                  <Link to="/orders" className="public-dropdown-item">
                    Orders
                  </Link>
                  <Link to="/account/profile" className="public-dropdown-item">
                    Profile
                  </Link>
                  <button type="button" className="public-dropdown-item public-dropdown-logout" onClick={handleLogout}>
                    <LogOut size={16} />
                    <span>Logout</span>
                  </button>
                </div>
              </div>
            ) : (
              <Link to="/login" className="public-login-btn">
                <UserCircle2 size={16} />
                Login
              </Link>
            )}
          </div>
        </div>
      </header>

      <main>
        <Outlet />
      </main>

      <footer id="contact" className="public-footer">
        <div className="public-container">
          <p>&copy; 2026 BioPestControl. All rights reserved.</p>
          <p className="public-footer-links">
            <a href="#top">Privacy Policy</a>
            <span>|</span>
            <a href="#top">Terms of Service</a>
            <span>|</span>
            <a href="#top">Contact Us</a>
          </p>
        </div>
      </footer>
    </div>
  );
};

