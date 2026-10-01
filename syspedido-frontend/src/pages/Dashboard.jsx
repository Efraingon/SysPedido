import React, { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { useAuth } from '../auth/AuthContext';
import { BarChart, Bar, XAxis, YAxis, Tooltip, CartesianGrid, ResponsiveContainer, LineChart, Line, PieChart, Pie, Cell, Legend, AreaChart, Area } from 'recharts';
import { FiDollarSign, FiShoppingCart, FiTrendingUp, FiCalendar, FiArrowLeft, FiActivity, FiPieChart, FiBarChart2 } from 'react-icons/fi';
import Navbar from '../components/Navbar';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

export default function Dashboard() {
  const { logout, user } = useAuth();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');

  if (user?.role !== 'Admin') {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen bg-slate-950 text-white">
        <div className="bg-white/5 backdrop-blur-xl p-10 rounded-3xl border border-white/10 text-center shadow-2xl animate-fadeIn">
          <h2 className="text-3xl font-black text-red-400 mb-3 tracking-tighter">Acceso Denegado</h2>
          <p className="text-slate-400 mb-6 text-sm font-medium">Privilegios insuficientes para el Dashboard Global.</p>
          <button onClick={() => window.location.href = '/pedidos'} className="px-6 py-2.5 bg-blue-600 hover:bg-blue-500 rounded-xl font-black uppercase tracking-widest transition-all text-[10px]">Cerrar</button>
        </div>
      </div>
    );
  }

  const loadData = async () => {
    setLoading(true);
    try {
      let url = '/Dashboard/resumen';
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      
      const res = await api.get(`${url}?${params.toString()}`);
      setData(res.data);
    } catch(err) {
      console.error(err);
      if (err.response?.status === 401) logout();
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [startDate, endDate]);

  const totalSales = data?.totalVentas || 0;
  const totalOrders = data?.totalPedidos || 0;

  return (
    <div className="relative min-h-screen flex flex-col font-sans overflow-x-hidden text-slate-100">
      <div className="fixed inset-0 bg-slate-950 -z-10">
        <div className="absolute inset-0 bg-gradient-to-tr from-indigo-950 via-slate-900 to-blue-900 opacity-80"></div>
        <div className="absolute top-[-20%] right-[-10%] w-[60%] h-[60%] bg-blue-600/10 rounded-full blur-[150px] animate-pulse"></div>
        <div className="absolute bottom-[-10%] left-[-5%] w-[50%] h-[50%] bg-indigo-600/10 rounded-full blur-[150px] animate-pulse" style={{ animationDelay: '3s' }}></div>
      </div>

      <Navbar />

      <main className="flex-1 p-4 md:p-8 max-w-[1600px] mx-auto w-full animate-fadeIn">
        <header className="mb-6 rounded-3xl bg-white/5 backdrop-blur-xl border border-white/10 p-6 flex flex-col md:flex-row justify-between items-center gap-6">
          <div>
            <div className="flex items-center gap-2 mb-1">
              <span className="h-0.5 w-6 bg-blue-500 rounded-full"></span>
              <p className="text-[10px] font-black uppercase tracking-[0.3em] text-blue-400">Inteligencia de Negocio</p>
            </div>
            <h1 className="text-3xl font-black text-white tracking-tighter">
              Hola, <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-indigo-400">{user?.username}</span>
            </h1>
          </div>

          <div className="flex items-center bg-white/5 rounded-2xl p-1.5 border border-white/10">
            <div className="flex items-center px-3 py-1 gap-3 text-slate-400">
               <FiCalendar size={14} className="text-blue-400" />
               <input type="date" className="bg-transparent border-none focus:ring-0 text-[10px] font-black uppercase outline-none cursor-pointer" value={startDate} onChange={e => setStartDate(e.target.value)} />
               <input type="date" className="bg-transparent border-none focus:ring-0 text-[10px] font-black uppercase outline-none cursor-pointer" value={endDate} onChange={e => setEndDate(e.target.value)} />
            </div>
            <button onClick={loadData} className="bg-blue-600 hover:bg-blue-500 text-white px-4 py-2 rounded-xl font-black text-[10px] tracking-widest uppercase transition-all active:scale-95">
               {loading ? "..." : "Actualizar"}
            </button>
          </div>
        </header>

        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
          {[
            { label: 'Ventas Totales', value: `$${totalSales.toLocaleString()}`, icon: FiDollarSign, color: 'text-emerald-400', bg: 'bg-emerald-400/10' },
            { label: 'Órdenes', value: totalOrders, icon: FiShoppingCart, color: 'text-blue-400', bg: 'bg-blue-400/10' },
            { label: 'Ticket Promedio', value: `$${totalOrders > 0 ? (totalSales/totalOrders).toFixed(0) : 0}`, icon: FiActivity, color: 'text-amber-400', bg: 'bg-amber-400/10' },
            { label: 'Sincronización', value: '100%', icon: FiActivity, color: 'text-indigo-400', bg: 'bg-indigo-400/10' },
          ].map((kpi, i) => (
            <div key={i} className="p-4 rounded-2xl bg-white/5 border border-white/10 backdrop-blur-xl">
              <div className={`w-8 h-8 rounded-lg ${kpi.bg} ${kpi.color} flex items-center justify-center mb-3`}>
                <kpi.icon size={16} />
              </div>
              <p className="text-[9px] text-slate-500 font-black uppercase tracking-widest mb-1">{kpi.label}</p>
              <h3 className="text-xl font-black text-white tracking-tighter">{kpi.value}</h3>
            </div>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="p-6 rounded-3xl bg-white/5 border border-white/10 backdrop-blur-xl">
            <h3 className="text-xs font-black text-slate-400 uppercase tracking-widest mb-6 flex items-center gap-2">
                <FiBarChart2 className="text-blue-500" /> Rendimiento de Ventas
            </h3>
            <div className="h-64 w-full">
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={data?.ventasSemanales || []}>
                  <defs>
                    <linearGradient id="colorSales" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="#3b82f6" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <XAxis dataKey="dia" hide />
                  <Tooltip 
                    contentStyle={{ backgroundColor: '#0f172a', border: '1px solid rgba(255,255,255,0.1)', borderRadius: '12px' }}
                    itemStyle={{ color: '#fff' }}
                  />
                  <Area type="monotone" dataKey="monto" stroke="#3b82f6" fillOpacity={1} fill="url(#colorSales)" />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </div>

          <div className="p-6 rounded-3xl bg-white/5 border border-white/10 backdrop-blur-xl">
            <h3 className="text-xs font-black text-slate-400 uppercase tracking-widest mb-6 flex items-center gap-2">
                <FiPieChart className="text-amber-500" /> Distribución
            </h3>
            <div className="h-64 w-full">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={data?.distribucionCategoria || []}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={90}
                    paddingAngle={8}
                    dataKey="cantidad"
                    nameKey="categoria"
                    label={({categoria, percent}) => `${categoria} ${(percent * 100).toFixed(0)}%`}
                    stroke="none"
                  >
                    {data?.distribucionCategoria?.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #ffffff10', borderRadius: '1rem', color: '#fff' }} />
                  <Legend verticalAlign="bottom" height={36}/>
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        <footer className="mt-12 py-8 border-t border-white/5 text-center">
            <p className="text-slate-600 text-[9px] font-black uppercase tracking-[0.4em]">Integrated Intelligence Suite v4.0</p>
        </footer>
      </main>
      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; transform: translateY(10px); }
          to { opacity: 1; transform: translateY(0); }
        }
        .animate-fadeIn {
          animation: fadeIn 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards;
        }
      `}</style>
    </div>
  );
}
