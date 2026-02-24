import { useState, useEffect, useRef } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Pages.css';

export default function AiAdvisor() {
    const { user } = useAuth();
    const [beneficiaries, setBeneficiaries] = useState([]);
    const [selectedBeneficiary, setSelectedBeneficiary] = useState(null);
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState('');
    const [loading, setLoading] = useState(false);
    const messagesEndRef = useRef(null);

    useEffect(() => {
        loadBeneficiaries();
        // Welcome message
        setMessages([{
            role: 'assistant',
            content: 'Xin chào! 👋 Tôi là trợ lý AI của Tuổi Vàng. Tôi có thể giúp bạn:\n\n🔹 Tư vấn dịch vụ chăm sóc phù hợp\n🔹 Đề xuất người chăm sóc dựa trên tình trạng sức khỏe\n🔹 Giải đáp thắc mắc về quy trình đặt dịch vụ\n\nBạn có thể hỏi tôi bất cứ điều gì! Nếu muốn tư vấn cá nhân hóa hơn, hãy chọn người thụ hưởng ở trên nhé.'
        }]);
    }, []);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const loadBeneficiaries = async () => {
        const res = await api.get('/customers/beneficiaries');
        if (res.isSuccess) setBeneficiaries(res.data || []);
    };

    const handleSelectBeneficiary = (e) => {
        const id = e.target.value;
        const b = beneficiaries.find(b => b.id === id);
        setSelectedBeneficiary(b || null);
        if (b) {
            setMessages(prev => [...prev, {
                role: 'assistant',
                content: `Đã chọn người thụ hưởng: **${b.fullName}** (${b.age} tuổi). Tôi sẽ tư vấn dựa trên thông tin sức khỏe và nhu cầu của ${b.gender === 0 ? 'ông' : 'bà'} ${b.fullName}. Bạn muốn hỏi gì?`
            }]);
        }
    };

    const quickQuestions = [
        'Tìm người chăm sóc phù hợp',
        'Tôi cần lưu ý gì về sức khỏe?',
        'Dịch vụ nào phù hợp nhất?',
        'Chi phí ước tính bao nhiêu?'
    ];

    const sendMessage = async (text) => {
        const message = text || input.trim();
        if (!message || loading) return;

        const userMsg = { role: 'user', content: message };
        setMessages(prev => [...prev, userMsg]);
        setInput('');
        setLoading(true);

        try {
            const history = messages.map(m => ({ role: m.role, content: m.content }));
            const res = await api.post('/chatbot/ask', {
                message,
                beneficiaryId: selectedBeneficiary?.id || null,
                history
            });

            setMessages(prev => [...prev, {
                role: 'assistant',
                content: res.reply || res.data?.reply || 'Xin lỗi, tôi không thể xử lý yêu cầu này.'
            }]);
        } catch {
            setMessages(prev => [...prev, {
                role: 'assistant',
                content: 'Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại.'
            }]);
        }
        setLoading(false);
    };

    const handleKeyDown = (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    };

    return (
        <div className="ai-advisor-page">
            <div className="ai-advisor-header">
                <div>
                    <h1 className="page-title">🤖 Trợ Lý AI Tuổi Vàng</h1>
                    <p className="page-subtitle">Tư vấn dịch vụ chăm sóc thông minh</p>
                </div>
                <div className="beneficiary-select-wrap">
                    <label>Người thụ hưởng:</label>
                    <select
                        className="form-input"
                        value={selectedBeneficiary?.id || ''}
                        onChange={handleSelectBeneficiary}
                    >
                        <option value="">-- Chọn người thụ hưởng --</option>
                        {beneficiaries.map(b => (
                            <option key={b.id} value={b.id}>{b.fullName} ({b.age} tuổi)</option>
                        ))}
                    </select>
                </div>
            </div>

            {selectedBeneficiary && (
                <div className="beneficiary-context-card card">
                    <div className="context-items">
                        <span className="context-tag">👤 {selectedBeneficiary.fullName}</span>
                        <span className="context-tag">🎂 {selectedBeneficiary.age} tuổi</span>
                        {selectedBeneficiary.mobilityLevel != null && (
                            <span className="context-tag">🦽 {['Vận động tốt', 'Hạn chế nhẹ', 'Hạn chế vừa', 'Hạn chế nặng', 'Nằm liệt giường'][selectedBeneficiary.mobilityLevel]}</span>
                        )}
                        {selectedBeneficiary.cognitiveStatus != null && (
                            <span className="context-tag">🧠 {['Bình thường', 'Suy giảm nhẹ', 'Suy giảm vừa', 'Suy giảm nặng'][selectedBeneficiary.cognitiveStatus]}</span>
                        )}
                        {selectedBeneficiary.medicalConditions && (
                            <span className="context-tag">🏥 {selectedBeneficiary.medicalConditions}</span>
                        )}
                    </div>
                </div>
            )}

            <div className="ai-chat-container card">
                <div className="ai-chat-messages">
                    {messages.map((msg, i) => (
                        <div key={i} className={`ai-chat-message ai-chat-${msg.role}`}>
                            <div className="ai-chat-avatar">
                                {msg.role === 'assistant' ? '🤖' : '👤'}
                            </div>
                            <div className="ai-chat-bubble">
                                <p>{msg.content}</p>
                            </div>
                        </div>
                    ))}
                    {loading && (
                        <div className="ai-chat-message ai-chat-assistant">
                            <div className="ai-chat-avatar">🤖</div>
                            <div className="ai-chat-bubble ai-typing">
                                <span></span><span></span><span></span>
                            </div>
                        </div>
                    )}
                    <div ref={messagesEndRef} />
                </div>

                <div className="ai-quick-questions">
                    {quickQuestions.map((q, i) => (
                        <button key={i} className="quick-question-btn" onClick={() => sendMessage(q)}>
                            {q}
                        </button>
                    ))}
                </div>

                <div className="ai-chat-input-area">
                    <textarea
                        className="form-input ai-chat-input"
                        value={input}
                        onChange={e => setInput(e.target.value)}
                        onKeyDown={handleKeyDown}
                        placeholder="Nhập câu hỏi của bạn..."
                        rows={1}
                    />
                    <button
                        className="btn btn-primary"
                        onClick={() => sendMessage()}
                        disabled={loading || !input.trim()}
                    >
                        {loading ? '⏳' : '📤'} Gửi
                    </button>
                </div>
            </div>
        </div>
    );
}
