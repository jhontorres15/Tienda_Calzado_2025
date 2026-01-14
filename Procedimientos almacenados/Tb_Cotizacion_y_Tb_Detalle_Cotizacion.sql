
CREATE PROCEDURE USP_Generar_NroCotizacion
@NroCotizacion CHAR(10) OUTPUT
AS
BEGIN
   -- Declarar variables auxiliares
   DECLARE @Total INT
   DECLARE @Max INT

   -- Obtener el total de registros en la tabla Tb_Cotizacion
   SELECT @Total = COUNT(NroCotizacion) FROM Tb_Cotizacion

   -- Evaluar si el total de registros es cero
   IF (@Total = 0)
   BEGIN
       -- Establecer el primer número de cotización
       SET @NroCotizacion = 'COT0000001'
   END
   ELSE
   BEGIN
       -- Obtener el valor numérico máximo de la parte derecha
       SELECT @Max = MAX(CAST(RIGHT(NroCotizacion, 7) AS INT)) + 1
       FROM Tb_Cotizacion
       
       -- Generar nuevo código con ceros a la izquierda
       IF (@Max BETWEEN 1 AND 9)
           SET @NroCotizacion = 'COT000000' + CONVERT(CHAR(1), @Max)
       ELSE IF (@Max BETWEEN 10 AND 99)
           SET @NroCotizacion = 'COT00000' + CONVERT(CHAR(2), @Max)
       ELSE IF (@Max BETWEEN 100 AND 999)
           SET @NroCotizacion = 'COT0000' + CONVERT(CHAR(3), @Max)
       ELSE IF (@Max BETWEEN 1000 AND 9999)
           SET @NroCotizacion = 'COT000' + CONVERT(CHAR(4), @Max)
       ELSE IF (@Max BETWEEN 10000 AND 99999)
           SET @NroCotizacion = 'COT00' + CONVERT(CHAR(5), @Max)
       ELSE IF (@Max BETWEEN 100000 AND 999999)
           SET @NroCotizacion = 'COT0' + CONVERT(CHAR(6), @Max)
       ELSE IF (@Max BETWEEN 1000000 AND 9999999)
           SET @NroCotizacion = 'COT' + CONVERT(CHAR(7), @Max)
   END

   -- Mostrar el código generado
   PRINT @NroCotizacion
END
GO


CREATE PROCEDURE USP_Listar_Proveedor
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CodProveedor,
	    Apellidos + ' ' + Nombres as nombre
    FROM Tb_Proveedor;
END
GO



CREATE TYPE TipoDetalleCotizacion AS TABLE
(
    CodSucursal CHAR(5),
    NroSerie_Producto CHAR(7),
    Prec_Compra NUMERIC(10,2),
    Cantidad INT,
    Importe NUMERIC(10,2),
    Porcentaje_Dscto NUMERIC(10,2),
    Dscto NUMERIC(10,2),
    SubTotal NUMERIC(10,2)
);
GO



CREATE PROCEDURE dbo.USP_Cotizacion_Guardar
    @NroCotizacion CHAR(10),
    @CodEmpleado CHAR(5),
    @CodProveedor CHAR(6),
    @Fec_Cotizacion DATE,
    @Total NUMERIC(10,2),
    @Estado_Cotizacion VARCHAR(20),
    @Detalle dbo.TipoDetalleCotizacion READONLY,
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS(SELECT 1 FROM Tb_Cotizacion WHERE NroCotizacion = @NroCotizacion)
        BEGIN
            SET @Mensaje = 'Error: El Nro de Cotización ya existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        INSERT INTO Tb_Cotizacion(NroCotizacion, CodEmpleado, CodProveedor, Fec_Cotizacion, Total, Estado_Cotizacion)
        VALUES(@NroCotizacion, @CodEmpleado, @CodProveedor, @Fec_Cotizacion, @Total, @Estado_Cotizacion);

        INSERT INTO Tb_Detalle_Cotizacion(
            NroCotizacion, CodSucursal, NroSerie_Producto, Prec_Compra, Cantidad, Importe, Porcentaje_Dscto, Dscto, SubTotal
        )
        SELECT @NroCotizacion, CodSucursal, NroSerie_Producto, Prec_Compra, Cantidad, Importe, Porcentaje_Dscto, Dscto, SubTotal
        FROM @Detalle;

        COMMIT TRANSACTION;
        SET @Mensaje = 'OK: Cotización registrada';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO


CREATE PROCEDURE dbo.USP_Listar_Cotizaciones
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT NroCotizacion, CodProveedor, CodEmpleado, Fec_Cotizacion, Total, Estado_Cotizacion
    FROM Tb_Cotizacion
    WHERE (@Filtro = '' OR NroCotizacion LIKE '%' + @Filtro + '%' OR CodProveedor LIKE '%' + @Filtro + '%')
    ORDER BY Fec_Cotizacion DESC;
END
GO

ALTER PROCEDURE dbo.USP_Listar_Cotizaciones
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.NroCotizacion,
        
        -- Traemos el nombre del proveedor (ajusta 'RazonSocial' si tu columna se llama 'Nombre')
        P.RazonSocial AS Proveedor, 
        
        -- Concatenamos el nombre completo del empleado
        (E.Nombres + ' ' + E.Apellidos) AS Empleado, 
        
        C.Fec_Cotizacion, 
        C.Total, 
        C.Estado_Cotizacion
    FROM Tb_Cotizacion C
    -- Unimos con la tabla de Proveedores
    INNER JOIN Tb_Proveedor P ON C.CodProveedor = P.CodProveedor
    -- Unimos con la tabla de Empleados
    INNER JOIN Tb_Empleado E ON C.CodEmpleado = E.CodEmpleado
    WHERE 
        (@Filtro = '' OR 
         C.NroCotizacion LIKE '%' + @Filtro + '%' OR 
         P.RazonSocial LIKE '%' + @Filtro + '%') -- Buscamos también por nombre de proveedor
    ORDER BY C.Fec_Cotizacion DESC;
