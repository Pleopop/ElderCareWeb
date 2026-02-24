import { createContext, useContext, useState, useEffect } from 'react';
import api from '../api/client';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const savedUser = localStorage.getItem('user');
        if (token && savedUser) {
            try {
                setUser(JSON.parse(savedUser));
            } catch {
                localStorage.removeItem('user');
            }
        }
        setLoading(false);
    }, []);

    const login = async (email, password) => {
        const result = await api.post('/auth/login', { emailOrPhone: email, password });
        if (result.isSuccess) {
            const { accessToken, refreshToken, user: userData } = result.data;
            api.setToken(accessToken);
            localStorage.setItem('user', JSON.stringify(userData));
            setUser(userData);
        }
        return result;
    };

    const registerCustomer = async (data) => {
        const result = await api.post('/auth/register/customer', data);
        if (result.isSuccess) {
            const { accessToken, refreshToken, user: userData } = result.data;
            api.setToken(accessToken);
            localStorage.setItem('user', JSON.stringify(userData));
            setUser(userData);
        }
        return result;
    };

    const registerCaregiver = async (data) => {
        const result = await api.post('/auth/register/caregiver', data);
        if (result.isSuccess) {
            const { accessToken, refreshToken, user: userData } = result.data;
            api.setToken(accessToken);
            localStorage.setItem('user', JSON.stringify(userData));
            setUser(userData);
        }
        return result;
    };

    const logout = () => {
        api.removeToken();
        setUser(null);
    };

    const value = {
        user,
        loading,
        login,
        registerCustomer,
        registerCaregiver,
        logout,
        isAuthenticated: !!user,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
}
