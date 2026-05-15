import React from 'react';
import { PageHeader } from '../../components/admin/PageHeader';
import { MessageSquare } from 'lucide-react';

/** Staff Feedbacks — UI khung; microservice feedback chưa có trong repo PRN232 */
const FeedbacksPage: React.FC = () => (
  <div className="admin-page">
    <PageHeader
      title="Feedback Management"
      subtitle="Review customer feedback and respond (read-only until feedback API is deployed)"
    />
    <div className="panel-card" style={{ textAlign: 'center', padding: '3rem' }}>
      <MessageSquare size={48} color="#94a3b8" style={{ margin: '0 auto 1rem' }} />
      <h3 style={{ margin: '0 0 0.5rem' }}>Feedback module</h3>
      <p className="text-muted" style={{ maxWidth: 420, margin: '0 auto' }}>
        Trang Staff/Feedbacks trong Blazor dùng <code>IFeedbackService</code> monolith. Khi có API
        feedback trong microservices, trang này sẽ được nối filter theo sản phẩm, khách hàng, rating
        và trạng thái phản hồi.
      </p>
    </div>
  </div>
);

export default FeedbacksPage;
