import { useState, useEffect } from 'react';
import api from '../../api/client';
import '../Pages.css';

export default function CustomerProfile({ user, onRefresh }) {
    const [beneficiaries, setBeneficiaries] = useState([]);
    const [loading, setLoading] = useState(false);
    const [showAddBeneficiary, setShowAddBeneficiary] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        loadBeneficiaries();
    }, []);

    const loadBeneficiaries = async () => {
        setLoading(true);
        const res = await api.get('/customers/beneficiaries');
        if (res.isSuccess) setBeneficiaries(res.data || []);
        setLoading(false);
    };

    const statusLabel = { 0: 'Đang chờ', 1: 'Hoạt động', 2: 'Bị khóa', 3: 'Đã xóa' };

    return (
        <div>
            <div className="page-header">
                <div>
                    <h1 className="page-title">👤 Hồ Sơ Khách Hàng</h1>
                    <p className="page-subtitle">Quản lý thông tin cá nhân và người được chăm sóc</p>
                </div>
            </div>

            <div className="profile-layout">
                {/* User Basic Info */}
                <div className="profile-card card">
                    <div className="profile-avatar-section">
                        <div className="profile-avatar-large">
                            {(user?.email || '?')[0].toUpperCase()}
                        </div>
                        <h2 className="profile-name">{user?.fullName || user?.email}</h2>
                        <span className="badge badge-info">Khách hàng</span>
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
                            <label>📋 Trạng thái</label>
                            <span className={`badge ${user?.status === 1 ? 'badge-success' : 'badge-warning'}`}>
                                {statusLabel[user?.status] || 'Không xác định'}
                            </span>
                        </div>
                    </div>
                </div>

                {/* Security Info */}
                <div className="profile-sidebar">
                    <div className="card">
                        <h3>🔒 Bảo mật</h3>
                        <div className="security-items">
                            <div className="security-item">
                                <span>Email xác thực</span>
                                <span className={user?.isEmailVerified ? 'text-success' : 'text-danger'}>
                                    {user?.isEmailVerified ? '✅ Đã xác thực' : '❌ Chưa xác thực'}
                                </span>
                            </div>
                            <div className="security-item">
                                <span>SĐT xác thực</span>
                                <span className={user?.isPhoneVerified ? 'text-success' : 'text-danger'}>
                                    {user?.isPhoneVerified ? '✅ Đã xác thực' : '❌ Chưa xác thực'}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Beneficiaries Section */}
            <div className="page-header" style={{ marginTop: '40px' }}>
                <div>
                    <h2 className="page-title">👥 Người Được Chăm Sóc</h2>
                    <p className="page-subtitle">Quản lý danh sách người được chăm sóc</p>
                </div>
                <button className="btn btn-primary" onClick={() => setShowAddBeneficiary(true)}>
                    + Thêm người được chăm sóc
                </button>
            </div>

            {error && <div className="auth-error">{error}</div>}

            {loading ? (
                <div className="loading-overlay"><div className="spinner spinner-lg" /></div>
            ) : beneficiaries.length === 0 ? (
                <div className="empty-state">
                    <div className="empty-state-icon">👥</div>
                    <div className="empty-state-title">Chưa có người được chăm sóc</div>
                    <p>Thêm một người được chăm sóc để bắt đầu tìm kiếm nhân viên chăm sóc phù hợp</p>
                </div>
            ) : (
                <div className="content-grid">
                    {beneficiaries.map((b) => (
                        <div key={b.id} className="card beneficiary-card">
                            <div className="beneficiary-header">
                                <h3>{b.fullName}</h3>
                                <span className="badge badge-info">{b.age} tuổi</span>
                            </div>
                            <div className="beneficiary-details">
                                {b.address && (
                                    <div className="detail-item">
                                        <label>📍 Địa chỉ:</label>
                                        <span>{b.address}</span>
                                    </div>
                                )}
                                {b.medicalConditions && (
                                    <div className="detail-item">
                                        <label>🏥 Tình trạng sức khỏe:</label>
                                        <span>{b.medicalConditions}</span>
                                    </div>
                                )}
                                {b.specialNeeds && (
                                    <div className="detail-item">
                                        <label>⚠️ Nhu cầu đặc biệt:</label>
                                        <span>{b.specialNeeds}</span>
                                    </div>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {showAddBeneficiary && (
                <AddBeneficiaryModal 
                    onClose={() => setShowAddBeneficiary(false)} 
                    onAdded={() => { loadBeneficiaries(); setShowAddBeneficiary(false); }}
                />
            )}
        </div>
    );
}

function AddBeneficiaryModal({ onClose, onAdded }) {
    const [form, setForm] = useState({
        fullName: '',
        dateOfBirth: '',
        address: '',
        medicalConditions: '',
        specialNeeds: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        const res = await api.post('/customers/beneficiaries', form);
        if (res.isSuccess) {
            onAdded();
        } else {
            setError(res.message || 'Không thể thêm người được chăm sóc');
        }
        setLoading(false);
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2 className="modal-title">➕ Thêm người được chăm sóc</h2>
                    <button className="btn btn-ghost btn-icon" onClick={onClose}>✕</button>
                </div>
                {error && <div className="auth-error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label className="form-label">Họ và tên</label>
                        <input
                            className="form-input"
                            value={form.fullName}
                            onChange={(e) => setForm({ ...form, fullName: e.target.value })}
                            required
                            placeholder="Nhập họ và tên"
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Ngày sinh</label>
                        <input
                            type="date"
                            className="form-input"
                            value={form.dateOfBirth}
                            onChange={(e) => setForm({ ...form, dateOfBirth: e.target.value })}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Địa chỉ</label>
                        <input
                            className="form-input"
                            value={form.address}
                            onChange={(e) => setForm({ ...form, address: e.target.value })}
                            placeholder="Nhập địa chỉ"
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Tình trạng sức khỏe</label>
                        <textarea
                            className="form-input"
                            rows={3}
                            value={form.medicalConditions}
                            onChange={(e) => setForm({ ...form, medicalConditions: e.target.value })}
                            placeholder="Mô tả tình trạng sức khỏe (ví dụ: Tiểu đường, Cao huyết áp)"
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Nhu cầu đặc biệt</label>
                        <textarea
                            className="form-input"
                            rows={2}
                            value={form.specialNeeds}
                            onChange={(e) => setForm({ ...form, specialNeeds: e.target.value })}
                            placeholder="Mô tả nhu cầu chăm sóc đặc biệt"
                        />
                    </div>
                    <button type="submit" className="btn btn-primary btn-lg auth-submit" disabled={loading}>
                        {loading ? 'Đang thêm...' : 'Thêm người được chăm sóc'}
                    </button>
                </form>
            </div>
        </div>
    );
}
