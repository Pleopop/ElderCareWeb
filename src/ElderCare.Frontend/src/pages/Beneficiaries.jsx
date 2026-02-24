import { useState, useEffect } from 'react';
import api from '../api/client';
import './Pages.css';

const genderLabels = ['Nam', 'Nữ', 'Khác'];
const mobilityLabels = ['Vận động tốt', 'Hạn chế nhẹ', 'Hạn chế vừa', 'Hạn chế nặng', 'Nằm liệt giường'];
const cognitiveLabels = ['Bình thường', 'Suy giảm nhẹ', 'Suy giảm vừa', 'Suy giảm nặng'];

const emptyForm = {
    fullName: '', dateOfBirth: '', gender: 0, address: '',
    medicalConditions: '', medications: '', allergies: '',
    mobilityLevel: 0, cognitiveStatus: 0,
    specialNeeds: '', personalityTraits: '', hobbies: ''
};

export default function Beneficiaries() {
    const [beneficiaries, setBeneficiaries] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingId, setEditingId] = useState(null);
    const [form, setForm] = useState({ ...emptyForm });
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const [expandedId, setExpandedId] = useState(null);

    useEffect(() => { loadBeneficiaries(); }, []);

    const loadBeneficiaries = async () => {
        setLoading(true);
        const res = await api.get('/customers/beneficiaries');
        if (res.isSuccess) setBeneficiaries(res.data || []);
        setLoading(false);
    };

    const openCreate = () => {
        setEditingId(null);
        setForm({ ...emptyForm });
        setError('');
        setShowModal(true);
    };

    const openEdit = (b) => {
        setEditingId(b.id);
        setForm({
            fullName: b.fullName,
            dateOfBirth: b.dateOfBirth?.split('T')[0] || '',
            gender: b.gender,
            address: b.address || '',
            medicalConditions: b.medicalConditions || '',
            medications: b.medications || '',
            allergies: b.allergies || '',
            mobilityLevel: b.mobilityLevel ?? 0,
            cognitiveStatus: b.cognitiveStatus ?? 0,
            specialNeeds: b.specialNeeds || '',
            personalityTraits: b.personalityTraits || '',
            hobbies: b.hobbies || ''
        });
        setError('');
        setShowModal(true);
    };

    const handleSave = async (e) => {
        e.preventDefault();
        if (!form.fullName || !form.dateOfBirth) {
            setError('Vui lòng nhập họ tên và ngày sinh');
            return;
        }
        setSaving(true);
        setError('');

        const payload = {
            ...form,
            gender: parseInt(form.gender),
            mobilityLevel: parseInt(form.mobilityLevel),
            cognitiveStatus: parseInt(form.cognitiveStatus)
        };

        let res;
        if (editingId) {
            res = await api.put(`/customers/beneficiaries/${editingId}`, payload);
        } else {
            res = await api.post('/customers/beneficiaries', payload);
        }

        if (res.isSuccess) {
            setShowModal(false);
            loadBeneficiaries();
        } else {
            setError(res.error || 'Đã xảy ra lỗi');
        }
        setSaving(false);
    };

    const handleDelete = async (id) => {
        if (!confirm('Bạn có chắc muốn xóa người thụ hưởng này?')) return;
        const res = await api.del(`/customers/beneficiaries/${id}`);
        if (res.isSuccess) loadBeneficiaries();
    };

    const setField = (key, val) => setForm(prev => ({ ...prev, [key]: val }));

    if (loading) return <div className="loading-spinner" />;

    return (
        <div>
            <div className="page-header">
                <div>
                    <h1 className="page-title">👨‍👩‍👧 Người Thụ Hưởng</h1>
                    <p className="page-subtitle">Quản lý thông tin người được chăm sóc</p>
                </div>
                <button className="btn btn-primary" onClick={openCreate}>+ Thêm mới</button>
            </div>

            {beneficiaries.length === 0 ? (
                <div className="card" style={{ textAlign: 'center', padding: 'var(--space-2xl)' }}>
                    <p style={{ fontSize: '3rem', marginBottom: 'var(--space-md)' }}>👴👵</p>
                    <h3>Chưa có người thụ hưởng</h3>
                    <p className="page-subtitle">Thêm thông tin người thân cần được chăm sóc để bắt đầu</p>
                    <button className="btn btn-primary" onClick={openCreate} style={{ marginTop: 'var(--space-lg)' }}>
                        + Thêm người thụ hưởng
                    </button>
                </div>
            ) : (
                <div className="content-grid">
                    {beneficiaries.map(b => (
                        <div key={b.id} className="card beneficiary-card">
                            <div className="beneficiary-card-header">
                                <div className="avatar avatar-lg">{b.fullName[0]}</div>
                                <div style={{ flex: 1 }}>
                                    <h3 className="beneficiary-name">{b.fullName}</h3>
                                    <span className="beneficiary-age">{b.age} tuổi • {genderLabels[b.gender]}</span>
                                </div>
                                <div className="beneficiary-card-actions">
                                    <button className="btn btn-outline btn-sm" onClick={() => openEdit(b)}>✏️</button>
                                    <button className="btn btn-danger btn-sm" onClick={() => handleDelete(b.id)}>🗑️</button>
                                </div>
                            </div>

                            <div className="beneficiary-tags">
                                {b.mobilityLevel != null && (
                                    <span className="context-tag">🦽 {mobilityLabels[b.mobilityLevel]}</span>
                                )}
                                {b.cognitiveStatus != null && (
                                    <span className="context-tag">🧠 {cognitiveLabels[b.cognitiveStatus]}</span>
                                )}
                            </div>

                            <button
                                className="btn btn-outline btn-sm"
                                style={{ width: '100%' }}
                                onClick={() => setExpandedId(expandedId === b.id ? null : b.id)}
                            >
                                {expandedId === b.id ? '▲ Thu gọn' : '▼ Xem chi tiết'}
                            </button>

                            {expandedId === b.id && (
                                <div className="beneficiary-details">
                                    <div className="detail-row"><span>📍 Địa chỉ:</span><span>{b.address || '—'}</span></div>
                                    <div className="detail-row"><span>🏥 Bệnh lý:</span><span>{b.medicalConditions || '—'}</span></div>
                                    <div className="detail-row"><span>💊 Thuốc:</span><span>{b.medications || '—'}</span></div>
                                    <div className="detail-row"><span>⚠️ Dị ứng:</span><span>{b.allergies || '—'}</span></div>
                                    <div className="detail-row"><span>📝 Nhu cầu:</span><span>{b.specialNeeds || '—'}</span></div>
                                    <div className="detail-row"><span>🎭 Tính cách:</span><span>{b.personalityTraits || '—'}</span></div>
                                    <div className="detail-row"><span>🎨 Sở thích:</span><span>{b.hobbies || '—'}</span></div>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            )}

            {/* CREATE/EDIT MODAL */}
            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal beneficiary-modal" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2>{editingId ? '✏️ Cập nhật' : '➕ Thêm'} Người Thụ Hưởng</h2>
                            <button className="modal-close" onClick={() => setShowModal(false)}>✕</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <div className="modal-body">
                                {error && <div className="alert alert-danger">{error}</div>}

                                <div className="form-section">
                                    <h4>Thông tin cơ bản</h4>
                                    <div className="form-grid">
                                        <div className="form-group">
                                            <label className="form-label">Họ và tên *</label>
                                            <input className="form-input" value={form.fullName}
                                                onChange={e => setField('fullName', e.target.value)} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Ngày sinh *</label>
                                            <input type="date" className="form-input" value={form.dateOfBirth}
                                                onChange={e => setField('dateOfBirth', e.target.value)} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Giới tính</label>
                                            <select className="form-input" value={form.gender}
                                                onChange={e => setField('gender', e.target.value)}>
                                                {genderLabels.map((l, i) => <option key={i} value={i}>{l}</option>)}
                                            </select>
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Địa chỉ</label>
                                            <input className="form-input" value={form.address}
                                                onChange={e => setField('address', e.target.value)} />
                                        </div>
                                    </div>
                                </div>

                                <div className="form-section">
                                    <h4>Tình trạng sức khỏe</h4>
                                    <div className="form-grid">
                                        <div className="form-group full-width">
                                            <label className="form-label">Bệnh lý nền</label>
                                            <textarea className="form-input" rows={2} value={form.medicalConditions}
                                                onChange={e => setField('medicalConditions', e.target.value)}
                                                placeholder="VD: Tiểu đường, huyết áp cao..." />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Thuốc đang dùng</label>
                                            <input className="form-input" value={form.medications}
                                                onChange={e => setField('medications', e.target.value)} />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Dị ứng</label>
                                            <input className="form-input" value={form.allergies}
                                                onChange={e => setField('allergies', e.target.value)} />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Khả năng vận động</label>
                                            <select className="form-input" value={form.mobilityLevel}
                                                onChange={e => setField('mobilityLevel', e.target.value)}>
                                                {mobilityLabels.map((l, i) => <option key={i} value={i}>{l}</option>)}
                                            </select>
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Tình trạng nhận thức</label>
                                            <select className="form-input" value={form.cognitiveStatus}
                                                onChange={e => setField('cognitiveStatus', e.target.value)}>
                                                {cognitiveLabels.map((l, i) => <option key={i} value={i}>{l}</option>)}
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div className="form-section">
                                    <h4>Thông tin khác</h4>
                                    <div className="form-grid">
                                        <div className="form-group full-width">
                                            <label className="form-label">Nhu cầu đặc biệt</label>
                                            <textarea className="form-input" rows={2} value={form.specialNeeds}
                                                onChange={e => setField('specialNeeds', e.target.value)} />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Tính cách</label>
                                            <input className="form-input" value={form.personalityTraits}
                                                onChange={e => setField('personalityTraits', e.target.value)}
                                                placeholder="VD: Hiền lành, thích nói chuyện..." />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Sở thích</label>
                                            <input className="form-input" value={form.hobbies}
                                                onChange={e => setField('hobbies', e.target.value)}
                                                placeholder="VD: Đọc sách, nghe nhạc..." />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="modal-footer">
                                <button type="button" className="btn btn-outline" onClick={() => setShowModal(false)}>Hủy</button>
                                <button type="submit" className="btn btn-primary" disabled={saving}>
                                    {saving ? 'Đang lưu...' : (editingId ? 'Cập nhật' : 'Tạo mới')}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
