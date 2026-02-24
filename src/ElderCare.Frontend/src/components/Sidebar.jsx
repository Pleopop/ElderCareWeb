import { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const menuItems = {
    Customer: [
        { path: '/dashboard', label: 'Tổng quan', icon: '🏠' },
        { path: '/ai-advisor', label: 'Trợ lý AI', icon: '🤖' },
        { path: '/beneficiaries', label: 'Người thụ hưởng', icon: '👨‍👩‍👧' },
        { path: '/bookings', label: 'Đặt lịch', icon: '📅' },
        { path: '/wallet', label: 'Ví tiền', icon: '💰' },
        { path: '/chat', label: 'Tin nhắn', icon: '💬' },
        { path: '/notifications', label: 'Thông báo', icon: '🔔' },
        { path: '/profile', label: 'Hồ sơ', icon: '👤' },
    ],
    Caregiver: [
        { path: '/dashboard', label: 'Tổng quan', icon: '🏠' },
        { path: '/bookings', label: 'Lịch chăm sóc', icon: '📅' },
        { path: '/wallet', label: 'Thu nhập', icon: '💰' },
        { path: '/chat', label: 'Tin nhắn', icon: '💬' },
        { path: '/notifications', label: 'Thông báo', icon: '🔔' },
    ],
    Admin: [
        { path: '/dashboard', label: 'Tổng quan', icon: '🏠' },
        { path: '/admin', label: 'Quản trị', icon: '⚙️' },
        { path: '/bookings', label: 'Đặt lịch', icon: '📅' },
        { path: '/notifications', label: 'Thông báo', icon: '🔔' },
    ],
};

export default function Sidebar({ isOpen, onClose }) {
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const role = user?.role || 'Customer';
    const items = menuItems[role] || menuItems.Customer;

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <>
            {isOpen && <div className="sidebar-overlay" onClick={onClose} />}
            <aside className={`sidebar ${isOpen ? 'sidebar-open' : ''}`}>
                <div className="sidebar-header">
                    <img src="/logo.png" alt="Tuổi Vàng" className="sidebar-logo" />
                    <span className="sidebar-brand">Tuổi Vàng</span>
                </div>

                <nav className="sidebar-nav">
                    {items.map((item) => (
                        <NavLink
                            key={item.path}
                            to={item.path}
                            className={({ isActive }) =>
                                `sidebar-link ${isActive ? 'sidebar-link-active' : ''}`
                            }
                            onClick={onClose}
                        >
                            <span className="sidebar-icon">{item.icon}</span>
                            <span>{item.label}</span>
                        </NavLink>
                    ))}
                </nav>

                <div className="sidebar-footer">
                    <div className="sidebar-user">
                        <div className="avatar avatar-sm">
                            {user?.fullName?.[0] || user?.email?.[0]?.toUpperCase() || 'U'}
                        </div>
                        <div className="sidebar-user-info">
                            <div className="sidebar-user-name">{user?.fullName || user?.email}</div>
                            <div className="sidebar-user-role">{role === 'Customer' ? 'Khách hàng' : role === 'Caregiver' ? 'Người chăm sóc' : 'Quản trị viên'}</div>
                        </div>
                    </div>
                    <button className="btn btn-ghost sidebar-logout" onClick={handleLogout}>
                        🚪 Đăng xuất
                    </button>
                </div>
            </aside>
        </>
    );
}
