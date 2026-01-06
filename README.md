# PuntoDeVentaUAPA

Descripción
-
PuntoDeVentaUAPA es una aplicación de punto de venta (POS) de escritorio desarrollada en .NET (WinForms) para gestionar operaciones de venta, clientes, productos, facturación, cuentas por cobrar y reportes básicos. La interfaz principal agrupa los módulos dentro de pestañas para una experiencia más integrada.

Características principales
-
- Gestión de clientes (CRUD).
- Gestión de productos (CRUD) y control de inventario.
- Emisión de facturas con detalle de productos.
- Consulta y anulación de facturas.
- Gestión de cuentas por cobrar (CxC).
- Reportes: stock y ventas diarias.
- Autenticación de usuarios y roles básicos.

Estructura y módulos
-
- `PuntoVentaPOS/Forms` : Formularios WinForms (Main, Login, Clientes, Productos, Factura, Reportes, etc.).
- `PuntoVentaPOS/Services` : Lógica de acceso a datos y operación (ClientesService, ProductosService, FacturasService, CxcService, ReportesService, UsuariosService, AuthService).
- `PuntoVentaPOS/Models` : Entidades del dominio (Cliente, Producto, Factura, FacturaDetalle, Usuario, CuentaPorCobrar, etc.).
- `PuntoVentaPOS/Data/Db.cs` : Conexión/abstracción de base de datos (MySQL).

Tecnologías, herramientas y versiones
-
- Plataforma: .NET 8.0 (target `net8.0-windows`).
- Lenguaje: C# (versión acorde a .NET 8 / C# 11).
- UI: Windows Forms (WinForms).
- Base de datos: MySQL (se utiliza `MySqlConnector` en el proyecto).
- Librerías: `MySqlConnector`, `Microsoft.Extensions.Logging.*` y utilidades estándar de .NET.
- SDK / herramientas recomendadas para desarrollo:
  - .NET SDK 8.x
  - Visual Studio 2022/2023 (con workload de .NET Desktop) o Visual Studio Code con extensiones C# y soporte para MSBuild

Entorno de ejecución y compilación
-
1. Abrir la solución `PuntoVentaPOS.sln` en Visual Studio o en un editor con soporte .NET.
2. Configurar la cadena de conexión a MySQL en la sección correspondiente (`PuntoVentaPOS/Data/Db.cs` o `AppConfig.cs`).
3. Restaurar dependencias y compilar:

```bash
dotnet build PuntoVentaPOS.sln
```

4. Ejecutar (desde Visual Studio use Start; desde CLI puede probar):

```bash
dotnet run --project PuntoVentaPOS --configuration Debug
```

Notas de base de datos
-
- El proyecto asume una base de datos MySQL. Debe crear la base y las tablas necesarias (no incluye un script de migración automatizado). Revise los servicios en `PuntoVentaPOS/Services` para identificar las tablas y campos esperados y prepare el esquema apropiado.

Mercado objetivo
-
PuntoDeVentaUAPA está pensado para negocios minoristas y micro/pequeñas empresas: tiendas físicas, panaderías, farmacias pequeñas, boutiques y comercios en general que requieren:
- Gestión simple de clientes y productos.
- Emisión de facturas y control básico de cuentas por cobrar.
- Reportes de stock y ventas diarias.

Optimizado para entornos Windows de PyMEs, especialmente mercados hispanohablantes (UI en español).

Escalabilidad y límites actuales
-
Limitaciones del diseño actual:
- Aplicación de escritorio monolítica (WinForms): escalabilidad horizontal limitada; no es multi-instancia con sincronización automática.
- Persistencia directa en MySQL sin capa de API REST intermedia.
- Artefactos compilados (`bin/` y `obj/`) no deben versionarse en un repo remoto.

Estrategias de escalado:
- Extraer la capa de datos y lógica de negocio a una API (Web API) para permitir clientes múltiples (web, móvil, escritorio).
- Migrar a una arquitectura basada en servicios o microservicios para componentes críticos (facturación, inventario, autenticación).
- Usar una base de datos gestionada (Cloud SQL / RDS) y backups automáticos.
- Contenerizar la app (Docker) para despliegue reproducible y facilitar CI/CD.
- Añadir un mecanismo de sincronización/offline si se requieren múltiples puntos de venta desconectados.

Ideas y mejoras futuras (roadmap sugerido)
-
- Mínimo viable (próximos pasos):
  - Añadir `.gitignore` para excluir `bin/`/`obj/` y archivos de usuario.
  - Centralizar helpers de UI (tema, colores, utilidades de DataGridView) para mantenimiento.
  - Corregir advertencias de nullability (CS8602) y añadir validaciones robustas.

- Funcionalidades de producto medio plazo:
  - Integración con impresoras de ticket y generación de recibos (PDF / ESC/POS).
  - Escaneo de código de barras y lector USB.
  - Reglas de inventario (alertas por bajo stock, reorder points).
  - Reportes avanzados (ventas por producto, márgenes, ventas por portal/usuario).
  - Control de sesiones concurrentes y bloqueo optimista para evitar inconsistencias.

- Escalado avanzado / Emprendimiento:
  - Exponer una API REST para integraciones (contabilidad, e-commerce, aplicaciones móviles).
  - Implementar autenticación centralizada (JWT / OAuth2) y roles más granulados.
  - Multi-tenant para ofrecer la solución como servicio SaaS.
  - Migración de cliente a una front-end web moderna (React / Blazor) o app móvil multiplataforma.

Calidad y operativa de desarrollo
-
- Añadir suite de pruebas unitarias e integración para la lógica en `Services`.
- Integrar CI (GitHub Actions / Azure Pipelines) para compilar y ejecutar pruebas automáticamente.
- Mantener un proceso de revisiones de código (pull requests) y convenciones de commits.

Contribuir
-
- Fork + branch feature/bugfix, pruebas locales, PR hacia `main`.
- Mantener commits pequeños y descriptivos.

Contacto y licenciamiento
-
- Este repositorio no incluye un archivo de licencia por defecto; agregar una licencia (MIT, Apache-2.0, etc.) según prefieras.
- Para preguntas y coordinación: contactar al mantenedor del repositorio.

-----------
Notas finales
-
He documentado la información central que necesitabas: funcionalidad, stack, instrucciones rápidas de build/run, mercado objetivo, opciones de escalado y roadmap de mejoras. Si quieres, puedo:
- Añadir scripts de inicialización de base de datos si compartes el esquema.
- Generar un `.gitignore` y limpiar `bin/` y `obj/` del repo y commitearlo.
- Traducir o resumir el README en inglés.
