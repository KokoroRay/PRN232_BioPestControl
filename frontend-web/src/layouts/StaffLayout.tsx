import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/admin/Sidebar';
import { Header } from '../components/admin/Header';

export const StaffLayout: React.FC = () => {
  return (
    <div className="admin-layout">
      <Sidebar role="staff" />
      <main className="main-content">
        <Header role="staff" />
        <Outlet />
      </main>
    </div>
  );
};
