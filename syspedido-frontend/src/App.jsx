import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './auth/AuthContext';
import Login from './pages/Login';
import Pedidos from './pages/Pedidos';
import Dashboard from './pages/Dashboard';
import CrearPedido from './pages/CrearPedido';
import './App.css'; // Mantenemos tu estilos

const PrivateRoute = ({ children }) => {
  const { token } = useAuth();
  return token ? children : <Navigate to="/login" />;
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/dashboard" element={
            <PrivateRoute>
              <Dashboard />
            </PrivateRoute>
          } />
          <Route path="/pedidos" element={
            <PrivateRoute>
              <Pedidos />
            </PrivateRoute>
          } />
          <Route path="/pedidos/nuevo" element={
            <PrivateRoute>
              <CrearPedido />
            </PrivateRoute>
          } />
          <Route path="*" element={<Navigate to="/dashboard" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
