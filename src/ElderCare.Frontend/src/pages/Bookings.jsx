import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Pages.css';

export default function Bookings() {
    const { user } = useAuth();
    const role = user?.role || 'Customer';
    const [bookings, setBookings] = useState([]);
    const [filter, setFilter] = useState('');
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);

    useEffect(() => { loadBookings(); }, [filter]);

    const loadBookings = async () => {
        setLoading(true);
        const endpoint = role === 'Caregiver' ? '/bookings/caregiver-bookings' : '/bookings/my-bookings';
        const query = filter ? `?status=${filter}` : '';
        const res = await api.get(endpoint + query);
        if (res.isSuccess) setBookings(res.data || []);
        setLoading(false);
    };

    const handleAction = async (id, action) => {
        const res = await api.post(`/bookings/${id}/${action}`);
        if (res.isSuccess) loadBookings();
    };

    const formatDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '';
    const formatCurrency = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

    const statusMap = { Pending: 'Chờ duyệt', Accepted: 'Đã duyệt', InProgress: 'Đang thực hiện', Completed: 'Hoàn thành', Cancelled: 'Đã hủy', Rejected: 'Từ chối', Disputed: 'Tranh chấp' };
    const badgeMap = { Pending: 'badge-warning', Accepted: 'badge-info', InProgress: 'badge-gold', Completed: 'badge-success', Cancelled: 'badge-danger', Rejected: 'badge-danger', Disputed: 'badge-danger' };

    return (
        <div className="page animate-fade">
            <div className="page-header">
                <div>
                    <h1 className="page-title">📅 {role === 'Caregiver' ? 'Lịch chăm sóc' : 'Đặt lịch chăm sóc'}</h1>
                    <p className="page-subtitle">Quản lý tất cả lịch chăm sóc của bạn</p>
                </div>
                {role === 'Customer' && (
                    <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>+ Đặt lịch mới</button>
                )}
            </div>

            {/* Filters */}
            <div className="filter-bar">
                {['', 'Pending', 'Accepted', 'InProgress', 'Completed', 'Cancelled'].map((s) => (
                    <button key={s} className={`filter-chip ${filter === s ? 'filter-chip-active' : ''}`} onClick={() => setFilter(s)}>
                        {s === '' ? 'Tất cả' : statusMap[s]}
                    </button>
                ))}
            </div>

            {loading ? (
                <div className="loading-overlay"><div className="spinner spinner-lg" /></div>
            ) : bookings.length === 0 ? (
                <div className="empty-state">
                    <div className="empty-state-icon">📅</div>
                    <div className="empty-state-title">Không có lịch chăm sóc</div>
                    <p>Chưa có booking nào {filter && `với trạng thái "${statusMap[filter]}"`}</p>
                </div>
            ) : (
                <div className="content-grid">
                    {bookings.map((b, i) => (
                        <div key={b.id || i} className="card booking-card">
                            <div className="booking-card-header">
                                <span className={`badge ${badgeMap[b.status] || 'badge-neutral'}`}>{statusMap[b.status] || b.status}</span>
                                <span className="booking-card-date">{formatDate(b.scheduledDate || b.createdAt)}</span>
                            </div>
                            <div className="booking-card-body">
                                <div className="booking-card-name">{b.beneficiaryName || b.caregiverName || `Booking #${i + 1}`}</div>
                                {b.totalAmount && <div className="booking-card-amount">{formatCurrency(b.totalAmount)}</div>}
                                {b.notes && <p className="booking-card-notes">{b.notes}</p>}
                            </div>
                            <div className="booking-card-actions">
                                {role === 'Caregiver' && b.status === 'Pending' && (
                                    <>
                                        <button className="btn btn-success btn-sm" onClick={() => handleAction(b.id, 'accept')}>✅ Nhận</button>
                                        <button className="btn btn-danger btn-sm" onClick={() => handleAction(b.id, 'reject')}>✕ Từ chối</button>
                                    </>
                                )}
                                {b.status === 'Pending' && role === 'Customer' && (
                                    <button className="btn btn-danger btn-sm" onClick={() => handleAction(b.id, 'cancel')}>Hủy</button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Create Booking Modal */}
            {showCreateModal && <CreateBookingModal onClose={() => setShowCreateModal(false)} onCreated={loadBookings} />}
        </div>
    );
}

function CreateBookingModal({ onClose, onCreated }) {
    const [form, setForm] = useState({ beneficiaryId: '', caregiverId: '', scheduledDate: '', durationHours: 4, serviceLocation: '', notes: '' });
    const [beneficiaries, setBeneficiaries] = useState([]);
    const [caregivers, setCaregivers] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => { loadBeneficiaries(); }, []);

    const loadBeneficiaries = async () => {
        const res = await api.get('/customers/beneficiaries');
        if (res.isSuccess) setBeneficiaries(res.data || []);
    };

    const loadCaregiversForBeneficiary = async (beneficiaryId) => {
        if (!beneficiaryId) { setCaregivers([]); return; }
        const res = await api.get(`/matching/top-matches/${beneficiaryId}?topN=20`);
        if (res.isSuccess) setCaregivers(res.data || []);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        // Build payload matching backend DTO (ScheduledStartTime/ScheduledEndTime)
        const start = new Date(form.scheduledDate);
        const end = new Date(start.getTime() + (form.durationHours || 0) * 60 * 60 * 1000);
        const beneficiaryName = beneficiaries.find(b => (b.id || b.beneficiaryId) === form.beneficiaryId)?.fullName || beneficiaries.find(b => (b.id || b.beneficiaryId) === form.beneficiaryId)?.name || '';
        const caregiverName = caregivers.find(c => (c.caregiverId || c.id) === form.caregiverId)?.caregiverName || caregivers.find(c => (c.caregiverId || c.id) === form.caregiverId)?.fullName || caregivers.find(c => (c.caregiverId || c.id) === form.caregiverId)?.name || '';

        const payload = {
            beneficiaryId: form.beneficiaryId,
            beneficiaryName: beneficiaryName,
            caregiverId: form.caregiverId,
            caregiverName: caregiverName,
            scheduledStartTime: start.toISOString(),
            scheduledEndTime: end.toISOString(),
            serviceLocation: form.serviceLocation || '',
            latitude: 0,
            longitude: 0,
            specialRequirements: form.notes,
        };

        const res = await api.post('/bookings', payload);
        if (res.isSuccess) { onCreated(); onClose(); }
        else setError(res.message || 'Không thể tạo booking');
        setLoading(false);
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2 className="modal-title">📅 Đặt lịch mới</h2>
                    <button className="btn btn-ghost btn-icon" onClick={onClose}>✕</button>
                </div>
                {error && <div className="auth-error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label className="form-label">Người được chăm sóc</label>
                        <select className="form-input" value={form.beneficiaryId} onChange={(e) => {
                            const val = e.target.value;
                            const selected = beneficiaries.find(b => (b.id || b.beneficiaryId) === val);
                            const location = selected?.address || selected?.Address || selected?.fullName ? (selected?.address || selected?.Address || 'Tại nhà') : 'Tại nhà';
                            setForm({ ...form, beneficiaryId: val, serviceLocation: location });
                            loadCaregiversForBeneficiary(val);
                        }} required>
                            <option value="">-- Chọn người được chăm sóc --</option>
                            {beneficiaries.map(b => (
                                <option key={b.id} value={b.id}>{b.fullName || b.name || b.displayName}</option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label className="form-label">Địa điểm dịch vụ</label>
                        <input className="form-input" value={form.serviceLocation || ''} onChange={(e) => setForm({ ...form, serviceLocation: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Chọn người chăm sóc</label>
                        <select className="form-input" value={form.caregiverId} onChange={(e) => setForm({ ...form, caregiverId: e.target.value })} required>
                            <option value="">-- Chọn người chăm sóc --</option>
                            {caregivers.map(c => (
                                <option key={c.caregiverId || c.id} value={c.caregiverId || c.id}>{c.caregiverName || c.fullName || c.name}</option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label className="form-label">Ngày giờ</label>
                        <input type="datetime-local" className="form-input" value={form.scheduledDate} onChange={(e) => setForm({ ...form, scheduledDate: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Thời lượng (giờ)</label>
                        <input type="number" className="form-input" value={form.durationHours} onChange={(e) => setForm({ ...form, durationHours: parseInt(e.target.value) })} min={1} max={24} />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Ghi chú</label>
                        <textarea className="form-input" rows={3} value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} placeholder="Ghi chú thêm..." />
                    </div>
                    <button type="submit" className="btn btn-primary btn-lg auth-submit" disabled={loading}>
                        {loading ? 'Đang tạo...' : 'Tạo đặt lịch'}
                    </button>
                </form>
            </div>
        </div>
    );
}
