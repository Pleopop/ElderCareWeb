import { Link } from 'react-router-dom';
import './Landing.css';

export default function Landing() {
    return (
        <div className="landing">
            {/* --- Navbar --- */}
            <nav className="landing-nav">
                <div className="container landing-nav-inner">
                    <Link to="/" className="landing-nav-brand">
                        <img src="/logo.png" alt="Tuổi Vàng" className="landing-nav-logo" />
                        <span className="landing-nav-name">Tuổi Vàng</span>
                    </Link>
                    <div className="landing-nav-links">
                        <a href="#features" className="landing-nav-link">Tính năng</a>
                        <a href="#how-it-works" className="landing-nav-link">Cách hoạt động</a>
                        <a href="#testimonials" className="landing-nav-link">Đánh giá</a>
                        <Link to="/login" className="btn btn-ghost">Đăng nhập</Link>
                        <Link to="/register" className="btn btn-primary">Đăng ký ngay</Link>
                    </div>
                </div>
            </nav>

            {/* --- Hero --- */}
            <section className="hero">
                <div className="hero-bg-pattern" />
                <div className="container hero-content">
                    <div className="hero-text animate-slide-up">
                        <div className="hero-badge">🤝 Nền tảng chăm sóc người cao tuổi #1 Việt Nam</div>
                        <h1 className="hero-title">
                            Nhẹ việc nhà,<br />
                            <span className="text-gold">ấm tuổi già</span>
                        </h1>
                        <p className="hero-description">
                            Kết nối gia đình với những người chăm sóc chuyên nghiệp, tận tâm.
                            Đảm bảo an toàn, minh bạch và chất lượng dịch vụ hàng đầu.
                        </p>
                        <div className="hero-actions">
                            <Link to="/register" className="btn btn-primary btn-lg">
                                ✨ Bắt đầu miễn phí
                            </Link>
                            <Link to="/register?role=caregiver" className="btn btn-secondary btn-lg">
                                Trở thành người chăm sóc
                            </Link>
                        </div>
                        <div className="hero-stats">
                            <div className="hero-stat">
                                <div className="hero-stat-number">1,200+</div>
                                <div className="hero-stat-label">Người chăm sóc</div>
                            </div>
                            <div className="hero-stat">
                                <div className="hero-stat-number">5,000+</div>
                                <div className="hero-stat-label">Gia đình tin dùng</div>
                            </div>
                            <div className="hero-stat">
                                <div className="hero-stat-number">98%</div>
                                <div className="hero-stat-label">Hài lòng</div>
                            </div>
                        </div>
                    </div>
                    <div className="hero-visual animate-scale">
                        <div className="hero-card-stack">
                            <div className="hero-card hero-card-1">
                                <div className="hero-card-icon">👵</div>
                                <div>
                                    <div className="hero-card-title">Bà Nguyễn Thị Lan</div>
                                    <div className="hero-card-sub">Đang được chăm sóc tận tình</div>
                                </div>
                                <span className="badge badge-success">● Đang hoạt động</span>
                            </div>
                            <div className="hero-card hero-card-2">
                                <div className="hero-card-icon">⭐</div>
                                <div>
                                    <div className="hero-card-title">Đánh giá 4.9/5</div>
                                    <div className="hero-card-sub">Từ 320 lượt đánh giá</div>
                                </div>
                            </div>
                            <div className="hero-card hero-card-3">
                                <div className="hero-card-icon">🛡️</div>
                                <div>
                                    <div className="hero-card-title">An toàn tuyệt đối</div>
                                    <div className="hero-card-sub">GPS + escrow thanh toán</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* --- Features --- */}
            <section id="features" className="features-section">
                <div className="container">
                    <div className="section-header animate-slide-up">
                        <div className="section-badge">✨ Tính năng nổi bật</div>
                        <h2 className="section-title">Tất cả những gì bạn cần</h2>
                        <p className="section-subtitle">Quản lý chăm sóc người cao tuổi dễ dàng, toàn diện và an toàn</p>
                    </div>
                    <div className="features-grid">
                        {[
                            { icon: '🤖', title: 'AI Matching', desc: 'Ghép nối thông minh dựa trên tính cách, kỹ năng và sở thích' },
                            { icon: '📅', title: 'Đặt lịch linh hoạt', desc: 'Đặt lịch chăm sóc theo giờ, ngày hoặc dài hạn' },
                            { icon: '📍', title: 'Theo dõi GPS', desc: 'Theo dõi vị trí người chăm sóc real-time, an tâm tuyệt đối' },
                            { icon: '💰', title: 'Thanh toán an toàn', desc: 'Hệ thống escrow đảm bảo tiền của bạn luôn được bảo vệ' },
                            { icon: '💬', title: 'Chat trực tiếp', desc: 'Nhắn tin trực tiếp giữa gia đình và người chăm sóc' },
                            { icon: '🔔', title: 'Thông báo tức thì', desc: 'Cập nhật mọi thay đổi qua hệ thống thông báo' },
                        ].map((f, i) => (
                            <div key={i} className="feature-card card animate-slide-up" style={{ animationDelay: `${i * 0.1}s` }}>
                                <div className="feature-icon">{f.icon}</div>
                                <h3 className="feature-title">{f.title}</h3>
                                <p className="feature-desc">{f.desc}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* --- How It Works --- */}
            <section id="how-it-works" className="how-section">
                <div className="container">
                    <div className="section-header animate-slide-up">
                        <div className="section-badge">🚀 Dễ dàng bắt đầu</div>
                        <h2 className="section-title">Chỉ 3 bước đơn giản</h2>
                    </div>
                    <div className="steps-grid">
                        {[
                            { step: '01', title: 'Đăng ký tài khoản', desc: 'Tạo tài khoản miễn phí, thêm thông tin người thân cần chăm sóc' },
                            { step: '02', title: 'Tìm người chăm sóc', desc: 'AI tự động gợi ý người chăm sóc phù hợp nhất cho gia đình bạn' },
                            { step: '03', title: 'Bắt đầu chăm sóc', desc: 'Đặt lịch, theo dõi GPS, thanh toán an toàn qua escrow' },
                        ].map((s, i) => (
                            <div key={i} className="step-card animate-slide-up" style={{ animationDelay: `${i * 0.15}s` }}>
                                <div className="step-number">{s.step}</div>
                                <h3 className="step-title">{s.title}</h3>
                                <p className="step-desc">{s.desc}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* --- Testimonials --- */}
            <section id="testimonials" className="testimonials-section">
                <div className="container">
                    <div className="section-header animate-slide-up">
                        <div className="section-badge">💛 Khách hàng nói gì</div>
                        <h2 className="section-title">Được tin yêu bởi hàng ngàn gia đình</h2>
                    </div>
                    <div className="testimonials-grid">
                        {[
                            { name: 'Chị Nguyễn Mai', role: 'Khách hàng tại TP.HCM', text: 'Tuổi Vàng giúp gia đình tôi yên tâm khi đi làm. Người chăm sóc rất tận tâm và chuyên nghiệp.' },
                            { name: 'Anh Trần Văn Hùng', role: 'Khách hàng tại Hà Nội', text: 'Tính năng GPS tracking rất hữu ích. Tôi luôn biết mẹ tôi đang được chăm sóc ở đâu.' },
                            { name: 'Cô Lê Thị Hoa', role: 'Người chăm sóc', text: 'Nền tảng giúp tôi có thu nhập ổn định và được ghi nhận. Thanh toán nhanh chóng, minh bạch.' },
                        ].map((t, i) => (
                            <div key={i} className="testimonial-card card animate-slide-up" style={{ animationDelay: `${i * 0.1}s` }}>
                                <div className="testimonial-stars">⭐⭐⭐⭐⭐</div>
                                <p className="testimonial-text">"{t.text}"</p>
                                <div className="testimonial-author">
                                    <div className="avatar">{t.name[0]}</div>
                                    <div>
                                        <div className="testimonial-name">{t.name}</div>
                                        <div className="testimonial-role">{t.role}</div>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* --- CTA --- */}
            <section className="cta-section">
                <div className="container cta-content">
                    <h2 className="cta-title">Bắt đầu hành trình chăm sóc ngay hôm nay</h2>
                    <p className="cta-subtitle">Đăng ký miễn phí và tìm người chăm sóc phù hợp trong vài phút</p>
                    <Link to="/register" className="btn btn-primary btn-lg">
                        ✨ Đăng ký ngay — Miễn phí
                    </Link>
                </div>
            </section>

            {/* --- Footer --- */}
            <footer className="landing-footer">
                <div className="container footer-content">
                    <div className="footer-brand">
                        <img src="/logo.png" alt="Tuổi Vàng" className="footer-logo" />
                        <h3>Tuổi Vàng</h3>
                        <p className="footer-tagline">Nhẹ việc nhà, ấm tuổi già</p>
                    </div>
                    <div className="footer-links">
                        <div>
                            <h4>Dịch vụ</h4>
                            <a href="#">Chăm sóc tại nhà</a>
                            <a href="#">Chăm sóc dài hạn</a>
                            <a href="#">Tư vấn sức khỏe</a>
                        </div>
                        <div>
                            <h4>Hỗ trợ</h4>
                            <a href="#">Câu hỏi thường gặp</a>
                            <a href="#">Liên hệ hỗ trợ</a>
                            <a href="#">Điều khoản sử dụng</a>
                        </div>
                        <div>
                            <h4>Liên hệ</h4>
                            <a href="#">📧 support@tuoivang.vn</a>
                            <a href="#">📞 1900-xxxx</a>
                            <a href="#">📍 TP. Hồ Chí Minh</a>
                        </div>
                    </div>
                </div>
                <div className="footer-bottom">
                    <div className="container">
                        <p>© 2026 Tuổi Vàng. All rights reserved.</p>
                    </div>
                </div>
            </footer>
        </div>
    );
}
