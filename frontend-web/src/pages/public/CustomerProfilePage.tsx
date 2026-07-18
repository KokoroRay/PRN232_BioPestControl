import React, { useEffect, useRef, useState } from 'react';
import { Camera, Save, Loader2 } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { AccountSidebar } from '../../components/public/AccountSidebar';
import { profileService, type Profile } from '../../services/profileService';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';

export default function CustomerProfilePage() {
  const navigate = useNavigate();
  const { refreshUser, isAuthenticated } = useAuth();
  const { showToast } = useToast();

  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploadingAvatar, setUploadingAvatar] = useState(false);

  const [form, setForm] = useState({ fullName: '', phoneNumber: '', address: '' });
  const [avatarUrl, setAvatarUrl] = useState<string>('');

  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/account/profile' } });
      return;
    }
    profileService.getProfile().then((p) => {
      setProfile(p);
      setForm({ fullName: p.fullName ?? '', phoneNumber: p.phoneNumber ?? '', address: p.address ?? '' });
      setAvatarUrl(p.avatarUrl ?? '');
      setLoading(false);
    }).catch(() => {
      showToast('Failed to load profile.', 'error');
      setLoading(false);
    });
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleAvatarClick = () => fileInputRef.current?.click();

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size > 1 * 1024 * 1024) {
      showToast('File must be under 1MB.', 'error');
      return;
    }
    setUploadingAvatar(true);
    try {
      const result = await profileService.uploadAvatar(file);
      if (result.success && result.url) {
        setAvatarUrl(result.url);
        showToast('Photo updated. Click Save to keep.', 'success');
      } else {
        showToast(result.message ?? 'Upload failed.', 'error');
      }
    } catch {
      showToast('Upload failed.', 'error');
    } finally {
      setUploadingAvatar(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const updated = await profileService.updateProfile({
        fullName: form.fullName || undefined,
        phoneNumber: form.phoneNumber || undefined,
        address: form.address || undefined,
        avatarUrl: avatarUrl || undefined,
      });
      refreshUser({ fullName: updated.fullName, avatarUrl: updated.avatarUrl });
      setProfile(updated);
      showToast('Profile updated successfully.', 'success');
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Update failed.';
      showToast(msg, 'error');
    } finally {
      setSaving(false);
    }
  };

  const memberSince = profile?.createdAt
    ? new Date(profile.createdAt).toLocaleDateString('en-US', { month: 'short', year: 'numeric' })
    : '';

  const lastUpdated = profile?.updatedAt
    ? new Date(profile.updatedAt).toLocaleString('en-US', {
        month: 'short', day: 'numeric', year: 'numeric',
        hour: '2-digit', minute: '2-digit',
      })
    : null;

  if (loading) {
    return (
      <div className="profile-page">
        <div className="profile-container">
          <div className="profile-loading">
            <Loader2 size={32} className="spin" />
            <p>Loading profile…</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="profile-page">
      <div className="profile-container">
        <aside className="profile-sidebar-col">
          <AccountSidebar active="profile" />
        </aside>

        <main className="profile-main">
          <div className="profile-card">
            {/* Avatar + name + membership */}
            <div className="profile-hero">
              <div className="profile-avatar-wrap">
                <div className="profile-avatar">
                  {avatarUrl ? (
                    <img src={avatarUrl} alt="Avatar" />
                  ) : (
                    <span className="profile-avatar-placeholder" />
                  )}
                </div>
                <button
                  type="button"
                  className="profile-avatar-btn"
                  onClick={handleAvatarClick}
                  title="Change photo"
                  disabled={uploadingAvatar}
                >
                  {uploadingAvatar ? <Loader2 size={16} className="spin" /> : <Camera size={16} />}
                </button>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".jpg,.jpeg,.png"
                  className="profile-file-input"
                  onChange={handleFileChange}
                />
              </div>

              <div className="profile-identity">
                <h1 className="profile-display-name">{profile?.fullName || profile?.email}</h1>
                <p className="profile-meta">Member since {memberSince}</p>
                <button type="button" className="profile-upload-btn" onClick={handleAvatarClick}>
                  <Camera size={16} />
                  Upload New Photo
                </button>
                <p className="profile-hint">Max 1MB. JPG, PNG only.</p>
              </div>

              <div className="profile-membership">
                <div className="profile-membership-icon">
                  <span className="material-symbols-outlined">military_tech</span>
                </div>
                <div>
                  <p className="profile-membership-label">Membership Level</p>
                  <p className="profile-membership-value">Member</p>
                </div>
              </div>
            </div>

            {/* Form */}
            <form className="profile-form" onSubmit={handleSubmit}>
              <div className="profile-fields">
                <div className="profile-field">
                  <label htmlFor="fullName">Full Name</label>
                  <input
                    id="fullName"
                    name="fullName"
                    type="text"
                    placeholder="e.g. John Doe"
                    value={form.fullName}
                    onChange={handleChange}
                  />
                </div>
                <div className="profile-field">
                  <label htmlFor="email">Email Address</label>
                  <input
                    id="email"
                    type="email"
                    value={profile?.email ?? ''}
                    readOnly
                    disabled
                  />
                </div>
                <div className="profile-field">
                  <label htmlFor="phoneNumber">Phone Number</label>
                  <input
                    id="phoneNumber"
                    name="phoneNumber"
                    type="tel"
                    placeholder="+84 ..."
                    value={form.phoneNumber}
                    onChange={handleChange}
                  />
                </div>
                <div className="profile-field" style={{ gridColumn: '1 / -1' }}>
                  <label htmlFor="address">Address (Tỉnh/Thành phố, Quận/Huyện, Phường/Xã, Số nhà)</label>
                  <input
                    id="address"
                    name="address"
                    type="text"
                    placeholder="e.g. 123 Green Way, Phường X, Quận Y, TP Z"
                    value={form.address}
                    onChange={handleChange}
                  />
                </div>
              </div>

              <div className="profile-footer">
                <div className="profile-timestamps">
                  {lastUpdated && <p className="profile-timestamp">Last updated: {lastUpdated}</p>}
                  {memberSince && <p className="profile-timestamp">Account Created: {memberSince}</p>}
                </div>
                <button
                  type="submit"
                  className="profile-save-btn"
                  disabled={saving}
                >
                  {saving ? <Loader2 size={16} className="spin" /> : <Save size={16} />}
                  Save Profile
                </button>
              </div>
            </form>
          </div>
        </main>
      </div>
    </div>
  );
};
