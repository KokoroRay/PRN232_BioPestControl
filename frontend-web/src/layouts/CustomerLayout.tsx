import React from 'react';
import { Outlet } from 'react-router-dom';

export const CustomerLayout: React.FC = () => {
  return (
    <div className="customer-layout">
      {/* TODO: Add Customer Header/Navbar here */}
      <main className="main-content">
        <Outlet />
      </main>
      {/* TODO: Add Customer Footer here */}
    </div>
  );
};
