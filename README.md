# 👠 Tienda de Calzado 2025





*   **🛒 Gestión de Ventas:**
    *   Generación de Pedidos y Cotizaciones.
    *   Facturación electrónica (Facturas y Boletas).
    *   Consulta de Catálogo de Productos.

*   **📦 Compras y Proveedores:**
    *   Registro de Órdenes de Compra.
    *   Recepción de mercadería y gestión de Proveedores.
    *   Facturación de Compras.

*   **📊 Inventario y Almacén:**
    *   Control de Stock mediante **Kardex**.
    *   Mantenimiento de Productos y Categorías.
    *   Generación y lectura de **Códigos de Barras** (Integración ZXing).
    *   Gestión de Guías de Remisión.

*   **👥 Administración:**
    *   Mantenimiento de Clientes y Empleados.
    *   Gestión de Accesos y Usuarios (Login/Logout).
    *   Gestión de Sucursales y Almacenes.

*   **📄 Reportes y Atención al Cliente:**
    *   Libro de Reclamaciones virtual.
    *   Reportes de productos.
    *   Integración para envío de mensajes vía **WhatsApp**.

## 🛠️ Tecnologías Utilizadas

Este proyecto ha sido desarrollado utilizando un stack tecnológico robusto y estándar en la industria:

*   **Backend:**
    *   [F_Csharp] C# (.NET Framework 4.7.2)
    *   ASP.NET Web Forms
    *   **Entity Framework 6.2.0** (ORM para acceso a datos)
    *   OWIN (Middleware para autenticación)

*   **Frontend:**
    *   HTML5 & CSS3
    *   **Bootstrap 3.4.1** (Diseño responsivo)
    *   **JavaScript / jQuery**

*   **Base de Datos:**
    *   SQL Server (Estructura relacional y procedimientos almacenados)

*   **Librerías Adicionales:**
    *   `ZXing.Net`: Para generación de códigos de barras.
    *   `Newtonsoft.Json`: Manejo de datos JSON.

## 🗄️ Base de Datos

El sistema utiliza una base de datos relacional robusta. A continuación se muestra el diagrama de la base de datos:


## ⚙️ Instalación y Configuración

Sigue estos pasos para configurar el proyecto en tu entorno local:

1.  **Clonar el Repositorio:**
    ```bash
    git clone https://github.com/jhontorres15/Tienda_Calzado_2025.git
    ```

2.  **Base de Datos:**
    *   Crea una base de datos en SQL Server.
    *   Ejecuta los scripts de tablas y procedimientos almacenados que se encuentran en el proyecto.
    *   Actualiza la cadena de conexión (`connectionStrings`) en el archivo `Web.config` con tus credenciales locales.

3.  **Ejecutar la Aplicación:**
    *   Abre el archivo de solución `.sln` en **Visual Studio** (2019 o superior recomendado).
    *   Restaura los paquetes NuGet (se debería hacer automáticamente al compilar).
    *   Presiona `F5` o ejecuta el proyecto en tu navegador preferido.

## 🤝 Contribución

Si deseas contribuir a este proyecto, por favor crea un *fork* del repositorio y envía un *pull request* con tus mejoras.

## ✒️ Autores

*   **jhontorres15** - *Desarrollo Inicial*
