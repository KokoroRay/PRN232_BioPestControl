import React, { useEffect, useState } from 'react';
import {
  DollarSign,
  ShoppingCart,
  Users,
  Package,
  TrendingUp,
  Receipt,
  Truck,
  Wallet,
} from 'lucide-react';
import { StatCard } from '../../components/admin/StatCard';
import { RevenueChart } from '../../components/admin/RevenueChart';
import { statisticsService } from '../../services/statisticsService';
import { customerService } from '../../services/customerService';
import { productService } from '../../services/productService';
import { orderService } from '../../services/orderService';
import type { DashboardStats, RevenueStat } from '../../types/statistics';

type OrderRow = { id: string; orderDate: string; totalAmount: number; status: string };

const AdminDashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [revenueChartData, setRevenueChartData] = useState<RevenueStat[]>([]);
  const [totalCustomers, setTotalCustomers] = useState(0);
  const [totalProducts, setTotalProducts] = useState(0);
  const [recentOrders, setRecentOrders] = useState<OrderRow[]>([]);
  const isLive = true;

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        setLoading(true);
        const from = new Date();
        from.setDate(from.getDate() - 90);
        const filter = { fromDate: from.toISOString(), toDate: new Date().toISOString() };

        const [summaryRes, chartRes, customers, products, ordersRes] = await Promise.all([
          statisticsService.getSummary(filter),
          statisticsService.getRevenueChart(filter),
          customerService.getAll({ pageSize: 100 }).catch(() => []),
          productService.getAll().catch(() => []),
          orderService.getAdminOrders({ page: 1, pageSize: 6 }).catch(() => ({ items: [] })),
        ]);

        if (summaryRes.success) setStats(summaryRes.data);
        if (chartRes.success) setRevenueChartData(chartRes.data);
        setTotalCustomers(customers.length);
        setTotalProducts(products.length);
        setRecentOrders(
          ordersRes.items.map((o) => ({
            id: o.id,
            orderDate: o.orderDate,
            totalAmount: o.totalAmount,
            status: o.status,
          })),
        );
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

  if (loading) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100vh', background: '#F8FAFC' }}>
        <div style={{ textAlign: 'center' }}>
          <div className="spinner" />
          <p style={{ color: '#64748b', marginTop: '1rem' }}>Loading dashboard data...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <div className="header-title">
          <h1>Dashboard Overview</h1>
          <p>Welcome back! Here&apos;s what&apos;s happening with your business today.</p>
        </div>
        <div className={`status-badge ${isLive ? 'status-live' : ''}`}>
          <span className="status-dot" />
          {isLive ? 'Live' : 'Offline'}
        </div>
      </div>

      <div className="stats-grid">
        <StatCard
          title="Total Revenue"
          value={formatCurrency(stats?.totalRevenue || 0)}
          subtitle="Last 90 days"
          icon={DollarSign}
          iconBgColor="#dbeafe"
          iconColor="#2563eb"
        />
        <StatCard
          title="Total Sold Qty"
          value={stats?.totalSoldQuantity || 0}
          subtitle="Units sold"
          icon={ShoppingCart}
          iconBgColor="#dcfce7"
          iconColor="#16a34a"
        />
        <StatCard
          title="Total Customers"
          value={totalCustomers}
          subtitle="Registered"
          icon={Users}
          iconBgColor="#f3e8ff"
          iconColor="#9333ea"
        />
        <StatCard
          title="Total Products"
          value={totalProducts || stats?.totalLinkedProducts || 0}
          subtitle="In catalog"
          icon={Package}
          iconBgColor="#ffedd5"
          iconColor="#ea580c"
        />
      </div>

      <div className="charts-grid">
        <div className="chart-container">
          <h3 className="panel-title">Revenue Trend</h3>
          <div style={{ height: '300px', width: '100%' }}>
            <RevenueChart data={revenueChartData} />
          </div>
        </div>
        <div className="panel-container">
          <h3 className="panel-title">Top Selling Products</h3>
          <p className="text-muted" style={{ padding: '1rem' }}>Connect advanced statistics API for product breakdown.</p>
        </div>
      </div>

      <div className="charts-grid">
        <div className="panel-container">
          <h3 className="panel-title">Sales by Region</h3>
          <p className="text-muted" style={{ padding: '1rem' }}>Regional data requires extended statistics API.</p>
        </div>
        <div className="panel-container">
          <h3 className="panel-title">Recent Orders</h3>
          <div className="item-list">
            {recentOrders.length === 0 ? (
              <p className="text-muted" style={{ padding: '1rem' }}>No recent orders</p>
            ) : (
              recentOrders.map((order) => (
                <div key={order.id} className="list-item" style={{ border: '1px solid #f1f5f9' }}>
                  <div className="item-info">
                    <div className="item-avatar" style={{ background: '#dbeafe' }}>
                      <Receipt size={18} color="#2563eb" />
                    </div>
                    <div>
                      <div className="item-name">#{String(order.id).slice(0, 8)}</div>
                      <div className="item-subtext">{new Date(order.orderDate).toLocaleString('vi-VN')}</div>
                    </div>
                  </div>
                  <div className="item-value">
                    <div>{formatCurrency(order.totalAmount)}</div>
                    <span className="status-pill" style={{ background: '#dcfce7', color: '#15803d' }}>
                      {order.status}
                    </span>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      <div className="footer-grid">
        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Gross Revenue</div>
            <div className="footer-stat-value">{formatCurrency(stats?.totalRevenue ? stats.totalRevenue * 1.1 : 0)}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#ccfbf1' }}>
            <Wallet size={24} color="#0d9488" />
          </div>
        </div>
        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Products in orders</div>
            <div className="footer-stat-value">{stats?.totalLinkedProducts ?? 0}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#e0e7ff' }}>
            <TrendingUp size={24} color="#4f46e5" />
          </div>
        </div>
        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Sold quantity</div>
            <div className="footer-stat-value">{stats?.totalSoldQuantity ?? 0}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#fef9c3' }}>
            <Truck size={24} color="#ca8a04" />
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
