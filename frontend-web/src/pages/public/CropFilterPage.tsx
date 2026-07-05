import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import './CropFilterPage.css';

const MOCK_CROPS = [
  {
    id: 'lua',
    name: 'Lúa (Rice)',
    image: 'https://images.unsplash.com/photo-1595856985285-0d297ff0361a?auto=format&fit=crop&q=80&w=800',
    description: 'Cây lương thực chính yếu, dễ gặp sâu đục thân, rầy nâu, đạo ôn.',
    products: [
      { id: 50, name: 'Regent', reason: 'Đặc trị rầy nâu, sâu đục thân hại lúa.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0050.jpg' },
      { id: 47, name: 'KEEP 300SC', reason: 'Đặc trị bệnh đạo ôn trên lúa.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0047.jpg' },
      { id: 3, name: 'TT SNAILTA GOLD', reason: 'Trừ ốc bươu vàng hại lúa non.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0003.jpg' }
    ]
  },
  {
    id: 'cay-an-trai',
    name: 'Cây Ăn Trái (Fruit Trees)',
    image: 'https://images.unsplash.com/photo-1601002242139-2ce137eec260?auto=format&fit=crop&q=80&w=800',
    description: 'Cây xoài, sầu riêng, cam bưởi... Cần nhiều vi lượng để nuôi trái, ra hoa.',
    products: [
      { id: 1, name: 'Vi lượng-BOROZINC', reason: 'Cung cấp Bo và Kẽm giúp đậu trái, chống rụng hoa.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0001.jpg' },
      { id: 4, name: 'TANO_606', reason: 'Kích thích ra hoa sớm, đồng loạt.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0004.jpg' },
      { id: 22, name: 'NPK HÀN VIỆT 20 20 15 TE', reason: 'Cung cấp dinh dưỡng NPK nuôi trái lớn.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0022.jpg' }
    ]
  },
  {
    id: 'rau-mau',
    name: 'Rau Màu (Vegetables)',
    image: 'https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?auto=format&fit=crop&q=80&w=800',
    description: 'Rau ăn lá, rau ăn củ. Ưu tiên các dòng thuốc sinh học an toàn.',
    products: [
      { id: 2, name: 'TT-ANONIN 1EC', reason: 'Thuốc sinh học 100% thảo mộc, an toàn, cách ly ngắn.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0002.jpg' },
      { id: 20, name: 'ORGANIC NOKAYO', reason: 'Phân hữu cơ làm tơi xốp đất, bổ sung mùn.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0020.jpg' },
      { id: 42, name: 'ZIN 80 WP', reason: 'Phòng trừ các loại nấm bệnh trên rau màu.', imageUrl: 'https://res.cloudinary.com/biopestcontrol/image/upload/products/sp0042.jpg' }
    ]
  }
];

export const CropFilterPage: React.FC = () => {
  const { t } = useTranslation();

  return (
    <div className="crop-filter-container public-container">
      <div className="crop-filter-header" style={{ marginTop: '2rem', marginBottom: '2rem' }}>
        <h1 style={{ fontSize: '2.5rem', color: 'var(--primary-color)' }}>{t('crops', 'Crops')}</h1>
        <p style={{ color: 'var(--text-light)', fontSize: '1.1rem' }}>Chọn cây trồng của bạn để tìm các sản phẩm phù hợp nhất.</p>
      </div>

      <div className="crop-list">
        {MOCK_CROPS.map(crop => (
          <div key={crop.id} className="crop-row">
            <div className="crop-info">
              <img src={crop.image} alt={crop.name} className="crop-image" />
              <div className="crop-details">
                <h2 style={{ fontSize: '1.8rem', color: 'var(--primary-dark)', marginBottom: '0.5rem' }}>{crop.name}</h2>
                <p style={{ color: 'var(--text-color)', marginBottom: '1rem', lineHeight: 1.5 }}>{crop.description}</p>
                <Link to={`/crops/${crop.id}`} className="view-more-btn">
                  Xem chi tiết & Toàn bộ SP
                </Link>
              </div>
            </div>

            <div className="crop-products-scroller">
              <div className="crop-products">
                {crop.products.map(prod => (
                  <div key={prod.id} className="crop-product-card">
                    <img src={prod.imageUrl} alt={prod.name} />
                    <div className="crop-product-info">
                      <h4 title={prod.name}>{prod.name}</h4>
                      <p className="reason">{prod.reason}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
