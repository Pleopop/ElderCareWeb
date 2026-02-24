import { useState, useEffect, useRef } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Pages.css';

export default function Chat() {
    const { user } = useAuth();
    const [conversations, setConversations] = useState([]);
    const [activeConvo, setActiveConvo] = useState(null);
    const [messages, setMessages] = useState([]);
    const [newMessage, setNewMessage] = useState('');
    const [loading, setLoading] = useState(true);
    const messagesEndRef = useRef(null);

    useEffect(() => { loadConversations(); }, []);
    useEffect(() => { messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' }); }, [messages]);

    const loadConversations = async () => {
        setLoading(true);
        const res = await api.get('/chat/conversations');
        if (res.isSuccess) setConversations(res.data || []);
        setLoading(false);
    };

    const openConversation = async (convo) => {
        setActiveConvo(convo);
        const res = await api.get(`/chat/conversations/${convo.id}/messages?pageSize=50`);
        if (res.isSuccess) setMessages(res.data?.items || res.data || []);
    };

    const sendMessage = async (e) => {
        e.preventDefault();
        if (!newMessage.trim() || !activeConvo) return;
        const res = await api.post(`/chat/conversations/${activeConvo.id}/messages`, { content: newMessage });
        if (res.isSuccess) {
            setMessages([...messages, res.data || { content: newMessage, senderId: user?.id, sentAt: new Date().toISOString() }]);
            setNewMessage('');
        }
    };

    const formatTime = (d) => d ? new Date(d).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) : '';

    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải tin nhắn...</p></div>;

    return (
        <div className="page animate-fade">
            <div className="page-header">
                <h1 className="page-title">💬 Tin nhắn</h1>
            </div>

            <div className="chat-container card">
                {/* Conversation List */}
                <div className="chat-sidebar">
                    <div className="chat-sidebar-header">
                        <h3>Hội thoại</h3>
                    </div>
                    {conversations.length === 0 ? (
                        <div className="chat-no-convos">
                            <p>Chưa có hội thoại nào</p>
                        </div>
                    ) : (
                        <div className="chat-convo-list">
                            {conversations.map((c) => (
                                <div
                                    key={c.id}
                                    className={`chat-convo-item ${activeConvo?.id === c.id ? 'chat-convo-active' : ''}`}
                                    onClick={() => openConversation(c)}
                                >
                                    <div className="avatar avatar-sm">{c.participantName?.[0] || '?'}</div>
                                    <div className="chat-convo-info">
                                        <div className="chat-convo-name">{c.participantName || 'Hội thoại'}</div>
                                        <div className="chat-convo-preview">{c.lastMessage || 'Bắt đầu trò chuyện...'}</div>
                                    </div>
                                    {c.unreadCount > 0 && <div className="chat-unread-badge">{c.unreadCount}</div>}
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {/* Message Area */}
                <div className="chat-main">
                    {!activeConvo ? (
                        <div className="chat-empty">
                            <div className="empty-state-icon">💬</div>
                            <div className="empty-state-title">Chọn một hội thoại</div>
                            <p>Chọn hội thoại bên trái để bắt đầu nhắn tin</p>
                        </div>
                    ) : (
                        <>
                            <div className="chat-header">
                                <div className="avatar avatar-sm">{activeConvo.participantName?.[0] || '?'}</div>
                                <h3>{activeConvo.participantName || 'Hội thoại'}</h3>
                            </div>
                            <div className="chat-messages">
                                {messages.map((m, i) => (
                                    <div key={i} className={`chat-message ${m.senderId === user?.id ? 'chat-message-mine' : 'chat-message-other'}`}>
                                        <div className="chat-bubble">
                                            <p>{m.content}</p>
                                            <span className="chat-time">{formatTime(m.sentAt)}</span>
                                        </div>
                                    </div>
                                ))}
                                <div ref={messagesEndRef} />
                            </div>
                            <form className="chat-input-area" onSubmit={sendMessage}>
                                <input
                                    className="form-input chat-input"
                                    placeholder="Nhập tin nhắn..."
                                    value={newMessage}
                                    onChange={(e) => setNewMessage(e.target.value)}
                                />
                                <button type="submit" className="btn btn-primary">📤</button>
                            </form>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}
