import React from 'react';
import { Link } from 'react-router-dom';
import { User, Receipt, Lock, LogOut } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

interface AccountSidebarProps {
  active?: 'profile' | 'orders' | 'changepassword';
}

export const AccountSidebar: React.FC<AccountSidebarProps> = ({ active = 'profile' }) => {
  const { logout } = useAuth();

  const linkClass = (key: string) =>
    `flex items-center gap-3 px-4 py-3 rounded-xl transition-colors font-medium ${
      active === key
        ? 'bg-[#28a745]/10 text-[#28a745] border border-[#28a745]/20 font-bold'
        : 'hover:bg-white text-slate-700'
    }`;

  const iconClass = (key: string) =>
    active === key ? '' : 'text-[#4c9a4c]';

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="account-sidebar">
      <Link to="/account/profile" className={linkClass('profile')}>
        <User size={18} className={iconClass('profile')} />
        Profile
      </Link>
      <Link to="/orders" className={linkClass('orders')}>
        <Receipt size={18} className={iconClass('orders')} />
        Orders
      </Link>
      <Link to="/account/change-password" className={linkClass('changepassword')}>
        <Lock size={18} className={iconClass('changepassword')} />
        Change password
      </Link>
      <div className="mt-4 pt-4 border-t border-gray-200">
        <button
          type="button"
          onClick={handleLogout}
          className="flex w-full items-center gap-3 px-4 py-3 rounded-xl hover:bg-white transition-colors font-medium text-red-500"
        >
          <LogOut size={18} />
          Sign Out
        </button>
      </div>
    </div>
  );
};
