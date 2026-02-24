import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Dashboard.css';

export default function Dashboard() {
    const { user } = useAuth();
    const role = user?.role || 'Customer';
    const [stats, setStats] = useState({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadDashboard();
    }, []);

    const loadDashboard = async () => {
        setLoading(true);
        try {
            const [walletRes, bookingsRes, notiRes] = await Promise.all([
                api.get('/payments/wallet'),
                api.get(`/bookings/${role === 'Caregiver' ? 'caregiver-bookings' : 'my-bookings'}`),
                api.get('/notifications?pageSize=5'),
            ]);
            setStats({
                wallet: walletRes.isSuccess ? walletRes.data : null,
                bookings: bookingsRes.isSuccess ? bookingsRes.data : [],
                notifications: notiRes.isSuccess ? notiRes.data : [],
            });
        } catch (e) { console.error(e); }
        setLoading(false);
    };

    const formatCurrency = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải dữ liệu...</p></div>;

    return (
        <div className="dashboard animate-fade">
            {/* Welcome */}
            <div className="dashboard-welcome">
                <div>
                    <h1 className="dashboard-title">
                        Xin chào, <span className="text-gold">{user?.fullName || user?.email}</span> 👋
                    </h1>
                    <p className="dashboard-subtitle">
                        {role === 'Customer' ? 'Quản lý dịch vụ chăm sóc của bạn' : role === 'Caregiver' ? 'Theo dõi lịch chăm sóc và thu nhập' : 'Tổng quan hệ thống quản trị'}
                    </p>
                </div>
            </div>

            {/* Stats Cards */}
            <div className="dashboard-stats">
                <div className="stat-card card card-gold">
                    <div className="stat-icon">💰</div>
                    <div className="stat-info">
                        <div className="stat-label">{role === 'Caregiver' ? 'Thu nhập' : 'Số dư ví'}</div>
                        <div className="stat-value">{formatCurrency(stats.wallet?.balance)}</div>
                    </div>
                </div>
                <div className="stat-card card">
                    <div className="stat-icon">📅</div>
                    <div className="stat-info">
                        <div className="stat-label">Tổng đặt lịch</div>
                        <div className="stat-value">{stats.bookings?.length || 0}</div>
                    </div>
                </div>
                <div className="stat-card card">
                    <div className="stat-icon">✅</div>
                    <div className="stat-info">
                        <div className="stat-label">Đã hoàn thành</div>
                        <div className="stat-value">{stats.bookings?.filter?.(b => b.status === 'Completed')?.length || 0}</div>
                    </div>
                </div>
                <div className="stat-card card">
                    <div className="stat-icon">🔔</div>
                    <div className="stat-info">
                        <div className="stat-label">Thông báo mới</div>
                        <div className="stat-value">{stats.notifications?.filter?.(n => !n.isRead)?.length || 0}</div>
                    </div>
                </div>
            </div>

            {/* Content Grid */}
            <div className="dashboard-grid">
                {/* Recent Bookings */}
                <div className="card">
                    <div className="card-header">
                        <h3>📅 Lịch chăm sóc gần đây</h3>
                        <Link to="/bookings" className="btn btn-ghost btn-sm">Xem tất cả →</Link>
                    </div>
                    {stats.bookings && stats.bookings.length > 0 ? (
                        <div className="booking-list">
                            {stats.bookings.slice(0, 5).map((b, i) => (
                                <div key={i} className="booking-item">
                                    <div className="booking-item-info">
                                        <div className="booking-item-name">{b.beneficiaryName || b.caregiverName || 'Booking #' + (i + 1)}</div>
                                        <div className="booking-item-date">{new Date(b.scheduledDate || b.createdAt).toLocaleDateString('vi-VN')}</div>
                                    </div>
                                    <span className={`badge ${b.status === 'Completed' ? 'badge-success' : b.status === 'InProgress' ? 'badge-gold' : b.status === 'Cancelled' ? 'badge-danger' : 'badge-info'}`}>
                                        {b.status === 'Pending' ? 'Chờ duyệt' : b.status === 'Accepted' ? 'Đã duyệt' : b.status === 'InProgress' ? 'Đang thực hiện' : b.status === 'Completed' ? 'Hoàn thành' : b.status === 'Cancelled' ? 'Đã hủy' : b.status}
                                    </span>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className="empty-state">
                            <div className="empty-state-icon">📅</div>
                            <div className="empty-state-title">Chưa có lịch chăm sóc</div>
                            <p>Bắt đầu đặt lịch ngay</p>
                        </div>
                    )}
                </div>

                {/* Quick Actions */}
                <div className="card">
                    <div className="card-header">
                        <h3>⚡ Thao tác nhanh</h3>
                    </div>
                    <div className="quick-actions">
                        {role === 'Customer' && (
                            <>
                                <Link to="/bookings" className="quick-action-btn">
                                    <span className="quick-action-icon">📅</span>
                                    <span>Đặt lịch mới</span>
                                </Link>
                                <Link to="/wallet" className="quick-action-btn">
                                    <span className="quick-action-icon">💰</span>
                                    <span>Nạp tiền</span>
                                </Link>
                                <Link to="/chat" className="quick-action-btn">
                                    <span className="quick-action-icon">💬</span>
                                    <span>Tin nhắn</span>
                                </Link>
                            </>
                        )}
                        {role === 'Caregiver' && (
                            <>
                                <Link to="/bookings" className="quick-action-btn">
                                    <span className="quick-action-icon">📋</span>
                                    <span>Xem lịch</span>
                                </Link>
                                <Link to="/wallet" className="quick-action-btn">
                                    <span className="quick-action-icon">💵</span>
                                    <span>Thu nhập</span>
                                </Link>
                                <Link to="/chat" className="quick-action-btn">
                                    <span className="quick-action-icon">💬</span>
                                    <span>Tin nhắn</span>
                                </Link>
                            </>
                        )}
                        {role === 'Admin' && (
                            <>
                                <Link to="/admin" className="quick-action-btn">
                                    <span className="quick-action-icon">✅</span>
                                    <span>Duyệt CG</span>
                                </Link>
                                <Link to="/admin" className="quick-action-btn">
                                    <span className="quick-action-icon">🛡️</span>
                                    <span>Fraud Alerts</span>
                                </Link>
                            </>
                        )}
                        <Link to="/notifications" className="quick-action-btn">
                            <span className="quick-action-icon">🔔</span>
                            <span>Thông báo</span>
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}
