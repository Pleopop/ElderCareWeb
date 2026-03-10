import { useState, useEffect } from 'react';
import api from '../../api/client';
import '../Pages.css';

export default function CaregiverProfile({ user, onRefresh }) {
    const [caregiverInfo, setCaregiverInfo] = useState(null);
    const [skills, setSkills] = useState([]);
    const [availability, setAvailability] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [editMode, setEditMode] = useState(false);

    useEffect(() => {
        loadCaregiverProfile();
    }, []);

    const loadCaregiverProfile = async () => {
        setLoading(true);
        try {
            // Fetch caregiver profile
            const profileRes = await api.get('/caregiver-profiles/me');
            if (profileRes.isSuccess) {
                setCaregiverInfo(profileRes.data);
            }

            // Fetch skills
            const skillsRes = await api.get('/caregiver-profiles/me/skills');
            if (skillsRes.isSuccess) {
                setSkills(skillsRes.data || []);
            }

            // Fetch availability
            const availRes = await api.get('/caregiver-profiles/me/availability');
            if (availRes.isSuccess) {
                setAvailability(availRes.data || []);
            }
        } catch (err) {
            setError('Không thể tải thông tin hồ sơ');
        }
        setLoading(false);
    };

    const verificationStatusLabel = {
        0: 'Chưa xác minh',
        1: 'Chờ xét duyệt',
        2: 'Đã xác minh',
        3: 'Bị từ chối',
    };

    const statusLabel = { 0: 'Đang chờ', 1: 'Hoạt động', 2: 'Bị khóa', 3: 'Đã xóa' };

    return (
        <div>
            <div className="page-header">
                <div>
                    <h1 className="page-title">👨‍⚕️ Hồ Sơ Người Chăm Sóc</h1>
                    <p className="page-subtitle">Quản lý thông tin hồ sơ, kỹ năng và lịch làm việc</p>
                </div>
            </div>

            {loading ? (
                <div className="loading-overlay"><div className="spinner spinner-lg" /></div>
            ) : (
                <div className="profile-layout">
                    {/* User Basic Info */}
                    <div className="profile-card card">
                        <div className="profile-avatar-section">
                            <div className="profile-avatar-large">
                                {(user?.email || '?')[0].toUpperCase()}
                            </div>
                            <h2 className="profile-name">{user?.fullName || user?.email}</h2>
                            <span className="badge badge-warning">Người chăm sóc</span>
                        </div>

                        <div className="profile-details">
                            <div className="profile-field">
                                <label>📧 Email</label>
                                <span>{user?.email}</span>
                                {user?.isEmailVerified && <span className="verified-badge">✅ Đã xác thực</span>}
                            </div>

                            <div className="profile-field">
                                <label>📱 Số điện thoại</label>
                                <span>{user?.phoneNumber || 'Chưa cung cấp'}</span>
                                {user?.isPhoneVerified && <span className="verified-badge">✅ Đã xác thực</span>}
                            </div>

                            <div className="profile-field">
                                <label>📋 Trạng thái tài khoản</label>
                                <span className={`badge ${user?.status === 1 ? 'badge-success' : 'badge-warning'}`}>
                                    {statusLabel[user?.status] || 'Không xác định'}
                                </span>
                            </div>
                        </div>
                    </div>

                    {/* Professional Info Sidebar */}
                    <div className="profile-sidebar">
                        <div className="card">
                            <h3>⭐ Đánh Giá</h3>
                            <div style={{ textAlign: 'center', padding: '10px 0' }}>
                                <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#0066CC' }}>
                                    {caregiverInfo?.averageRating?.toFixed(1) || '―'}
                                </div>
                                <div style={{ color: '#666', fontSize: '12px' }}>
                                    ({caregiverInfo?.totalReviews || 0} đánh giá)
                                </div>
                            </div>
                        </div>

                        <div className="card">
                            <h3>✅ Xác Minh</h3>
                            <div className="security-items">
                                <div className="security-item">
                                    <span>Trạng thái xác minh</span>
                                    <span className="badge badge-info">
                                        {verificationStatusLabel[caregiverInfo?.verificationStatus] || 'Không xác định'}
                                    </span>
                                </div>
                                {caregiverInfo?.rejectionReason && (
                                    <div className="security-item">
                                        <span>Lý do từ chối</span>
                                        <span style={{ color: '#D32F2F', fontSize: '12px' }}>
                                            {caregiverInfo.rejectionReason}
                                        </span>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Professional Details Section */}
            <div style={{ marginTop: '40px' }} >
                <div className="page-header">
                    <h2 className="page-title">📋 Thông Tin Chuyên Nghiệp</h2>
                </div>

                {error && <div className="auth-error">{error}</div>}

                <div className="content-grid">
                    {/* Experience & Rate Card */}
                    <div className="card">
                        <h3>🎯 Kinh Nghiệm & Giá Dịch Vụ</h3>
                        <div className="detail-item">
                            <label>📊 Kinh nghiệm:</label>
                            <span>{caregiverInfo?.experienceYears || '―'} năm</span>
                        </div>
                        <div className="detail-item">
                            <label>💰 Giá giờ:</label>
                            <span>
                                {caregiverInfo?.hourlyRate 
                                    ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(caregiverInfo.hourlyRate)
                                    : '―'}
                            </span>
                        </div>
                        <div className="detail-item">
                            <label>📍 Bán kính phục vụ:</label>
                            <span>{caregiverInfo?.serviceRadiusKm || '―'} km</span>
                        </div>
                    </div>

                    {/* Bio Card */}
                    <div className="card">
                        <h3>📝 Giới Thiệu</h3>
                        <p style={{ whiteSpace: 'pre-wrap', color: '#666', lineHeight: 1.6 }}>
                            {caregiverInfo?.bio || 'Chưa có thông tin giới thiệu'}
                        </p>
                    </div>

                    {/* Location Card */}
                    <div className="card">
                        <h3>📍 Địa Chỉ</h3>
                        <div className="detail-item">
                            <label>Địa chỉ:</label>
                            <span>{caregiverInfo?.address || 'Chưa cung cấp'}</span>
                        </div>
                        {caregiverInfo?.latitude && caregiverInfo?.longitude && (
                            <div className="detail-item">
                                <label>Tọa độ:</label>
                                <span>
                                    {caregiverInfo.latitude.toFixed(4)}, {caregiverInfo.longitude.toFixed(4)}
                                </span>
                            </div>
                        )}
                    </div>

                    {/* Personality Type Card */}
                    {caregiverInfo?.personalityType && (
                        <div className="card">
                            <h3>🧠 Kiểu Tính Cách</h3>
                            <div style={{ fontSize: '18px', fontWeight: 'bold', color: '#0066CC', margin: '10px 0' }}>
                                {caregiverInfo.personalityType}
                            </div>
                            <p style={{ fontSize: '12px', color: '#666' }}>
                                Dựa trên đánh giá toàn diện về tính cách
                            </p>
                        </div>
                    )}
                </div>
            </div>

            {/* Skills Section */}
            <div style={{ marginTop: '40px' }}>
                <div className="page-header">
                    <h2 className="page-title">🛠️ Kỹ Năng</h2>
                </div>
                <div className="content-grid">
                    {skills.length > 0 ? (
                        skills.map((skill) => (
                            <div key={skill.id} className="card">
                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '10px' }}>
                                    <h4>{skill.skillName}</h4>
                                    <span className="badge badge-info">⭐ {skill.proficiencyLevel}/5</span>
                                </div>
                                {skill.description && <p style={{ fontSize: '12px', color: '#666' }}>{skill.description}</p>}
                                {skill.certificateUrl && (
                                    <a href={skill.certificateUrl} target="_blank" rel="noopener noreferrer" className="link">
                                        📄 Xem chứng chỉ
                                    </a>
                                )}
                            </div>
                        ))
                    ) : (
                        <div className="empty-state">
                            <div className="empty-state-icon">🛠️</div>
                            <div className="empty-state-title">Chưa có kỹ năng</div>
                            <p>Thêm kỹ năng để giúp khách hàng hiểu rõ hơn về năng lực của bạn</p>
                        </div>
                    )}
                </div>
            </div>

            {/* Availability Section */}
            <div style={{ marginTop: '40px' }}>
                <div className="page-header">
                    <h2 className="page-title">📅 Lịch Làm Việc</h2>
                </div>
                <div className="card">
                    {availability.length > 0 ? (
                        <div style={{ overflowX: 'auto' }}>
                            <table style={{ width: '100%', textAlign: 'left' }}>
                                <thead>
                                    <tr style={{ borderBottom: '1px solid #ddd' }}>
                                        <th style={{ padding: '10px' }}>Thứ</th>
                                        <th style={{ padding: '10px' }}>Giờ bắt đầu</th>
                                        <th style={{ padding: '10px' }}>Giờ kết thúc</th>
                                        <th style={{ padding: '10px' }}>Trạng thái</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {availability.map((slot) => (
                                        <tr key={slot.id} style={{ borderBottom: '1px solid #eee' }}>
                                            <td style={{ padding: '10px' }}>{['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'][slot.dayOfWeek]}</td>
                                            <td style={{ padding: '10px' }}>{slot.startTime}</td>
                                            <td style={{ padding: '10px' }}>{slot.endTime}</td>
                                            <td style={{ padding: '10px' }}>
                                                <span className={`badge ${slot.isAvailable ? 'badge-success' : 'badge-secondary'}`}>
                                                    {slot.isAvailable ? '✅ Có sẵn' : '❌ Không có'}
                                                </span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    ) : (
                        <div className="empty-state">
                            <div className="empty-state-icon">📅</div>
                            <div className="empty-state-title">Chưa cập nhật lịch làm việc</div>
                            <p>Cập nhật lịch làm việc của bạn để khách hàng có thể tìm thấy bạn</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
