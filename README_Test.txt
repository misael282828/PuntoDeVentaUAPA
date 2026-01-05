INSTRUCCIONES DE PRUEBAS MANUALES (UI) - PuntoVentaPOS

Objetivo: Verificar interfaz, navegación en pestañas, comportamiento de formularios y estabilidad visual.

1) Inicio / Login
- Abrir la aplicación.
- Intentar iniciar sesión con credenciales válidas: verificar que se abre el menú principal.
- Intentar iniciar sesión con usuario inválido: comprobar mensaje de error.
- Probar campos vacíos: validar mensajes de validación.

2) Menú principal y pestañas
- Abrir varias opciones del menú (Clientes, Productos, Usuarios, Nueva Factura, Reportes, CxC) y verificar que cada uno abre una pestaña nueva.
- Abrir el mismo módulo dos veces y comprobar que se reutiliza la pestaña existente (no duplica).
- Seleccionar pestañas y comprobar que el contenido visible corresponde al módulo.
- Verificar que el color de fondo es claro (no gris oscuro) y coherrente en todas las pestañas.

3) Clientes (CRUD)
- Abrir "Clientes".
- Crear cliente nuevo: completar campos obligatorios, guardar y verificar que aparece en la lista.
- Editar cliente existente: cambiar nombre/dirección/email, guardar y comprobar cambios.
- Eliminar cliente: confirmar diálogo y verificar eliminación.
- Buscar cliente por texto: probar coincidenci1as parciales y exactas.
- Comprobar que la lista se refresca correctamente tras cada operación.
- Validar mensajes de error/aviso cuando falten datos.

4) Productos (CRUD)
- Abrir "Productos".
- Crear producto nuevo con precio/cantidad; guardar y verificar aparición.
- Editar producto y verificar cambios.
- Eliminar producto (confirmar diálogo).
- Buscar producto por nombre/código.

5) Facturación
- Abrir "Nueva Factura".
- Seleccionar cliente y agregar varios productos (probar cantidad 1 y >1).
- Verificar cálculo de subtotal, impuestos y total.
- Guardar factura y verificar número/registro.
- Abrir "Consultar Facturas", buscar por fecha/cliente y ver detalles.
- Probar anulación (si el rol lo permite) y verificar estado/registro.

6) Cuentas por cobrar (CxC)
- Abrir "Gestion de CxC".
- Ver facturas pendientes, registrar pago parcial y total, verificar saldos.
- Probar validaciones (monto inválido, selección vacía).

7) Reportes y exportación
- Abrir los reportes (Stock, Ventas Diarias).
- Generar reportes con y sin datos; verificar mensajes cuando no hay datos.
- Probar exportar/imprimir si aplica (guardar archivo o mostrar diálogo).

8) Usuarios y permisos
- Abrir "Usuarios" (si el rol es Admin): crear/editar/eliminar usuarios.
- Iniciar sesión con un rol sin permisos y comprobar que los menús restringidos están deshabilitados.

9) Comportamiento de ventanas y pestañas
- Maximizar/restaurar la ventana principal y verificar re-layout de controles.
- Abrir/ cerrar pestañas repetidamente y monitorear que la UI no se rompa.
- Verificar que al cerrar pestañas se libera visualmente el contenido (sin controles huérfanos).

10) Mensajes y diálogos
- Probar todos los diálogos de confirmación y mensajes de error: que sean claros y no bloqueen indefinidamente.
- Verificar que los MessageBox no queden detrás de la ventana principal.

11) Rendimiento básico
- Abrir múltiples pestañas (5-10) y verificar responsividad.
- En tablas grandes (si hay muchos registros), probar ordenamiento/scroll y detectar lags.

12) Persistencia y datos
- Crear varios registros, cerrar la aplicación y volver a iniciar sesión; comprobar que los datos creados persisten.

13) Internacionalización / formatos
- Verificar formatos de fecha/número en facturas y reportes.

14) Estética y accesibilidad
- Comprobar contraste de texto/fondo, legibilidad de botones y campos.
- Verificar que los elementos interactivos son accesibles con teclado (tab, enter, escape).

15) Casos extremos / errores
- Intentar operaciones con datos inválidos (números en campos texto, cadenas muy largas).
- Probar desconexión de BD (si es posible simular) y comprobar manejo de errores.

16) Log out / Cerrar sesión
- Cerrar sesión y verificar que vuelve a la pantalla de login.

17) Comportamiento en segundo plano
- Minimizar la app y volver: verificar estado de pestañas y formularios.

Notas finales:
- Anotar pasos exactos, datos usados (ejemplo: usuario, cliente, producto), y resultados esperados vs reales.
- Si encuentras errores, tomar captura de pantalla y registrar la ruta de la acción.

FIN
