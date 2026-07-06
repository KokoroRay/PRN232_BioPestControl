import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { cropService } from '../../services/cropService';
import type { CropProfileResponse } from '../../services/cropService';
import { productService } from '../../services/productService';
import type { Product } from '../../types/catalog';
import './CropProfilePage.css';

export const CropProfilePage: React.FC = () => {
  const { id } = useParams<{ id: string }>(); // in dynamic, this is slug
  const { addToCart } = useCart();
  
  const [crop, setCrop] = useState<CropProfileResponse | null>(null);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCropAndProducts = async () => {
      if (!id) return;
      try {
        setLoading(true);
        // Get crop details by slug
        const cropData = await cropService.getCropBySlug(id);
        setCrop(cropData);

        if (cropData.products.length > 0) {
          // fetch products details
          const productIds = cropData.products.map(p => p.productId);
          const response = await productService.getAll({ pageSize: 100 });
          const allProds = response.items || response; 
          const filtered = (allProds as Product[]).filter(p => productIds.includes(p.id));
          setProducts(filtered);
        } else {
          setProducts([]);
        }
      } catch (err) {
        console.error('Failed to fetch crop profile', err);
      } finally {
        setLoading(false);
      }
    };
    fetchCropAndProducts();
  }, [id]);

  if (loading && !crop) {
    return (
      <div className="public-container" style={{ padding: '4rem 0', textAlign: 'center' }}>
        <h2>Đang tải thông tin cây trồng...</h2>
      </div>
    );
  }

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
      <div className="crop-hero" style={{ backgroundImage: `url(${crop.imageUrl})` }}>
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
        ) : products.length === 0 ? (
          <div className="no-products">
            <p>Không tìm thấy sản phẩm phù hợp nào trong hệ thống.</p>
          </div>
        ) : (
          <div className="crop-products-grouped">
            {Object.entries(
              products.reduce((acc, prod) => {
                const category = prod.category?.name || 'Sản phẩm khác';
                if (!acc[category]) acc[category] = [];
                acc[category].push(prod);
                return acc;
              }, {} as Record<string, Product[]>)
            ).map(([categoryName, catProducts]) => (
              <div key={categoryName} className="crop-product-category-section">
                <h3 className="category-section-title">{categoryName}</h3>
                <div className="crop-products-grid">
                  {catProducts.map(prod => (
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
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
