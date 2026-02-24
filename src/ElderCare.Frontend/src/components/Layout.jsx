import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Navbar from './Navbar';
import './Layout.css';

export default function Layout() {
    const [sidebarOpen, setSidebarOpen] = useState(false);

    return (
        <div className="layout">
            <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />
            <div className="layout-main">
                <Navbar onToggleSidebar={() => setSidebarOpen(!sidebarOpen)} />
                <main className="layout-content">
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
