// API Client with JWT authentication
const BASE_URL = 'https://eldercareweb.onrender.com/api';
class ApiClient {
    constructor() {
        this.baseUrl = BASE_URL;
    }

    getToken() {
        return localStorage.getItem('token');
    }

    setToken(token) {
        localStorage.setItem('token', token);
    }

    removeToken() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const token = this.getToken();

        const config = {
            headers: {
                'Content-Type': 'application/json',
                ...(token && { Authorization: `Bearer ${token}` }),
                ...options.headers,
            },
            ...options,
        };

        // Remove Content-Type for FormData
        if (options.body instanceof FormData) {
            delete config.headers['Content-Type'];
        }

        try {
            const response = await fetch(url, config);

            if (response.status === 401) {
                this.removeToken();
                window.location.href = '/login';
                return { isSuccess: false, message: 'Phiên đăng nhập hết hạn' };
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error('API Error:', error);
            return { isSuccess: false, message: 'Lỗi kết nối server' };
        }
    }

    get(endpoint) {
        return this.request(endpoint, { method: 'GET' });
    }

    post(endpoint, body) {
        return this.request(endpoint, {
            method: 'POST',
            body: JSON.stringify(body),
        });
    }

    put(endpoint, body) {
        return this.request(endpoint, {
            method: 'PUT',
            body: JSON.stringify(body),
        });
    }

    delete(endpoint) {
        return this.request(endpoint, { method: 'DELETE' });
    }

    upload(endpoint, formData) {
        return this.request(endpoint, {
            method: 'POST',
            body: formData,
        });
    }
}

const api = new ApiClient();
export default api;
