import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useToast } from '../context/ToastContext';
import bgImage from '../assets/Background_1.png';
import '../styles/login-admin.css';

const ForgotPassword: React.FC = () => {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [step, setStep] = useState<'email' | 'otp' | 'reset'>('email');
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [error, setError] = useState('');

  const handleSendOtp = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5240/api/auth/password/forgot', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email }),
      });
      const data = await response.json();
      if (response.ok) {
        setStep('otp');
        showToast('OTP đã được gửi đến email của bạn.', 'success');
      } else {
        setError(data.message || 'Không thể gửi OTP. Vui lòng thử lại.');
      }
    } catch {
      setError('Không thể kết nối đến server.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOtp = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (otp.length < 6) {
      setError('Vui lòng nhập đầy đủ mã OTP.');
      return;
    }
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5240/api/auth/password/verify-otp', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, otp }),
      });
      const data = await response.json();
      if (response.ok) {
        setStep('reset');
        showToast('Xác thực thành công!', 'success');
      } else {
        setError(data.message || 'Mã OTP không hợp lệ.');
      }
    } catch {
      setError('Không thể kết nối đến server.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (newPassword.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự.');
      return;
    }
    if (newPassword !== confirmPassword) {
      setError('Mật khẩu xác nhận không khớp.');
      return;
    }
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5240/api/auth/password/reset', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, otp, newPassword }),
      });
      const data = await response.json();
      if (response.ok) {
        showToast('Đặt lại mật khẩu thành công!', 'success');
        navigate('/login');
      } else {
        setError(data.message || 'Không thể đặt lại mật khẩu.');
      }
    } catch {
      setError('Không thể kết nối đến server.');
    } finally {
      setLoading(false);
    }
  };

  const getTitle = () => {
    if (step === 'email') return 'Forgot Password';
    if (step === 'otp') return 'Verify OTP';
    return 'Reset Password';
  };

  const getSubtitle = () => {
    if (step === 'email') return 'Enter your email to receive OTP';
    if (step === 'otp') return 'Enter the code sent to your email';
    return 'Enter your new password';
  };

  return (
    <div className="relative min-h-screen w-full flex items-center justify-center p-4 overflow-y-auto">
      <div className="fixed inset-0 z-0">
        <img alt="" className="w-full h-full object-cover" src={bgImage} />
        <div className="absolute inset-0 bg-primary/20 backdrop-brightness-90"></div>
      </div>

      <main className="relative z-10 w-full max-w-[400px] glass-panel rounded-[20px] p-6 md:p-8 flex flex-col items-center my-4">
        <div className="mb-4 flex flex-col items-center gap-2">
          <div className="w-12 h-12 bg-white rounded-full flex items-center justify-center backdrop-blur-md">
            <span className="material-symbols-outlined text-[#2d5016] text-2xl" style={{ fontVariationSettings: "'FILL' 1" }}>
              lock_reset
            </span>
          </div>
          <h1 className="font-h2 text-2xl text-white text-center">{getTitle()}</h1>
          <p className="font-body-md text-xs text-white/70 text-center">{getSubtitle()}</p>
        </div>

        {error && (
          <div className="admin-login-error w-full mb-3 text-center text-xs" style={{ backgroundColor: 'rgba(239, 68, 68, 0.2)', border: '1px solid rgb(239, 68, 68)', padding: '0.5rem', borderRadius: '0.375rem', color: '#f87171' }}>
            {error}
          </div>
        )}

        {step === 'email' && (
          <form onSubmit={handleSendOtp} className="w-full space-y-4">
            <div className="space-y-1.5">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="email">
                Email Address
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  mail
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2.5 pl-10 pr-4 text-sm transition-all duration-200"
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="farmer@biopest.com"
                  required
                />
              </div>
            </div>
            <div className="pt-2">
              <button
                className="w-full text-white font-label-sm text-base py-3 rounded-[10px] active:scale-[0.98] transition-all shadow-xl bg-[#2d5016] hover:bg-[#173901] shadow-black/20 disabled:opacity-50"
                type="submit"
                disabled={loading}
              >
                {loading ? 'Sending...' : 'Send OTP'}
              </button>
            </div>
          </form>
        )}

        {step === 'otp' && (
          <form onSubmit={handleVerifyOtp} className="w-full space-y-4">
            <div className="space-y-1.5">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="otp">
                OTP Code
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  dialpad
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2.5 pl-10 pr-4 text-sm transition-all duration-200 text-center tracking-[0.3em] text-lg"
                  id="otp"
                  type="text"
                  value={otp}
                  onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
                  placeholder="• • • • • •"
                  maxLength={6}
                  required
                />
              </div>
            </div>
            <div className="pt-2">
              <button
                className="w-full text-white font-label-sm text-base py-3 rounded-[10px] active:scale-[0.98] transition-all shadow-xl bg-[#2d5016] hover:bg-[#173901] shadow-black/20 disabled:opacity-50"
                type="submit"
                disabled={loading}
              >
                {loading ? 'Verifying...' : 'Verify OTP'}
              </button>
            </div>
            <div className="text-center">
              <button
                type="button"
                onClick={() => setStep('email')}
                className="text-white/60 hover:text-white text-xs transition-colors bg-transparent border-none cursor-pointer"
              >
                ← Back to email
              </button>
            </div>
          </form>
        )}

        {step === 'reset' && (
          <form onSubmit={handleResetPassword} className="w-full space-y-4">
            <div className="space-y-1.5">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="newPassword">
                New Password
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  lock
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2.5 pl-10 pr-10 text-sm transition-all duration-200"
                  id="newPassword"
                  type={showPassword ? 'text' : 'password'}
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  placeholder="••••••••"
                  required
                />
                <button
                  className="material-symbols-outlined absolute right-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg hover:text-white transition-colors z-10 bg-transparent border-none cursor-pointer"
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                >
                  {showPassword ? 'visibility_off' : 'visibility'}
                </button>
              </div>
            </div>
            <div className="space-y-1.5">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="confirmPassword">
                Confirm Password
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  lock
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2.5 pl-10 pr-10 text-sm transition-all duration-200"
                  id="confirmPassword"
                  type={showConfirm ? 'text' : 'password'}
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="••••••••"
                  required
                />
                <button
                  className="material-symbols-outlined absolute right-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg hover:text-white transition-colors z-10 bg-transparent border-none cursor-pointer"
                  type="button"
                  onClick={() => setShowConfirm((v) => !v)}
                >
                  {showConfirm ? 'visibility_off' : 'visibility'}
                </button>
              </div>
            </div>
            <div className="pt-2">
              <button
                className="w-full text-white font-label-sm text-base py-3 rounded-[10px] active:scale-[0.98] transition-all shadow-xl bg-[#2d5016] hover:bg-[#173901] shadow-black/20 disabled:opacity-50"
                type="submit"
                disabled={loading}
              >
                {loading ? 'Resetting...' : 'Reset Password'}
              </button>
            </div>
          </form>
        )}

        <div className="mt-5 flex flex-col items-center gap-3 justify-center w-full">
          <Link to="/login" className="font-body-md text-xs text-white/70 hover:text-white transition-colors flex items-center gap-1">
            <span className="material-symbols-outlined text-sm">chevron_left</span>
            Back to Sign In
          </Link>
        </div>
      </main>
    </div>
  );
};

export default ForgotPassword;
