import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ShieldCheck, Loader2, Eye, EyeOff } from 'lucide-react';
import { AccountSidebar } from '../../components/public/AccountSidebar';
import { profileService } from '../../services/profileService';
import { useToast } from '../../context/ToastContext';

export default function ChangePasswordPage() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  const [form, setForm] = useState({ oldPassword: '', newPassword: '', confirmPassword: '' });
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [visible, setVisible] = useState({ old: false, new: false, confirm: false });

  const toggle = (key: 'old' | 'new' | 'confirm') =>
    setVisible((prev) => ({ ...prev, [key]: !prev[key] }));

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    setErrors([]);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const { oldPassword, newPassword, confirmPassword } = form;

    const validationErrors: string[] = [];
    if (!oldPassword) validationErrors.push('Current password is required.');
    if (!newPassword) validationErrors.push('New password is required.');
    if (newPassword.length < 8) validationErrors.push('New password must be at least 8 characters.');
    if (newPassword !== confirmPassword) validationErrors.push('Passwords do not match.');

    if (validationErrors.length > 0) {
      setErrors(validationErrors);
      return;
    }

    setSaving(true);
    try {
      await profileService.changePassword({ oldPassword, newPassword });
      showToast('Password changed successfully.', 'success');
      setForm({ oldPassword: '', newPassword: '', confirmPassword: '' });
      setTimeout(() => navigate('/account/profile'), 1500);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Failed to change password.';
      setErrors([msg]);
    } finally {
      setSaving(false);
    }
  };

  const Field: React.FC<{ name: 'oldPassword' | 'newPassword' | 'confirmPassword'; label: string; placeholder: string; hint?: string }> = ({
    name, label, placeholder, hint,
  }) => (
    <div className="change-pwd-field">
      <label htmlFor={name}>{label}</label>
      <div className="change-pwd-input-wrap">
        <input
          id={name}
          name={name}
          type={visible[name === 'oldPassword' ? 'old' : name === 'newPassword' ? 'new' : 'confirm'] ? 'text' : 'password'}
          placeholder={placeholder}
          value={form[name]}
          onChange={handleChange}
          className="change-pwd-input"
        />
        <button
          type="button"
          className="change-pwd-toggle"
          onClick={() => toggle(name === 'oldPassword' ? 'old' : name === 'newPassword' ? 'new' : 'confirm')}
          aria-label="Toggle visibility"
        >
          {visible[name === 'oldPassword' ? 'old' : name === 'newPassword' ? 'new' : 'confirm'] ? <EyeOff size={18} /> : <Eye size={18} />}
        </button>
      </div>
      {hint && name === 'newPassword' && <p className="change-pwd-hint">{hint}</p>}
    </div>
  );

  return (
    <div className="change-pwd-page">
      <div className="profile-container">
        <aside className="profile-sidebar-col">
          <AccountSidebar active="changepassword" />
        </aside>

        <main className="profile-main">
          <div className="profile-card">
            <div className="change-pwd-header">
              <h1>Change Password</h1>
              <p>Update your security credentials to keep your account safe.</p>
            </div>

            {errors.length > 0 && (
              <div className="change-pwd-error">
                {errors.map((e, i) => <p key={i}>{e}</p>)}
              </div>
            )}

            <form className="profile-form" onSubmit={handleSubmit}>
              <div className="profile-fields">
                <Field name="oldPassword" label="Current Password" placeholder="••••••••" />
                <Field
                  name="newPassword"
                  label="New Password"
                  placeholder="••••••••"
                  hint="Minimum 8 characters."
                />
                <Field name="confirmPassword" label="Confirm New Password" placeholder="••••••••" />
              </div>

              <div className="profile-footer">
                <div />
                <div className="change-pwd-actions">
                  <Link to="/account/profile" className="change-pwd-cancel">
                    Cancel
                  </Link>
                  <button type="submit" className="profile-save-btn" disabled={saving}>
                    {saving ? <Loader2 size={16} className="spin" /> : <ShieldCheck size={16} />}
                    Update Password
                  </button>
                </div>
              </div>
            </form>
          </div>
        </main>
      </div>
    </div>
  );
}
