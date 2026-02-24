import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';
import './Pages.css';

export default function Wallet() {
    const { user } = useAuth();
    const [wallet, setWallet] = useState(null);
    const [transactions, setTransactions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showDeposit, setShowDeposit] = useState(false);
    const [depositAmount, setDepositAmount] = useState('');
    const [depositing, setDepositing] = useState(false);

    useEffect(() => { loadData(); }, []);

    const loadData = async () => {
        setLoading(true);
        const [wRes, tRes] = await Promise.all([
            api.get('/payments/wallet'),
            api.get('/payments/transactions?pageSize=20'),
        ]);
        if (wRes.isSuccess) setWallet(wRes.data);
        if (tRes.isSuccess) setTransactions(tRes.data?.items || tRes.data || []);
        setLoading(false);
    };

    const handleDeposit = async () => {
        if (!depositAmount || parseFloat(depositAmount) <= 0) return;
        setDepositing(true);
        const res = await api.post('/payments/deposit', { amount: parseFloat(depositAmount) });
        if (res.isSuccess) { loadData(); setShowDeposit(false); setDepositAmount(''); }
        setDepositing(false);
    };

    const formatCurrency = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);
    const formatDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '';

    const typeMap = { Deposit: { label: 'Nạp tiền', icon: '💰', badge: 'badge-success' }, Withdrawal: { label: 'Rút tiền', icon: '💸', badge: 'badge-warning' }, EscrowHold: { label: 'Tạm giữ', icon: '🔒', badge: 'badge-info' }, EscrowRelease: { label: 'Giải phóng', icon: '🔓', badge: 'badge-gold' }, Commission: { label: 'Hoa hồng', icon: '📊', badge: 'badge-neutral' }, Refund: { label: 'Hoàn tiền', icon: '↩️', badge: 'badge-success' } };

    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải ví...</p></div>;

    return (
        <div className="page animate-fade">
            <div className="page-header">
                <div>
                    <h1 className="page-title">💰 {user?.role === 'Caregiver' ? 'Thu nhập' : 'Ví tiền'}</h1>
                    <p className="page-subtitle">Quản lý tài chính của bạn</p>
                </div>
            </div>

            {/* Wallet Card */}
            <div className="wallet-card">
                <div className="wallet-card-bg" />
                <div className="wallet-info">
                    <div className="wallet-label">Số dư hiện tại</div>
                    <div className="wallet-balance">{formatCurrency(wallet?.balance)}</div>
                    <div className="wallet-actions-row">
                        <button className="btn btn-primary" onClick={() => setShowDeposit(true)}>💰 Nạp tiền</button>
                    </div>
                </div>
            </div>

            {/* Deposit Modal */}
            {showDeposit && (
                <div className="modal-overlay" onClick={() => setShowDeposit(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2 className="modal-title">💰 Nạp tiền vào ví</h2>
                            <button className="btn btn-ghost btn-icon" onClick={() => setShowDeposit(false)}>✕</button>
                        </div>
                        <div className="form-group">
                            <label className="form-label">Số tiền (VND)</label>
                            <input type="number" className="form-input" placeholder="100000" value={depositAmount} onChange={(e) => setDepositAmount(e.target.value)} min={1000} />
                        </div>
                        <div className="deposit-presets">
                            {[100000, 500000, 1000000, 2000000].map((v) => (
                                <button key={v} className="btn btn-ghost btn-sm" onClick={() => setDepositAmount(v.toString())}>{formatCurrency(v)}</button>
                            ))}
                        </div>
                        <button className="btn btn-primary btn-lg auth-submit" onClick={handleDeposit} disabled={depositing}>
                            {depositing ? 'Đang xử lý...' : `Nạp ${depositAmount ? formatCurrency(parseFloat(depositAmount)) : ''}`}
                        </button>
                    </div>
                </div>
            )}

            {/* Transaction History */}
            <div className="card" style={{ marginTop: 'var(--space-xl)' }}>
                <div className="card-header">
                    <h3>📋 Lịch sử giao dịch</h3>
                </div>
                {transactions.length === 0 ? (
                    <div className="empty-state"><div className="empty-state-icon">📋</div><div className="empty-state-title">Chưa có giao dịch</div></div>
                ) : (
                    <div className="transaction-list">
                        {transactions.map((t, i) => {
                            const info = typeMap[t.type] || { label: t.type, icon: '💳', badge: 'badge-neutral' };
                            return (
                                <div key={i} className="transaction-item">
                                    <div className="transaction-icon">{info.icon}</div>
                                    <div className="transaction-info">
                                        <div className="transaction-label">{info.label}</div>
                                        <div className="transaction-date">{formatDate(t.createdAt)}</div>
                                    </div>
                                    <div className={`transaction-amount ${t.type === 'Deposit' || t.type === 'Refund' || t.type === 'EscrowRelease' ? 'amount-positive' : 'amount-negative'}`}>
                                        {(t.type === 'Deposit' || t.type === 'Refund' || t.type === 'EscrowRelease') ? '+' : '-'}{formatCurrency(t.amount)}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}
            </div>
        </div>
    );
}
