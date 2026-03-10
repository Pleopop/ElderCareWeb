import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Pages.css';
import CustomerProfile from './profiles/CustomerProfile';
import CaregiverProfile from './profiles/CaregiverProfile';

export default function Profile() {
    const { user } = useAuth();
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadProfile();
    }, []);

    const loadProfile = async () => {
        setLoading(true);
        const res = await api.get('/auth/me');
        if (res.isSuccess) setProfile(res.data);
        setLoading(false);
    };

    if (loading) return <div className="loading-spinner" />;

    // Route to appropriate profile component based on role
    // 0 = Customer, 1 = Caregiver, 2 = Admin
    if (profile?.role === 0) {
        return <CustomerProfile user={profile} onRefresh={loadProfile} />;
    } else if (profile?.role === 1) {
        return <CaregiverProfile user={profile} onRefresh={loadProfile} />;
    } else {
        // Admin or unknown role - show generic profile
        return <GenericProfile profile={profile} />;
    }
}

function GenericProfile({ profile }) {
    const roleLabel = { 0: 'Khách hàng', 1: 'Người chăm sóc', 2: 'Quản trị viên' };
    const statusLabel = { 0: 'Đang chờ', 1: 'Hoạt động', 2: 'Bị khóa', 3: 'Đã xóa' };

    return (
        <div>
            <div className="page-header">
                <div>
                    <h1 className="page-title">👤 Hồ Sơ Cá Nhân</h1>
                    <p className="page-subtitle">Xem và quản lý thông tin tài khoản</p>
                </div>
            </div>

            <div className="profile-layout">
                <div className="profile-card card">
                    <div className="profile-avatar-section">
                        <div className="profile-avatar-large">
                            {(profile?.email || '?')[0].toUpperCase()}
                        </div>
                        <h2 className="profile-name">{profile?.fullName || profile?.email}</h2>
                        <span className="badge badge-success">{roleLabel[profile?.role] || 'Người dùng'}</span>
                    </div>

                    <div className="profile-details">
                        <div className="profile-field">
                            <label>📧 Email</label>
                            <span>{profile?.email}</span>
                            {profile?.isEmailVerified && <span className="verified-badge">✅ Đã xác thực</span>}
                        </div>

                        <div className="profile-field">
                            <label>📱 Số điện thoại</label>
                            <span>{profile?.phoneNumber || 'Chưa cung cấp'}</span>
                            {profile?.isPhoneVerified && <span className="verified-badge">✅ Đã xác thực</span>}
                        </div>

                        <div className="profile-field">
                            <label>📋 Trạng thái</label>
                            <span className={`badge ${profile?.status === 1 ? 'badge-success' : 'badge-warning'}`}>
                                {statusLabel[profile?.status] || 'Không xác định'}
                            </span>
                        </div>

                        <div className="profile-field">
                            <label>🔑 Vai trò</label>
                            <span>{roleLabel[profile?.role] || 'Không xác định'}</span>
                        </div>
                    </div>
                </div>

                <div className="profile-sidebar">
                    <div className="card">
                        <h3>🔒 Bảo mật</h3>
                        <div className="security-items">
                            <div className="security-item">
                                <span>Email xác thực</span>
                                <span className={profile?.isEmailVerified ? 'text-success' : 'text-danger'}>
                                    {profile?.isEmailVerified ? '✅ Đã xác thực' : '❌ Chưa xác thực'}
                                </span>
                            </div>
                            <div className="security-item">
                                <span>SĐT xác thực</span>
                                <span className={profile?.isPhoneVerified ? 'text-success' : 'text-danger'}>
                                    {profile?.isPhoneVerified ? '✅ Đã xác thực' : '❌ Chưa xác thực'}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div className="card">
                        <h3>💡 Hướng dẫn</h3>
                        <p style={{ fontSize: 'var(--font-size-sm)', color: 'var(--text-secondary)', lineHeight: 1.6 }}>
                            Để cập nhật thông tin cá nhân, vui lòng liên hệ bộ phận hỗ trợ qua email hoặc hotline.
                            Chúng tôi sẽ xác minh và cập nhật thông tin trong vòng 24 giờ.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
