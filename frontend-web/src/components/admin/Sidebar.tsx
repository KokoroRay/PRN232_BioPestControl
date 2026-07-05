import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import {
  LayoutDashboard,
  Warehouse,
  Package,
  Tags,
  ShoppingCart,
  TicketPercent,
  FileText,
  ShieldCheck,
  UserRoundCog,
  Users,
  Leaf,
  Settings,
  MessageSquare,
} from 'lucide-react';

interface SidebarProps {
  role?: 'admin' | 'staff';
}

export const Sidebar: React.FC<SidebarProps> = ({ role = 'admin' }) => {
  const { user } = useAuth();
  const basePath = role === 'admin' ? '/admin' : '/staff';
  const initials = (user?.fullName?.[0] ?? user?.email?.[0] ?? (role === 'admin' ? 'A' : 'S')).toUpperCase();
  const link = (path: string) => `${basePath}/${path}`;
  const navClass = ({ isActive }: { isActive: boolean }) => `nav-link ${isActive ? 'active' : ''}`;

  return (
    <aside className={`sidebar ${role === 'staff' ? 'sidebar-staff' : ''}`}>
      <div className="sidebar-header">
        <div className="brand-container">
          <div className="brand-icon">
            <Leaf size={24} strokeWidth={2.25} fill="currentColor" />
          </div>
          <div>
            <span className="brand-name">BioPestControl</span>
            <div className="brand-subtitle">{role === 'admin' ? 'Admin Panel' : 'Staff Panel'}</div>
          </div>
        </div>
      </div>

      <nav className="sidebar-nav">
        {role === 'staff' ? (
          <>
            <NavLink to={link('dashboard')} className={navClass}>
              <LayoutDashboard size={20} />
              <span>Dashboard</span>
            </NavLink>
            <NavLink to={link('feedbacks')} className={navClass}>
              <MessageSquare size={20} />
              <span>Feedback</span>
            </NavLink>
            <NavLink to={link('products')} className={navClass}>
              <Package size={20} />
              <span>Products</span>
            </NavLink>
            <NavLink to={link('discounts')} className={navClass}>
              <TicketPercent size={20} />
              <span>Discounts</span>
            </NavLink>
            <NavLink to={link('category')} className={navClass}>
              <Tags size={20} />
              <span>Category</span>
            </NavLink>
            <NavLink to={link('orders')} className={navClass}>
              <ShoppingCart size={20} />
              <span>Orders</span>
            </NavLink>
            <NavLink to={link('customers')} className={navClass}>
              <Users size={20} />
              <span>Customers</span>
            </NavLink>
            <NavLink to={link('articles')} className={navClass}>
              <FileText size={20} />
              <span>Articles / News</span>
            </NavLink>
            <NavLink to={link('warehouse')} className={navClass}>
              <Warehouse size={20} />
              <span>Warehouse</span>
            </NavLink>
            <NavLink to={link('chemicalsafety')} className={navClass}>
              <ShieldCheck size={20} />
              <span>Chemical Safety</span>
            </NavLink>
          </>
        ) : (
          <>
            <NavLink to={link('dashboard')} className={navClass}>
              <LayoutDashboard size={20} />
              <span>Dashboard</span>
            </NavLink>
            <div className="nav-section-title">Inventory</div>
            <NavLink to={link('warehouse')} className={navClass}>
              <Warehouse size={20} />
              <span>Warehouse</span>
            </NavLink>
            <NavLink to={link('products')} className={navClass}>
              <Package size={20} />
              <span>Products</span>
            </NavLink>
            <NavLink to={link('category')} className={navClass}>
              <Tags size={20} />
              <span>Categories</span>
            </NavLink>
            <NavLink to={link('crops')} className={navClass}>
              <Leaf size={20} />
              <span>Crops</span>
            </NavLink>
            <div className="nav-section-title">Sales</div>
            <NavLink to={link('orders')} className={navClass}>
              <ShoppingCart size={20} />
              <span>Orders</span>
            </NavLink>
            <NavLink to={link('discounts')} className={navClass}>
              <TicketPercent size={20} />
              <span>Discounts</span>
            </NavLink>
            <div className="nav-section-title">Content</div>
            <NavLink to={link('articles')} className={navClass}>
              <FileText size={20} />
              <span>Articles / News</span>
            </NavLink>
            <div className="nav-section-title">Safety</div>
            <NavLink to={link('chemicalsafety')} className={navClass}>
              <ShieldCheck size={20} />
              <span>Chemical Safety</span>
            </NavLink>
            <div className="nav-section-title">Users</div>
            <NavLink to={link('staff')} className={navClass}>
              <UserRoundCog size={20} />
              <span>Staff</span>
            </NavLink>
            <NavLink to={link('customers')} className={navClass}>
              <Users size={20} />
              <span>Customers</span>
            </NavLink>
          </>
        )}
      </nav>

      <div className="sidebar-footer">
        <div className="user-profile">
          <div
            className="user-avatar"
            style={{
              backgroundColor: role === 'staff' ? '#dcfce7' : '',
              color: role === 'staff' ? '#15803d' : '',
            }}
          >
            {initials}
          </div>
          <div className="user-info">
            <div className="user-name">{user?.fullName ?? user?.email ?? 'User'}</div>
            <div className="user-role">{user?.role ?? (role === 'admin' ? 'Administrator' : 'Staff')}</div>
          </div>
          <Settings size={18} color="#64748b" />
        </div>
      </div>
    </aside>
  );
};
