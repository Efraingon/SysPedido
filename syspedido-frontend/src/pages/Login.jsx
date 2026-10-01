import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { FiUser, FiLock, FiLogIn, FiAlertCircle, FiArrowRight, FiLoader } from 'react-icons/fi';

function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    
    try {
        const success = await login(username, password);
        if (success) {
          navigate('/pedidos');
        } else {
          setError('Credenciales incorrectas o usuario no encontrado.');
        }
    } catch (err) {
        setError('Error de conexión con el servidor.');
    } finally {
        setLoading(false);
    }
  };

  return (
    <div className="relative flex items-center justify-center min-h-screen overflow-hidden font-sans">
      {/* Dynamic Background */}
      <div className="absolute inset-0 bg-blue-900 border-none">
        <div className="absolute inset-0 bg-gradient-to-tr from-indigo-900 via-blue-800 to-indigo-700 opacity-90"></div>
        <div className="absolute top-0 left-0 w-full h-full bg-[url('https://images.unsplash.com/photo-1557683316-973673baf926?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80')] bg-cover bg-center mix-blend-overlay"></div>
        {/* Animated Orbs */}
        <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-blue-500/10 rounded-full blur-[120px] animate-pulse"></div>
        <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-indigo-500/10 rounded-full blur-[120px] animate-pulse" style={{ animationDelay: '2s' }}></div>
      </div>

      <div className="relative w-full max-w-md px-6 animate-fadeIn">
        <div className="bg-white/10 backdrop-blur-2xl rounded-3xl shadow-[0_20px_60px_rgba(0,0,0,0.4)] overflow-hidden border border-white/20">
          <div className="p-8 md:p-10">
            <div className="text-center mb-10">
               <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-blue-400 to-indigo-600 text-white rounded-2xl mb-4 shadow-xl rotate-2 hover:rotate-0 transition-transform duration-500">
                  <FiLogIn size={32} />
               </div>
               <h2 className="text-4xl font-black text-white tracking-tighter mb-1">SysPedido</h2>
               <div className="flex items-center justify-center gap-2">
                 <div className="h-px w-6 bg-blue-400/30"></div>
                 <p className="text-blue-100/60 text-[10px] font-black uppercase tracking-[0.2em]">Acceso Corporativo</p>
                 <div className="h-px w-6 bg-blue-400/30"></div>
               </div>
            </div>

            {error && (
              <div className="mb-6 p-4 bg-red-500/20 backdrop-blur-md border border-red-500/30 text-red-100 rounded-xl flex items-center gap-3 animate-shake">
                <FiAlertCircle className="shrink-0 text-lg text-red-400" />
                <span className="text-xs font-bold uppercase tracking-tight">{error}</span>
              </div>
            )}

            <form onSubmit={handleLogin} className="space-y-6">
              <div className="space-y-2">
                <label className="text-[10px] font-black text-blue-200 uppercase tracking-widest ml-1">Usuario</label>
                <div className="relative group">
                  <span className="absolute inset-y-0 left-0 pl-4 flex items-center text-blue-300 group-focus-within:text-white transition-colors duration-300">
                    <FiUser size={18} />
                  </span>
                  <input 
                    type="text" 
                    placeholder="ID de comercial" 
                    value={username} 
                    onChange={e => setUsername(e.target.value)} 
                    className="block w-full pl-12 pr-6 py-3.5 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-blue-300/30 focus:outline-none focus:ring-4 focus:ring-blue-500/20 focus:border-blue-400/30 focus:bg-white/10 transition-all duration-300 text-sm font-bold"
                    required
                    autoFocus
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-blue-200 uppercase tracking-widest ml-1">Contraseña</label>
                <div className="relative group">
                  <span className="absolute inset-y-0 left-0 pl-4 flex items-center text-blue-300 group-focus-within:text-white transition-colors duration-300">
                    <FiLock size={18} />
                  </span>
                  <input 
                    type="password" 
                    placeholder="••••••••" 
                    value={password} 
                    onChange={e => setPassword(e.target.value)} 
                    className="block w-full pl-12 pr-6 py-3.5 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-blue-300/30 focus:outline-none focus:ring-4 focus:ring-blue-500/20 focus:border-blue-400/30 focus:bg-white/10 transition-all duration-300 text-sm font-bold"
                    required
                  />
                </div>
              </div>

              <div className="pt-2">
                <button 
                  type="submit" 
                  disabled={loading}
                  className="group relative w-full flex justify-center items-center py-4 px-6 font-black rounded-2xl text-white bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 focus:outline-none focus:ring-4 focus:ring-blue-500/30 shadow-lg shadow-blue-600/20 transition-all duration-300 active:scale-95 disabled:opacity-70 disabled:cursor-not-allowed uppercase tracking-widest text-xs"
                >
                  {loading ? (
                    <FiLoader className="animate-spin h-5 w-5 text-white" />
                  ) : (
                    <>
                      Iniciar Sesión
                      <FiArrowRight className="ml-2 group-hover:translate-x-1 transition-transform" />
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
          <div className="bg-white/5 py-4 text-center border-t border-white/5">
             <span className="text-[9px] text-blue-200/40 font-black uppercase tracking-widest">SysPedido • v3.1 Enterprise</span>
          </div>
        </div>
      </div>
      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; transform: translateY(20px); }
          to { opacity: 1; transform: translateY(0); }
        }
        .animate-fadeIn {
          animation: fadeIn 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards;
        }
        @keyframes shake {
          0%, 100% { transform: translateX(0); }
          25% { transform: translateX(-4px); }
          75% { transform: translateX(4px); }
        }
        .animate-shake {
          animation: shake 0.2s ease-in-out 0s 2;
        }
      `}</style>
    </div>
  );
}

export default Login;