END
GO



CREATE PROCEDURE dbo.USP_Cotizacion_ObtenerDatos
    @NroCotizacion CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT NroCotizacion, CodEmpleado, CodProveedor, Fec_Cotizacion, Total, Estado_Cotizacion
    FROM Tb_Cotizacion WHERE NroCotizacion = @NroCotizacion;

    SELECT CodSucursal, NroSerie_Producto, Prec_Compra, Cantidad, Importe, Porcentaje_Dscto, Dscto, SubTotal
    FROM Tb_Detalle_Cotizacion WHERE NroCotizacion = @NroCotizacion;
END
GO



ALTER PROCEDURE USP_Buscar_Producto_Cotizacion
    @SucursalNombre VARCHAR(100), 
    @Marca VARCHAR(100),
    @Modelo VARCHAR(100),
    @Talla VARCHAR(20),
    @Color VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Buscar código de sucursal
    DECLARE @CodSucursal CHAR(5);
    SELECT @CodSucursal = CodSucursal 
    FROM Tb_Sucursal 
    WHERE RTRIM(LTRIM(Sucursal)) = RTRIM(LTRIM(@SucursalNombre));

    -- Si no encontramos sucursal, igual devolvemos estructuras vacías
    IF @CodSucursal IS NULL
    BEGIN
        SELECT 1 WHERE 1=0; SELECT 1 WHERE 1=0; SELECT 1 WHERE 1=0; SELECT 1 WHERE 1=0; SELECT 1 WHERE 1=0;
        RETURN;
    END

    -- 1. Crear tabla temporal con los datos del CATÁLOGO DE PRODUCTOS
    SELECT
        -- Datos del Producto (Tabla Principal ahora es P)
        P.CodProducto,
        P.Producto,
         (P.Producto + ' - T:' + CAST(T.Talla AS VARCHAR(10)) + ' - C:' + P.Color) AS ProductoDescripcion,
        -- Concatenado útil para que el comprador identifique el producto
        (P.Producto + ' - ' + M.Marca + ' - ' + MOD.Modelo) AS NombreComercial,
       
       
        -- Datos para referencia del comprador
        M.Marca,
        MOD.Modelo,
        T.Talla,
        P.Color,
        
        -- STOCK ACTUAL (Referencial para Compras)
        -- Usamos LEFT JOIN: Si no existe en el almacén, mostramos 0
        ISNULL(A.Stock_Actual, 0) AS Stock_Actual,
        
        -- ID de Serie (Puede ser NULL si el producto es nuevo en esta sucursal)
        ISNULL(A.NroSerie_Producto, '') AS NroSerie_Producto,
        
        -- Precios de Venta referenciales (para saber margen)
        P.Prec_Venta_Menor,
        P.Prec_Venta_Mayor

    INTO
        #ProductosFiltrados
    FROM
        Tb_Producto AS P  -- <--- CAMBIO IMPORTANTE: La base es el catálogo completo
    INNER JOIN
        Tb_Modelo AS MOD ON P.CodModelo = MOD.CodModelo
    INNER JOIN
        Tb_Marca AS M ON MOD.CodMarca = M.CodMarca
    INNER JOIN
        Tb_Talla AS T ON P.CodTalla = T.CodTalla
    LEFT JOIN  -- <--- LEFT JOIN: Trae el stock SI EXISTE en esa sucursal, sino trae NULL
        Tb_Almacen_Sucursal AS A 
        ON P.CodProducto = A.CodProducto AND A.CodSucursal = @CodSucursal
    WHERE
        P.Estado_Producto = 'Disponible' -- Opcional: Solo productos activos
        AND (@Marca = '' OR M.Marca = @Marca)
        AND (@Modelo = '' OR MOD.Modelo = @Modelo)
        AND (@Talla = '' OR T.Talla = @Talla)
        AND (@Color = '' OR P.Color = @Color);

    
    -- Resultado 1: GridView Principal
    SELECT 
        CodProducto,            -- Clave principal para compras
        NroSerie_Producto,      -- Referencia (puede estar vacío)
        Producto,               
        ProductoDescripcion,   
        NombreComercial,        
        Stock_Actual,           -- Ahora verás 0 si no hay stock
        Prec_Venta_Menor, 
        Prec_Venta_Mayor,
        Marca,
        Modelo,
        Talla,
        Color
    FROM #ProductosFiltrados
    ORDER BY Producto;

    -- Resultado 2: Filtros (Marcas)
    SELECT DISTINCT Marca FROM #ProductosFiltrados ORDER BY Marca;

    -- Resultado 3: Filtros (Modelos)
    SELECT DISTINCT Modelo FROM #ProductosFiltrados ORDER BY Modelo;

    -- Resultado 4: Filtros (Tallas)
    SELECT DISTINCT Talla FROM #ProductosFiltrados ORDER BY Talla;

    -- Resultado 5: Filtros (Colores)
    SELECT DISTINCT Color FROM #ProductosFiltrados ORDER BY Color;

    DROP TABLE #ProductosFiltrados;
END
GO

