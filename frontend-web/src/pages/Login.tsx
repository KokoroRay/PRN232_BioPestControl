import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import bgImage from '../assets/Background_1.png';
import '../styles/login-admin.css';

const Login: React.FC = () => {
  const { login, googleLogin } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [remember, setRemember] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const googleLoginRef = React.useRef(googleLogin);
  useEffect(() => {
    googleLoginRef.current = googleLogin;
  }, [googleLogin]);

  const handleGoogleCredentialResponse = React.useCallback(async (response: any) => {
    setError('');
    setLoading(true);
    try {
      const idToken = response.credential;
      const role = await googleLoginRef.current(idToken, { allowedRoles: ['Customer', 'Admin', 'Staff'] });
      
      if (role === 'Admin') {
        navigate('/admin/dashboard', { replace: true });
      } else if (role === 'Staff') {
        navigate('/staff/dashboard', { replace: true });
      } else {
        navigate('/', { replace: true });
      }
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
    const saved = localStorage.getItem('remember_email');
    if (saved) {
      setEmail(saved);
      setRemember(true);
    }

    const initializeGoogle = () => {
      const google = (window as any).google;
      if (google) {
        google.accounts.id.initialize({
          client_id: '2040302279-84hbkip8oc8l1c52hgq6nvvvut09p9ik.apps.googleusercontent.com',
          callback: handleGoogleCredentialResponse,
        });
        google.accounts.id.renderButton(
          document.getElementById('google-signin-btn-container'),
          {
            theme: 'outline',
            size: 'large',
            text: 'continue_with',
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
    setLoading(true);
    try {
      // Allow any role to log in from the main portal (Customer, Admin, Staff)
      const role = await login({ email, password }, { allowedRoles: ['Customer', 'Admin', 'Staff'] });
      
      if (remember) {
        localStorage.setItem('remember_email', email);
      } else {
        localStorage.removeItem('remember_email');
      }

      if (role === 'Admin') {
        navigate('/admin/dashboard', { replace: true });
      } else if (role === 'Staff') {
        navigate('/staff/dashboard', { replace: true });
      } else {
        navigate('/', { replace: true });
      }
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        (err instanceof Error ? err.message : 'Login failed');
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

      {/* Main Glassmorphism Login Panel */}
      <main className="relative z-10 w-full max-w-[400px] glass-panel rounded-[20px] p-6 md:p-8 flex flex-col items-center my-4">
        {/* Brand Identity */}
        <div className="mb-4 flex flex-col items-center gap-2">
          <div className="w-12 h-12 bg-white rounded-full flex items-center justify-center backdrop-blur-md">
            <span className="material-symbols-outlined text-[#2d5016] text-2xl" style={{ fontVariationSettings: "'FILL' 1" }}>
              eco
            </span>
          </div>
          <h1 className="font-h2 text-2xl text-white text-center">Welcome Back</h1>
          <p className="font-body-md text-xs text-white/70 text-center">Secure access to your botanical data</p>
        </div>

        {error && (
          <div className="admin-login-error w-full mb-3 text-center text-xs" style={{ backgroundColor: 'rgba(239, 68, 68, 0.2)', border: '1px solid rgb(239, 68, 68)', padding: '0.5rem', borderRadius: '0.375rem', color: '#f87171' }}>
            {error}
          </div>
        )}

        {/* Login Form */}
        <form onSubmit={handleSubmit} className="w-full space-y-4">
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

          <div className="space-y-1.5">
            <div className="flex justify-between items-center px-1">
              <label className="font-label-sm text-xs text-white/80 block" htmlFor="password">
                Password
              </label>
              <a className="font-label-sm text-[11px] text-white/60 hover:text-white transition-colors" href="#">
                Forgot Password?
              </a>
            </div>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg z-10">
                lock
              </span>
              <input
                className="glass-input w-full rounded-[10px] py-2.5 pl-10 pr-10 text-sm transition-all duration-200"
                id="password"
                type={showPassword ? 'text' : 'password'}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                required
              />
              <button
                className="material-symbols-outlined absolute right-3.5 top-1/2 -translate-y-1/2 text-white/50 text-lg hover:text-white transition-colors z-10"
                type="button"
                onClick={() => setShowPassword((v) => !v)}
              >
                {showPassword ? 'visibility_off' : 'visibility'}
              </button>
            </div>
          </div>

          <div className="flex items-center ml-1">
            <input
              id="remember"
              type="checkbox"
              checked={remember}
              onChange={(e) => setRemember(e.target.checked)}
              className="rounded border-white/20 bg-white/10 text-[#2d5016] w-3.5 h-3.5 focus:ring-0 focus:ring-offset-0"
            />
            <label htmlFor="remember" className="ml-2 font-label-sm text-xs text-white/80 cursor-pointer">
              Remember me
            </label>
          </div>

          <div className="pt-2">
            <button
              className="w-full text-white font-label-sm text-base py-3 rounded-[10px] active:scale-[0.98] transition-all shadow-xl bg-[#2d5016] hover:bg-[#173901] shadow-black/20"
              type="submit"
              disabled={loading}
            >
              {loading ? 'Signing in...' : 'Sign In'}
            </button>
          </div>
        </form>

        {/* Bottom Actions */}
        <div className="mt-5 flex flex-col items-center gap-3 justify-center w-full">
          <p className="font-body-md text-xs text-white/70">
            Don't have an account?
            <button
              type="button"
              onClick={() => navigate('/signup')}
              className="text-white font-bold hover:underline decoration-2 underline-offset-4 ml-1 bg-transparent border-none p-0 cursor-pointer font-body-md"
            >
              Create an Account
            </button>
          </p>
          <div className="flex items-center justify-center gap-3 w-full mt-1">
            <div className="relative w-11 h-11">
              {/* Custom styled Google button */}
              <button
                className="absolute inset-0 w-full h-full glass-panel rounded-lg flex items-center justify-center hover:bg-white/20 transition-all text-white/90 hover:text-white pointer-events-none"
                type="button"
                title="Sign in with Google"
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
                id="google-signin-btn-container"
                className="absolute inset-0 w-full h-full opacity-0 overflow-hidden cursor-pointer [&>div]:w-full [&>div]:h-full [&_iframe]:w-full [&_iframe]:h-full"
              ></div>
            </div>
            <button
              onClick={() => document.getElementById('email')?.focus()}
              className="w-11 h-11 glass-panel rounded-lg flex items-center justify-center hover:bg-white/20 transition-all text-white/90 hover:text-white"
              type="button"
              title="Sign in with Email"
            >
              <span className="material-symbols-outlined text-xl">mail</span>
            </button>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Login;
