import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import Landing from './pages/Landing';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Bookings from './pages/Bookings';
import Wallet from './pages/Wallet';
import Chat from './pages/Chat';
import Notifications from './pages/Notifications';
import Admin from './pages/Admin';
import AiAdvisor from './pages/AiAdvisor';
import Profile from './pages/Profile';
import Beneficiaries from './pages/Beneficiaries';

function ProtectedRoute({ children }) {
    const { isAuthenticated, loading } = useAuth();
    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải...</p></div>;
    return isAuthenticated ? children : <Navigate to="/login" replace />;
}

function PublicRoute({ children }) {
    const { isAuthenticated, loading } = useAuth();
    if (loading) return <div className="loading-overlay"><div className="spinner spinner-lg" /><p>Đang tải...</p></div>;
    return !isAuthenticated ? children : <Navigate to="/dashboard" replace />;
}

export default function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    {/* Public */}
                    <Route path="/" element={<PublicRoute><Landing /></PublicRoute>} />
                    <Route path="/login" element={<PublicRoute><Login /></PublicRoute>} />
                    <Route path="/register" element={<PublicRoute><Register /></PublicRoute>} />

                    {/* Protected with Layout */}
                    <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
                        <Route path="/dashboard" element={<Dashboard />} />
                        <Route path="/bookings" element={<Bookings />} />
                        <Route path="/wallet" element={<Wallet />} />
                        <Route path="/chat" element={<Chat />} />
                        <Route path="/notifications" element={<Notifications />} />
                        <Route path="/admin" element={<Admin />} />
                        <Route path="/ai-advisor" element={<AiAdvisor />} />
                        <Route path="/profile" element={<Profile />} />
                        <Route path="/beneficiaries" element={<Beneficiaries />} />
                    </Route>

                    {/* Fallback */}
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    );
}
