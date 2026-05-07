import React from 'react';
import { NavLink } from 'react-router-dom';
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
  FlaskConical,
  Settings
} from 'lucide-react';

interface SidebarProps {
  role?: 'admin' | 'staff';
}

export const Sidebar: React.FC<SidebarProps> = ({ role = 'admin' }) => {
  const basePath = role === 'admin' ? '/admin' : '/staff';

  return (
    <aside className="sidebar">
      {/* Brand Header */}
      <div className="sidebar-header">
        <div className="brand-container">
          <div className="brand-icon">
            <FlaskConical size={24} />
          </div>
          <div>
            <span className="brand-name">BioPestControl</span>
            <div className="brand-subtitle">{role === 'admin' ? 'Admin Panel' : 'Staff Panel'}</div>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="sidebar-nav">
        <NavLink to={`${basePath}/dashboard`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <LayoutDashboard size={20} />
          <span>Dashboard</span>
        </NavLink>

        <div className="nav-section-title">Inventory</div>
        <NavLink to={`${basePath}/warehouse`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <Warehouse size={20} />
          <span>Warehouse</span>
        </NavLink>
        <NavLink to={`${basePath}/products`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <Package size={20} />
          <span>Products</span>
        </NavLink>
        <NavLink to={`${basePath}/category`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <Tags size={20} />
          <span>Categories</span>
        </NavLink>

        <div className="nav-section-title">Sales</div>
        <NavLink to={`${basePath}/orders`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <ShoppingCart size={20} />
          <span>Orders</span>
        </NavLink>
        <NavLink to={`${basePath}/discounts`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <TicketPercent size={20} />
          <span>Discounts</span>
        </NavLink>

        <div className="nav-section-title">Content</div>
        <NavLink to={`${basePath}/articles`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <FileText size={20} />
          <span>Articles / News</span>
        </NavLink>

        <div className="nav-section-title">Safety</div>
        <NavLink to={`${basePath}/chemicalsafety`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <ShieldCheck size={20} />
          <span>Chemical Safety</span>
        </NavLink>

        <div className="nav-section-title">Users</div>
        {role === 'admin' && (
          <NavLink to={`${basePath}/staff`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <UserRoundCog size={20} />
            <span>Staff</span>
          </NavLink>
        )}
        <NavLink to={`${basePath}/customers`} className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
          <Users size={20} />
          <span>Customers</span>
        </NavLink>
      </nav>

      {/* Footer Profile */}
      <div className="sidebar-footer">
        <div className="user-profile">
          <div className="user-avatar" style={{ backgroundColor: role === 'staff' ? '#dcfce7' : '', color: role === 'staff' ? '#15803d' : '' }}>
            {role === 'admin' ? 'AD' : 'ST'}
          </div>
          <div className="user-info">
            <div className="user-name">{role === 'admin' ? 'Admin User' : 'Staff User'}</div>
            <div className="user-role">{role === 'admin' ? 'Administrator' : 'Staff Member'}</div>
          </div>
          <Settings size={18} color="#64748b" />
        </div>
      </div>
    </aside>
  );
};
