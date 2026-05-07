import React from 'react';
import { Bell, ChevronDown } from 'lucide-react';

interface HeaderProps {
  role?: 'admin' | 'staff';
}

export const Header: React.FC<HeaderProps> = ({ role = 'admin' }) => {
  const brandName = role === 'admin' ? 'BioPestControl Admin' : 'BioPestControl Staff';
  const avatarInitials = role === 'admin' ? 'AD' : 'ST';
  const userName = role === 'admin' ? 'Admin User' : 'Staff User';

  return (
    <header className="admin-header">
      <div className="header-left">
        <h1 className="header-brand">{brandName}</h1>
      </div>

      <div className="header-right">
        <div className="header-actions">
          <button className="icon-btn">
            <Bell size={20} />
            <span className="notification-dot"></span>
          </button>
          
          <div className="avatar-dropdown">
            <div className="avatar-mini" style={{ backgroundColor: role === 'staff' ? '#dcfce7' : '#e0e7ff', color: role === 'staff' ? '#15803d' : '#4f46e5' }}>
              {avatarInitials}
            </div>
            <span className="avatar-name">{userName}</span>
            <ChevronDown size={16} color="#64748b" />
          </div>
        </div>
      </div>
    </header>
  );
};
