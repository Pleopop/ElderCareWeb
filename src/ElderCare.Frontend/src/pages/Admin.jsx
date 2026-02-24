import { useState, useEffect } from 'react';
import api from '../api/client';
import './Pages.css';

export default function Admin() {
    const [pendingCaregivers, setPendingCaregivers] = useState([]);
    const [fraudAlerts, setFraudAlerts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('approvals');

    useEffect(() => { loadData(); }, []);

    const loadData = async () => {
        setLoading(true);
        const [cgRes, fraudRes] = await Promise.all([
            api.get('/admin/caregivers/pending'),
            api.get('/frauddetection/alerts'),
        ]);
        if (cgRes.isSuccess) setPendingCaregivers(cgRes.data || []);
        if (fraudRes.isSuccess) setFraudAlerts(fraudRes.data || []);
        setLoading(false);
    };

    const handleApprove = async (id) => {
        const res = await api.post(`/admin/caregivers/${id}/approve`);
        if (res.isSuccess) setPendingCaregivers(pendingCaregivers.filter(c => c.id !== id));
    };

    const handleReject = async (id) => {
        const res = await api.post('/admin/caregivers/reject', { caregiverId: id, reason: 'Không đạt yêu cầu' });
        if (res.isSuccess) setPendingCaregivers(pendingCaregivers.filter(c => c.id !== id));
    };

    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải quản trị...</p></div>;

    return (
        <div className="page animate-fade">
            <div className="page-header">
                <h1 className="page-title">⚙️ Bảng quản trị</h1>
                <p className="page-subtitle">Quản lý hệ thống Tuổi Vàng</p>
            </div>

            {/* Stats */}
            <div className="admin-stats">
                <div className="stat-card card card-gold">
                    <div className="stat-icon">👥</div>
                    <div className="stat-info">
                        <div className="stat-label">Chờ duyệt</div>
                        <div className="stat-value">{pendingCaregivers.length}</div>
                    </div>
                </div>
                <div className="stat-card card">
                    <div className="stat-icon">🛡️</div>
                    <div className="stat-info">
                        <div className="stat-label">Cảnh báo gian lận</div>
                        <div className="stat-value">{fraudAlerts.length}</div>
                    </div>
                </div>
            </div>

            {/* Tabs */}
            <div className="admin-tabs">
                <button className={`filter-chip ${activeTab === 'approvals' ? 'filter-chip-active' : ''}`} onClick={() => setActiveTab('approvals')}>
                    👥 Duyệt người chăm sóc ({pendingCaregivers.length})
                </button>
                <button className={`filter-chip ${activeTab === 'fraud' ? 'filter-chip-active' : ''}`} onClick={() => setActiveTab('fraud')}>
                    🛡️ Cảnh báo gian lận ({fraudAlerts.length})
                </button>
            </div>

            {/* Pending Caregivers */}
            {activeTab === 'approvals' && (
                pendingCaregivers.length === 0 ? (
                    <div className="empty-state"><div className="empty-state-icon">✅</div><div className="empty-state-title">Không có yêu cầu chờ duyệt</div></div>
                ) : (
                    <div className="content-grid">
                        {pendingCaregivers.map((c) => (
                            <div key={c.id} className="card">
                                <div className="caregiver-card-header">
                                    <div className="avatar">{c.fullName?.[0] || '?'}</div>
                                    <div>
                                        <div className="caregiver-name">{c.fullName}</div>
                                        <div className="caregiver-email">{c.email}</div>
                                    </div>
                                </div>
                                {c.specializations && (
                                    <div className="caregiver-specs">
                                        {(Array.isArray(c.specializations) ? c.specializations : [c.specializations]).map((s, i) => (
                                            <span key={i} className="badge badge-gold">{s}</span>
                                        ))}
                                    </div>
                                )}
                                <div className="caregiver-meta">
                                    {c.yearsOfExperience && <span>📆 {c.yearsOfExperience} năm KN</span>}
                                    {c.hourlyRate && <span>💰 {new Intl.NumberFormat('vi-VN').format(c.hourlyRate)} VND/h</span>}
                                </div>
                                <div className="booking-card-actions">
                                    <button className="btn btn-success btn-sm" onClick={() => handleApprove(c.id)}>✅ Duyệt</button>
                                    <button className="btn btn-danger btn-sm" onClick={() => handleReject(c.id)}>✕ Từ chối</button>
                                </div>
                            </div>
                        ))}
                    </div>
                )
            )}

            {/* Fraud Alerts */}
            {activeTab === 'fraud' && (
                fraudAlerts.length === 0 ? (
                    <div className="empty-state"><div className="empty-state-icon">🛡️</div><div className="empty-state-title">Không có cảnh báo gian lận</div></div>
                ) : (
                    <div className="content-grid">
                        {fraudAlerts.map((a, i) => (
                            <div key={i} className="card card-gold">
                                <div className="fraud-alert-header">
                                    <span className={`badge ${a.severity === 'High' ? 'badge-danger' : a.severity === 'Medium' ? 'badge-warning' : 'badge-info'}`}>
                                        {a.severity || 'Medium'}
                                    </span>
                                    <span className="fraud-alert-type">{a.alertType || a.type}</span>
                                </div>
                                <p className="fraud-alert-desc">{a.description || a.details}</p>
                                <div className="fraud-alert-meta">
                                    <span>👤 User: {a.userId?.substring(0, 8)}...</span>
                                    <span>📅 {new Date(a.createdAt || a.detectedAt).toLocaleDateString('vi-VN')}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                )
            )}
        </div>
    );
}
