import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AdminLayout } from './layouts/AdminLayout';
import { StaffLayout } from './layouts/StaffLayout';
import { CustomerLayout } from './layouts/CustomerLayout';
import AdminDashboard from './pages/admin/Dashboard';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Admin Routes */}
        <Route path="/admin" element={<AdminLayout />}>
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<AdminDashboard />} />
          <Route path="warehouse" element={<div className="dashboard-container"><h1>Warehouse Management</h1><p>Module coming soon...</p></div>} />
          <Route path="products" element={<div className="dashboard-container"><h1>Product Management</h1><p>Module coming soon...</p></div>} />
          <Route path="category" element={<div className="dashboard-container"><h1>Category Management</h1><p>Module coming soon...</p></div>} />
          <Route path="orders" element={<div className="dashboard-container"><h1>Order Management</h1><p>Module coming soon...</p></div>} />
          <Route path="discounts" element={<div className="dashboard-container"><h1>Discount Management</h1><p>Module coming soon...</p></div>} />
          <Route path="articles" element={<div className="dashboard-container"><h1>Article Management</h1><p>Module coming soon...</p></div>} />
          <Route path="chemicalsafety" element={<div className="dashboard-container"><h1>Chemical Safety</h1><p>Module coming soon...</p></div>} />
          <Route path="staff" element={<div className="dashboard-container"><h1>Staff Management</h1><p>Module coming soon...</p></div>} />
          <Route path="customers" element={<div className="dashboard-container"><h1>Customer Management</h1><p>Module coming soon...</p></div>} />
        </Route>

        {/* Staff Routes */}
        <Route path="/staff" element={<StaffLayout />}>
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<AdminDashboard />} />
          <Route path="warehouse" element={<div className="dashboard-container"><h1>Warehouse Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="products" element={<div className="dashboard-container"><h1>Product Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="category" element={<div className="dashboard-container"><h1>Category Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="orders" element={<div className="dashboard-container"><h1>Order Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="discounts" element={<div className="dashboard-container"><h1>Discount Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="articles" element={<div className="dashboard-container"><h1>Article Management (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="chemicalsafety" element={<div className="dashboard-container"><h1>Chemical Safety (Staff)</h1><p>Module coming soon...</p></div>} />
          <Route path="customers" element={<div className="dashboard-container"><h1>Customer Management (Staff)</h1><p>Module coming soon...</p></div>} />
        </Route>

        {/* Customer Routes */}
        <Route path="/" element={<CustomerLayout />}>
          {/* Temporary redirect to admin, usually it would be Customer Home */}
          <Route index element={<Navigate to="/admin/dashboard" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
