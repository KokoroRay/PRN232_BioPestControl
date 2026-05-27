import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Receipt, Users, AlertTriangle } from 'lucide-react';
import { LoadingState } from '../../components/admin/LoadingState';
import { orderService } from '../../services/orderService';
import { customerService } from '../../services/customerService';
import { productService } from '../../services/productService';
import { inventoryService } from '../../services/inventoryService';

const StaffDashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [todayOrders, setTodayOrders] = useState(0);
  const [totalOrders, setTotalOrders] = useState(0);
  const [totalCustomers, setTotalCustomers] = useState(0);
  const [totalProducts, setTotalProducts] = useState(0);
  const [lowStock, setLowStock] = useState(0);
  const [revenueToday, setRevenueToday] = useState(0);
  const [recentOrders, setRecentOrders] = useState<
    { id: string; totalAmount: number; status: string; orderDate: string }[]
  >([]);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const startOfDay = new Date();
        startOfDay.setHours(0, 0, 0, 0);

        const [ordersRes, customers, products, stock] = await Promise.all([
          orderService.getStaffOrders({ pageSize: 100 }),
          customerService.getAllStaff({ pageSize: 100 }),
          productService.getAll(),
          inventoryService.getStock().catch(() => []),
        ]);

        const orders = ordersRes.items;
        setTotalOrders(orders.length);
        setTodayOrders(
          orders.filter((o) => new Date(o.orderDate) >= startOfDay).length,
        );
        setRevenueToday(
          orders
            .filter((o) => new Date(o.orderDate) >= startOfDay && o.status !== 'Cancelled')
            .reduce((s, o) => s + o.totalAmount, 0),
        );
        setRecentOrders(
          orders.slice(0, 5).map((o) => ({
            id: o.id,
            totalAmount: o.totalAmount,
            status: o.status,
            orderDate: o.orderDate,
          })),
        );
        setTotalCustomers(customers.length);
        setTotalProducts(products.length);
        setLowStock(stock.filter((s) => s.isLowStock).length);
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const formatMoney = (v: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);

  if (loading) return <LoadingState message="Loading staff dashboard..." />;

  return (
    <div className="admin-page staff-dashboard">
      <div className="page-header">
        <div>
          <h1 className="page-title">Staff Dashboard</h1>
          <p className="page-subtitle">Overview of today&apos;s operations</p>
        </div>
      </div>

      <div className="stats-grid staff-stats-grid">
        <div className="staff-stat-card">
          <div>
            <p className="staff-stat-label">Đơn hàng hôm nay</p>
            <p className="staff-stat-value">{todayOrders}</p>
            <p className="text-muted">Tổng đơn: {totalOrders}</p>
          </div>
          <Receipt className="staff-stat-icon" color="#2563eb" />
        </div>
        <div className="staff-stat-card">
          <div>
            <p className="staff-stat-label">Doanh thu hôm nay</p>
            <p className="staff-stat-value">{formatMoney(revenueToday)}</p>
          </div>
          <Receipt className="staff-stat-icon" color="#16a34a" />
        </div>
        <div className="staff-stat-card">
          <div>
            <p className="staff-stat-label">Tổng khách hàng</p>
            <p className="staff-stat-value">{totalCustomers}</p>
          </div>
          <Users className="staff-stat-icon" color="#9333ea" />
        </div>
        <div className="staff-stat-card">
          <div>
            <p className="staff-stat-label">Sản phẩm tồn thấp</p>
            <p className="staff-stat-value">{lowStock}</p>
            <p className="text-muted">Tổng SP: {totalProducts}</p>
          </div>
          <AlertTriangle className="staff-stat-icon" color="#ca8a04" />
        </div>
      </div>

      <div className="panel-card">
        <div className="panel-card-header">
          <div>
            <h3>Đơn hàng gần đây</h3>
            <p className="text-muted">5 đơn mới nhất</p>
          </div>
          <Link to="/staff/orders" className="btn-link">
            Xem tất cả
          </Link>
        </div>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Mã đơn</th>
                <th>Giá trị</th>
                <th>Trạng thái</th>
                <th>Ngày tạo</th>
              </tr>
            </thead>
            <tbody>
              {recentOrders.length === 0 ? (
                <tr>
                  <td colSpan={4} className="empty-cell">
                    Chưa có đơn hàng
                  </td>
                </tr>
              ) : (
                recentOrders.map((o) => (
                  <tr key={o.id}>
                    <td>
                      <code>#{String(o.id).slice(0, 8)}</code>
                    </td>
                    <td>{formatMoney(o.totalAmount)}</td>
                    <td>
                      <span className="pill">{o.status}</span>
                    </td>
                    <td>{new Date(o.orderDate).toLocaleString('vi-VN')}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default StaffDashboard;
