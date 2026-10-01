# SysPedido

**SysPedido** es una solución integral y moderna (Full Stack) para la gestión, emisión y seguimiento de pedidos, cotizaciones y ventas en tiempo real, diseñada con arquitectura limpia, soporte multitenant (multi-empresa) y geolocalización de pedidos en ruta.

---

## 🎯 Objetivo y Descripción del Proyecto

> **SysPedido** es una plataforma integral para la gestión comercial y automatización de la fuerza de ventas. Su objetivo principal es permitir la captura, administración, trazabilidad y facturación de pedidos y cotizaciones tanto en oficina como en ruta de campo. Incorpora geolocalización satelital para registrar la ubicación exacta de cada transacción, un esquema multi-empresa (Multi-Tenant) para aislar los datos entre diferentes organizaciones y tableros con métricas en tiempo real.

### Principales Funcionalidades
- **Autenticación y Control de Acceso:** Login seguro multi-empresa con roles y sesiones persistentes vía JWT.
- **Gestión Integral de Pedidos y Cotizaciones:** Creación, desglose por renglones, cálculo automático de impuestos/totales en moneda local o extranjera y conversión de cotizaciones a pedidos.
- **Geolocalización en Campo:** Captura automática de coordenadas (latitud/longitud) y visualización en mapas interactivos al momento del despacho o toma del pedido por el vendedor.
- **Panel de Control (Dashboard):** Métricas y estadísticas comerciales interactivas para análisis de ventas y rendimiento en tiempo real.
- **Arquitectura Multi-tenant:** Aislamiento y segmentación segura de datos por empresa.

---

## 🚀 Tecnologías Utilizadas

### **Backend**
- **Plataforma y Lenguaje:** [.NET 9](https://dotnet.microsoft.com/) con **C#**.
- **Arquitectura Limpia (Clean Architecture):** Separación estricta de responsabilidades en capas:
  - `SysPedido.API`: Endpoints RESTful con ASP.NET Core y documentación interactiva con Swagger/OpenAPI.
  - `SysPedido.Application`: Casos de uso, servicios y DTOs de negocio.
  - `SysPedido.Domain`: Entidades del dominio, reglas e interfaces de tenencia múltiple (`IMultiTenantEntity`).
  - `SysPedido.Infrastructure`: Persistencia con Entity Framework Core, repositorios y servicios externos.
  - `SysPedido.CrossCutting`: Utilidades transversales y seguridad.
- **Base de Datos:** Microsoft SQL Server gestionado a través de **Entity Framework Core 9** (Code-First / Migrations).
- **Seguridad y Autenticación:** JWT (*JSON Web Tokens*) mediante `Microsoft.AspNetCore.Authentication.JwtBearer` y hashing de contraseñas con `BCrypt.Net-Next`.
- **Comunicación en Tiempo Real:** **ASP.NET Core SignalR** (`PedidoHub`) para notificaciones y sincronización de pedidos en vivo.

### **Frontend**
- **Framework & Bundler:** **React 19** empaquetado y servido con **Vite**.
- **Diseño & Estilos:** **TailwindCSS** + PostCSS para interfaces responsivas, modernas y modulares.
- **Enrutamiento:** `react-router-dom` (v7).
- **Geolocalización & Mapas:** **Leaflet** y **React-Leaflet** para geolocalizar y visualizar la ubicación GPS exacta donde se levantan los pedidos.
- **Gráficos & Dashboard:** **Recharts** para métricas comerciales y gráficos analíticos.
- **HTTP & Estado:** **Axios** con interceptores para inyección automática de tokens JWT.
- **Feedback al Usuario:** `react-hot-toast` para notificaciones emergentes y `react-icons` para iconografía.

---

## 🛠️ Estructura del Repositorio

```plaintext
SysPedido/
├── SysPedido.Backend/          # Solución .NET 9
│   ├── SysPedido.API/          # Controladores y Endpoints REST
│   ├── SysPedido.Application/  # Lógica de aplicación
│   ├── SysPedido.Domain/       # Entidades y reglas de negocio
│   ├── SysPedido.Infrastructure/# DbContext y persistencia SQL Server
│   └── SysPedido.CrossCutting/ # Componentes transversales
├── syspedido-frontend/         # Single Page Application (React 19 + Vite)
│   ├── src/
│   │   ├── api/                # Cliente Axios y configuración de llamadas HTTP
│   │   ├── auth/               # Contexto y gestión de autenticación
│   │   ├── components/         # Componentes UI reutilizables
│   │   ├── pages/              # Vistas (Dashboard, Pedidos, CrearPedido, Login)
│   │   └── routes/             # Enrutamiento de la aplicación
└── setup_db.sql                # Scripts de base de datos
```

---

## 💻 Instrucciones para Ejecución Local

### Prerrequisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Node.js](https://nodejs.org/) (versión 18 o superior)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server/)

### 1. Backend
```bash
cd SysPedido.Backend/SysPedido.API
dotnet restore
dotnet run
```
La API estará disponible por defecto en el puerto configurado en `launchSettings.json`.

### 2. Frontend
```bash
cd syspedido-frontend
npm install
npm run dev
```
La aplicación web se ejecutará en `http://localhost:5173`.