import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { register } from '../services/authService';
import bgImage from '../assets/Background_1.png';
import '../styles/login-admin.css';

const Signup: React.FC = () => {
  const navigate = useNavigate();
  const { googleLogin } = useAuth();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const googleLoginRef = useRef(googleLogin);
  useEffect(() => {
    googleLoginRef.current = googleLogin;
  }, [googleLogin]);

  const handleGoogleCredentialResponse = useCallback(async (response: any) => {
    setError('');
    setSuccess('');
    setLoading(true);
    try {
      const idToken = response.credential;
      const role = await googleLoginRef.current(idToken, { allowedRoles: ['Customer', 'Admin', 'Staff'] });
      setSuccess('Google sign-in successful! Redirecting...');
      setTimeout(() => {
        if (role === 'Admin') {
          navigate('/admin/dashboard', { replace: true });
        } else if (role === 'Staff') {
          navigate('/staff/dashboard', { replace: true });
        } else {
          navigate('/', { replace: true });
        }
      }, 1500);
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        (err instanceof Error ? err.message : 'Google login failed');
      setError(msg);
    } finally {
      setLoading(false);
    }
  }, [navigate]);

  useEffect(() => {
    const initializeGoogle = () => {
      const google = (window as any).google;
      if (google) {
        google.accounts.id.initialize({
          client_id: '2040302279-84hbkip8oc8l1c52hgq6nvvvut09p9ik.apps.googleusercontent.com',
          callback: handleGoogleCredentialResponse,
        });
        google.accounts.id.renderButton(
          document.getElementById('google-signup-btn-container'),
          {
            theme: 'outline',
            size: 'large',
            text: 'signup_with',
            shape: 'circle',
            type: 'icon',
          }
        );
      }
    };

    initializeGoogle();

    let attempts = 0;
    const interval = setInterval(() => {
      attempts++;
      const google = (window as any).google;
      if (google) {
        initializeGoogle();
        clearInterval(interval);
      } else if (attempts >= 10) {
        clearInterval(interval);
      }
    }, 500);

    return () => clearInterval(interval);
  }, [handleGoogleCredentialResponse]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    setLoading(true);
    try {
      await register({ email, password, fullName });
      setSuccess('Registration successful! Redirecting to login...');
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        (err instanceof Error ? err.message : 'Registration failed');
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen w-full flex items-center justify-center p-4 overflow-y-auto">
      {/* Background Image */}
      <div className="fixed inset-0 z-0">
        <img
          alt=""
          className="w-full h-full object-cover"
          src={bgImage}
        />
        <div className="absolute inset-0 bg-primary/20 backdrop-brightness-90"></div>
      </div>

      {/* Main Glassmorphism Signup Panel */}
      <main className="relative z-10 w-full max-w-[540px] glass-panel rounded-[20px] p-6 md:p-8 flex flex-col items-center my-4">
        {/* Brand Identity */}
        <div className="mb-4 flex flex-col items-center gap-2">
          <div className="w-12 h-12 bg-white rounded-full flex items-center justify-center backdrop-blur-md">
            <span className="material-symbols-outlined text-[#2d5016] text-2xl" style={{ fontVariationSettings: "'FILL' 1" }}>
              eco
            </span>
          </div>
          <h1 className="font-h2 text-2xl text-white text-center">Create an Account</h1>
          <p className="font-body-md text-white/70 text-center text-xs mt-1">Join our scientific botanical community</p>
        </div>

        {error && (
          <div className="admin-login-error w-full mb-3 text-center text-xs" style={{ backgroundColor: 'rgba(239, 68, 68, 0.2)', border: '1px solid rgb(239, 68, 68)', padding: '0.5rem', borderRadius: '0.375rem', color: '#f87171' }}>
            {error}
          </div>
        )}

        {success && (
          <div className="w-full mb-3 text-center text-xs" style={{ backgroundColor: 'rgba(16, 185, 129, 0.2)', border: '1px solid rgb(16, 185, 129)', padding: '0.5rem', borderRadius: '0.375rem', color: '#34d399' }}>
            {success}
          </div>
        )}

        {/* Signup Form */}
        <form onSubmit={handleSubmit} className="w-full space-y-3">
          {/* Full Name */}
          <div className="space-y-1">
            <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="fullName">
              Full Name
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                person
              </span>
              <input
                className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                id="fullName"
                type="text"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                placeholder="Enter your name"
                required
              />
            </div>
          </div>

          {/* Email & Phone */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <div className="space-y-1">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="email">
                Email Address
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  mail
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="email@example.com"
                  required
                />
              </div>
            </div>
            <div className="space-y-1">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="phone">
                Phone Number
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  call
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                  id="phone"
                  type="tel"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  placeholder="+1 (555) 000-0000"
                />
              </div>
            </div>
          </div>

          {/* Physical Address */}
          <div className="space-y-1">
            <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="address">
              Physical Address
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                location_on
              </span>
              <input
                className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                id="address"
                type="text"
                value={address}
                onChange={(e) => setAddress(e.target.value)}
                placeholder="Street, City, State, ZIP"
              />
            </div>
          </div>

          {/* Password & Confirm Password */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <div className="space-y-1">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="password">
                Password
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  lock
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                  id="password"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="••••••••"
                  required
                />
              </div>
            </div>
            <div className="space-y-1">
              <label className="font-label-sm text-xs text-white/80 block ml-1" htmlFor="confirmPassword">
                Confirm Password
              </label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                  verified_user
                </span>
                <input
                  className="glass-input w-full rounded-[10px] py-2 pl-10 pr-4 text-sm transition-all duration-200"
                  id="confirmPassword"
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="••••••••"
                  required
                />
              </div>
            </div>
          </div>

          <div className="pt-2 flex justify-center">
            <button
              className="w-full max-w-sm text-white font-label-sm text-base py-2.5 rounded-full active:scale-[0.98] transition-all shadow-lg bg-[#2d5016] hover:bg-[#173901] shadow-black/20 flex items-center justify-center gap-2"
              type="submit"
              disabled={loading}
            >
              {loading ? 'Creating...' : 'Sign Up'}
              <span className="material-symbols-outlined text-lg">arrow_forward</span>
            </button>
          </div>
        </form>

        {/* Footer of Card */}
        <div className="mt-4 text-center w-full">
          <button
            type="button"
            onClick={() => navigate('/login')}
            className="font-body-md text-xs text-white/90 hover:text-white underline decoration-white/30 underline-offset-4 transition-colors bg-transparent border-none cursor-pointer"
          >
            Already have an account? Sign In
          </button>
          
          <div className="relative my-3 flex items-center justify-center">
            <div className="w-full border-t border-white/20"></div>
            <span className="bg-transparent px-4 font-label-sm text-white/60 absolute uppercase tracking-widest text-[9px]">
              Or continue with
            </span>
          </div>

          <div className="flex items-center justify-center gap-3 w-full mt-1">
            <div className="relative w-11 h-11">
              {/* Custom styled Google button */}
              <button
                className="absolute inset-0 w-full h-full glass-panel rounded-lg flex items-center justify-center hover:bg-white/20 transition-all text-white/90 hover:text-white pointer-events-none"
                type="button"
                title="Sign up with Google"
              >
                <svg className="w-5 h-5" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                  <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"></path>
                  <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"></path>
                  <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"></path>
                  <path d="M12 5.38c1.62 0 3.06.56 4.21 1.66l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"></path>
                </svg>
              </button>
              {/* Real invisible Google login container */}
              <div
                id="google-signup-btn-container"
                className="absolute inset-0 w-full h-full opacity-0 overflow-hidden cursor-pointer [&>div]:w-full [&>div]:h-full [&_iframe]:w-full [&_iframe]:h-full"
              ></div>
            </div>
            <button
              onClick={() => document.getElementById('fullName')?.focus()}
              className="w-11 h-11 glass-panel rounded-lg flex items-center justify-center hover:bg-white/20 transition-all text-white/90 hover:text-white"
              type="button"
              title="Sign up with Email"
            >
              <span className="material-symbols-outlined text-xl">mail</span>
            </button>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Signup;
