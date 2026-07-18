import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { articleService } from '../../services/articleService';
import type { Article } from '../../types/article';

const ArticlesPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);

  const searchQuery = searchParams.get('search') || '';
  const selectedType = searchParams.get('type') || ''; // '1' (News), '2' (Instruction), '3' (Promotion) or '' (All)

  // Map backend article types
  const getTypeName = (status: string) => {
    // Backend type status representation
    if (status === '1') return 'News';
    if (status === '2') return 'Instruction';
    if (status === '3') return 'Promotion';
    return 'News';
  };

  useEffect(() => {
    const loadArticles = async () => {
      try {
        setLoading(true);
        // Call article service to get all articles
        const data = await articleService.getAll();
        
        setArticles(data);
      } catch (err) {
        console.error('Error fetching articles', err);
      } finally {
        setLoading(false);
      }
    };
    loadArticles();
  }, []);

  // Filter articles
  const filteredArticles = useMemo(() => {
    let result = [...articles];

    // Filter by type
    if (selectedType) {
      result = result.filter(a => a.status === selectedType);
    }

    // Filter by search query
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      result = result.filter(a => 
        a.title.toLowerCase().includes(q) || 
        (a.summary && a.summary.toLowerCase().includes(q)) ||
        (a.content && a.content.toLowerCase().includes(q))
      );
    }

    return result;
  }, [articles, selectedType, searchQuery]);

  const featuredArticle = useMemo(() => {
    // Return first item or null
    return filteredArticles[0] || null;
  }, [filteredArticles]);

  const recentArticles = useMemo(() => {
    // Return items excluding featured
    return featuredArticle ? filteredArticles.filter(a => a.id !== featuredArticle.id) : filteredArticles;
  }, [filteredArticles, featuredArticle]);

  const popularPosts = useMemo(() => {
    // Take first 3 articles from full list as popular
    return articles.slice(0, 3);
  }, [articles]);

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
    setSearchParams(params);
  };

  const handleTypeSelect = (typeVal: string) => {
    const params = new URLSearchParams(searchParams);
    if (typeVal) {
      params.set('type', typeVal);
    } else {
      params.delete('type');
    }
    setSearchParams(params);
  };

  return (
    <div className="max-w-[1280px] mx-auto px-6 lg:px-8 pb-24 pt-32 text-on-background font-body-md overflow-x-hidden">
      {/* Breadcrumbs */}
      <div className="flex items-center gap-2 mb-8 text-xs font-semibold text-on-surface-variant">
        <Link to="/" className="hover:text-primary transition-colors">Home</Link>
        <span className="material-symbols-outlined text-[10px]">chevron_right</span>
        <span className="text-primary font-bold">News &amp; Articles</span>
      </div>

      {loading ? (
        <div className="flex justify-center items-center py-40 text-primary">
          <span className="material-symbols-outlined text-4xl animate-spin">hourglass_empty</span>
        </div>
      ) : (
        <>
          {/* Featured Article Hero */}
          {featuredArticle && (
            <section className="grid grid-cols-1 lg:grid-cols-12 gap-12 mb-20 items-center bg-white dark:bg-surface-container rounded-3xl overflow-hidden border border-outline-variant/10 p-6 lg:p-8 shadow-xl">
              <Link to={`/articles/${featuredArticle.id}`} className="lg:col-span-7 block relative group rounded-2xl overflow-hidden shadow-lg border border-outline-variant/5">
                <div className="aspect-[16/9] w-full bg-surface-container-low">
                  <img
                    className="w-full h-full object-cover group-hover:scale-[1.02] transition-transform duration-500"
                    alt={featuredArticle.title}
                    src={featuredArticle.thumbnailUrl || 'https://via.placeholder.com/1200x675'}
                  />
                </div>
              </Link>
              <div className="lg:col-span-5 flex flex-col gap-5 p-2">
                <span className="inline-flex self-start px-3.5 py-1.5 rounded-full text-[10px] font-bold uppercase tracking-wider bg-primary/10 text-primary border border-primary/20">
                  {getTypeName(featuredArticle.status)}
                </span>
                <Link to={`/articles/${featuredArticle.id}`} className="group block">
                  <h2 className="font-h1 text-3xl lg:text-4xl font-bold leading-tight text-primary group-hover:text-[#173901] transition-colors">
                    {featuredArticle.title}
                  </h2>
                </Link>
                <p className="text-sm text-on-surface-variant font-light leading-relaxed">
                  {featuredArticle.summary}
                </p>
                <Link
                  to={`/articles/${featuredArticle.id}`}
                  className="self-start text-xs font-bold text-secondary flex items-center gap-1 hover:underline pt-2"
                >
                  Đọc toàn bộ bài viết <span className="material-symbols-outlined text-xs">arrow_forward</span>
                </Link>
              </div>
            </section>
          )}

          {/* Main Feed with Sidebar */}
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-start">
            {/* Left Feed */}
            <div className="lg:col-span-8 space-y-8">
              {/* Type Tabs Bar */}
              <div className="flex items-center justify-between border-b border-outline-variant/10 pb-4 mb-8">
                <h3 className="font-h3 text-xl font-bold text-primary">Recent Agricultural News</h3>
                <div className="flex gap-4" role="tablist">
                  <button
                    onClick={() => handleTypeSelect('')}
                    className={`text-xs font-bold transition-all border-b-2 pb-1 ${
                      selectedType === ''
                        ? 'text-primary border-primary'
                        : 'text-on-surface-variant/60 border-transparent hover:text-primary'
                    }`}
                  >
                    All Posts
                  </button>
                  <button
                    onClick={() => handleTypeSelect('1')}
                    className={`text-xs font-bold transition-all border-b-2 pb-1 ${
                      selectedType === '1'
                        ? 'text-primary border-primary'
                        : 'text-on-surface-variant/60 border-transparent hover:text-primary'
                    }`}
                  >
                    News
                  </button>
                  <button
                    onClick={() => handleTypeSelect('2')}
                    className={`text-xs font-bold transition-all border-b-2 pb-1 ${
                      selectedType === '2'
                        ? 'text-primary border-primary'
                        : 'text-on-surface-variant/60 border-transparent hover:text-primary'
                    }`}
                  >
                    Instruction
                  </button>
                  <button
                    onClick={() => handleTypeSelect('3')}
                    className={`text-xs font-bold transition-all border-b-2 pb-1 ${
                      selectedType === '3'
                        ? 'text-primary border-primary'
                        : 'text-on-surface-variant/60 border-transparent hover:text-primary'
                    }`}
                  >
                    Promotion
                  </button>
                </div>
              </div>

              {/* Articles Feed */}
              {recentArticles.length > 0 ? (
                <div className="space-y-6">
                  {recentArticles.map(a => (
                    <div
                      key={a.id}
                      onClick={() => navigate(`/articles/${a.id}`)}
                      className="group bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/10 p-5 hover:shadow-xl hover:border-primary/30 transition-all duration-300 flex gap-5 items-center cursor-pointer shadow-sm"
                    >
                      <div className="size-20 lg:size-24 rounded-xl overflow-hidden shrink-0 bg-surface-container-low relative border border-outline-variant/5">
                        <img
                          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                          alt={a.title}
                          src={a.thumbnailUrl || 'https://via.placeholder.com/160'}
                        />
                      </div>
                      <div className="flex-grow space-y-2">
                        <div className="flex flex-wrap items-center gap-3">
                          <span className="px-2.5 py-0.5 rounded-full text-[9px] font-bold uppercase tracking-wider bg-primary/5 text-primary border border-primary/10">
                            {getTypeName(a.status)}
                          </span>
                          <span className="text-[10px] text-on-surface-variant font-medium">
                            {new Date(a.createdAt).toLocaleDateString('vi-VN')}
                          </span>
                        </div>
                        <h4 className="font-bold text-sm lg:text-base text-primary leading-snug group-hover:text-[#173901] transition-colors line-clamp-1">
                          {a.title}
                        </h4>
                        {a.summary && (
                          <p className="text-xs text-on-surface-variant font-light line-clamp-1 leading-relaxed">
                            {a.summary}
                          </p>
                        )}
                      </div>
                      <span className="material-symbols-outlined text-on-surface-variant/50 group-hover:text-primary transition-colors text-xl font-bold ml-2">
                        visibility
                      </span>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center py-20 text-on-surface-variant font-light bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/10 shadow-sm flex flex-col items-center">
                  <span className="material-symbols-outlined text-5xl text-on-surface-variant/35 mb-3">
                    article
                  </span>
                  Không tìm thấy bài viết nào phù hợp.
                </div>
              )}
            </div>

            {/* Sidebar Right */}
            <aside className="lg:col-span-4 space-y-8 sticky top-32">
              {/* Sidebar Search */}
              <div className="bg-white dark:bg-surface-container p-6 rounded-2xl shadow-xl border border-outline-variant/10">
                <h4 className="font-bold text-sm text-primary uppercase tracking-wider mb-4 border-b border-outline-variant/10 pb-2">
                  Tìm kiếm
                </h4>
                <form onSubmit={handleSearch} className="relative">
                  <input
                    name="search"
                    defaultValue={searchQuery}
                    className="w-full bg-background dark:bg-surface border border-outline-variant/30 rounded-xl px-4 py-2.5 text-xs focus:ring-1 focus:ring-primary/20 focus:outline-none"
                    placeholder="Keywords..."
                    type="text"
                  />
                  <button type="submit" className="absolute right-3 top-1/2 -translate-y-1/2 text-primary">
                    <span className="material-symbols-outlined text-lg">search</span>
                  </button>
                </form>
              </div>

              {/* Popular Posts */}
              <div className="bg-white dark:bg-surface-container p-6 rounded-2xl shadow-xl border border-outline-variant/10">
                <h4 className="font-bold text-sm text-primary uppercase tracking-wider mb-5 border-b border-outline-variant/10 pb-2">
                  Bài viết phổ biến
                </h4>
                <div className="flex flex-col gap-6">
                  {popularPosts.length > 0 ? (
                    popularPosts.map(post => (
                      <Link
                        key={post.id}
                        className="flex gap-4 group"
                        to={`/articles/${post.id}`}
                      >
                        <div className="size-14 rounded-lg overflow-hidden shrink-0 bg-surface-container-low border border-outline-variant/5">
                          <img
                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                            alt={post.title}
                            src={post.thumbnailUrl || 'https://via.placeholder.com/160'}
                          />
                        </div>
                        <div className="space-y-1">
                          <h5 className="text-xs font-bold leading-snug text-primary group-hover:text-secondary transition-colors line-clamp-2">
                            {post.title}
                          </h5>
                          <p className="text-[9px] text-on-surface-variant/80 uppercase font-bold tracking-wider">
                            {getTypeName(post.status)} •{' '}
                            {new Date(post.createdAt).toLocaleDateString('en-US', {
                              month: 'short',
                              day: 'numeric',
                              year: 'numeric',
                            })}
                          </p>
                        </div>
                      </Link>
                    ))
                  ) : (
                    <p className="text-xs text-on-surface-variant">Không có bài viết nào phổ biến.</p>
                  )}
                </div>
              </div>
            </aside>
          </div>
        </>
      )}
    </div>
  );
};

export default ArticlesPage;
