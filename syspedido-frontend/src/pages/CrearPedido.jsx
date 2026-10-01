import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axiosConfig';
import Navbar from '../components/Navbar';
import { FiSave, FiPlus, FiTrash2, FiMapPin, FiArrowLeft, FiLoader, FiShoppingBag, FiInfo, FiTag } from 'react-icons/fi';
import toast from 'react-hot-toast';

const CrearPedido = () => {
    const navigate = useNavigate();
    const [clientes, setClientes] = useState([]);
    const [productos, setProductos] = useState([]);
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [isLocating, setIsLocating] = useState(false);

    const [pedido, setPedido] = useState({
        codigoCliente: '',
        observaciones: '',
        latitud: 0,
        longitud: 0,
        renglones: []
    });

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [resCli, resProd] = await Promise.all([
                    api.get('/Cliente'),
                    api.get('/Producto')
                ]);
                setClientes(Array.isArray(resCli.data) ? resCli.data : []);
                // Filter: LineaDeProducto in SERVICIOS CLUB, MARINA, HOTELERIA AND Codigo starts with S0000
                const allowedLines = ['SERVICIOS CLUB', 'MARINA', 'HOTELERIA'];
                const filteredProducts = (Array.isArray(resProd.data) ? resProd.data : [])
                    .filter(p => {
                        const linea = (p.lineaDeProducto || "").trim();
                        const codigo = (p.codigo || p.Codigo || "").trim();
                        return allowedLines.includes(linea) && codigo.startsWith("S0000");
                    });

                setProductos(filteredProducts);
            } catch (err) {
                toast.error("Error cargando catálogos");
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    const handleAddRenglon = () => {
        setPedido({
            ...pedido,
            renglones: [
                ...pedido.renglones,
                { codigoArticulo: '', cantidad: 1, precioSinIVA: 0, precioConIVA: 0, totalRenglon: 0, descripcion: '' }
            ]
        });
    };

    const handleRemoveRenglon = (index) => {
        const newRenglones = [...pedido.renglones];
        newRenglones.splice(index, 1);
        setPedido({ ...pedido, renglones: newRenglones });
    };

    const handleRenglonChange = (index, field, value) => {
        const newRenglones = [...pedido.renglones];
        const renglon = { ...newRenglones[index] };

        if (field === 'codigoArticulo') {
            const prod = productos.find(p => (p.codigo || p.Codigo) === value);
            renglon.codigoArticulo = value;
            renglon.descripcion = prod ? (prod.descripcion || prod.Descripcion || '') : '';
            renglon.precioSinIVA = prod ? (prod.precioSinIva ?? prod.PrecioSinIva ?? 0) : 0;
            renglon.precioConIVA = prod ? (prod.precioConIva ?? prod.PrecioConIva ?? 0) : 0;
        } else {
            renglon[field] = value;
        }

        renglon.totalRenglon = renglon.cantidad * renglon.precioConIVA;
        newRenglones[index] = renglon;
        setPedido({ ...pedido, renglones: newRenglones });
    };

    const handleGetLocation = () => {
        setIsLocating(true);
        if (!navigator.geolocation) {
            toast.error("Geolocalización no soportada");
            setIsLocating(false);
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (pos) => {
                setPedido({ ...pedido, latitud: pos.coords.latitude, longitud: pos.coords.longitude });
                toast.success("Ubicación detectada");
                setIsLocating(false);
            },
            () => {
                toast.error("Error al obtener ubicación");
                setIsLocating(false);
            }
        );
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!pedido.codigoCliente) return toast.error("Seleccione un cliente");
        if (pedido.renglones.length === 0) return toast.error("Añada al menos un artículo");

        setSubmitting(true);
        try {
            await api.post('/Pedido', {
                ...pedido,
                fecha: new Date().toISOString(),
                total: pedido.renglones.reduce((acc, curr) => acc + curr.totalRenglon, 0)
            });
            toast.success("Pedido creado correctamente");
            navigate('/pedidos');
        } catch (err) {
            toast.error("Error al crear pedido");
        } finally {
            setSubmitting(false);
        }
    };

    if (loading) return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center">
            <FiLoader className="animate-spin text-5xl text-blue-500 mb-4" />
            <p className="text-white font-black uppercase tracking-[0.4em] text-xs">Preparando Catálogos...</p>
        </div>
    );

    const totalCalculado = pedido.renglones.reduce((acc, r) => acc + (r.totalRenglon || 0), 0);

    return (
        <div className="relative min-h-screen flex flex-col font-sans overflow-x-hidden text-slate-100">
            {/* Background Layer */}
            <div className="fixed inset-0 bg-slate-950 -z-10">
                <div className="absolute inset-0 bg-gradient-to-br from-indigo-950 via-slate-900 to-slate-950 opacity-90"></div>
                <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-blue-500/10 rounded-full blur-[120px] animate-pulse"></div>
            </div>

            <Navbar />

            <div className="flex-1 w-full max-w-[1100px] mx-auto px-4 md:px-6 py-8">
                <div className="mb-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => navigate('/pedidos')}
                            className="p-3 bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 text-white transition shadow-xl"
                        >
                            <FiArrowLeft size={20} />
                        </button>
                        <div>
                            <div className="flex items-center gap-2 mb-1">
                                <div className="h-0.5 w-4 bg-emerald-500"></div>
                                <p className="text-emerald-400 text-[10px] font-black uppercase tracking-[0.3em]">Operativa</p>
                            </div>
                            <h1 className="text-2xl md:text-3xl font-black text-white tracking-tighter">Nueva Orden</h1>
                        </div>
                    </div>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6 animate-fadeIn">
                    {/* Header Info - Client & Tags */}
                    <div className="bg-white/5 backdrop-blur-2xl p-6 md:p-8 rounded-3xl border border-white/10 shadow-2xl space-y-6">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div className="space-y-2">
                                <label className="flex items-center gap-2 text-[10px] font-black text-blue-300 uppercase tracking-widest ml-1">
                                    <FiShoppingBag /> Cliente
                                </label>
                                <select
                                    className="w-full p-4 bg-slate-900 border border-white/10 rounded-2xl text-white outline-none focus:border-blue-500/50 focus:ring-4 focus:ring-blue-500/20 transition-all text-sm cursor-pointer font-bold"
                                    value={pedido.codigoCliente}
                                    onChange={e => {
                                        console.log("Seleccionado cliente:", e.target.value);
                                        setPedido({ ...pedido, codigoCliente: e.target.value });
                                    }}
                                    required
                                >
                                    <option value="" className="bg-slate-900 text-slate-400">Seleccionar cliente...</option>
                                    {clientes.map(c => (
                                        <option key={c.consecutivo} value={c.codigo} className="bg-slate-900 text-white">
                                            {c.nombre} [{c.codigo}]
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="space-y-2">
                                <label className="flex items-center gap-2 text-[10px] font-black text-blue-300 uppercase tracking-widest ml-1">
                                    <FiInfo /> Observaciones
                                </label>
                                <input
                                    type="text"
                                    className="w-full p-3.5 bg-white/5 border border-white/10 rounded-2xl text-white outline-none focus:border-blue-500/50 focus:ring-4 focus:ring-blue-500/20 transition-all text-sm placeholder-white/20 font-bold"
                                    placeholder="Nota de entrega..."
                                    value={pedido.observaciones}
                                    onChange={e => setPedido({ ...pedido, observaciones: e.target.value })}
                                />
                            </div>
                        </div>

                        {/* GPS Section */}
                        <div className="p-4 bg-gradient-to-r from-blue-600/20 to-indigo-600/10 rounded-2xl border border-blue-500/20 flex flex-col md:flex-row items-center gap-4">
                            <button
                                type="button"
                                onClick={handleGetLocation}
                                disabled={isLocating}
                                className={`flex items-center justify-center gap-3 px-6 py-3 rounded-xl font-black uppercase tracking-widest transition-all text-[10px] h-12 min-w-[180px] shadow-lg ${isLocating ? 'bg-slate-800 text-slate-500 cursor-not-allowed' : 'bg-blue-600 text-white hover:bg-blue-500 active:scale-95 shadow-blue-600/20'}`}
                            >
                                {isLocating ? <FiLoader className="animate-spin" /> : <FiMapPin />}
                                {isLocating ? 'Capturando...' : 'Localización GPS'}
                            </button>
                            <div className="flex-1 text-center md:text-left">
                                <p className="text-[9px] text-blue-300/60 font-black uppercase tracking-widest mb-0.5">Metadatos de Ubicación</p>
                                <div className="text-sm font-bold text-white tracking-widest font-mono">
                                    [{pedido.latitud.toFixed(4)}, {pedido.longitud.toFixed(4)}]
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Items Section */}
                    <div className="bg-white/5 backdrop-blur-2xl p-6 md:p-8 rounded-3xl border border-white/10 shadow-2xl">
                        <div className="flex justify-between items-center mb-6">
                            <h3 className="text-lg font-black text-white tracking-tighter uppercase flex items-center gap-2">
                                <FiTag className="text-emerald-500" /> Líneas de Carga
                            </h3>
                            <button
                                type="button"
                                onClick={handleAddRenglon}
                                className="flex items-center gap-2 bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 px-4 py-2 rounded-xl hover:bg-emerald-500/20 transition-all font-black uppercase tracking-widest text-[10px] active:scale-95"
                            >
                                <FiPlus size={14} /> Añadir Artículo
                            </button>
                        </div>

                        <div className="overflow-x-auto">
                            <table className="w-full border-separate border-spacing-y-2">
                                <thead className="text-left">
                                    <tr>
                                        <th className="px-4 py-2 text-[9px] font-black text-slate-600 uppercase tracking-widest">Artículo / SKU</th>
                                        <th className="px-4 py-2 text-[9px] font-black text-slate-600 uppercase tracking-widest">Cant.</th>
                                        <th className="px-4 py-2 text-right text-[9px] font-black text-slate-600 uppercase tracking-widest">P. Unit</th>
                                        <th className="px-4 py-2 text-right text-[9px] font-black text-slate-600 uppercase tracking-widest text-emerald-600">Total</th>
                                        <th className="w-12"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {pedido.renglones.length === 0 && (
                                        <tr>
                                            <td colSpan="5" className="py-12 text-center">
                                                <p className="text-slate-600 font-bold tracking-widest uppercase text-[10px]">No hay artículos en transición</p>
                                            </td>
                                        </tr>
                                    )}
                                    {pedido.renglones.map((r, idx) => (
                                        <tr key={idx} className="bg-white/[0.03] group hover:bg-white/[0.06] transition-all">
                                            <td className="px-4 py-3 rounded-l-2xl border-y border-l border-white/5">
                                                <select
                                                    className="w-full p-1 bg-transparent border-none text-white font-bold outline-none cursor-pointer text-xs"
                                                    value={r.codigoArticulo}
                                                    onChange={e => handleRenglonChange(idx, 'codigoArticulo', e.target.value)}
                                                >
                                                    <option value="" className="bg-slate-900">Seleccionar SKU...</option>
                                                    {productos.map(p => (
                                                        <option key={p.codigo || p.Codigo} value={p.codigo || p.Codigo} className="bg-slate-900">
                                                            {p.descripcion || p.Descripcion}
                                                        </option>
                                                    ))}
                                                </select>
                                            </td>
                                            <td className="px-4 py-3 w-28 border-y border-white/5">
                                                <input
                                                    type="number"
                                                    className="w-full p-1 bg-white/5 border border-white/10 rounded-lg text-center text-white font-black text-xs"
                                                    min="1"
                                                    value={r.cantidad}
                                                    onChange={e => handleRenglonChange(idx, 'cantidad', parseFloat(e.target.value))}
                                                />
                                            </td>
                                            <td className="px-4 py-3 text-right border-y border-white/5">
                                                <span className="text-slate-500 font-mono text-[10px] font-bold">${r.precioConIVA.toFixed(2)}</span>
                                            </td>
                                            <td className="px-4 py-3 text-right border-y border-white/5 text-sm font-black text-emerald-400">
                                                ${(r.totalRenglon || 0).toFixed(2)}
                                            </td>
                                            <td className="px-4 py-3 rounded-r-2xl border-y border-r border-white/5 text-center">
                                                <button onClick={() => handleRemoveRenglon(idx)} type="button" className="p-2 text-red-500/40 hover:text-red-400 hover:bg-red-500/10 rounded-lg transition-all active:scale-90">
                                                    <FiTrash2 size={16} />
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>

                        {/* Summary Section */}
                        <div className="mt-8 pt-6 border-t border-white/10 flex flex-col md:flex-row items-end justify-between gap-6">
                            <div className="w-full md:w-auto space-y-1">
                                <div className="flex justify-between md:justify-start gap-8 text-slate-600 text-[9px] font-black uppercase tracking-widest">
                                    <span>Base Imponible:</span>
                                    <span className="text-white">${(totalCalculado / 1.16).toFixed(2)}</span>
                                </div>
                                <div className="flex justify-between md:justify-start gap-8 text-slate-600 text-[9px] font-black uppercase tracking-widest">
                                    <span>Impuesto IVA:</span>
                                    <span className="text-white">${(totalCalculado - (totalCalculado / 1.16)).toFixed(2)}</span>
                                </div>
                            </div>

                            <div className="flex flex-col items-end gap-5 w-full md:w-auto">
                                <div className="text-right">
                                    <p className="text-[9px] font-black text-emerald-500 uppercase tracking-[0.3em] mb-0.5">Total Liquidación</p>
                                    <div className="text-3xl font-black text-white tracking-tighter">
                                        ${totalCalculado.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                                    </div>
                                </div>

                                <button
                                    type="submit"
                                    disabled={submitting}
                                    className={`group relative flex items-center justify-center gap-3 w-full md:w-[240px] py-4 rounded-2xl text-white font-black text-xs uppercase tracking-[0.2em] shadow-xl transition-all duration-300 active:scale-95 ${submitting ? 'bg-slate-700 cursor-not-allowed opacity-50' : 'bg-gradient-to-r from-emerald-600 to-green-600 hover:from-emerald-500 hover:to-green-500 shadow-emerald-600/10'}`}
                                >
                                    {submitting ? <FiLoader className="animate-spin" /> : <FiSave />}
                                    {submitting ? 'Transmitiendo...' : 'Publicar Orden'}
                                </button>
                            </div>
                        </div>
                    </div>
                </form>

                <footer className="mt-12 py-6 text-center">
                    <p className="text-slate-700 text-[9px] font-black uppercase tracking-[0.4em]">Engine v3.1 • SysPedido Core</p>
                </footer>
            </div>

            <style>{`
                @keyframes fadeIn {
                    from { opacity: 0; transform: translateY(15px); }
                    to { opacity: 1; transform: translateY(0); }
                }
                .animate-fadeIn {
                    animation: fadeIn 0.6s cubic-bezier(0.16, 1, 0.3, 1) forwards;
                }
            `}</style>
        </div>
    );
};

export default CrearPedido;
