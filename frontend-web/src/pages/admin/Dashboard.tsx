import React, { useEffect, useState } from 'react';
import { 
  DollarSign, 
  ShoppingCart, 
  Users, 
  Package, 
  TrendingUp, 
  Receipt,
  Truck,
  Wallet
} from 'lucide-react';
import { StatCard } from '../../components/admin/StatCard';
import { RevenueChart } from '../../components/admin/RevenueChart';
import { statisticsService } from '../../services/api';
import type { DashboardStats, RevenueStat } from '../../types/statistics';

const AdminDashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [revenueChartData, setRevenueChartData] = useState<RevenueStat[]>([]);
  const isLive = true;

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        setLoading(true);
        const [summaryRes, chartRes] = await Promise.all([
          statisticsService.getSummary(),
          statisticsService.getRevenueChart()
        ]);

        if (summaryRes.success) setStats(summaryRes.data);
        if (chartRes.success) setRevenueChartData(chartRes.data);
      } catch (error) {
        console.error("Error fetching dashboard data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
  };

  if (loading) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100vh', background: '#F8FAFC' }}>
        <div style={{ textAlign: 'center' }}>
          <div className="spinner"></div>
          <p style={{ color: '#64748b', marginTop: '1rem' }}>Loading dashboard data...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      {/* Header */}
      <div className="dashboard-header">
        <div className="header-title">
          <h1>Dashboard Overview</h1>
          <p>Welcome back! Here's what's happening with your business today.</p>
        </div>
        <div className={`status-badge ${isLive ? 'status-live' : ''}`}>
          <span className="status-dot"></span>
          {isLive ? "Live" : "Offline"}
        </div>
      </div>

      {/* Stats Grid */}
      <div className="stats-grid">
        <StatCard 
          title="Total Revenue"
          value={formatCurrency(stats?.totalRevenue || 0)}
          subtitle="Last 30 days"
          icon={DollarSign}
          iconBgColor="#dbeafe"
          iconColor="#2563eb"
          growth={12.5}
        />
        <StatCard 
          title="Total Orders"
          value={stats?.totalSoldQuantity || 0}
          subtitle={`${stats?.totalSoldQuantity || 0} products sold`}
          icon={ShoppingCart}
          iconBgColor="#dcfce7"
          iconColor="#16a34a"
        />
        <StatCard 
          title="Total Customers"
          value={120}
          subtitle="Active accounts"
          icon={Users}
          iconBgColor="#f3e8ff"
          iconColor="#9333ea"
        />
        <StatCard 
          title="Total Products Sold"
          value={stats?.totalLinkedProducts || 0}
          subtitle="In catalog"
          icon={Package}
          iconBgColor="#ffedd5"
          iconColor="#ea580c"
        />
      </div>

      {/* Charts Grid */}
      <div className="charts-grid">
        <div className="chart-container">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
            <h3 className="panel-title" style={{ margin: 0 }}>Revenue Trend</h3>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button className="btn-range active">Daily</button>
              <button className="btn-range">Monthly</button>
            </div>
          </div>
          <div style={{ height: '300px', width: '100%' }}>
            <RevenueChart data={revenueChartData} />
          </div>
        </div>

        <div className="panel-container">
          <h3 className="panel-title">Top Selling Products</h3>
          <div className="item-list">
            {[1, 2, 3, 4, 5].map((i) => (
              <div key={i} className="list-item">
                <div className="item-info">
                  <div className="item-avatar" style={{ background: '#dcfce7', color: '#15803d' }}>
                    P{i}
                  </div>
                  <div>
                    <div className="item-name">Product Name {i}</div>
                    <div className="item-subtext">{10 - i} units sold</div>
                  </div>
                </div>
                <div className="item-value">{formatCurrency(100000 * (10-i))}</div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Region and Recent Orders */}
      <div className="charts-grid">
        <div className="panel-container">
          <h3 className="panel-title">Sales by Region</h3>
          <div className="progress-container">
            {[
              { name: "Hồ Chí Minh", revenue: 15000000, orders: 45 },
              { name: "Hà Nội", revenue: 12000000, orders: 38 },
              { name: "Đà Nẵng", revenue: 8000000, orders: 24 },
              { name: "Cần Thơ", revenue: 5000000, orders: 15 },
            ].map((region, i) => {
              const max = 15000000;
              const percentage = (region.revenue / max) * 100;
              return (
                <div key={i} className="progress-item">
                  <div className="progress-header">
                    <span style={{ fontWeight: 500 }}>{region.name}</span>
                    <span style={{ fontWeight: 700 }}>{formatCurrency(region.revenue)}</span>
                  </div>
                  <div className="progress-bar-bg">
                    <div className="progress-bar-fill" style={{ width: `${percentage}%` }}></div>
                  </div>
                  <div className="item-subtext">{region.orders} orders</div>
                </div>
              );
            })}
          </div>
        </div>

        <div className="panel-container">
          <h3 className="panel-title">Recent Orders</h3>
          <div className="item-list">
            {[1, 2, 3, 4, 5].map((i) => (
              <div key={i} className="list-item" style={{ border: '1px solid #f1f5f9' }}>
                <div className="item-info">
                  <div className="item-avatar" style={{ background: '#dbeafe' }}>
                    <Receipt size={18} color="#2563eb" />
                  </div>
                  <div>
                    <div className="item-name">#ORD-00{i}</div>
                    <div className="item-subtext">07/05/2026 14:30</div>
                  </div>
                </div>
                <div className="item-value">
                  <div>{formatCurrency(500000)}</div>
                  <span className="status-pill" style={{ 
                    background: i % 2 === 0 ? '#dcfce7' : '#fef9c3',
                    color: i % 2 === 0 ? '#15803d' : '#854d0e'
                  }}>
                    {i % 2 === 0 ? "Delivered" : "Pending"}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Footer Stats Grid */}
      <div className="footer-grid">
        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Avg Order Value</div>
            <div className="footer-stat-value">{formatCurrency(450000)}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#e0e7ff' }}>
            <TrendingUp size={24} color="#4f46e5" />
          </div>
        </div>

        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Shipping Fees</div>
            <div className="footer-stat-value">{formatCurrency(25000)}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#fef9c3' }}>
            <Truck size={24} color="#ca8a04" />
          </div>
        </div>

        <div className="footer-stat-item">
          <div>
            <div className="footer-stat-label">Gross Revenue</div>
            <div className="footer-stat-value">{formatCurrency(stats?.totalRevenue ? stats.totalRevenue * 1.1 : 0)}</div>
          </div>
          <div className="footer-stat-icon" style={{ background: '#ccfbf1' }}>
            <Wallet size={24} color="#0d9488" />
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
