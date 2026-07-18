import React, { useState } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { createFeedback } from '../../services/feedbackService';
import { AccountSidebar } from '../../components/public/AccountSidebar';

const CustomerFeedbackPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();

  const orderId = searchParams.get('orderId');
  const productId = searchParams.get('productId');
  const productName = searchParams.get('productName');

  const [formRating, setFormRating] = useState(5);
  const [formComment, setFormComment] = useState('');
  const [formImagesPreviews, setFormImagesPreviews] = useState<string[]>([]);

  if (!isAuthenticated) {
    navigate('/login');
    return null;
  }

  const handleImagePreviews = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files) return;
    const files = Array.from(e.target.files).slice(0, 5);
    const previews = files.map(file => URL.createObjectURL(file));
    setFormImagesPreviews(previews);
  };

  const handleAddReview = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!orderId || !productId) {
      showToast('Thông tin đơn hàng hoặc sản phẩm không hợp lệ', 'error');
      return;
    }

    try {
      await createFeedback({
        productId: Number(productId),
        userId: user?.id || '00000000-0000-0000-0000-000000000000',
        userName: user?.fullName || user?.email || 'Khách hàng',
        rating: formRating,
        comment: formComment
      });

      showToast('Gửi đánh giá thành công! Cảm ơn đóng góp của bạn.', 'success');
      navigate(`/orders/${orderId}`);
    } catch (err) {
      console.error('Failed to submit feedback', err);
      showToast('Có lỗi xảy ra khi gửi đánh giá, vui lòng thử lại.', 'error');
    }
  };

  return (
    <div className="orders-page">
      <div className="orders-container">
        <aside className="orders-sidebar-col">
          <AccountSidebar active="orders" />
        </aside>

        <main className="orders-main bg-white dark:bg-surface-container rounded-2xl p-8 shadow-xl">
          <div className="mb-6 flex justify-between items-center">
            <h1 className="text-2xl font-bold text-primary">Đánh giá sản phẩm</h1>
            <Link to={`/orders/${orderId}`} className="text-sm text-primary hover:underline font-semibold">
              Quay lại đơn hàng
            </Link>
          </div>

          <div className="bg-surface-container border border-outline-variant/10 rounded-xl p-4 mb-6 flex items-center justify-between">
            <div>
              <p className="text-xs text-on-surface-variant uppercase font-bold tracking-wide">Sản phẩm</p>
              <p className="font-bold text-primary">{productName || `Product #${productId}`}</p>
            </div>
            <div className="text-right">
              <p className="text-xs text-on-surface-variant uppercase font-bold tracking-wide">Đơn hàng</p>
              <p className="font-bold">#{orderId?.slice(0,8)}</p>
            </div>
          </div>

          <form onSubmit={handleAddReview} className="space-y-6">
            <div>
              <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Đánh giá sao</label>
              <div className="flex gap-1" id="starRating">
                {[1, 2, 3, 4, 5].map(i => (
                  <button
                    key={i}
                    type="button"
                    onClick={() => setFormRating(i)}
                    className="hover:scale-110 transition-transform focus:outline-none"
                  >
                    <svg 
                      xmlns="http://www.w3.org/2000/svg" 
                      viewBox="0 0 24 24" 
                      fill={i <= formRating ? "currentColor" : "none"} 
                      stroke="currentColor" 
                      strokeWidth="1.5"
                      strokeLinecap="round" 
                      strokeLinejoin="round" 
                      className={`w-10 h-10 ${i <= formRating ? 'text-amber-400 drop-shadow-md' : 'text-slate-300'}`}
                    >
                      <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                    </svg>
                  </button>
                ))}
              </div>
            </div>

            <div>
              <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Bình luận nhận xét</label>
              <textarea
                value={formComment}
                onChange={(e) => setFormComment(e.target.value)}
                rows={5}
                className="w-full border border-outline-variant/30 rounded-xl p-4 text-sm focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface focus:outline-none"
                placeholder="Chia sẻ trải nghiệm thực tế sử dụng sản phẩm..."
                required
              ></textarea>
            </div>

            <div>
              <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Hình ảnh đính kèm (tối đa 5 hình)</label>
              <input
                type="file"
                multiple
                accept="image/*"
                onChange={handleImagePreviews}
                className="w-full border border-outline-variant/30 rounded-xl p-3 text-xs bg-background dark:bg-surface"
              />
              {formImagesPreviews.length > 0 && (
                <div className="flex gap-2.5 mt-3 flex-wrap">
                  {formImagesPreviews.map((preview, index) => (
                    <img key={index} src={preview} alt="Form preview" className="w-16 h-16 object-cover rounded-xl border" />
                  ))}
                </div>
              )}
            </div>

            <div className="flex justify-end pt-4 border-t border-outline-variant/10">
              <button type="submit" className="bg-primary hover:bg-[#173901] text-white font-bold px-8 py-3 rounded-xl text-sm shadow-md transition-colors">
                Lưu đánh giá
              </button>
            </div>
          </form>
        </main>
      </div>
    </div>
  );
};

export default CustomerFeedbackPage;
