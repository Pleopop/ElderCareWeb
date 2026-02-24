import { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Auth.css';

export default function Register() {
    const { registerCustomer, registerCaregiver } = useAuth();
    const [searchParams] = useSearchParams();
    const initialRole = searchParams.get('role') === 'caregiver' ? 'caregiver' : 'customer';

    const [role, setRole] = useState(initialRole);
    const [form, setForm] = useState({
        email: '', password: '', confirmPassword: '',
        fullName: '', phoneNumber: '', address: '',
        specializations: '',
        yearsOfExperience: '',
        hourlyRate: '',
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const updateField = (field) => (e) => setForm({ ...form, [field]: e.target.value });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (form.password !== form.confirmPassword) {
            setError('Mật khẩu xác nhận không khớp');
            return;
        }

        setLoading(true);
        let result;
        if (role === 'customer') {
            result = await registerCustomer({
                email: form.email,
                password: form.password,
                fullName: form.fullName,
                phoneNumber: form.phoneNumber,
                address: form.address,
            });
        } else {
            result = await registerCaregiver({
                email: form.email,
                password: form.password,
                fullName: form.fullName,
                phoneNumber: form.phoneNumber,
                specializations: form.specializations.split(',').map(s => s.trim()),
                yearsOfExperience: parseInt(form.yearsOfExperience) || 0,
                hourlyRate: parseFloat(form.hourlyRate) || 0,
            });
        }

        if (!result.isSuccess) {
            setError(result.message || 'Đăng ký thất bại');
        }
        setLoading(false);
    };

    return (
        <div className="auth-page">
            <div className="auth-container auth-container-wide animate-scale">
                <div className="auth-header">
                    <Link to="/">
                        <img src="/logo.png" alt="Tuổi Vàng" className="auth-logo" />
                    </Link>
                    <h1 className="auth-title">Tạo tài khoản</h1>
                    <p className="auth-subtitle">Tham gia cộng đồng Tuổi Vàng</p>
                </div>

                {/* Role Tabs */}
                <div className="auth-tabs">
                    <button
                        className={`auth-tab ${role === 'customer' ? 'auth-tab-active' : ''}`}
                        onClick={() => setRole('customer')}
                        type="button"
                    >
                        👨‍👩‍👧 Khách hàng
                    </button>
                    <button
                        className={`auth-tab ${role === 'caregiver' ? 'auth-tab-active' : ''}`}
                        onClick={() => setRole('caregiver')}
                        type="button"
                    >
                        🤝 Người chăm sóc
                    </button>
                </div>

                {error && <div className="auth-error">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="auth-form-grid">
                        <div className="form-group">
                            <label className="form-label">Họ và tên</label>
                            <input type="text" className="form-input" placeholder="Nguyễn Văn A" value={form.fullName} onChange={updateField('fullName')} required />
                        </div>
                        <div className="form-group">
                            <label className="form-label">Số điện thoại</label>
                            <input type="tel" className="form-input" placeholder="0901234567" value={form.phoneNumber} onChange={updateField('phoneNumber')} required />
                        </div>
                    </div>

                    <div className="form-group">
                        <label className="form-label">Email</label>
                        <input type="email" className="form-input" placeholder="your@email.com" value={form.email} onChange={updateField('email')} required />
                    </div>

                    <div className="auth-form-grid">
                        <div className="form-group">
                            <label className="form-label">Mật khẩu</label>
                            <input type="password" className="form-input" placeholder="••••••••" value={form.password} onChange={updateField('password')} required minLength={6} />
                        </div>
                        <div className="form-group">
                            <label className="form-label">Xác nhận mật khẩu</label>
                            <input type="password" className="form-input" placeholder="••••••••" value={form.confirmPassword} onChange={updateField('confirmPassword')} required />
                        </div>
                    </div>

                    {role === 'customer' && (
                        <div className="form-group">
                            <label className="form-label">Địa chỉ</label>
                            <input type="text" className="form-input" placeholder="Số nhà, đường, quận..." value={form.address} onChange={updateField('address')} />
                        </div>
                    )}

                    {role === 'caregiver' && (
                        <>
                            <div className="form-group">
                                <label className="form-label">Chuyên môn (cách nhau bằng dấu phẩy)</label>
                                <input type="text" className="form-input" placeholder="Chăm sóc người cao tuổi, Vật lý trị liệu..." value={form.specializations} onChange={updateField('specializations')} required />
                            </div>
                            <div className="auth-form-grid">
                                <div className="form-group">
                                    <label className="form-label">Số năm kinh nghiệm</label>
                                    <input type="number" className="form-input" placeholder="3" value={form.yearsOfExperience} onChange={updateField('yearsOfExperience')} required min={0} />
                                </div>
                                <div className="form-group">
                                    <label className="form-label">Giá theo giờ (VND)</label>
                                    <input type="number" className="form-input" placeholder="150000" value={form.hourlyRate} onChange={updateField('hourlyRate')} required min={0} />
                                </div>
                            </div>
                        </>
                    )}

                    <button type="submit" className="btn btn-primary btn-lg auth-submit" disabled={loading}>
                        {loading ? <><div className="spinner" /> Đang xử lý...</> : 'Đăng ký tài khoản'}
                    </button>
                </form>

                <p className="auth-footer-text">
                    Đã có tài khoản? <Link to="/login">Đăng nhập</Link>
                </p>
            </div>
        </div>
    );
}
