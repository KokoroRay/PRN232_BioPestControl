import React, { useMemo } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { Leaf, LogOut, ShoppingCart, UserCircle2, Globe } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';
import { AIAssistantWidget } from '../ai/AIAssistantWidget';
import { useTranslation } from 'react-i18next';
import { cropService } from '../../services/cropService';
import type { CropResponse } from '../../services/cropService';

export const PublicLayout: React.FC = () => {
  const { t, i18n } = useTranslation();
  const { isAuthenticated, user, logout } = useAuth();
  const { itemCount, loading } = useCart();
  const navigate = useNavigate();
  const displayName = useMemo(() => user?.fullName || user?.email?.split('@')[0] || 'User', [user]);

  const [crops, setCrops] = React.useState<CropResponse[]>([]);

  React.useEffect(() => {
    cropService.getAllCrops().then(setCrops).catch(console.error);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const toggleLanguage = () => {
    const nextLng = i18n.language === 'vi' ? 'en' : 'vi';
    i18n.changeLanguage(nextLng);
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
            <Link to="/">{t('home', 'Home')}</Link>
            <Link to="/products">{t('products', 'Products')}</Link>

            <div className="public-nav-dropdown">
              <Link to="/crops" className="public-nav-dropdown-btn">{t('crops', 'Crops')}</Link>
              <div className="public-nav-dropdown-menu">
                {crops.map(c => (
                  <Link key={c.id} to={`/crops/${c.slug}`} className="public-dropdown-item">
                    {c.name}
                  </Link>
                ))}
              </div>
            </div>

            <Link to="/about">{t('about', 'About')}</Link>
            <Link to="/contact">{t('contact', 'Contact')}</Link>
            <Link to="/articles">{t('news', 'News/Article')}</Link>
          </nav>

          <div className="public-auth-actions">
            <button
              type="button"
              onClick={toggleLanguage}
              className="public-cart-link"
              title={t('language', 'Language')}
              style={{ background: 'none', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px', fontSize: '14px', fontWeight: 500, color: 'var(--text-color)' }}
            >
              <Globe size={20} />
              {i18n.language === 'vi' ? 'VI' : 'EN'}
            </button>
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
                  <button type="button" className="public-dropdown-item" onClick={() => window.dispatchEvent(new Event('open-ai-chat'))} style={{ border: 'none', background: 'none', width: '100%', textAlign: 'left', fontFamily: 'inherit', fontSize: 'inherit', cursor: 'pointer', padding: '0.5rem 1rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                    <span>{t('aiAssistant', 'AI Assistant')}</span>
                  </button>
                  <Link to="/agri-calculations" className="public-dropdown-item">
                    {t('agriCalculators', 'Agri Calculators')}
                  </Link>
                  <Link to="/orders" className="public-dropdown-item">
                    {t('orders', 'Orders')}
                  </Link>
                  <Link to="/account/profile" className="public-dropdown-item">
                    {t('profile', 'Profile')}
                  </Link>
                  <button type="button" className="public-dropdown-item public-dropdown-logout" onClick={handleLogout}>
                    <LogOut size={16} />
                    <span>{t('logout', 'Logout')}</span>
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

      <AIAssistantWidget />
    </div>
  );
};
