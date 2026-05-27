import React from 'react';

const AboutPage: React.FC = () => {
  return (
    <div className="max-w-[1280px] mx-auto px-6 lg:px-8 space-y-24 pb-24 pt-32 text-on-background font-body-md overflow-x-hidden">
      {/* Hero Section */}
      <section className="relative rounded-3xl overflow-hidden min-h-[440px] flex items-center shadow-2xl border border-outline-variant/20 bg-gradient-to-br from-primary via-primary-container to-secondary">
        <div className="absolute inset-0 z-0">
          <div
            className="absolute inset-0 opacity-10"
            style={{
              backgroundImage: 'radial-gradient(circle at 2px 2px, white 1px, transparent 0)',
              backgroundSize: '40px 40px',
            }}
          ></div>
          <div className="absolute -right-20 -bottom-20 opacity-10 rotate-12 select-none pointer-events-none">
            <span className="material-symbols-outlined text-[400px] text-white">eco</span>
          </div>
        </div>

        <div className="relative z-10 p-8 lg:p-20 max-w-3xl space-y-6">
          <div className="inline-flex items-center gap-2 px-3.5 py-1.5 bg-white/10 backdrop-blur-md text-white rounded-full text-xs font-bold uppercase tracking-wider border border-white/20">
            <span className="material-symbols-outlined text-sm">verified</span>
            Tương lai của Nông nghiệp
          </div>
          <h1 className="font-h1 text-4xl lg:text-5xl font-bold text-white leading-tight">
            Giải pháp Sinh học <br />
            <span className="text-[#a8d38a] italic font-normal">Kiểm soát Sâu bệnh</span> Toàn diện
          </h1>
          <p className="text-base lg:text-lg text-white/90 font-light leading-relaxed max-w-2xl">
            BioPestControl là nền tảng quản lý chuyên sâu cho các sản phẩm kiểm soát sâu bệnh sinh học
            hiện đại, tích hợp trí tuệ nhân tạo (AI) giúp bảo vệ môi trường và tối ưu năng suất cây
            trồng của bạn.
          </p>
          <div className="pt-4">
            <a
              href="/products"
              className="bg-white text-primary hover:bg-surface-container hover:scale-[1.02] active:scale-[0.98] transition-all px-8 py-3.5 rounded-full font-bold shadow-xl hover:shadow-2xl inline-flex items-center gap-2 text-sm"
            >
              <span className="material-symbols-outlined text-sm">shopping_basket</span> Khám phá
              Sản phẩm
            </a>
          </div>
        </div>
      </section>

      {/* Content Sections */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-16 items-start">
        {/* Text Content */}
        <div className="space-y-12">
          <div className="space-y-4">
            <div>
              <h2 className="text-secondary font-bold text-sm uppercase tracking-widest mb-1">
                Giới Thiệu
              </h2>
              <h3 className="font-h2 text-3xl font-bold text-primary">
                Nền tảng quản lý thông minh
              </h3>
            </div>
            <p className="text-on-surface-variant leading-relaxed font-light text-base">
              BioPestControl cung cấp các giải pháp sinh học hiện đại được hỗ trợ bởi trí tuệ nhân
              tạo (AI) giúp các hộ nông dân dễ dàng lựa chọn sản phẩm phù hợp nhất cho loại đất và cây
              trồng của mình, đảm bảo hiệu suất tối đa và an toàn môi trường.
            </p>
          </div>

          <div className="space-y-4">
            <div>
              <h2 className="text-secondary font-bold text-sm uppercase tracking-widest mb-1">
                Sứ Mệnh
              </h2>
              <h3 className="font-h2 text-3xl font-bold text-primary">
                Bảo vệ thiên nhiên, đồng hành cùng nhà nông
              </h3>
            </div>
            <p className="text-on-surface-variant leading-relaxed font-light text-base">
              Sứ mệnh của chúng tôi là mang công nghệ nông nghiệp tiên tiến đến từng địa phương, giúp
              người nông dân tối ưu hóa năng suất thông qua các giải pháp sinh học an toàn, bền vững
              và hiệu quả kinh tế cao.
            </p>
          </div>

          <div className="grid grid-cols-2 gap-8 pt-4">
            <div className="p-6 bg-secondary/5 rounded-2xl border border-outline-variant/20 hover:bg-secondary/10 transition-colors">
              <div className="text-4xl font-bold text-primary mb-1">10K+</div>
              <div className="text-[10px] text-on-surface-variant font-bold uppercase tracking-widest">
                Nông dân tin dùng
              </div>
            </div>
            <div className="p-6 bg-secondary/5 rounded-2xl border border-outline-variant/20 hover:bg-secondary/10 transition-colors">
              <div className="text-4xl font-bold text-primary mb-1">98%</div>
              <div className="text-[10px] text-on-surface-variant font-bold uppercase tracking-widest">
                Hiệu quả bảo vệ
              </div>
            </div>
          </div>
        </div>

        {/* Features Grid */}
        <div className="space-y-5">
          <h4 className="font-h3 text-2xl font-bold text-primary mb-6 px-1">Tại sao chọn chúng tôi?</h4>

          <div className="group p-6 bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/20 hover:border-primary/40 shadow-sm hover:shadow-md transition-all hover:-translate-y-1 cursor-default">
            <div className="flex items-start gap-5">
              <div className="w-12 h-12 bg-primary/5 text-primary rounded-xl flex items-center justify-center flex-shrink-0">
                <span className="material-symbols-outlined">smart_toy</span>
              </div>
              <div>
                <h5 className="font-bold text-primary text-base mb-1">Công nghệ AI hiện đại</h5>
                <p className="text-sm text-on-surface-variant font-light leading-relaxed">
                  Khuyến nghị sản phẩm chuẩn xác dựa trên dữ liệu phân tích đất và cây trồng thực tế.
                </p>
              </div>
            </div>
          </div>

          <div className="group p-6 bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/20 hover:border-primary/40 shadow-sm hover:shadow-md transition-all hover:-translate-y-1 cursor-default">
            <div className="flex items-start gap-5">
              <div className="w-12 h-12 bg-primary/5 text-primary rounded-xl flex items-center justify-center flex-shrink-0">
                <span className="material-symbols-outlined">verified_user</span>
              </div>
              <div>
                <h5 className="font-bold text-primary text-base mb-1">Sản phẩm chứng nhận an toàn</h5>
                <p className="text-sm text-on-surface-variant font-light leading-relaxed">
                  Cam kết an toàn tuyệt đối cho sức khỏe con người và bảo vệ hệ sinh thái bền vững.
                </p>
              </div>
            </div>
          </div>

          <div className="group p-6 bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/20 hover:border-primary/40 shadow-sm hover:shadow-md transition-all hover:-translate-y-1 cursor-default">
            <div className="flex items-start gap-5">
              <div className="w-12 h-12 bg-primary/5 text-primary rounded-xl flex items-center justify-center flex-shrink-0">
                <span className="material-symbols-outlined">local_shipping</span>
              </div>
              <div>
                <h5 className="font-bold text-primary text-base mb-1">Giao hàng nhanh chóng</h5>
                <p className="text-sm text-on-surface-variant font-light leading-relaxed">
                  Hệ thống kho vận hiện đại, đảm bảo giao hàng tận nơi nhanh chóng trên toàn quốc.
                </p>
              </div>
            </div>
          </div>

          <div className="group p-6 bg-white dark:bg-surface-container rounded-2xl border border-outline-variant/20 hover:border-primary/40 shadow-sm hover:shadow-md transition-all hover:-translate-y-1 cursor-default">
            <div className="flex items-start gap-5">
              <div className="w-12 h-12 bg-primary/5 text-primary rounded-xl flex items-center justify-center flex-shrink-0">
                <span className="material-symbols-outlined">psychology</span>
              </div>
              <div>
                <h5 className="font-bold text-primary text-base mb-1">Chuyên gia đồng hành</h5>
                <p className="text-sm text-on-surface-variant font-light leading-relaxed">
                  Đội ngũ kỹ thuật viên giàu kinh nghiệm luôn sẵn sàng tư vấn và giải đáp thắc mắc.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Contact Info CTA */}
      <section className="bg-primary rounded-3xl p-8 lg:p-16 text-white relative overflow-hidden group border border-primary/20 shadow-2xl">
        <div className="absolute right-0 top-0 opacity-10 transform translate-x-1/4 -translate-y-1/4 rotate-12 transition-transform duration-700 group-hover:scale-110 select-none pointer-events-none">
          <span className="material-symbols-outlined text-[300px]">eco</span>
        </div>

        <div className="relative z-10 grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div className="space-y-4">
            <h2 className="font-h2 text-3xl lg:text-4xl font-bold leading-tight">
              Bạn cần tư vấn? <br />
              <span className="text-[#a8d38a] italic font-normal">Hãy cùng chúng tôi bảo vệ cây trồng.</span>
            </h2>
            <p className="text-white/80 font-light text-base">
              Đội ngũ kỹ thuật của BioPestControl luôn sẵn sàng hỗ trợ các hộ nông dân 24/7.
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
            <div className="bg-white/10 backdrop-blur-md rounded-2xl p-6 border border-white/10 hover:bg-white/20 transition-all">
              <div className="flex items-center gap-3 mb-2">
                <span className="material-symbols-outlined text-sm">mail</span>
                <span className="text-[10px] font-bold uppercase tracking-widest text-[#a8d38a]">
                  Email Hỗ Trợ
                </span>
              </div>
              <p className="font-bold text-sm lg:text-base break-all">info@biopestcontrol.com</p>
            </div>
            <div className="bg-white/10 backdrop-blur-md rounded-2xl p-6 border border-white/10 hover:bg-white/20 transition-all">
              <div className="flex items-center gap-3 mb-2">
                <span className="material-symbols-outlined text-sm">phone_in_talk</span>
                <span className="text-[10px] font-bold uppercase tracking-widest text-[#a8d38a]">
                  Hotline 24/7
                </span>
              </div>
              <p className="font-bold text-base lg:text-lg">0942004995</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default AboutPage;
