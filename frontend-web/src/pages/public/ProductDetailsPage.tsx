import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { productService } from '../../services/productService';
import type { Product } from '../../types/catalog';
import { useAuth } from '../../context/AuthContext';
import { useAddToCart } from '../../hooks/useAddToCart';

import { getFeedbacksByProductId, createFeedback as createFeedbackApi } from '../../services/feedbackService';

interface Feedback {
  id: string;
  userName: string;
  customerAvatar?: string;
  rating: number;
  comment: string;
  createdAt: string;
  helpfulCount?: number;
  images?: string[];
  replyMessage?: string;
  repliedAt?: string;
}

interface RatingFilter {
  minRating: number | null;
  showWithImages: boolean;
}

const ProductDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const { handleAddToCart } = useAddToCart();

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);

  // Gallery state
  const [selectedImage, setSelectedImage] = useState<string>('');

  // Order selection for reviews
  const [qty, setQty] = useState(1);
  const [ratingFilter, setRatingFilter] = useState<RatingFilter>({ minRating: null, showWithImages: false });
  const [addingToCart, setAddingToCart] = useState(false);

  // Review submission state
  const [formRating, setFormRating] = useState(5);
  const [formComment, setFormComment] = useState('');
  const [formImagesPreviews, setFormImagesPreviews] = useState<string[]>([]);
  const [formOrderId, setFormOrderId] = useState('');
  
  const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
  const [toast, setToast] = useState<{ show: boolean; msg: string; type: 'success' | 'error' }>({ show: false, msg: '', type: 'success' });

  // Load details
  useEffect(() => {
    if (!id) return;
    const loadDetails = async () => {
      try {
        setLoading(true);
        const data = await productService.getById(Number(id));
        setProduct(data);
        if (data.imageUrl) {
          setSelectedImage(data.imageUrl);
        }
        
        // Load feedbacks from API
        try {
          const apiFeedbacks = await getFeedbacksByProductId(Number(id));
          setFeedbacks(apiFeedbacks);
        } catch (error) {
          console.error('Failed to load feedbacks:', error);
          setFeedbacks([]);
        }
      } catch (err) {
        console.error('Error loading product details', err);
        navigate('/products');
      } finally {
        setLoading(false);
      }
    };
    loadDetails();
  }, [id, navigate]);

  const showToastMsg = (msg: string, type: 'success' | 'error' = 'success') => {
    setToast({ show: true, msg, type });
    setTimeout(() => {
      setToast(prev => ({ ...prev, show: false }));
    }, 3000);
  };

  const handleImagePreviews = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files) return;
    const files = Array.from(e.target.files).slice(0, 5);

    const previews = files.map(file => URL.createObjectURL(file));
    setFormImagesPreviews(previews);
  };

  const handleHelpfulClick = (feedbackId: string) => {
    setFeedbacks(prev => prev.map(f => {
      if (f.id === feedbackId) {
        return { ...f, helpfulCount: (f.helpfulCount || 0) + 1 };
      }
      return f;
    }));
    showToastMsg('Cảm ơn bạn đã phản hồi review này hữu ích!');
  };

  const handleAddReview = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isAuthenticated) {
      showToastMsg('Vui lòng đăng nhập để gửi đánh giá!', 'error');
      return;
    }
    if (!formOrderId) {
      showToastMsg('Vui lòng chọn đơn hàng chứa sản phẩm này!', 'error');
      return;
    }

    try {
      const created = await createFeedbackApi({
        productId: product?.id,
        userId: user?.id || '00000000-0000-0000-0000-000000000000',
        userName: user?.fullName || user?.email || 'Khách hàng',
        rating: formRating,
        comment: formComment
      });

      // Optimistic UI update or fetch from server again, here we do optimistic:
      const newFeedback: Feedback = {
        id: created.id,
        userName: created.userName,
        rating: created.rating,
        comment: created.comment,
        createdAt: created.createdAt,
        helpfulCount: 0,
        images: formImagesPreviews.length > 0 ? formImagesPreviews : undefined
      };

      setFeedbacks(prev => [newFeedback, ...prev]);
      showToastMsg('Gửi đánh giá thành công! Cảm ơn đóng góp của bạn.');
      
      // Clear form state
      setFormComment('');
      setFormImagesPreviews([]);
      setFormOrderId('');
    } catch (err) {
      console.error('Failed to submit feedback', err);
      showToastMsg('Có lỗi xảy ra khi gửi đánh giá, vui lòng thử lại.', 'error');
    }
  };

  const handleCartAdd = async (isBuyNow: boolean) => {
    if (!product) return;
    setAddingToCart(true);
    try {
      await handleAddToCart(product, { quantity: qty, buyNow: isBuyNow });
    } finally {
      setAddingToCart(false);
    }
  };

  const filteredFeedbacks = useMemo(() => {
    let result = [...feedbacks];
    // Filter by minimum rating (e.g., 4 stars = show 4 and 5 stars)
    if (ratingFilter.minRating !== null) {
      result = result.filter(f => f.rating >= ratingFilter.minRating!);
    }
    if (ratingFilter.showWithImages) {
      result = result.filter(f => f.images && f.images.length > 0);
    }
    return result;
  }, [feedbacks, ratingFilter]);

  const ratingSummary = useMemo(() => {
    const total = feedbacks.length;
    if (total === 0) return { avg: 5, rates: { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 }, total: 0 };
    
    const sum = feedbacks.reduce((acc, f) => acc + f.rating, 0);
    const avg = Number((sum / total).toFixed(1));
    
    const rates = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };
    feedbacks.forEach(f => {
      const rate = f.rating as 5|4|3|2|1;
      if (rates[rate] !== undefined) {
        rates[rate] += 1;
      }
    });

    return { avg, rates, total };
  }, [feedbacks]);

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);

  if (loading) {
    return (
      <div className="flex justify-center items-center py-40 min-h-screen text-primary">
        <span className="material-symbols-outlined text-4xl animate-spin">hourglass_empty</span>
      </div>
    );
  }

  if (!product) return null;

  // Generate 4 mock image thumbnails for gallery including the main image
  const imagesList = [
    product.imageUrl || '',
    'https://lh3.googleusercontent.com/aida-public/AB6AXuD1v5QtMLjXFjj9tiYQpdFudr4bUHPkjnN15jmWWr9kIk6dVkb96F7NOkQBADgCQ3gpxQb657Jh27EEBWwqj_F7rQ6vYoh01kN9o_NuwHI14uPk_-aeFA99mlMqz2qfWSOaEP6i6n_KyWYPNqYa3QuctpslYEJshjA5W0ZuryVfxkz_Tif_fswotI6HwqQj9xB6AFD3TurhjQw-A1L3HtibASM3hd7ITGWIJ63mlfyICxrUFBwQ9IqWatDp5zDPezvULUkM-MxFPIc',
  ].filter(Boolean);

  return (
    <div className="max-w-[1280px] mx-auto px-6 lg:px-8 pb-24 pt-32 text-on-background font-body-md overflow-x-hidden">
      {/* Toast Alert */}
      {toast.show && (
        <div className={`fixed top-24 right-6 z-50 px-4 py-3 rounded-lg border shadow-xl flex items-center gap-2 text-sm transition-all duration-300 ${
          toast.type === 'success' 
            ? 'bg-green-50 border-green-200 text-green-700 dark:bg-green-900/30 dark:border-green-800 dark:text-green-300' 
            : 'bg-red-50 border-red-200 text-red-700 dark:bg-red-900/30 dark:border-red-800 dark:text-red-300'
        }`}>
          <span className="material-symbols-outlined text-lg">
            {toast.type === 'success' ? 'check_circle' : 'error'}
          </span>
          {toast.msg}
        </div>
      )}

      {/* Breadcrumbs */}
      <nav className="text-xs mb-8 flex items-center gap-2 text-on-surface-variant font-medium">
        <Link to="/" className="hover:text-primary transition-colors">Home</Link>
        <span className="material-symbols-outlined text-[10px]">chevron_right</span>
        <Link to="/products" className="hover:text-primary transition-colors">Products</Link>
        <span className="material-symbols-outlined text-[10px]">chevron_right</span>
        <span className="font-bold text-primary">{product.name}</span>
      </nav>

      {/* Primary Info Grid */}
      <section className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-start mb-16">
        {/* Left Side: Images Gallery */}
        <div className="lg:col-span-7 flex flex-col gap-4">
          <div className="w-full aspect-[4/3] rounded-2xl overflow-hidden bg-surface-container relative border border-outline-variant/10 shadow-lg">
            <div className="absolute inset-0 organic-gradient opacity-30"></div>
            {selectedImage ? (
              <img
                src={selectedImage}
                alt={product.name}
                className="w-full h-full object-cover relative z-10"
              />
            ) : (
              <div className="w-full h-full flex items-center justify-center">
                <span className="material-symbols-outlined text-8xl text-primary/10 select-none">
                  science
                </span>
              </div>
            )}
            <div className="absolute top-4 left-4 z-20 flex gap-2">
              <span className="bg-primary text-white text-[10px] uppercase font-bold tracking-wider px-3 py-1 rounded-full shadow-md border border-white/10">
                {product.categoryName || 'ACTIVE'}
              </span>
              <span className="bg-white text-primary text-[10px] uppercase font-bold tracking-wider px-3 py-1 rounded-full shadow-md border border-outline-variant/20">
                Eco Safe
              </span>
            </div>
          </div>

          {/* Thumbnails grid */}
          <div className="grid grid-cols-5 gap-3">
            {imagesList.map((img, index) => (
              <button
                key={index}
                className={`aspect-square rounded-xl overflow-hidden border-2 transition-all p-0.5 bg-white dark:bg-surface-container ${
                  selectedImage === img ? 'border-primary shadow-md' : 'border-transparent hover:border-primary/40'
                }`}
                type="button"
                onClick={() => setSelectedImage(img)}
              >
                <div
                  className="w-full h-full rounded-[10px] bg-center bg-cover"
                  style={{ backgroundImage: `url('${img}')` }}
                ></div>
              </button>
            ))}
          </div>
        </div>

        {/* Right Side: Product configuration and price */}
        <div className="lg:col-span-5 bg-white dark:bg-surface-container p-8 rounded-2xl border border-outline-variant/10 shadow-xl space-y-6">
          <div>
            <h1 className="font-h1 text-3xl font-bold text-primary mb-2">{product.name}</h1>
            <div className="flex items-center gap-4 text-sm">
              <span className="text-on-surface-variant bg-surface-container-high px-2 py-0.5 rounded font-bold text-xs uppercase tracking-tight">
                SKU: {product.sku}
              </span>
              <div className="flex items-center gap-0.5">
                <span
                  className="material-symbols-outlined text-amber-500 text-sm"
                  style={{ fontVariationSettings: "'FILL' 1" }}
                >
                  star
                </span>
                <span className="text-xs text-on-surface-variant font-medium">
                  {ratingSummary.avg} ({ratingSummary.total} reviews)
                </span>
              </div>
            </div>
          </div>

          <div className="border-y border-outline-variant/10 py-4 flex items-center justify-between">
            <span className="text-2xl font-black text-primary">
              {formatPrice(product.unitPrice)}
            </span>
            <span className="text-xs text-on-surface-variant font-medium">
              Đơn vị: {product.unit || 'Chai 500ml'}
            </span>
          </div>

          {/* Availability */}
          <div className="grid grid-cols-2 gap-4 text-xs font-semibold text-on-surface-variant">
            <div className="bg-background dark:bg-surface p-4 rounded-xl border border-outline-variant/10">
              <div className="text-[10px] text-on-surface-variant/60 uppercase tracking-wider mb-1">
                Trạng thái
              </div>
              <div className={product.isActive ? 'text-green-600 font-bold' : 'text-red-600 font-bold'}>
                {product.isActive ? 'Đang kinh doanh' : 'Ngừng kinh doanh'}
              </div>
            </div>
            <div className="bg-background dark:bg-surface p-4 rounded-xl border border-outline-variant/10">
              <div className="text-[10px] text-on-surface-variant/60 uppercase tracking-wider mb-1">
                Tồn kho khả dụng
              </div>
              <div className="text-primary font-bold">120 sản phẩm</div>
            </div>
          </div>

          {/* Attributes Grid */}
          <div className="grid grid-cols-2 gap-x-4 gap-y-3.5 text-xs text-on-surface-variant pt-2">
            <div>
              <div className="text-on-surface-variant/60">Bào chế (Formulation)</div>
              <div className="font-bold text-primary">Dung dịch lỏng tinh chất</div>
            </div>
            <div>
              <div className="text-on-surface-variant/60">Độc tính (Toxicity)</div>
              <div className="font-bold text-primary">Mức 5 (An toàn hữu cơ)</div>
            </div>
            <div>
              <div className="text-on-surface-variant/60">Thời gian cách ly</div>
              <div className="font-bold text-primary">1-2 ngày</div>
            </div>
            <div>
              <div className="text-on-surface-variant/60">Tỷ lệ pha chế</div>
              <div className="font-bold text-primary">10ml / 8L Nước</div>
            </div>
          </div>

          {/* Action row */}
          {product.isActive && (
            <div className="space-y-4 pt-4 border-t border-outline-variant/10">
              <div className="flex items-center gap-3">
                <span className="text-xs font-bold text-on-surface-variant uppercase tracking-wider ml-1">
                  Số lượng:
                </span>
                <div className="flex items-center border border-outline-variant/30 rounded-lg overflow-hidden bg-background">
                  <button
                    onClick={() => setQty(q => Math.max(1, q - 1))}
                    disabled={addingToCart}
                    type="button"
                    className="w-10 h-10 flex items-center justify-center hover:bg-surface-container transition-colors font-bold text-sm disabled:opacity-50"
                  >
                    -
                  </button>
                  <input
                    type="number"
                    value={qty}
                    readOnly
                    className="w-12 h-10 border-0 bg-transparent text-center text-sm font-bold outline-none ring-0 focus:ring-0"
                  />
                  <button
                    onClick={() => setQty(q => q + 1)}
                    disabled={addingToCart}
                    type="button"
                    className="w-10 h-10 flex items-center justify-center hover:bg-surface-container transition-colors font-bold text-sm disabled:opacity-50"
                  >
                    +
                  </button>
                </div>
              </div>

              <div className="flex gap-3">
                <button
                  onClick={() => handleCartAdd(true)}
                  disabled={addingToCart}
                  type="button"
                  className="flex-1 h-12 border-2 border-primary text-primary hover:bg-primary/5 active:scale-[0.98] transition-all font-bold rounded-xl flex items-center justify-center gap-2 text-sm disabled:opacity-50"
                >
                  <span className="material-symbols-outlined text-lg">{addingToCart ? 'hourglass_empty' : 'shopping_bag'}</span>
                  {addingToCart ? 'Đang xử lý...' : 'Mua ngay'}
                </button>
                <button
                  onClick={() => handleCartAdd(false)}
                  disabled={addingToCart}
                  type="button"
                  className="flex-1 h-12 bg-primary hover:bg-[#173901] text-white active:scale-[0.98] transition-all font-bold rounded-xl shadow-lg shadow-primary/20 flex items-center justify-center gap-2 text-sm disabled:opacity-50"
                >
                  <span className="material-symbols-outlined text-lg">{addingToCart ? 'hourglass_empty' : 'add_shopping_cart'}</span>
                  {addingToCart ? 'Đang thêm...' : 'Thêm vào giỏ'}
                </button>
              </div>
            </div>
          )}
        </div>
      </section>

      {/* Description & Technical Specs */}
      <section className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-start mb-16">
        <div className="lg:col-span-7 space-y-8 bg-white dark:bg-surface-container p-8 rounded-2xl border border-outline-variant/10 shadow-xl">
          <div>
            <h3 className="font-h3 text-xl font-bold text-primary mb-3">Mô tả sản phẩm</h3>
            <div className="text-sm text-on-surface-variant font-light leading-relaxed prose max-w-none">
              {product.description ? (
                <div dangerouslySetInnerHTML={{ __html: product.description }}></div>
              ) : (
                <p>Chế phẩm sinh học đột phá kết hợp các chủng vi sinh vật đối kháng tự nhiên chuyên trị các loại sâu hại, rầy rệp sáp. Hoàn toàn lành tính, an toàn với người sử dụng và môi trường sinh thái.</p>
              )}
            </div>
          </div>

          <div className="border-t border-outline-variant/10 pt-6">
            <h3 className="font-h3 text-xl font-bold text-primary mb-3">Hướng dẫn sử dụng</h3>
            <p className="text-sm text-on-surface-variant font-light leading-relaxed">
              Pha tỷ lệ 10-15ml chế phẩm cho bình 8-10 lít nước phun sương đẫm mặt lá cây trồng vào lúc sáng sớm hoặc chiều mát khi côn trùng gây hại bắt đầu hoạt động mạnh nhất. Phun định kỳ 5-7 ngày/lần.
            </p>
          </div>

          <div className="border-t border-outline-variant/10 pt-6">
            <h3 className="font-h3 text-xl font-bold text-primary mb-3">Thông tin an toàn</h3>
            <p className="text-sm text-on-surface-variant font-light leading-relaxed">
              Sản phẩm sinh học hữu cơ lành tính không yêu cầu trang bị bảo hộ chuyên dụng khắt khe. Tránh để tiếp xúc trực tiếp quá lâu với mắt hoặc vết thương hở. Bảo quản nơi khô ráo thoáng mát, tránh ánh nắng mặt trời chiếu trực tiếp.
            </p>
          </div>
        </div>

        {/* Chemical/Microbiology Composition */}
        <div className="lg:col-span-5 bg-white dark:bg-surface-container p-8 rounded-2xl border border-outline-variant/10 shadow-xl space-y-6">
          <h3 className="font-h3 text-lg font-bold text-primary border-b border-outline-variant/10 pb-3 uppercase tracking-wider">
            Thành phần vi sinh / sinh học
          </h3>
          <ul className="space-y-4">
            <li className="flex justify-between border-b border-outline-variant/5 pb-2 text-sm">
              <span className="font-bold text-primary">Bacillus thuringiensis (Bt)</span>
              <span className="text-xs text-on-surface-variant font-semibold">10^9 CFU/g</span>
            </li>
            <li className="flex justify-between border-b border-outline-variant/5 pb-2 text-sm">
              <span className="font-bold text-primary">Beauveria bassiana</span>
              <span className="text-xs text-on-surface-variant font-semibold">10^8 CFU/g</span>
            </li>
            <li className="flex justify-between border-b border-outline-variant/5 pb-2 text-sm">
              <span className="font-bold text-primary">Metarhizium anisopliae</span>
              <span className="text-xs text-on-surface-variant font-semibold">10^8 CFU/g</span>
            </li>
            <li className="flex justify-between pb-2 text-sm">
              <span className="font-bold text-primary">Nước tinh khiết và phụ gia</span>
              <span className="text-xs text-on-surface-variant font-semibold">Vừa đủ 100%</span>
            </li>
          </ul>
        </div>
      </section>

      {/* Reviews/Feedbacks Section */}
      <section id="reviews" className="bg-white dark:bg-surface-container rounded-2xl p-8 border border-outline-variant/10 shadow-xl gap-4">
        <h3 className="font-h3 text-2xl font-bold text-primary mb-6">Khách hàng Đánh giá ({ratingSummary.total})</h3>

        {/* Top Filters Bar */}
        <div className="flex flex-wrap items-center justify-between gap-4 mb-6 border-b border-outline-variant/10 pb-4">
          <div className="flex gap-2 flex-wrap">
            <button
              onClick={() => setRatingFilter({ minRating: null, showWithImages: false })}
              className={`px-3 py-1.5 rounded-lg border text-xs font-semibold transition-all ${
                ratingFilter.minRating === null && !ratingFilter.showWithImages
                  ? 'bg-primary border-primary text-white shadow-md'
                  : 'bg-background dark:bg-surface border-outline-variant/30 text-on-surface-variant hover:border-primary/50'
              }`}
            >
              Tất cả
            </button>
            {[4, 3, 2, 1].map(r => (
              <button
                key={r}
                onClick={() => setRatingFilter({ minRating: r, showWithImages: false })}
                className={`px-3 py-1.5 rounded-lg border text-xs font-semibold transition-all ${
                  ratingFilter.minRating === r && !ratingFilter.showWithImages
                    ? 'bg-primary border-primary text-white shadow-md'
                    : 'bg-background dark:bg-surface border-outline-variant/30 text-on-surface-variant hover:border-primary/50'
                }`}
              >
                {r}+ ★
              </button>
            ))}
            <button
              onClick={() => setRatingFilter(prev => ({ ...prev, showWithImages: !prev.showWithImages, minRating: null }))}
              className={`px-3 py-1.5 rounded-lg border text-xs font-semibold transition-all flex items-center gap-1 ${
                ratingFilter.showWithImages
                  ? 'bg-primary border-primary text-white shadow-md'
                  : 'bg-background dark:bg-surface border-outline-variant/30 text-on-surface-variant hover:border-primary/50'
              }`}
            >
              <span className="material-symbols-outlined text-xs">image</span> Đánh giá có ảnh
            </button>
          </div>
          <div className="text-xs text-on-surface-variant font-medium">
            Đang hiển thị {filteredFeedbacks.length} trên {ratingSummary.total} đánh giá
          </div>
        </div>

        {/* Reviews Feed Grid */}
        <div className="grid grid-cols-1 md:grid-cols-12 gap-8 items-start mb-10">
          {/* Summary Panel */}
          <div className="md:col-span-4 bg-background dark:bg-surface p-6 rounded-2xl border border-outline-variant/10 text-center flex flex-col items-center">
            <h4 className="font-bold text-sm text-on-surface-variant uppercase tracking-wider mb-2">Điểm đánh giá trung bình</h4>
            <div className="flex items-baseline gap-1 mt-1">
              <span className="text-5xl font-black text-primary leading-none">{ratingSummary.avg}</span>
              <span className="text-sm text-on-surface-variant font-bold">/5</span>
            </div>
            <div className="mt-2.5 flex gap-0.5">
              {[1, 2, 3, 4, 5].map(i => (
                <span
                  key={i}
                  className="material-symbols-outlined text-lg text-amber-500"
                  style={{ fontVariationSettings: `'FILL' ${i <= Math.round(ratingSummary.avg) ? '1' : '0'}` }}
                >
                  star
                </span>
              ))}
            </div>
            <div className="text-xs text-on-surface-variant mt-2 font-medium">({ratingSummary.total} nhận xét từ khách đã mua)</div>
          </div>

          {/* Progress Rating Bars */}
          <div className="md:col-span-8 space-y-2.5">
            {[5, 4, 3, 2, 1].map(r => {
              const count = ratingSummary.rates[r as 5|4|3|2|1] || 0;
              const percent = ratingSummary.total === 0 ? 0 : (count / ratingSummary.total) * 100;
              return (
                <div key={r} className="flex items-center gap-3.5 text-xs text-on-surface-variant">
                  <span className="w-8 font-semibold text-right">{r}★</span>
                  <div className="flex-1 h-2.5 bg-background dark:bg-surface rounded-full overflow-hidden border border-outline-variant/10">
                    <div className="h-full bg-amber-500 rounded-full" style={{ width: `${percent}%` }}></div>
                  </div>
                  <span className="w-8 font-bold text-primary">{count}</span>
                </div>
              );
            })}
          </div>
        </div>

        {/* Feedbacks list */}
        {filteredFeedbacks.length > 0 ? (
          <div className="space-y-6 border-b border-outline-variant/10 pb-10 mb-10">
            {filteredFeedbacks.map(fb => (
              <div key={fb.id} className="border border-outline-variant/20 rounded-2xl p-5 hover:border-primary/20 transition-all bg-white dark:bg-surface-container shadow-sm flex flex-col gap-4">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 rounded-full overflow-hidden bg-primary/5 flex items-center justify-center shrink-0 border border-outline-variant/10 text-primary">
                    <span className="material-symbols-outlined text-xl">person</span>
                  </div>
                  <div className="flex-grow">
                    <div className="flex flex-wrap items-center justify-between gap-2">
                      <span className="font-bold text-primary text-sm">{fb.userName}</span>
                      <span className="text-[10px] text-on-surface-variant font-medium">
                        {new Date(fb.createdAt).toLocaleDateString('vi-VN')}
                      </span>
                    </div>

                    <div className="mt-1 flex gap-0.5">
                      {[1, 2, 3, 4, 5].map(i => (
                        <span
                          key={i}
                          className="material-symbols-outlined text-base text-amber-500"
                          style={{ fontVariationSettings: `'FILL' ${i <= fb.rating ? '1' : '0'}` }}
                        >
                          star
                        </span>
                      ))}
                    </div>

                    <p className="mt-3 text-sm text-on-background/90 leading-relaxed font-light">{fb.comment}</p>

                    {fb.images && fb.images.length > 0 && (
                      <div className="flex gap-2.5 mt-4 flex-wrap">
                        {fb.images.map((img, idx) => (
                          <img
                            key={idx}
                            src={img}
                            alt="Attached feedback"
                            onClick={() => window.open(img, '_blank')}
                            className="w-20 h-20 object-cover rounded-xl border border-outline-variant/10 cursor-pointer hover:scale-105 transition-transform duration-300"
                          />
                        ))}
                      </div>
                    )}

                    {fb.replyMessage && (
                      <div className="mt-5 bg-background dark:bg-surface p-4 rounded-xl border border-outline-variant/10 border-l-4 border-l-primary space-y-1">
                        <div className="flex items-center justify-between mb-1">
                          <div className="text-[10px] font-bold text-primary uppercase tracking-wider">BioPestControl Phản hồi</div>
                          {fb.repliedAt && (
                            <span className="text-[10px] text-on-surface-variant/70">
                              {new Date(fb.repliedAt).toLocaleDateString('vi-VN')}
                            </span>
                          )}
                        </div>
                        <p className="text-sm text-on-surface-variant font-light leading-relaxed">{fb.replyMessage}</p>
                      </div>
                    )}

                    <div className="mt-4 flex items-center gap-3 border-t border-outline-variant/10 pt-3">
                      <button
                        onClick={() => handleHelpfulClick(fb.id)}
                        type="button"
                        className="flex items-center gap-1.5 text-xs font-semibold text-on-surface-variant hover:text-primary transition-colors cursor-pointer"
                      >
                        <span className="material-symbols-outlined text-sm">thumb_up</span>
                        Hữu ích <span className="font-bold">({fb.helpfulCount})</span>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-12 text-on-surface-variant font-light border border-outline-variant/20 rounded-2xl bg-background dark:bg-surface flex flex-col items-center justify-center mb-10">
            <span className="material-symbols-outlined text-4xl text-on-surface-variant/40 mb-2">rate_review</span>
            Chưa có đánh giá nào phù hợp bộ lọc.
          </div>
        )}

        {/* Submit Review Form */}
        <div className="mt-10 border-t border-outline-variant/10 pt-8">
          <h4 className="font-h3 text-xl font-bold text-primary mb-4">Gửi đánh giá của bạn</h4>

          {!isAuthenticated ? (
            <div className="bg-background dark:bg-surface p-6 rounded-xl border border-outline-variant/10 flex flex-col items-center text-center gap-4">
              <p className="text-sm text-on-surface-variant font-light">Bạn cần đăng nhập để gửi đánh giá cho sản phẩm này.</p>
              <Link to="/login" className="bg-primary text-white font-bold px-6 py-2.5 rounded-lg text-xs shadow-md">Đăng nhập ngay</Link>
            </div>
          ) : (
            <form onSubmit={handleAddReview} className="space-y-5 max-w-2xl">
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Chọn đơn hàng đã giao</label>
                <select
                  value={formOrderId}
                  onChange={(e) => setFormOrderId(e.target.value)}
                  className="w-full border border-outline-variant/30 rounded-xl p-3 text-sm focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface focus:outline-none cursor-pointer"
                  required
                >
                  <option value="">Chọn đơn hàng chứa sản phẩm này...</option>
                  <option value="90012">Đơn hàng #90012 - Đã giao ngày 15/05/2026</option>
                  <option value="90054">Đơn hàng #90054 - Đã giao ngày 22/05/2026</option>
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Đánh giá sao</label>
                <div className="flex gap-1" id="starRating">
                  {[1, 2, 3, 4, 5].map(i => (
                    <button
                      key={i}
                      type="button"
                      onClick={() => setFormRating(i)}
                      className={`material-symbols-outlined text-4xl hover:scale-110 transition-transform ${
                        i <= formRating ? 'text-amber-500' : 'text-slate-300'
                      }`}
                      style={{ fontVariationSettings: `'FILL' ${i <= formRating ? '1' : '0'}` }}
                    >
                      star
                    </button>
                  ))}
                </div>
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-2 ml-1">Bình luận nhận xét</label>
                <textarea
                  value={formComment}
                  onChange={(e) => setFormComment(e.target.value)}
                  rows={4}
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

              <div className="flex justify-end">
                <button type="submit" className="bg-primary hover:bg-[#173901] text-white font-bold px-8 py-3 rounded-xl text-sm shadow-md">Gửi nhận xét</button>
              </div>
            </form>
          )}
        </div>
      </section>
    </div>
  );
};

export default ProductDetailsPage;
