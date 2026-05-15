import React, { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { UserCog } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

const StaffLogin: React.FC = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [remember, setRemember] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const from =
    (location.state as { from?: { pathname: string } })?.from?.pathname ?? '/staff/dashboard';

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const role = await login({ email, password }, { allowedRoles: ['Staff'] });
      if (remember) localStorage.setItem('remember_email', email);
      navigate(role === 'Staff' ? from : '/staff/dashboard', { replace: true });
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
    <div className="login-page login-page-staff">
      <div className="login-card">
        <div className="login-accent login-accent-staff" />
        <div className="login-brand">
          <div className="login-icon login-icon-staff">
            <UserCog size={28} />
          </div>
          <h2>Staff Portal</h2>
          <p>Sign in to manage daily operations</p>
        </div>
        {error && <div className="login-error">{error}</div>}
        <form onSubmit={handleSubmit} className="login-form">
          <label>
            Email Address
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="staff@example.com"
              required
            />
          </label>
          <label>
            Password
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </label>
          <label className="login-remember">
            <input type="checkbox" checked={remember} onChange={(e) => setRemember(e.target.checked)} />
            Remember me
          </label>
          <button type="submit" className="btn-primary login-submit" disabled={loading}>
            {loading ? 'Signing in...' : 'Login'}
          </button>
        </form>
        <p className="login-footer-link">
          <Link to="/login">Admin login</Link>
        </p>
      </div>
    </div>
  );
};

export default StaffLogin;
