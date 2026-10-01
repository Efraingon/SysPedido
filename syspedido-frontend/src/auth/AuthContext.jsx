import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../api/axiosConfig';
import {jwtDecode} from 'jwt-decode';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [user, setUser] = useState(null);

  useEffect(() => {
    if (token) {
      try {
        const decoded = jwtDecode(token);
        setUser({
          username: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
          role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
          empresaId: decoded["EmpresaId"],
          empresaNombre: decoded["EmpresaNombre"]
        });
      } catch (err) {
        console.error("Token inválido", err);
        logout();
      }
    } else {
      setUser(null);
    }
  }, [token]);

  const login = async (username, password) => {
    try {
      const res = await api.post('/Auth/login', { username, password });
      if (res.data.token) {
        setToken(res.data.token);
        localStorage.setItem('token', res.data.token);
        return true;
      }
    } catch(err) {
      return false;
    }
    return false;
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('token');
  };

  return (
    <AuthContext.Provider value={{ token, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
