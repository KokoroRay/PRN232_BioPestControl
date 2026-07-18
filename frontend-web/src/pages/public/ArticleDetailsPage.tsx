import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { articleService } from '../../services/articleService';
import type { Article } from '../../types/article';

const ArticleDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  
  const [article, setArticle] = useState<Article | null>(null);
  const [loading, setLoading] = useState(true);
  const [popularPosts, setPopularPosts] = useState<Article[]>([]);

  // Map backend article types
  const getTypeName = (status: string) => {
    if (status === '1') return 'News';
    if (status === '2') return 'Instruction';
    if (status === '3') return 'Promotion';
    return 'News';
  };

  useEffect(() => {
    if (!id) return;
    const loadDetails = async () => {
      try {
        setLoading(true);
        const data = await articleService.getById(id);
        setArticle(data);

        // Fetch other articles as popular posts
        const allList = await articleService.getAll();
        setPopularPosts(allList.filter(a => a.id !== id).slice(0, 3));
      } catch (err) {
        console.error('Error fetching article details', err);
      } finally {
        setLoading(false);
      }
    };
    loadDetails();
  }, [id]);

  if (loading) {
    return (
      <div className="flex justify-center items-center py-40 min-h-screen text-primary">
        <span className="material-symbols-outlined text-4xl animate-spin">hourglass_empty</span>
      </div>
    );
  }

  if (!article) return null;

  // Mock secondary images gallery
  const secondaryImages = [
    'https://lh3.googleusercontent.com/aida-public/AB6AXuD1v5QtMLjXFjj9tiYQpdFudr4bUHPkjnN15jmWWr9kIk6dVkb96F7NOkQBADgCQ3gpxQb657Jh27EEBWwqj_F7rQ6vYoh01kN9o_NuwHI14uPk_-aeFA99mlMqz2qfWSOaEP6i6n_KyWYPNqYa3QuctpslYEJshjA5W0ZuryVfxkz_Tif_fswotI6HwqQj9xB6AFD3TurhjQw-A1L3HtibASM3hd7ITGWIJ63mlfyICxrUFBwQ9IqWatDp5zDPezvULUkM-MxFPIc',
    'https://lh3.googleusercontent.com/aida-public/AB6AXuD1v5QtMLjXFjj9tiYQpdFudr4bUHPkjnN15jmWWr9kIk6dVkb96F7NOkQBADgCQ3gpxQb657Jh27EEBWwqj_F7rQ6vYoh01kN9o_NuwHI14uPk_-aeFA99mlMqz2qfWSOaEP6i6n_KyWYPNqYa3QuctpslYEJshjA5W0ZuryVfxkz_Tif_fswotI6HwqQj9xB6AFD3TurhjQw-A1L3HtibASM3hd7ITGWIJ63mlfyICxrUFBwQ9IqWatDp5zDPezvULUkM-MxFPIc'
  ];

  return (
    <div className="max-w-[1280px] mx-auto px-6 lg:px-8 pb-24 pt-32 text-on-background font-body-md overflow-x-hidden">
      {/* Breadcrumbs */}
      <div className="flex items-center gap-2 mb-8 text-xs font-semibold text-on-surface-variant">
        <Link to="/" className="hover:text-primary transition-colors">Home</Link>
        <span className="material-symbols-outlined text-[10px]">chevron_right</span>
        <Link to="/articles" className="hover:text-primary transition-colors">News &amp; Articles</Link>
        <span className="material-symbols-outlined text-[10px]">chevron_right</span>
        <span className="text-primary font-bold line-clamp-1">{article.title}</span>
      </div>

      <article className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-start">
        {/* Left Side: Article Content */}
        <div className="lg:col-span-8 space-y-8">
          {/* Cover image banner */}
          <div className="rounded-2xl overflow-hidden shadow-2xl border border-outline-variant/10 flex justify-center bg-surface-container-low max-h-96 relative">
            <div className="absolute inset-0 organic-gradient opacity-30 select-none pointer-events-none"></div>
            <img
              className="max-w-full h-96 object-contain relative z-10 w-full"
              alt={article.title}
              src={article.thumbnailUrl || 'https://via.placeholder.com/1200x675'}
            />
          </div>

          {/* Meta Info */}
          <div>
            <div className="flex items-center gap-3.5 mb-3">
              <span className="inline-flex px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider bg-primary/10 text-primary border border-primary/20">
                {getTypeName(article.status)}
              </span>
              <span className="text-xs text-on-surface-variant font-medium">
                {new Date(article.createdAt).toLocaleDateString('vi-VN', {
                  month: 'short',
                  day: 'numeric',
                  year: 'numeric',
                })}
              </span>
            </div>
            <h1 className="font-h1 text-3xl lg:text-4xl font-bold text-primary leading-tight mb-4">
              {article.title}
            </h1>
          </div>

          {/* Summary Box */}
          {article.summary && (
            <div className="bg-primary/5 p-6 rounded-2xl border border-primary/20 shadow-sm border-l-4 border-l-primary space-y-2">
              <h4 className="text-xs font-bold text-primary uppercase tracking-wider">Tóm tắt bài viết</h4>
              <p className="text-sm text-primary font-medium leading-relaxed italic">{article.summary}</p>
            </div>
          )}

          {/* Core Content */}
          <div className="bg-white dark:bg-surface-container p-8 rounded-2xl border border-outline-variant/10 shadow-xl space-y-6">
            <h4 className="font-h3 text-lg font-bold text-primary border-b border-outline-variant/10 pb-2 uppercase tracking-wider">Nội dung chi tiết</h4>
            <div 
              className="text-sm text-on-surface-variant font-light leading-relaxed prose dark:prose-invert max-w-none"
              dangerouslySetInnerHTML={{ __html: article.content }}
            ></div>
          </div>

          {/* Secondary Gallery */}
          {secondaryImages.length > 0 && (
            <div className="mt-10 grid grid-cols-2 gap-4">
              {secondaryImages.map((img, index) => (
                <div key={index} className="rounded-xl overflow-hidden border border-outline-variant/20 shadow-md">
                  <img className="w-full h-44 object-cover" src={img} alt="Article supplement" />
                </div>
              ))}
            </div>
          )}

          {/* Back btn */}
          <div className="pt-6 border-t border-outline-variant/10">
            <Link
              className="inline-flex items-center gap-1 text-sm font-bold text-secondary hover:underline"
              to="/articles"
            >
              <span className="material-symbols-outlined text-sm">chevron_left</span>
              Quay lại Danh sách Tin tức
            </Link>
          </div>
        </div>

        {/* Right Side: Popular Posts recommendations */}
        <aside className="lg:col-span-4 space-y-8 sticky top-32">
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
      </article>
    </div>
  );
};

export default ArticleDetailsPage;
