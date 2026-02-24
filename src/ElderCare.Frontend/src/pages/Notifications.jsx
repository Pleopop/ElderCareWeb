import { useState, useEffect } from 'react';
import api from '../api/client';
import './Pages.css';

export default function Notifications() {
    const [notifications, setNotifications] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => { loadNotifications(); }, []);

    const loadNotifications = async () => {
        setLoading(true);
        const res = await api.get('/notifications?pageSize=50');
        if (res.isSuccess) setNotifications(res.data?.items || res.data || []);
        setLoading(false);
    };

    const markAsRead = async (id) => {
        await api.post(`/notifications/${id}/read`);
        setNotifications(notifications.map(n => n.id === id ? { ...n, isRead: true } : n));
    };

    const markAllRead = async () => {
        await api.post('/notifications/read-all');
        setNotifications(notifications.map(n => ({ ...n, isRead: true })));
    };

    const formatDate = (d) => {
        if (!d) return '';
        const date = new Date(d);
        const now = new Date();
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);
        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;
        const diffHours = Math.floor(diffMins / 60);
        if (diffHours < 24) return `${diffHours} giờ trước`;
        return date.toLocaleDateString('vi-VN');
    };

    const typeIcons = { Info: 'ℹ️', Success: '✅', Warning: '⚠️', Error: '❌' };
    const categoryIcons = { Booking: '📅', Payment: '💰', Review: '⭐', Dispute: '⚖️', System: '🔧', Account: '👤' };

    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải thông báo...</p></div>;

    return (
        <div className="page animate-fade">
            <div className="page-header">
                <div>
                    <h1 className="page-title">🔔 Thông báo</h1>
                    <p className="page-subtitle">{notifications.filter(n => !n.isRead).length} thông báo chưa đọc</p>
                </div>
                {notifications.some(n => !n.isRead) && (
                    <button className="btn btn-ghost" onClick={markAllRead}>✓ Đánh dấu tất cả đã đọc</button>
                )}
            </div>

            {notifications.length === 0 ? (
                <div className="empty-state"><div className="empty-state-icon">🔔</div><div className="empty-state-title">Không có thông báo</div></div>
            ) : (
                <div className="notification-list">
                    {notifications.map((n) => (
                        <div
                            key={n.id}
                            className={`notification-item card ${!n.isRead ? 'notification-unread' : ''}`}
                            onClick={() => !n.isRead && markAsRead(n.id)}
                        >
                            <div className="notification-icon">
                                {categoryIcons[n.category] || typeIcons[n.type] || '🔔'}
                            </div>
                            <div className="notification-content">
                                <div className="notification-title">{n.title}</div>
                                <div className="notification-message">{n.message}</div>
                                <div className="notification-time">{formatDate(n.createdAt)}</div>
                            </div>
                            {!n.isRead && <div className="notification-dot" />}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
