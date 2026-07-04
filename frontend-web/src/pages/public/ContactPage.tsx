import React, { useState } from 'react';
import { submitContact } from '../../services/feedbackService';

const ContactPage: React.FC = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [message, setMessage] = useState('');
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    try {
      await submitContact({
        name,
        email,
        phone,
        message
      });
      setSuccess('Cảm ơn bạn đã gửi lời nhắn! Đội ngũ tư vấn sẽ liên hệ lại với bạn trong vòng 24h.');
      setName('');
      setEmail('');
      setPhone('');
      setMessage('');
    } catch (err) {
      console.error('Failed to submit contact', err);
      setError('Đã xảy ra lỗi khi gửi lời nhắn, vui lòng thử lại sau.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-background text-on-background font-body-md overflow-x-hidden pt-16">
      {/* Hero Section */}
      <section className="relative h-[400px] flex items-center justify-center overflow-hidden">
        <div className="absolute inset-0 z-0">
          <div className="absolute inset-0 bg-gradient-to-r from-black/60 to-primary/40 z-10"></div>
          <img
            alt="Healthy green crops in field"
            className="w-full h-full object-cover"
            src="https://lh3.googleusercontent.com/aida-public/AB6AXuD1v5QtMLjXFjj9tiYQpdFudr4bUHPkjnN15jmWWr9kIk6dVkb96F7NOkQBADgCQ3gpxQb657Jh27EEBWwqj_F7rQ6vYoh01kN9o_NuwHI14uPk_-aeFA99mlMqz2qfWSOaEP6i6n_KyWYPNqYa3QuctpslYEJshjA5W0ZuryVfxkz_Tif_fswotI6HwqQj9xB6AFD3TurhjQw-A1L3HtibASM3hd7ITGWIJ63mlfyICxrUFBwQ9IqWatDp5zDPezvULUkM-MxFPIc"
          />
        </div>
        <div className="relative z-20 text-center px-6 max-w-3xl mx-auto">
          <h1 className="font-h1 text-4xl md:text-5xl lg:text-6xl font-black text-white mb-6 drop-shadow-lg leading-tight">
            Liên Hệ Với Chúng Tôi
          </h1>
          <p className="font-body-lg text-lg md:text-xl text-white/90 font-light max-w-2xl mx-auto">
            Đội ngũ chuyên gia của BioPestControl luôn sẵn sàng hỗ trợ bà con nông dân vì một nền
            nông nghiệp bền vững.
          </p>
          <div className="mt-8">
            <a
              className="inline-flex items-center gap-2 bg-white text-primary px-8 py-3.5 rounded-full font-bold text-base hover:bg-primary hover:text-white transition-all transform hover:scale-105 shadow-xl hover:shadow-2xl"
              href="#contact-section"
            >
              <span className="material-symbols-outlined text-base">mail</span>
              Gửi lời nhắn ngay
            </a>
          </div>
        </div>
      </section>

      {/* Main Content Grid */}
      <section className="max-w-[1280px] mx-auto px-6 py-20 sm:px-8" id="contact-section">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-start">
          {/* Contact Form */}
          <div className="lg:col-span-7 bg-white dark:bg-surface-container p-8 rounded-2xl shadow-xl border border-outline-variant/10">
            <div className="mb-8">
              <h2 className="font-h3 text-2xl font-bold mb-2 flex items-center gap-2 text-primary">
                <span className="material-symbols-outlined">chat</span>
                Gửi tin nhắn cho chúng tôi
              </h2>
              <p className="text-sm text-on-surface-variant">
                Để lại thông tin, chúng tôi sẽ phản hồi trong vòng 24h làm việc.
              </p>
            </div>

            {success && (
              <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg mb-6 flex items-center gap-2 text-sm">
                <span className="material-symbols-outlined text-lg">check_circle</span>
                {success}
              </div>
            )}

            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6 flex items-center gap-2 text-sm">
                <span className="material-symbols-outlined text-lg">error</span>
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-bold uppercase tracking-wider text-on-surface-variant block ml-1">
                    Họ và tên
                  </label>
                  <div className="relative">
                    <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant text-xl">
                      person
                    </span>
                    <input
                      value={name}
                      onChange={(e) => setName(e.target.value)}
                      className="w-full pl-12 pr-4 py-3 border border-outline-variant/30 rounded-xl focus:border-primary focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface dark:text-white text-sm"
                      placeholder="Nguyễn Văn A"
                      type="text"
                      required
                    />
                  </div>
                </div>
                <div className="flex flex-col gap-1.5">
                  <label className="text-xs font-bold uppercase tracking-wider text-on-surface-variant block ml-1">
                    Email
                  </label>
                  <div className="relative">
                    <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant text-xl">
                      alternate_email
                    </span>
                    <input
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="w-full pl-12 pr-4 py-3 border border-outline-variant/30 rounded-xl focus:border-primary focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface dark:text-white text-sm"
                      placeholder="email@vi-du.com"
                      type="email"
                      required
                    />
                  </div>
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-bold uppercase tracking-wider text-on-surface-variant block ml-1">
                  Số điện thoại
                </label>
                <div className="relative">
                  <span className="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant text-xl">
                    call
                  </span>
                  <input
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    className="w-full pl-12 pr-4 py-3 border border-outline-variant/30 rounded-xl focus:border-primary focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface dark:text-white text-sm"
                    placeholder="0901 234 567"
                    type="tel"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-bold uppercase tracking-wider text-on-surface-variant block ml-1">
                  Lời nhắn
                </label>
                <textarea
                  value={message}
                  onChange={(e) => setMessage(e.target.value)}
                  className="w-full p-4 border border-outline-variant/30 rounded-xl focus:border-primary focus:ring-1 focus:ring-primary/20 bg-background dark:bg-surface dark:text-white text-sm focus:outline-none"
                  placeholder="Nội dung cần tư vấn..."
                  rows={5}
                  required
                ></textarea>
              </div>

              <button
                disabled={loading}
                className="w-full md:w-auto px-12 py-3.5 bg-primary text-white font-bold rounded-xl hover:bg-primary/95 transition-all shadow-lg hover:shadow-xl active:scale-[0.98] shadow-primary/20 flex items-center justify-center gap-2 text-sm"
                type="submit"
              >
                {loading ? 'Đang gửi...' : 'Gửi Tin Nhắn'}
                <span className="material-symbols-outlined text-sm">send</span>
              </button>
            </form>
          </div>

          {/* Sidebar Information */}
          <div className="lg:col-span-5 space-y-8">
            <div className="bg-primary/5 dark:bg-primary/10 p-8 rounded-2xl border border-primary/20 space-y-8">
              <h3 className="font-h3 text-xl font-bold uppercase tracking-wider text-primary border-b border-primary/20 pb-3">
                Thông tin liên hệ
              </h3>
              <div className="space-y-6">
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center flex-shrink-0 text-white">
                    <span className="material-symbols-outlined">apartment</span>
                  </div>
                  <div>
                    <p className="text-[10px] font-bold text-primary/60 uppercase tracking-wider">
                      Công ty
                    </p>
                    <p className="text-base font-bold text-primary">Công ty BioPestControl</p>
                  </div>
                </div>
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center flex-shrink-0 text-white">
                    <span className="material-symbols-outlined">location_on</span>
                  </div>
                  <div>
                    <p className="text-[10px] font-bold text-primary/60 uppercase tracking-wider">
                      Địa chỉ
                    </p>
                    <p className="text-sm font-medium leading-relaxed">
                      600 Nguyễn Văn Cừ Nối Dài, An Bình, Bình Thủy, Cần Thơ 900000
                    </p>
                  </div>
                </div>
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center flex-shrink-0 text-white">
                    <span className="material-symbols-outlined">support_agent</span>
                  </div>
                  <div>
                    <p className="text-[10px] font-bold text-primary/60 uppercase tracking-wider">
                      Hotline hỗ trợ
                    </p>
                    <p className="text-2xl font-black text-primary leading-tight">0942004995</p>
                  </div>
                </div>
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center flex-shrink-0 text-white">
                    <span className="material-symbols-outlined">mail</span>
                  </div>
                  <div>
                    <p className="text-[10px] font-bold text-primary/60 uppercase tracking-wider">
                      Email
                    </p>
                    <p className="text-sm font-medium">info@biopestcontrol.com</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Map Container */}
            <div className="rounded-2xl overflow-hidden shadow-2xl border-4 border-white dark:border-slate-800 h-80 relative">
              <iframe
                title="Google Map Can Tho"
                src="https://www.google.com/maps?q=600+Nguyễn+Văn+Cừ+Nối+Dài,+An+Bình,+Bình+Thủy,+Cần+Thơ&output=embed"
                width="100%"
                height="100%"
                style={{ border: 0 }}
                allowFullScreen={true}
                loading="lazy"
                referrerPolicy="no-referrer-when-downgrade"
                className="absolute inset-0 w-full h-full object-cover"
              ></iframe>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default ContactPage;
