import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/admin/Sidebar';
import { Header } from '../components/admin/Header';

export const AdminLayout: React.FC = () => {
  return (
    <div className="admin-layout">
      <Sidebar />
      <main className="main-content">
        <Header />
        <Outlet />
      </main>
    </div>
  );
};
