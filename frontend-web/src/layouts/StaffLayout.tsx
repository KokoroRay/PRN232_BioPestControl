import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/admin/Sidebar';
import { Header } from '../components/admin/Header';
import { PageModeProvider } from '../context/PageModeContext';

export const StaffLayout: React.FC = () => {
  return (
    <PageModeProvider mode="staff">
      <div className="admin-layout staff-layout">
        <Sidebar role="staff" />
        <main className="main-content">
          <Header role="staff" />
          <Outlet />
        </main>
      </div>
    </PageModeProvider>
  );
};
