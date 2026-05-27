import React, { useMemo } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { Leaf, LogOut, ShoppingCart, UserCircle2 } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

export const PublicLayout: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();
  const displayName = useMemo(() => user?.fullName || user?.email || 'User', [user]);

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
          </nav>

          <div className="public-auth-actions">
            <button type="button" className="public-icon-btn" aria-label="Cart">
              <ShoppingCart size={20} />
            </button>
            {isAuthenticated ? (
              <div className="public-user-box">
                <span className="public-user-name">Hi, {displayName}</span>
                <button type="button" className="public-login-btn" onClick={handleLogout}>
                  <LogOut size={16} />
                  Logout
                </button>
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

