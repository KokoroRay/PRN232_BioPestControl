import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoryService } from '../../services/categoryService';
import { productService } from '../../services/productService';
import { useAddToCart } from '../../hooks/useAddToCart';
import type { Category, Product } from '../../types/catalog';
import screen1 from '../../assets/screen.1.png';
import screen2 from '../../assets/screen,2.png';

const getCategoryDetails = (name: string) => {
  const lowercaseName = name.toLowerCase();
  if (lowercaseName.includes('sâu')) {
    return { icon: 'pest_control', label: 'Pesticides', desc: 'Eliminate pests with beneficial bacteria.' };
  }
  if (lowercaseName.includes('bón') || lowercaseName.includes('hóa') || lowercaseName.includes('chất')) {
    return { icon: 'compost', label: 'Fertilizers', desc: 'Organic nutrition for natural growth.' };
  }
  if (lowercaseName.includes('cỏ') || lowercaseName.includes('đất')) {
    return { icon: 'landslide', label: 'Soil Improvement', desc: 'Balance pH and enrich soil health.' };
  }
  return { icon: 'microbiology', label: 'Microorganisms', desc: 'Biological protection for roots and leaves.' };
};

const HomePage: React.FC = () => {
  const { handleAddToCart } = useAddToCart();
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [addingToCartId, setAddingToCartId] = useState<number | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const [c, p] = await Promise.all([categoryService.getAll(), productService.getAll()]);
        setCategories(c);
        setProducts(p.items.filter((item: any) => item.isActive));
      } catch {
        setCategories([]);
        setProducts([]);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const featuredProducts = useMemo(() => {
    const shuffled = [...products].slice().sort(() => Math.random() - 0.5);
    return shuffled.slice(0, 6); // Standard grid is 3 columns, so 6 is ideal
  }, [products]);

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);

  return (
    <div id="top" className="bg-background text-on-background font-body-md overflow-x-hidden">
      {/* Hero Section */}
      <header className="pt-32 pb-20 relative overflow-hidden min-h-[600px] flex items-center">
        <div className="absolute inset-0 z-0">
          <img
            alt="Healthy crop field at sunrise"
            className="w-full h-full object-cover"
            src={screen1}
          />
          <div className="absolute inset-0 bg-gradient-to-b from-primary/60 via-primary/40 to-surface"></div>
        </div>
        <div className="max-w-[1280px] mx-auto px-6 text-center relative z-10">
          <div className="inline-flex items-center gap-2 bg-secondary-container/50 text-on-secondary-container px-4 py-1.5 rounded-full text-xs font-bold mb-8">
            <span
              className="material-symbols-outlined text-[14px]"
              style={{ fontVariationSettings: "'FILL' 1" }}
            >
              verified
            </span>{' '}
            #1 Biological Platform in Vietnam
          </div>
          <h1 className="font-h1 text-h1 mb-6 leading-tight max-w-4xl mx-auto text-white drop-shadow-md">
            Protect crops <span className="italic font-normal">naturally</span> — <br />
            real effectiveness
          </h1>
          <p className="font-body-lg text-body-lg mb-10 max-w-2xl mx-auto text-white/90 drop-shadow-sm">
            Advanced biological solutions replacing harmful chemicals with the power of nature.
          </p>
          <div className="flex flex-wrap justify-center gap-4 mb-20">
            <button
              onClick={() => navigate('/products')}
              className="bg-secondary text-on-secondary px-8 py-4 rounded-xl font-bold flex items-center gap-2 transition-all hover:shadow-md cursor-pointer"
            >
              Shop Now{' '}
              <span className="material-symbols-outlined">
                arrow_forward
              </span>
            </button>
            <button
              onClick={() => navigate('/products')}
              className="bg-white border border-outline-variant text-primary px-8 py-4 rounded-xl font-bold hover:bg-surface-container transition-all cursor-pointer"
            >
              View Categories
            </button>
          </div>
          <div className="grid grid-cols-3 max-w-2xl mx-auto pt-10 border-t border-white/20">
            <div>
              <div className="font-h3 text-h3 text-white">500+</div>
              <div className="text-sm uppercase tracking-wider font-bold mt-1 text-white/70">
                Farms
              </div>
            </div>
            <div>
              <div className="font-h3 text-h3 text-white">2.000+</div>
              <div className="text-sm uppercase tracking-wider font-bold mt-1 text-white/70">
                Products
              </div>
            </div>
            <div>
              <div className="font-h3 text-h3 text-white">15+</div>
              <div className="text-sm uppercase tracking-wider font-bold mt-1 text-white/70">
                Certifications
              </div>
            </div>
          </div>
        </div>
      </header>

      {/* Trust Bar */}
      <section className="bg-primary/5 py-12 border-y border-outline-variant/20">
        <div className="max-w-[1280px] mx-auto px-6 flex flex-wrap justify-around gap-8">
          <div className="flex items-center gap-3 text-primary">
            <span className="material-symbols-outlined">
              workspace_premium
            </span>
            <span className="font-medium text-sm">Organic Certified</span>
          </div>
          <div className="flex items-center gap-3 text-primary">
            <span className="material-symbols-outlined">
              local_shipping
            </span>
            <span className="font-medium text-sm">Nationwide Delivery</span>
          </div>
          <div className="flex items-center gap-3 text-primary">
            <span className="material-symbols-outlined">
              assignment_return
            </span>
            <span className="font-medium text-sm">30-Day Returns</span>
          </div>
          <div className="flex items-center gap-3 text-primary">
            <span className="material-symbols-outlined">
              support_agent
            </span>
            <span className="font-medium text-sm">24/7 Technical Support</span>
          </div>
        </div>
      </section>

      {/* Categories Section */}
      <section className="py-24 bg-surface">
        <div className="max-w-[1280px] mx-auto px-6">
          <div className="flex justify-between items-end mb-16">
            <div>
              <h2 className="font-h2 text-h2 text-primary mb-2">Explore Categories</h2>
              <p className="text-on-surface-variant">Specialized solutions for each growth stage</p>
            </div>
            <button
              onClick={() => navigate('/products')}
              className="text-secondary font-bold flex items-center gap-1 hover:underline cursor-pointer bg-transparent border-none"
            >
              View All{' '}
              <span className="material-symbols-outlined">
                east
              </span>
            </button>
          </div>
          {loading ? (
            <div className="flex justify-center items-center py-20">
              <span className="material-symbols-outlined text-4xl animate-spin text-primary">hourglass_empty</span>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {categories.map((category) => {
              const details = getCategoryDetails(category.name);
              return (
                <div
                  key={category.id}
                  onClick={() => navigate(`/products?categoryId=${category.id}`)}
                  className="group cursor-pointer border border-outline-variant/30 rounded-xl p-8 hover:bg-surface-container-low transition-all"
                >
                  <div className="w-12 h-12 bg-primary/5 rounded-full flex items-center justify-center text-primary mb-6 transition-transform group-hover:-translate-y-1">
                    <span className="material-symbols-outlined text-2xl">
                      {details.icon}
                    </span>
                  </div>
                  <h3 className="font-h3 text-xl text-primary mb-2">{category.name}</h3>
                  <p className="text-sm text-on-surface-variant">
                    {category.description || details.desc}
                  </p>
                </div>
              );
            })}
          </div>
          )}
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-24 bg-surface-container-lowest border-y border-outline-variant/10">
        <div className="max-w-[1280px] mx-auto px-6">
          <div className="text-center mb-16">
            <h2 className="font-h2 text-h2 text-primary mb-6">Featured Products</h2>
            <div className="flex justify-center gap-2">
              <button className="px-6 py-2 bg-primary text-on-primary rounded-full font-bold text-sm cursor-pointer">
                Best Sellers
              </button>
              <button className="px-6 py-2 bg-white text-on-surface-variant border border-outline-variant rounded-full font-bold text-sm hover:border-primary transition-all cursor-pointer">
                New Arrivals
              </button>
              <button className="px-6 py-2 bg-white text-on-surface-variant border border-outline-variant rounded-full font-bold text-sm hover:border-primary transition-all cursor-pointer">
                Offers
              </button>
            </div>
          </div>
          {loading ? (
            <div className="flex justify-center items-center py-20">
              <span className="material-symbols-outlined text-4xl animate-spin text-primary">hourglass_empty</span>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-12">
            {featuredProducts.map((product) => {
              const fallbackIcon =
                product.id % 3 === 0
                  ? 'biotech'
                  : product.id % 3 === 1
                  ? 'energy_savings_leaf'
                  : 'science';

              const rating = ((product.id % 5) / 10 + 4.5).toFixed(1);
              const reviews = (product.id * 17) % 150 + 10;

              return (
                <div key={product.id} className="group">
                  <div className="relative aspect-square rounded-xl overflow-hidden bg-surface-container mb-6 border border-outline-variant/10">
                    <div className="absolute inset-0 organic-gradient opacity-60"></div>
                    <div className="absolute inset-0 flex items-center justify-center">
                      {product.imageUrl ? (
                        <img
                          src={product.imageUrl}
                          alt={product.name}
                          className="w-full h-full object-cover relative z-10"
                        />
                      ) : (
                        <span className="material-symbols-outlined text-7xl text-primary/10">
                          {fallbackIcon}
                        </span>
                      )}
                    </div>
                    <span className="absolute top-4 left-4 bg-white/90 px-2 py-1 rounded text-[10px] font-bold tracking-tight text-primary shadow-sm">
                      {product.categoryName || 'BIO-ACTIVE'}
                    </span>
                  </div>
                  <h3 className="font-h3 text-lg text-primary mb-1">{product.name}</h3>
                  <div className="flex items-center gap-1 mb-4">
                    <span
                      className="material-symbols-outlined text-amber-500 text-sm"
                      style={{ fontVariationSettings: "'FILL' 1" }}
                    >
                      star
                    </span>
                    <span className="text-xs text-on-surface-variant">
                      {rating} ({reviews})
                    </span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-xl font-bold text-primary">
                      {formatPrice(product.unitPrice)}
                    </span>
                    <button
                      type="button"
                      disabled={addingToCartId === product.id}
                      className="p-3 border border-primary text-primary rounded-lg hover:bg-primary hover:text-white transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                      onClick={async () => {
                        setAddingToCartId(product.id);
                        await handleAddToCart(product);
                        setAddingToCartId(null);
                      }}
                    >
                      <span className="material-symbols-outlined text-xl">
                        {addingToCartId === product.id ? 'hourglass_empty' : 'add_shopping_cart'}
                      </span>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
          )}
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="py-24 bg-surface">
        <div className="max-w-[1280px] mx-auto px-6 grid lg:grid-cols-2 gap-20 items-center">
          <div>
            <h2 className="font-h2 text-h2 text-primary mb-8 leading-tight">
              Why do farms trust BioPestControl?
            </h2>
            <div className="space-y-10">
              <div className="flex gap-6">
                <div className="w-12 h-12 bg-secondary-container/30 text-secondary rounded-xl flex items-center justify-center shrink-0">
                  <span className="material-symbols-outlined">
                    health_and_safety
                  </span>
                </div>
                <div>
                  <h4 className="font-h3 text-lg text-primary mb-2">Eco-Friendly &amp; Safe</h4>
                  <p className="text-on-surface-variant">
                    Solutions safe for humans, pets, and the local ecosystem.
                  </p>
                </div>
              </div>
              <div className="flex gap-6">
                <div className="w-12 h-12 bg-secondary-container/30 text-secondary rounded-xl flex items-center justify-center shrink-0">
                  <span className="material-symbols-outlined">
                    lab_research
                  </span>
                </div>
                <div>
                  <h4 className="font-h3 text-lg text-primary mb-2">Advanced Biotech</h4>
                  <p className="text-on-surface-variant">
                    Utilizing the latest nano-formulations and microorganisms.
                  </p>
                </div>
              </div>
              <div className="flex gap-6">
                <div className="w-12 h-12 bg-secondary-container/30 text-secondary rounded-xl flex items-center justify-center shrink-0">
                  <span className="material-symbols-outlined">
                    speed
                  </span>
                </div>
                <div>
                  <h4 className="font-h3 text-lg text-primary mb-2">Efficient &amp; Low Cost</h4>
                  <p className="text-on-surface-variant">
                    High concentration reduces costs and spray frequency.
                  </p>
                </div>
              </div>
            </div>
          </div>
          <div className="relative">
            <div className="aspect-[4/3] rounded-[40px] organic-gradient overflow-hidden border border-outline-variant/20 shadow-2xl shadow-primary/5 flex items-center justify-center">
              <img
                alt="Thriving organic vegetable farm"
                className="w-full h-full object-cover"
                src={screen2}
              />
            </div>
            <div className="absolute -bottom-6 -left-6 bg-white p-6 rounded-2xl shadow-xl border border-outline-variant/10 max-w-xs">
              <p className="text-sm font-medium italic text-primary">
                "Since using BioGuard, my orange orchard has no chemical residue while yields have
                still increased by 20%."
              </p>
              <p className="text-xs text-on-surface-variant mt-4 font-bold">
                — Mr. Minh, Hoa Binh Farm
              </p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
