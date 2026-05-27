import React from 'react';
import { Bell, LogOut } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

interface HeaderProps {
  role?: 'admin' | 'staff';
}

export const Header: React.FC<HeaderProps> = ({ role = 'admin' }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const brandName = role === 'admin' ? 'BioPestControl Admin' : 'BioPestControl Staff';
  const initials = (user?.fullName?.[0] ?? user?.email?.[0] ?? 'U').toUpperCase();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="admin-header">
      <div className="header-left">
        <h1 className="header-brand">{brandName}</h1>
      </div>
      <div className="header-right">
        <div className="header-actions">
          <button type="button" className="icon-btn" aria-label="Notifications">
            <Bell size={20} />
            <span className="notification-dot" />
          </button>
          <button type="button" className="avatar-dropdown" onClick={handleLogout} title="Logout">
            <div className="avatar-mini">{initials}</div>
            <span className="avatar-name">{user?.fullName ?? user?.email}</span>
            <LogOut size={16} color="#64748b" />
          </button>
        </div>
      </div>
    </header>
  );
};
