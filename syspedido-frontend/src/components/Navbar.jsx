import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { FiHome, FiList, FiLogOut, FiUser } from 'react-icons/fi';

const Navbar = () => {
    const { user, logout } = useAuth();
    const location = useLocation();

    const isAdmin = user?.role === 'Admin';

    return (
        <nav className="bg-slate-950/60 backdrop-blur-xl text-white border-b border-white/5 px-4 md:px-8 py-2.5 flex justify-between items-center sticky top-0 z-[100]">
            <div className="flex items-center gap-8">
                <div className="flex items-center gap-2.5 group cursor-pointer" onClick={() => window.location.href = '/pedidos'}>
                    <div className="w-7 h-7 rounded-lg bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center shadow-lg shadow-blue-500/20 group-hover:rotate-6 transition-transform">
                        <span className="text-white font-black text-[10px]">S</span>
                    </div>
                    <div className="text-lg font-black tracking-tighter uppercase italic">
                        <span className="text-blue-400">Sys</span>Pedido
                    </div>
                </div>
                
                <div className="hidden md:flex items-center gap-5">
                    {isAdmin && (
                        <Link 
                            to="/dashboard" 
                            className={`text-[9px] font-black uppercase tracking-[0.2em] transition-all ${location.pathname === '/dashboard' ? 'text-blue-400 border-b-2 border-blue-400 pb-1' : 'text-slate-500 hover:text-white'}`}
                        >
                            Inteligencia
                        </Link>
                    )}
                    <Link 
                        to="/pedidos" 
                        className={`text-[9px] font-black uppercase tracking-[0.2em] transition-all ${location.pathname === '/pedidos' ? 'text-blue-400 border-b-2 border-blue-400 pb-1' : 'text-slate-500 hover:text-white'}`}
                    >
                        Operaciones
                    </Link>
                </div>
            </div>

            <div className="flex items-center gap-5">
                <div className="hidden sm:flex flex-col items-end">
                    <span className="text-[9px] font-black uppercase tracking-widest text-slate-100 flex items-center gap-1.5">
                        {user?.username} <div className="w-1.5 h-1.5 rounded-full bg-emerald-500"></div>
                    </span>
                    <span className="text-[8px] text-slate-600 font-bold uppercase tracking-widest leading-none mt-0.5">{user?.empresaNombre}</span>
                </div>
                
                <button 
                    onClick={logout}
                    className="p-2.5 bg-white/5 hover:bg-red-500/10 text-slate-500 hover:text-red-400 border border-white/10 rounded-xl transition-all duration-300 group"
                    title="Cerrar Sesión"
                >
                    <FiLogOut size={16} className="group-hover:-translate-x-0.5 transition-transform" />
                </button>
            </div>
        </nav>
    );
};

export default Navbar;
