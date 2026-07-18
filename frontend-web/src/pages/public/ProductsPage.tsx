import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { productService } from '../../services/productService';
import { categoryService } from '../../services/categoryService';
import { useAddToCart } from '../../hooks/useAddToCart';
import type { Product, Category } from '../../types/catalog';

const ITEMS_PER_PAGE = 12;

const ProductsPage: React.FC = () => {
  const navigate = useNavigate();
  const { handleAddToCart } = useAddToCart();
  const [searchParams, setSearchParams] = useSearchParams();

  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalProducts, setTotalProducts] = useState(0);

  // Get filter state from query parameters
  const searchQuery = searchParams.get('search') || '';
  const selectedCategoryId = searchParams.get('categoryId') ? Number(searchParams.get('categoryId')) : null;
  const currentSort = searchParams.get('sort') || 'relevance';
  const currentPage = Number(searchParams.get('page') || '1');

  // Real-time search state
  const [realtimeSearch, setRealtimeSearch] = useState(searchQuery);

  // Debounce timer for real-time search
  useEffect(() => {
    const timer = setTimeout(() => {
      if (realtimeSearch !== searchQuery) {
        const params = new URLSearchParams(searchParams);
        if (realtimeSearch.trim()) {
          params.set('search', realtimeSearch);
        } else {
          params.delete('search');
        }
        params.delete('page'); // Reset to page 1 when search changes
        setSearchParams(params);
      }
    }, 300);
    return () => clearTimeout(timer);
  }, [realtimeSearch, searchQuery, searchParams, setSearchParams]);

  // Load products and categories on mount or when filters change
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        let sortBy = '';
        let ascending = true;
        if (currentSort === 'price-low-to-high') {
          sortBy = 'price';
          ascending = true;
        } else if (currentSort === 'price-high-to-low') {
          sortBy = 'price';
          ascending = false;
        }

        const [cList, pList] = await Promise.all([
          categoryService.getAll(),
          productService.getAll({
            name: searchQuery || undefined,
            categoryId: selectedCategoryId || undefined,
            sortBy: sortBy || undefined,
            ascending,
            page: currentPage,
            pageSize: ITEMS_PER_PAGE
          })
        ]);
        setCategories(cList);
        setProducts(pList.items.filter(p => p.isActive));
        setTotalProducts(pList.totalCount);
      } catch (err) {
        console.error('Error loading catalog data', err);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, [searchQuery, selectedCategoryId, currentSort, currentPage]);

  const paginatedProducts = products;
  const totalPages = Math.ceil(totalProducts / ITEMS_PER_PAGE);

  const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const data = new FormData(e.currentTarget);
    const searchVal = data.get('search') as string;

    const params = new URLSearchParams(searchParams);
    if (searchVal.trim()) {
      params.set('search', searchVal);
    } else {
      params.delete('search');
    }
    params.delete('page'); // Reset to page 1
    setSearchParams(params);
    setRealtimeSearch(searchVal);
  };

  const handleCategorySelect = (catId: number | null) => {
    const params = new URLSearchParams(searchParams);
    if (catId !== null) {
      params.set('categoryId', String(catId));
    } else {
      params.delete('categoryId');
    }
    params.delete('page'); // Reset to page 1
    setSearchParams(params);
  };

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const params = new URLSearchParams(searchParams);
    params.set('sort', e.target.value);
    params.delete('page'); // Reset to page 1
    setSearchParams(params);
  };

  const handlePageChange = (page: number) => {
    const params = new URLSearchParams(searchParams);
    params.set('page', String(page));
    setSearchParams(params);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleClearAll = () => {
    setSearchParams({});
    setRealtimeSearch('');
  };

  const handleBuyNow = async (e: React.MouseEvent, p: Product) => {
    e.preventDefault();
    e.stopPropagation();
    await handleAddToCart(p, { buyNow: true });
  };

  const handleAddToCartClick = async (e: React.MouseEvent, p: Product) => {
    e.preventDefault();
    e.stopPropagation();
    await handleAddToCart(p);
  };

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);

  return (
    <div className="max-w-[1280px] mx-auto px-6 lg:px-8 pb-24 pt-32 text-on-background font-body-md overflow-x-hidden">
      {/* Header & Search */}
      <div className="mb-10">
        <h1 className="font-h1 text-4xl font-bold text-primary mb-6">Biological Solutions Catalog</h1>
        <div className="bg-white dark:bg-surface-container p-4 rounded-2xl border border-outline-variant/20 shadow-xl">
          <form onSubmit={handleSearch} className="relative group">
            <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-on-surface-variant">
              <span className="material-symbols-outlined text-xl">search</span>
            </div>
            <input
              name="search"
              value={realtimeSearch}
              onChange={(e) => setRealtimeSearch(e.target.value)}
              className="block w-full pl-11 pr-24 py-4 bg-background dark:bg-surface border border-outline-variant/30 focus:border-primary focus:bg-white dark:focus:bg-black/30 rounded-xl text-on-background placeholder-on-surface-variant/60 focus:outline-none focus:ring-1 focus:ring-primary/20 transition-all text-base"
              placeholder="Search by product name, target pest, or SKU code..."
              type="text"
            />
            <div className="absolute inset-y-0 right-2 flex items-center">
              <button
                type="submit"
                className="bg-primary hover:bg-[#173901] text-white font-bold h-10 px-6 rounded-lg transition-colors flex items-center gap-2 text-sm shadow-md"
              >
                Search
              </button>
            </div>
          </form>
        </div>
      </div>

      <div className="flex flex-col lg:flex-row gap-8 items-start">
        {/* Sidebar Filters */}
        <aside className="w-full lg:w-72 flex-shrink-0">
          <div className="sticky top-32 space-y-6">
            <div className="bg-white dark:bg-surface-container rounded-xl border border-outline-variant/20 p-6 shadow-xl">
              <div className="flex items-center justify-between mb-4 border-b border-outline-variant/10 pb-3">
                <h3 className="font-bold text-lg text-primary">Filters</h3>
                <button
                  onClick={handleClearAll}
                  className="text-xs text-secondary font-medium hover:underline cursor-pointer"
                >
                  Clear all
                </button>
              </div>

              {/* Categories Checklist */}
              <div>
                <h4 className="text-xs font-bold text-on-surface-variant uppercase tracking-wider mb-4">
                  Category
                </h4>
                <div className="space-y-3">
                  <label className="flex items-center gap-3 cursor-pointer group">
                    <input
                      checked={selectedCategoryId === null}
                      onChange={() => handleCategorySelect(null)}
                      className="form-radio h-5 w-5 text-primary border-outline-variant/50 bg-transparent focus:ring-primary focus:ring-offset-0 focus:ring-0 cursor-pointer"
                      type="radio"
                      name="categoryId"
                    />
                    <span className="text-sm text-on-background group-hover:text-primary transition-colors">
                      All
                    </span>
                  </label>
                  {categories.map(c => (
                    <label key={c.id} className="flex items-center gap-3 cursor-pointer group">
                      <input
                        checked={selectedCategoryId === c.id}
                        onChange={() => handleCategorySelect(c.id)}
                        className="form-radio h-5 w-5 text-primary border-outline-variant/50 bg-transparent focus:ring-primary focus:ring-offset-0 focus:ring-0 cursor-pointer"
                        type="radio"
                        name="categoryId"
                      />
                      <span className="text-sm text-on-background group-hover:text-primary transition-colors">
                        {c.name}
                      </span>
                    </label>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </aside>

        {/* Product Listing */}
        <div className="flex-1 w-full">
          {/* Top Sort Bar */}
          <div className="flex flex-wrap items-center justify-between gap-4 mb-6 border-b border-outline-variant/10 pb-4">
            <p className="text-sm text-on-surface-variant">
              Showing{' '}
              <span className="font-bold text-primary">{totalProducts}</span>{' '}
              results
            </p>
            <div className="flex items-center gap-2">
              <span className="text-xs font-medium text-on-surface-variant hidden sm:inline">
                Sort by:
              </span>
              <select
                value={currentSort}
                onChange={handleSortChange}
                className="bg-white dark:bg-surface-container border border-outline-variant/30 py-2 pl-3 pr-8 rounded-lg text-xs font-semibold text-primary focus:ring-1 focus:ring-primary/20 cursor-pointer shadow-sm focus:outline-none"
              >
                <option value="relevance">Relevance</option>
                <option value="price-low-to-high">Price: Low to High</option>
                <option value="price-high-to-low">Price: High to Low</option>
                <option value="top-rated">Top Rated</option>
              </select>
            </div>
          </div>

          {/* Cards Grid */}
          {loading ? (
            <div className="flex justify-center items-center py-20 w-full">
              <span className="material-symbols-outlined text-4xl animate-spin text-primary">
                hourglass_empty
              </span>
            </div>
          ) : paginatedProducts.length > 0 ? (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                {paginatedProducts.map(p => {
                  const simulatedRating = ((p.id % 5) / 10 + 4.5).toFixed(1);
                  const simulatedReviews = (p.id * 17) % 150 + 10;

                  return (
                    <div
                      key={p.id}
                      onClick={() => navigate(`/products/${p.id}`)}
                      className="group bg-white dark:bg-surface-container rounded-xl border border-outline-variant/10 overflow-hidden hover:shadow-2xl hover:border-primary/40 transition-all duration-300 flex flex-col h-full cursor-pointer shadow-sm"
                    >
                      {/* Thumbnail */}
                      <div className="relative aspect-[4/3] w-full overflow-hidden bg-surface-container-low border-b border-outline-variant/5">
                        <div className="absolute inset-0 organic-gradient opacity-40"></div>
                        <div className="absolute inset-0 flex items-center justify-center">
                          {p.imageUrl ? (
                            <img
                              src={p.imageUrl}
                              alt={p.name}
                              className="w-full h-full object-cover group-hover:scale-[1.03] transition-transform duration-500"
                            />
                          ) : (
                            <span className="material-symbols-outlined text-6xl text-primary/10 select-none">
                              science
                            </span>
                          )}
                        </div>
                        <span className="absolute top-3.5 left-3.5 bg-white/90 px-2 py-0.5 rounded text-[9px] font-bold tracking-tight text-primary shadow-sm uppercase border border-outline-variant/10">
                          {p.categoryName || 'ACTIVE'}
                        </span>
                      </div>

                      {/* Meta Body */}
                      <div className="p-5 flex flex-col flex-grow">
                        <h3 className="font-h3 text-base font-bold text-primary mb-1.5 leading-tight line-clamp-1">
                          {p.name}
                        </h3>

                        {/* Rating */}
                        <div className="flex items-center gap-0.5 mb-3">
                          <span
                            className="material-symbols-outlined text-amber-500 text-sm"
                            style={{ fontVariationSettings: "'FILL' 1" }}
                          >
                            star
                          </span>
                          <span className="text-[11px] text-on-surface-variant font-medium">
                            {simulatedRating} ({simulatedReviews} reviews)
                          </span>
                        </div>

                        {/* Description */}
                        <p className="text-on-surface-variant text-xs mb-4 leading-relaxed line-clamp-2 font-light">
                          {p.description || 'Chế phẩm sinh học giúp tiêu diệt sâu hại, tăng sức đề kháng tự nhiên cho các loại rau ăn lá và cây công nghiệp.'}
                        </p>

                        {/* Price Action Footer */}
                        <div className="mt-auto pt-4 border-t border-dashed border-outline-variant/20 flex items-center justify-between gap-3">
                          <span className="text-lg font-bold text-primary">
                            {formatPrice(p.unitPrice)}
                          </span>
                          <div className="flex items-center gap-2">
                            <button
                              onClick={(e) => handleBuyNow(e, p)}
                              type="button"
                              className="border border-primary text-primary hover:bg-primary/5 active:scale-[0.97] font-bold h-9 px-3.5 rounded-lg flex items-center gap-1.5 transition-all text-xs"
                            >
                              <span className="material-symbols-outlined text-sm">shopping_bag</span>
                              Buy
                            </button>
                            <button
                              onClick={(e) => handleAddToCartClick(e, p)}
                              type="button"
                              className="bg-primary hover:bg-[#173901] text-white font-bold h-9 px-3.5 rounded-lg flex items-center gap-1.5 active:scale-[0.97] transition-all text-xs shadow-md"
                            >
                              <span className="material-symbols-outlined text-sm">add_shopping_cart</span>
                              Add
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="flex justify-center items-center gap-2 mt-10">
                  <button
                    onClick={() => handlePageChange(currentPage - 1)}
                    disabled={currentPage === 1}
                    className="px-4 py-2 rounded-lg border border-outline-variant/30 bg-white dark:bg-surface-container text-primary font-semibold disabled:opacity-50 disabled:cursor-not-allowed hover:bg-primary/5 transition-colors"
                  >
                    <span className="material-symbols-outlined">chevron_left</span>
                  </button>

                  {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => {
                    if (
                      page === 1 ||
                      page === totalPages ||
                      (page >= currentPage - 1 && page <= currentPage + 1)
                    ) {
                      return (
                        <button
                          key={page}
                          onClick={() => handlePageChange(page)}
                          className={`w-10 h-10 rounded-lg font-semibold transition-colors ${
                            page === currentPage
                              ? 'bg-primary text-white shadow-md'
                              : 'border border-outline-variant/30 bg-white dark:bg-surface-container text-primary hover:bg-primary/5'
                          }`}
                        >
                          {page}
                        </button>
                      );
                    } else if (page === currentPage - 2 || page === currentPage + 2) {
                      return <span key={page}>...</span>;
                    }
                    return null;
                  })}

                  <button
                    onClick={() => handlePageChange(currentPage + 1)}
                    disabled={currentPage === totalPages}
                    className="px-4 py-2 rounded-lg border border-outline-variant/30 bg-white dark:bg-surface-container text-primary font-semibold disabled:opacity-50 disabled:cursor-not-allowed hover:bg-primary/5 transition-colors"
                  >
                    <span className="material-symbols-outlined">chevron_right</span>
                  </button>
                </div>
              )}
            </>
          ) : (
            <div className="text-center py-20 text-on-surface-variant font-light bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/10 shadow-sm flex flex-col items-center">
              <span className="material-symbols-outlined text-5xl text-on-surface-variant/40 mb-3">
                inventory_2
              </span>
              Không tìm thấy sản phẩm phù hợp.
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProductsPage;
