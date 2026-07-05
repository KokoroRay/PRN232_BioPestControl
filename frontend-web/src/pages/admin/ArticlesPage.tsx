import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Pencil, Trash2, Eye } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { ConfirmModal } from '../../components/admin/ConfirmModal';
import { useToast } from '../../context/ToastContext';
import { articleService } from '../../services/articleService';
import type { Article, CreateArticleRequest } from '../../types/article';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';

const ArticlesPage: React.FC = () => {
  const { showToast } = useToast();
  const [all, setAll] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTitle, setSearchTitle] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [viewArticle, setViewArticle] = useState<Article | null>(null);
  const [editId, setEditId] = useState<string | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateArticleRequest>({
    title: '',
    content: '',
    summary: '',
    thumbnailUrl: '',
    status: 'Published',
    tags: '',
  });

  const uploadToCloudinary = async (file: File) => {
    const cloudName = import.meta.env.VITE_CLOUDINARY_CLOUD_NAME || 'dfv00htk8'; // Fallback for testing
    const uploadPreset = import.meta.env.VITE_CLOUDINARY_UPLOAD_PRESET || 'biopest_preset';
    
    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', uploadPreset);
    
    const res = await fetch(`https://api.cloudinary.com/v1_1/${cloudName}/image/upload`, {
      method: 'POST',
      body: formData,
    });
    if (!res.ok) throw new Error('Upload failed');
    const data = await res.json();
    return data.secure_url;
  };

  const imageHandler = useCallback(() => {
    const input = document.createElement('input');
    input.setAttribute('type', 'file');
    input.setAttribute('accept', 'image/*');
    input.click();

    input.onchange = async () => {
      const file = input.files?.[0];
      if (!file) return;
      try {
        const url = await uploadToCloudinary(file);
        // Find quill instance
        const quill = (document.querySelector('.ql-editor') as any)?.parentElement?.__quill;
        if (quill) {
          const range = quill.getSelection(true);
          quill.insertEmbed(range.index, 'image', url);
        }
      } catch {
        showToast('Image upload failed', 'error');
      }
    };
  }, [showToast]);

  const quillModules = useMemo(() => ({
    toolbar: {
      container: [
        [{ 'header': [1, 2, 3, false] }],
        ['bold', 'italic', 'underline', 'strike'],
        [{ 'list': 'ordered'}, { 'list': 'bullet' }],
        ['link', 'image'],
        ['clean']
      ],
      handlers: {
        image: imageHandler
      }
    }
  }), [imageHandler]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await articleService.getAll();
      setAll(data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()));
    } catch {
      showToast('Failed to load articles', 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    load();
  }, [load]);

  const filtered = useMemo(() => {
    let q = all;
    const s = searchTitle.trim().toLowerCase();
    if (s) q = q.filter((a) => a.title.toLowerCase().includes(s));
    if (statusFilter) q = q.filter((a) => a.status === statusFilter);
    return q;
  }, [all, searchTitle, statusFilter]);

  const openCreate = () => {
    setEditId(null);
    setForm({ title: '', content: '', summary: '', thumbnailUrl: '', status: 'Published', tags: '' });
    setDrawerOpen(true);
  };

  const openEdit = (a: Article) => {
    setEditId(a.id);
    setForm({
      title: a.title,
      content: a.content,
      summary: a.summary ?? '',
      thumbnailUrl: a.thumbnailUrl ?? '',
      status: a.status,
      tags: a.tags ?? '',
    });
    setDrawerOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editId) await articleService.update(editId, form);
      else await articleService.create(form);
      showToast(editId ? 'Article updated' : 'Article created');
      setDrawerOpen(false);
      load();
    } catch {
      showToast('Save failed', 'error');
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await articleService.delete(deleteId);
      showToast('Article deleted');
      setDeleteId(null);
      load();
    } catch {
      showToast('Delete failed', 'error');
    }
  };

  return (
    <div className="admin-page">
      <PageHeader title="News / Articles" subtitle="Manage news and articles." actionLabel="Add Article" onAction={openCreate} />
      <div className="filter-bar grid-3">
        <input placeholder="Search by title..." value={searchTitle} onChange={(e) => setSearchTitle(e.target.value)} />
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All status</option>
          <option value="Draft">Draft</option>
          <option value="Published">Published</option>
          <option value="Archived">Archived</option>
        </select>
      </div>
      {loading ? (
        <LoadingState message="Loading articles..." />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr><th>Title</th><th>Status</th><th>Created</th><th></th></tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr><td colSpan={4} className="empty-cell">No articles found</td></tr>
              ) : (
                filtered.map((a) => (
                  <tr key={a.id}>
                    <td><strong>{a.title}</strong>{a.summary && <div className="text-muted">{a.summary}</div>}</td>
                    <td><span className="pill pill-blue">{a.status}</span></td>
                    <td>{new Date(a.createdAt).toLocaleString('vi-VN')}</td>
                    <td className="actions-cell">
                      <button type="button" className="btn-icon" onClick={() => setViewArticle(a)}><Eye size={18} /></button>
                      <button type="button" className="btn-icon" onClick={() => openEdit(a)}><Pencil size={18} /></button>
                      <button type="button" className="btn-icon danger" onClick={() => setDeleteId(a.id)}><Trash2 size={18} /></button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
      <Drawer open={drawerOpen} title={editId ? 'Edit Article' : 'Add Article'} onClose={() => setDrawerOpen(false)} wide>
        <form onSubmit={handleSubmit} className="form-stack">
          <label>Title *<input value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} required /></label>
          <label>Status<select value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })}><option>Draft</option><option>Published</option><option>Archived</option></select></label>
          <label>Summary<textarea value={form.summary} onChange={(e) => setForm({ ...form, summary: e.target.value })} rows={2} /></label>
          <div className="quill-wrapper" style={{ marginBottom: '1rem' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Content *</label>
            <ReactQuill 
              theme="snow"
              value={form.content} 
              onChange={(val) => setForm({ ...form, content: val })} 
              modules={quillModules}
              style={{ height: '300px', marginBottom: '40px' }}
            />
          </div>
          <label>Thumbnail URL<input value={form.thumbnailUrl} onChange={(e) => setForm({ ...form, thumbnailUrl: e.target.value })} /></label>
          <label>Tags<input value={form.tags} onChange={(e) => setForm({ ...form, tags: e.target.value })} /></label>
          <div className="form-actions">
            <button type="button" className="btn-secondary" onClick={() => setDrawerOpen(false)}>Cancel</button>
            <button type="submit" className="btn-primary">{editId ? 'Update' : 'Create'}</button>
          </div>
        </form>
      </Drawer>
      <Drawer open={!!viewArticle} title="View Article" onClose={() => setViewArticle(null)} wide>
        {viewArticle && (
          <article>
            <h3>{viewArticle.title}</h3>
            <p className="text-muted">{viewArticle.status} · {new Date(viewArticle.createdAt).toLocaleString('vi-VN')}</p>
            {viewArticle.thumbnailUrl && <img src={viewArticle.thumbnailUrl} alt="" className="article-thumb" style={{ maxWidth: '100%', marginBottom: '1rem', borderRadius: '8px' }} />}
            {viewArticle.summary && <p style={{ fontWeight: '500', marginBottom: '1rem' }}>{viewArticle.summary}</p>}
            <div className="article-content" dangerouslySetInnerHTML={{ __html: viewArticle.content }}></div>
          </article>
        )}
      </Drawer>
      <ConfirmModal open={!!deleteId} title="Delete Article" message="Delete this article?" danger confirmLabel="Delete" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </div>
  );
};

export default ArticlesPage;
