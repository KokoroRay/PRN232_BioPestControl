import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { productService } from '../../services/productService';
import type { Product } from '../../types/catalog';
import './CropProfilePage.css';

const MOCK_CROPS = {
  'lua': {
    name: 'Lúa (Rice)',
    image: 'https://images.unsplash.com/photo-1595856985285-0d297ff0361a?auto=format&fit=crop&q=80&w=1200',
    description: 'Cây lương thực chính yếu, dễ gặp sâu đục thân, rầy nâu, đạo ôn. Cần được chăm sóc kỹ lưỡng qua các giai đoạn: đẻ nhánh, làm đòng, trổ bông.',
    productIds: [50, 65, 3, 32, 47, 75] // Regent, Lúa Vàng, TT SNAILTA, LACASOTO, KEEP 300SC, CHUBECA
  },
  'cay-an-trai': {
    name: 'Cây Ăn Trái (Fruit Trees)',
    image: 'https://images.unsplash.com/photo-1601002242139-2ce137eec260?auto=format&fit=crop&q=80&w=1200',
    description: 'Bao gồm các loại cây như xoài, sầu riêng, bưởi, cam. Rất cần các nguyên tố vi lượng (Bo, Kẽm) để hỗ trợ quá trình ra hoa, đậu trái và chống rụng trái sinh lý.',
    productIds: [1, 4, 22, 36, 40] 
  },
  'rau-mau': {
    name: 'Rau Màu (Vegetables)',
    image: 'https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?auto=format&fit=crop&q=80&w=1200',
    description: 'Các loại rau ăn lá, ăn củ. Thường xuyên bị côn trùng chích hút và các bệnh do nấm. Khuyến nghị sử dụng các dòng thuốc sinh học an toàn, có thời gian cách ly ngắn.',
    productIds: [2, 20, 42, 44, 49]
  }
};

export const CropProfilePage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { addToCart } = useCart();
  
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  const crop = id ? MOCK_CROPS[id as keyof typeof MOCK_CROPS] : null;

  useEffect(() => {
    const fetchProducts = async () => {
      if (!crop) return;
      try {
        setLoading(true);
        // Lấy tất cả sản phẩm, có thể tối ưu hơn nếu backend hỗ trợ filter theo mảng ID
        const response = await productService.getAll({ pageSize: 100 });
        const allProds = response.items || response; // handle both array and paginated format safely
        
        const filtered = (allProds as Product[]).filter(p => crop.productIds.includes(p.id));
        setProducts(filtered);
      } catch (err) {
        console.error('Failed to fetch products for crop', err);
      } finally {
        setLoading(false);
      }
    };
    fetchProducts();
  }, [crop]);

  if (!crop) {
    return (
      <div className="public-container" style={{ padding: '4rem 0', textAlign: 'center' }}>
        <h2>Không tìm thấy cây trồng</h2>
        <Link to="/crops">Quay lại danh sách</Link>
      </div>
    );
  }

  return (
    <div className="crop-profile-page">
      <div className="crop-hero" style={{ backgroundImage: `url(${crop.image})` }}>
        <div className="crop-hero-overlay">
          <div className="public-container">
            <Link to="/crops" className="back-link"><ArrowLeft size={20} /> Trở về danh sách</Link>
            <h1>{crop.name}</h1>
            <p>{crop.description}</p>
          </div>
        </div>
      </div>

      <div className="public-container crop-profile-content">
        <div className="section-header">
          <h2>Sản phẩm khuyên dùng cho {crop.name}</h2>
          <div className="title-underline"></div>
        </div>

        {loading ? (
          <div className="loading-spinner">Đang tải sản phẩm...</div>
        ) : (
          <div className="crop-products-grid">
            {products.map(prod => (
              <div key={prod.id} className="product-card">
                <Link to={`/products/${prod.id}`} className="product-image-wrap">
                  <img src={prod.imageUrl} alt={prod.name} />
                  {!prod.isActive && <span className="status-badge inactive">Hết hàng</span>}
                </Link>
                <div className="product-info">
                  <Link to={`/products/${prod.id}`}>
                    <h3 className="product-title" title={prod.name}>{prod.name}</h3>
                  </Link>
                  <div className="product-price">
                    {prod.unitPrice.toLocaleString('vi-VN')} ₫
                  </div>
                  <button 
                    className="add-to-cart-btn" 
                    onClick={() => addToCart(prod, 1)}
                    disabled={!prod.isActive}
                  >
                    Thêm vào giỏ
                  </button>
                </div>
              </div>
            ))}
            
            {products.length === 0 && !loading && (
              <div className="no-products">
                <p>Không tìm thấy sản phẩm phù hợp nào trong hệ thống.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};
