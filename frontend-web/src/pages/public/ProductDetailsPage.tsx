import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { productService } from '../../services/productService';
import type { Product } from '../../types/catalog';
import { useAddToCart } from '../../hooks/useAddToCart';

const ProductDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { handleAddToCart } = useAddToCart();

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);

  // Gallery state
  const [selectedImage, setSelectedImage] = useState<string>('');

  const [qty, setQty] = useState(1);
  const [addingToCart, setAddingToCart] = useState(false);

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
      } catch (err) {
        console.error('Error loading product details', err);
        navigate('/products');
      } finally {
        setLoading(false);
      }
    };
    loadDetails();
  }, [id, navigate]);





  const handleCartAdd = async (isBuyNow: boolean) => {
    if (!product) return;
    setAddingToCart(true);
    try {
      await handleAddToCart(product, { quantity: qty, buyNow: isBuyNow });
    } finally {
      setAddingToCart(false);
    }
  };



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
                {product.categoryName || 'Uncategorized'}
              </span>
              {product.chemicalName && (
                <span className="bg-white text-primary text-[10px] uppercase font-bold tracking-wider px-3 py-1 rounded-full shadow-md border border-outline-variant/20">
                  {product.chemicalName}
                </span>
              )}
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
                  5.0 (0 reviews)
                </span>
              </div>
            </div>
          </div>

          <div className="border-y border-outline-variant/10 py-4 flex items-center justify-between">
            <span className="text-2xl font-black text-primary">
              {formatPrice(product.unitPrice)}
            </span>
            <span className="text-xs text-on-surface-variant font-medium">
              Đơn vị: {product.unit || 'Chai'}
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


    </div>
  );
};

export default ProductDetailsPage;
