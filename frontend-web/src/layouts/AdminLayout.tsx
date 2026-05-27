import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/admin/Sidebar';
import { Header } from '../components/admin/Header';
import { PageModeProvider } from '../context/PageModeContext';

export const AdminLayout: React.FC = () => {
  return (
    <PageModeProvider mode="admin">
      <div className="admin-layout">
        <Sidebar />
        <main className="main-content">
          <Header />
          <Outlet />
        </main>
      </div>
    </PageModeProvider>
  );
};
