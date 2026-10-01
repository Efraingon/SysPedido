import React, { useEffect, useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axiosConfig';
import { useAuth } from '../auth/AuthContext';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import Navbar from '../components/Navbar';
import { FiPlus, FiSearch, FiChevronDown, FiChevronUp, FiMapPin, FiPackage, FiCalendar, FiUser, FiTrash2, FiRefreshCw, FiEye, FiDollarSign, FiHash, FiLoader, FiAlertCircle, FiGrid, FiList as FiListIcon, FiActivity } from 'react-icons/fi';
import toast, { Toaster } from 'react-hot-toast';

// Marker Fix
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';

let DefaultIcon = L.icon({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

function MapUpdater({ coords }) {
  const map = useMap();
  useEffect(() => {
    if (coords) {
      setTimeout(() => {
        map.invalidateSize();
        map.setView(coords, 16);
      }, 100);
    }
  }, [coords, map]);
  return null;
}

function Pedidos() {
  const [pedidos, setPedidos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { logout, user } = useAuth();
  const [selectedLocation, setSelectedLocation] = useState(null);
  const [expandedRow, setExpandedRow] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState('table');
  const navigate = useNavigate();

  const loadPedidos = async () => {
    setLoading(true);
    try {
      const res = await api.get('/Pedido');
      setPedidos(Array.isArray(res.data) ? res.data : []);
    } catch(err) {
      if(err.response?.status === 401) logout();
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPedidos();
  }, []);

  const filteredPedidos = useMemo(() => {
    const list = Array.isArray(pedidos) ? pedidos : [];
    if (!searchTerm) return list;
    const term = searchTerm.toLowerCase();
    return list.filter(p => 
      String(p.numero || '').toLowerCase().includes(term) ||
      String(p.nombreCliente || p.codigoCliente || '').toLowerCase().includes(term)
    );
  }, [pedidos, searchTerm]);

  const totalGeneral = useMemo(() => 
    filteredPedidos.reduce((sum, p) => sum + (p.total || 0), 0), 
    [filteredPedidos]
  );

  const formatCurrency = (val) => {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(val || 0);
  };

  const mapCoords = selectedLocation?.latitud && selectedLocation?.longitud 
    ? [Number(selectedLocation.latitud), Number(selectedLocation.longitud)] 
    : null;

  return (
    <div className="relative min-h-screen flex flex-col font-sans bg-slate-950 text-slate-100 overflow-x-hidden">
      <div className="fixed inset-0 pointer-events-none opacity-40">
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-blue-600/20 rounded-full blur-[120px]"></div>
        <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-indigo-600/20 rounded-full blur-[120px]"></div>
      </div>

      <Toaster position="top-right" />
      <Navbar />

      <main className="relative z-10 flex-1 p-4 lg:p-8 max-w-[1600px] mx-auto w-full">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6 mb-8">
          <div>
            <div className="flex items-center gap-2 mb-1">
              <span className="h-1 w-6 bg-blue-500 rounded-full"></span>
              <p className="text-blue-400 text-[10px] font-black uppercase tracking-[0.4em]">Logística Inteligente</p>
            </div>
            <h1 className="text-3xl font-black text-white tracking-tighter">Gestión de <span className="text-blue-500">Pedidos</span></h1>
          </div>
          
          <div className="flex flex-wrap items-center gap-4">
            <div className="bg-white/5 backdrop-blur-xl border border-white/10 rounded-2xl px-6 py-3 flex items-center gap-4">
               <div className="p-2 bg-blue-500/10 rounded-lg"><FiActivity className="text-blue-400" size={14} /></div>
               <div>
                  <p className="text-[8px] font-black text-slate-500 uppercase tracking-widest">Registros</p>
                  <p className="text-sm font-black text-white leading-tight">{filteredPedidos.length}</p>
               </div>
            </div>
            <div className="bg-white/5 backdrop-blur-xl border border-white/10 rounded-2xl px-6 py-3 flex items-center gap-4">
               <div className="p-2 bg-emerald-500/10 rounded-lg"><FiDollarSign className="text-emerald-400" size={14} /></div>
               <div>
                  <p className="text-[8px] font-black text-slate-500 uppercase tracking-widest">Cartera Total</p>
                  <p className="text-sm font-black text-white leading-tight">{formatCurrency(totalGeneral)}</p>
               </div>
            </div>
            <button onClick={() => navigate('/pedidos/nuevo')} className="flex items-center justify-center gap-3 px-8 py-4 bg-blue-600 hover:bg-blue-500 text-white font-black uppercase rounded-2xl transition-all shadow-xl shadow-blue-600/30 text-[11px] active:scale-95">
              <FiPlus size={16} /> Nueva Orden
            </button>
          </div>
        </div>

        <div className="flex flex-col xl:flex-row gap-8">
          <div className="flex-1 space-y-6">
            <div className="flex flex-col sm:flex-row items-center gap-4 p-2 bg-white/5 backdrop-blur-md rounded-[2rem] border border-white/10 shadow-xl">
              <div className="relative flex-1 w-full">
                <FiSearch className="absolute left-6 top-1/2 -translate-y-1/2 text-slate-500" size={16} />
                <input type="text" placeholder="Buscar pedidos..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="w-full pl-14 pr-4 py-4 bg-transparent border-none text-white font-bold outline-none text-xs" />
              </div>
              <div className="flex items-center gap-2 pr-2 border-l border-white/10 pl-4">
                 <button onClick={() => setViewMode('table')} className={`p-3 rounded-xl ${viewMode === 'table' ? 'bg-blue-600' : 'text-slate-500'}`}><FiListIcon size={18} /></button>
                 <button onClick={() => setViewMode('cards')} className={`p-3 rounded-xl ${viewMode === 'cards' ? 'bg-blue-600' : 'text-slate-500'}`}><FiGrid size={18} /></button>
                 <button onClick={loadPedidos} className="p-3 text-slate-500 ml-2"><FiRefreshCw size={18} className={loading ? "animate-spin" : ""} /></button>
              </div>
            </div>

            {viewMode === 'table' ? (
              <div className="bg-white/5 backdrop-blur-xl border border-white/10 rounded-[2.5rem] overflow-hidden shadow-2xl">
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead>
                      <tr className="bg-white/5 border-b border-white/5 text-[10px] text-slate-500 uppercase font-black tracking-widest">
                         <th className="px-8 py-5 text-left">Orden</th>
                         <th className="px-8 py-5 text-left">Cliente</th>
                         <th className="px-8 py-5 text-right">Total</th>
                         <th className="px-8 py-5 text-center">Gps</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-white/5">
                      {filteredPedidos.map(p => (
                        <React.Fragment key={p.numero}>
                          <tr className="hover:bg-white/[0.03] cursor-pointer" onClick={() => setExpandedRow(expandedRow === p.numero ? null : p.numero)}>
                            <td className="px-8 py-4"><span className="text-blue-400 font-black">#{p.numero}</span></td>
                            <td className="px-8 py-4 text-sm font-bold text-white uppercase">{p.nombreCliente || p.codigoCliente}</td>
                            <td className="px-8 py-4 text-right font-black text-emerald-400">{formatCurrency(p.total)}</td>
                            <td className="px-8 py-4 text-center">
                              {(p.latitud || p.longitud) && (
                                <button onClick={(e) => { e.stopPropagation(); setSelectedLocation(p); }} className="p-2.5 rounded-xl bg-blue-600/20 text-blue-400 hover:bg-blue-600 transition-all shadow-lg active:scale-90"><FiMapPin size={16} /></button>
                              )}
                            </td>
                          </tr>
                          {expandedRow === p.numero && (
                            <tr>
                              <td colSpan="4" className="p-6 bg-slate-900/60">
                                 <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                                    {p.renglones?.map((r, i) => (
                                      <div key={i} className="bg-white/5 p-4 rounded-2xl flex justify-between border border-white/5">
                                         <span className="text-[10px] font-black text-blue-300">{(r.descripcion || r.codigoArticulo)} <span className="text-slate-500 ml-2">x{r.cantidad}</span></span>
                                         <span className="text-[11px] font-black text-white">${r.totalRenglon?.toFixed(2)}</span>
                                      </div>
                                    ))}
                                 </div>
                              </td>
                            </tr>
                          )}
                        </React.Fragment>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            ) : (
              /* CARD VIEW */
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 pb-20">
                {filteredPedidos.map(p => (
                   <div key={p.numero} className="bg-white/5 backdrop-blur-xl border border-white/10 rounded-[2.5rem] p-8 hover:bg-white/[0.08] transition-all group shadow-2xl">
                      <div className="flex justify-between items-start mb-6">
                         <div>
                            <p className="text-blue-400 text-[10px] font-black uppercase tracking-widest mb-1">Orden #{p.numero}</p>
                            <h4 className="text-lg font-black text-white uppercase tracking-tighter leading-tight">{p.nombreCliente || p.codigoCliente}</h4>
                         </div>
                         <div className="flex gap-2">
                           {(p.latitud || p.longitud) && (
                              <button onClick={() => setSelectedLocation(p)} className="p-3 bg-blue-600/20 text-blue-400 rounded-xl hover:bg-blue-600 hover:text-white transition-all"><FiMapPin size={18} /></button>
                           )}
                           <button onClick={() => setExpandedRow(expandedRow === p.numero ? null : p.numero)} className="p-3 bg-white/5 text-slate-400 rounded-xl hover:bg-white/10 transition-all">
                              {expandedRow === p.numero ? <FiChevronUp size={18} /> : <FiChevronDown size={18} />}
                           </button>
                         </div>
                      </div>

                      <div className="flex items-center justify-between mt-auto">
                         <div className="flex items-center gap-3">
                            <div className="p-2.5 bg-emerald-500/10 rounded-lg"><FiDollarSign className="text-emerald-500" size={14} /></div>
                            <span className="text-xl font-black text-white">{formatCurrency(p.total)}</span>
                         </div>
                         <div className="text-[10px] font-black text-slate-500 uppercase tracking-widest">{p.renglones?.length || 0} Items</div>
                      </div>

                      {expandedRow === p.numero && (
                        <div className="mt-8 pt-8 border-t border-white/10 space-y-4">
                           {p.renglones?.map((r, i) => (
                             <div key={i} className="flex justify-between items-center text-[10px] font-bold uppercase tracking-widest p-4 bg-white/5 rounded-2xl">
                                <span className="text-slate-400">{r.descripcion || r.codigoArticulo} <span className="text-blue-500 font-black ml-2">x{r.cantidad}</span></span>
                                <span className="text-white">${r.totalRenglon?.toFixed(2)}</span>
                             </div>
                           ))}
                        </div>
                      )}
                   </div>
                ))}
              </div>
            )}
          </div>

          <div className="xl:w-[450px]">
             <div className="bg-white/5 backdrop-blur-3xl border border-white/10 rounded-[3rem] overflow-hidden sticky top-24 shadow-2xl">
                <div className="px-8 py-5 border-b border-white/10 bg-white/5 flex items-center justify-between">
                   <h3 className="font-black text-[11px] text-blue-200 uppercase tracking-widest">Localización</h3>
                   {selectedLocation && <span className="bg-blue-500 text-black px-2 py-0.5 rounded-full text-[8px] font-black uppercase">En Ruta</span>}
                </div>
                <div className="h-[400px] w-full bg-[#0d1117] relative">
                   {mapCoords ? (
                      <MapContainer 
                        key={selectedLocation.numero}
                        center={mapCoords} 
                        zoom={16} 
                        className="h-full w-full"
                      >
                         <TileLayer url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png" attribution='&copy; CARTO' />
                         <Marker position={mapCoords} />
                         <MapUpdater coords={mapCoords} />
                      </MapContainer>
                   ) : (
                      <div className="h-full flex flex-col items-center justify-center p-12 text-center">
                         <FiMapPin className="text-slate-800 mb-4 animate-bounce" size={48} />
                         <p className="text-[10px] text-slate-600 font-black uppercase tracking-widest leading-relaxed">Seleccione un pedido para activar el monitor GPS.</p>
                      </div>
                   )}
                </div>
                <div className="p-8">
                   {selectedLocation ? (
                      <div className="space-y-4 text-xs font-bold text-slate-400">
                         <div className="flex justify-between uppercase"><span>Orden:</span> <span className="text-blue-400">#{selectedLocation.numero}</span></div>
                         <div className="flex justify-between uppercase"><span>Lat:</span> <span className="text-white">{selectedLocation.latitud}</span></div>
                         <div className="flex justify-between uppercase"><span>Lng:</span> <span className="text-white">{selectedLocation.longitud}</span></div>
                      </div>
                   ) : (
                      <div className="h-10 bg-white/5 rounded-2xl animate-pulse"></div>
                   )}
                </div>
             </div>
          </div>
        </div>
      </main>
      <style>{`
        .leaflet-container { width: 100%; height: 100%; z-index: 1; }
      `}</style>
    </div>
  );
}

export default Pedidos;
