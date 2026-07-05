import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { cropService, CropProfileResponse } from '../../services/cropService';
import './CropFilterPage.css';

export const CropFilterPage: React.FC = () => {
  const { t } = useTranslation();
  const [crops, setCrops] = useState<CropProfileResponse[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCrops = async () => {
      try {
        const cropList = await cropService.getAllCrops();
        // Lấy chi tiết từng cây để có danh sách products
        const detailedCrops = await Promise.all(
          cropList.map(c => cropService.getCropBySlug(c.slug))
        );
        setCrops(detailedCrops);
      } catch (error) {
        console.error("Lỗi khi tải danh sách cây trồng:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchCrops();
  }, []);

  if (loading) {
    return <div className="public-container" style={{ padding: '2rem' }}>Đang tải danh sách cây trồng...</div>;
  }

  return (
    <div className="crop-filter-container public-container">
      <div className="crop-filter-header" style={{ marginTop: '2rem', marginBottom: '2rem' }}>
        <h1 style={{ fontSize: '2.5rem', color: 'var(--primary-color)' }}>{t('crops', 'Crops')}</h1>
        <p style={{ color: 'var(--text-light)', fontSize: '1.1rem' }}>Chọn cây trồng của bạn để tìm các sản phẩm phù hợp nhất.</p>
      </div>

      <div className="crop-list">
        {crops.map(crop => (
          <div key={crop.id} className="crop-row">
            <div className="crop-info">
              <img src={crop.imageUrl} alt={crop.name} className="crop-image" />
              <div className="crop-details">
                <h2 style={{ fontSize: '1.8rem', color: 'var(--primary-dark)', marginBottom: '0.5rem' }}>{crop.name}</h2>
                <p style={{ color: 'var(--text-color)', marginBottom: '1rem', lineHeight: 1.5 }}>{crop.description}</p>
                <Link to={`/crops/${crop.slug}`} className="view-more-btn">
                  Xem chi tiết & Toàn bộ SP
                </Link>
              </div>
            </div>

            <div className="crop-products-scroller">
              <div className="crop-products">
                {crop.products.map(prod => (
                  <Link key={prod.productId} to={`/products/${prod.productId}`} className="crop-product-card" style={{ textDecoration: 'none' }}>
                    <img src={prod.productImageUrl || 'https://via.placeholder.com/150'} alt={prod.productName} />
                    <div className="crop-product-info">
                      <h4 title={prod.productName}>{prod.productName}</h4>
                      <p className="reason">{prod.usageInstruction}</p>
                    </div>
                  </Link>
                ))}
                {crop.products.length === 0 && (
                  <div style={{ padding: '1rem', color: '#666' }}>Đang cập nhật sản phẩm...</div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
