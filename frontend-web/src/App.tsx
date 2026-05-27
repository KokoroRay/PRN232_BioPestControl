import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './components/ProtectedRoute';
import { AdminLayout } from './layouts/AdminLayout';
import { StaffLayout } from './layouts/StaffLayout';
import { CustomerLayout } from './layouts/CustomerLayout';
import Login from './pages/Login';
import Signup from './pages/Signup';
import StaffLogin from './pages/StaffLogin';
import AdminLogin from './pages/admin/AdminLogin';
import AdminDashboard from './pages/admin/Dashboard';
import StaffDashboard from './pages/staff/StaffDashboard';
import CategoryPage from './pages/admin/CategoryPage';
import ProductsPage from './pages/admin/ProductsPage';
import OrdersPage from './pages/admin/OrdersPage';
import ArticlesPage from './pages/admin/ArticlesPage';
import DiscountsPage from './pages/admin/DiscountsPage';
import StaffPage from './pages/admin/StaffPage';
import CustomersPage from './pages/admin/CustomersPage';
import ChemicalSafetyPage from './pages/admin/ChemicalSafetyPage';
import WarehousePage from './pages/admin/WarehousePage';
import FeedbacksPage from './pages/staff/FeedbacksPage';
import HomePage from './pages/public/HomePage';
import PublicAboutPage from './pages/public/AboutPage';
import PublicContactPage from './pages/public/ContactPage';
import PublicProductsPage from './pages/public/ProductsPage';
import PublicProductDetailsPage from './pages/public/ProductDetailsPage';
import PublicArticlesPage from './pages/public/ArticlesPage';
import PublicArticleDetailsPage from './pages/public/ArticleDetailsPage';


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />
        <Route path="/admin/login" element={<AdminLogin />} />
        <Route path="/admin/Login" element={<AdminLogin />} />
        <Route path="/staff/login" element={<StaffLogin />} />

        <Route
          path="/admin"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<AdminDashboard />} />
          <Route path="warehouse" element={<WarehousePage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="category" element={<CategoryPage />} />
          <Route path="orders" element={<OrdersPage />} />
          <Route path="discounts" element={<DiscountsPage />} />
          <Route path="articles" element={<ArticlesPage />} />
          <Route path="chemicalsafety" element={<ChemicalSafetyPage />} />
          <Route path="staff" element={<StaffPage />} />
          <Route path="customers" element={<CustomersPage />} />
        </Route>

        <Route
          path="/staff"
          element={
            <ProtectedRoute roles={['Staff']}>
              <StaffLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<StaffDashboard />} />
          <Route path="feedbacks" element={<FeedbacksPage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="discounts" element={<DiscountsPage />} />
          <Route path="category" element={<CategoryPage />} />
          <Route path="orders" element={<OrdersPage />} />
          <Route path="customers" element={<CustomersPage />} />
          <Route path="articles" element={<ArticlesPage />} />
          <Route path="warehouse" element={<WarehousePage />} />
          <Route path="chemicalsafety" element={<ChemicalSafetyPage />} />
        </Route>

        <Route path="/" element={<CustomerLayout />}>
          <Route index element={<HomePage />} />
          <Route path="about" element={<PublicAboutPage />} />
          <Route path="contact" element={<PublicContactPage />} />
          <Route path="products" element={<PublicProductsPage />} />
          <Route path="products/:id" element={<PublicProductDetailsPage />} />
          <Route path="articles" element={<PublicArticlesPage />} />
          <Route path="articles/:id" element={<PublicArticleDetailsPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
