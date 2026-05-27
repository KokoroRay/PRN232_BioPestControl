import React, { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Eye, EyeOff, Leaf, Lock, Mail, type LucideIcon } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import bgImage from '../assets/Background_1.png';
import '../styles/login-admin.css';

export interface PortalLoginConfig {
  variant: 'admin' | 'staff';
  title: string;
  subtitle: string;
  allowedRoles: string[];
  successRole: string;
  defaultRedirect: string;
  emailPlaceholder: string;
  otherPortal: { prompt: string; to: string; label: string };
  BrandIcon: LucideIcon;
}

interface PortalLoginPageProps {
  config: PortalLoginConfig;
}

export const PortalLoginPage: React.FC<PortalLoginPageProps> = ({ config }) => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [remember, setRemember] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const idPrefix = config.variant;
  const { BrandIcon } = config;

  const from =
    (location.state as { from?: { pathname: string } })?.from?.pathname ?? config.defaultRedirect;

  useEffect(() => {
    const saved = localStorage.getItem('remember_email');
    if (saved) {
      setEmail(saved);
      setRemember(true);
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const role = await login({ email, password }, { allowedRoles: config.allowedRoles });
      if (remember) localStorage.setItem('remember_email', email);
      else localStorage.removeItem('remember_email');
      navigate(role === config.successRole ? from : config.defaultRedirect, { replace: true });
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        (err instanceof Error ? err.message : 'Login failed');
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={`admin-login admin-login--${config.variant}`}>
      <div className="admin-login-bg" style={{ backgroundImage: `url(${bgImage})` }} aria-hidden />
      <div className="admin-login-bg-overlay" aria-hidden />

      <header className="admin-login-header">
        <div className="admin-login-header-brand">
          <Leaf size={24} strokeWidth={2.5} />
          <span>BioPestControl</span>
        </div>
      </header>

      <main className="admin-login-main">
        <div className="admin-login-glass">
          <div className="admin-login-glass-brand">
            <div className="admin-login-glass-icon">
              <BrandIcon size={26} strokeWidth={2.5} />
            </div>
            <h1>{config.title}</h1>
            <p>{config.subtitle}</p>
          </div>

          {error ? <div className="admin-login-error">{error}</div> : null}

          <form onSubmit={handleSubmit} className="admin-login-form">
            <div className="admin-login-field">
              <label htmlFor={`${idPrefix}-email`}>Email Address</label>
              <div className="admin-login-input-wrap">
                <Mail />
                <input
                  id={`${idPrefix}-email`}
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder={config.emailPlaceholder}
                  autoComplete="email"
                  required
                />
              </div>
            </div>

            <div className="admin-login-field">
              <label htmlFor={`${idPrefix}-password`}>Password</label>
              <div className="admin-login-input-wrap admin-login-input-wrap--password">
                <Lock />
                <input
                  id={`${idPrefix}-password`}
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Enter password"
                  autoComplete="current-password"
                  required
                />
                <button
                  type="button"
                  className="admin-login-toggle-pw"
                  onClick={() => setShowPassword((v) => !v)}
                  aria-label={showPassword ? 'Hide password' : 'Show password'}
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
            </div>

            <label className="admin-login-remember">
              <input
                type="checkbox"
                checked={remember}
                onChange={(e) => setRemember(e.target.checked)}
              />
              Remember me
            </label>

            <button type="submit" className="admin-login-submit" disabled={loading}>
              {loading ? 'Signing in...' : 'Login'}
            </button>
          </form>

          <p className="admin-login-staff-link">
            {config.otherPortal.prompt}
            <Link to={config.otherPortal.to}>{config.otherPortal.label}</Link>
          </p>
        </div>
      </main>

      <footer className="admin-login-footer">
        <span>{'\u00A9'} 2026 BioPestControl. Scientific Botanical Harmony.</span>
      </footer>
    </div>
  );
};
