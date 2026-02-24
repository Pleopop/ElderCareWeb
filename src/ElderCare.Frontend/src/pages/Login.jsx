import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Auth.css';

export default function Login() {
    const { login } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);
        const result = await login(email, password);
        if (!result.isSuccess) {
            setError(result.message || 'Đăng nhập thất bại');
        }
        setLoading(false);
    };

    return (
        <div className="auth-page">
            <div className="auth-container animate-scale">
                <div className="auth-header">
                    <Link to="/">
                        <img src="/logo.png" alt="Tuổi Vàng" className="auth-logo" />
                    </Link>
                    <h1 className="auth-title">Đăng nhập</h1>
                    <p className="auth-subtitle">Chào mừng trở lại Tuổi Vàng</p>
                </div>

                {error && <div className="auth-error">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label className="form-label">Email</label>
                        <input
                            type="email"
                            className="form-input"
                            placeholder="your@email.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Mật khẩu</label>
                        <input
                            type="password"
                            className="form-input"
                            placeholder="••••••••"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary btn-lg auth-submit" disabled={loading}>
                        {loading ? <><div className="spinner" /> Đang xử lý...</> : 'Đăng nhập'}
                    </button>
                </form>

                <p className="auth-footer-text">
                    Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
                </p>
            </div>
        </div>
    );
}
