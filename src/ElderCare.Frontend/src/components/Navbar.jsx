import { useAuth } from '../context/AuthContext';

export default function Navbar({ onToggleSidebar }) {
    const { user } = useAuth();

    return (
        <header className="navbar">
            <button className="btn btn-ghost navbar-toggle" onClick={onToggleSidebar}>
                ☰
            </button>
            <div className="navbar-title">
                <img src="/logo.png" alt="Tuổi Vàng" className="navbar-logo-mobile" />
            </div>
            <div className="navbar-actions">
                <span className="navbar-greeting hide-mobile">
                    Xin chào, <strong>{user?.fullName || user?.email || 'Bạn'}</strong>
                </span>
            </div>
        </header>
    );
}
