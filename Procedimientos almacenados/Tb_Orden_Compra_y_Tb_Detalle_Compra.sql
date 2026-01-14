

--PROCEDIMIENTOS ALMACENADOS PARA Tb_Orden_Compra Y Tb_Detalle_Compra

CREATE PROCEDURE USP_Generar_NroCompra
@NroCompra CHAR(10) OUTPUT
AS
BEGIN
    -- Declarar variables auxiliares
    DECLARE @Total INT
    DECLARE @Max INT

    -- Obtener el total de registros en la tabla Tb_Orden_Compra
    SELECT @Total = COUNT(*) FROM Tb_Orden_Compra

    -- Evaluar si el total de registros es cero
    IF (@Total = 0)
    BEGIN
        -- Establecer el primer Número de Compra (Prefijo COM + 7 dígitos)
        SET @NroCompra = 'COM0000001'
    END
    ELSE
    BEGIN
        -- Obtener el valor máximo numérico actual (tomando los 7 de la derecha) y sumar 1
        SELECT @Max = MAX(CAST(RIGHT(NroCompra, 7) AS INT)) + 1 
        FROM Tb_Orden_Compra
        
        -- Generar el nuevo código con ceros a la izquierda según el valor
        IF (@Max BETWEEN 1 AND 9)
            SET @NroCompra = 'COM000000' + CONVERT(CHAR(1), @Max)
        ELSE IF (@Max BETWEEN 10 AND 99)
            SET @NroCompra = 'COM00000' + CONVERT(CHAR(2), @Max)
        ELSE IF (@Max BETWEEN 100 AND 999)
            SET @NroCompra = 'COM0000' + CONVERT(CHAR(3), @Max)
        ELSE IF (@Max BETWEEN 1000 AND 9999)
            SET @NroCompra = 'COM000' + CONVERT(CHAR(4), @Max)
        ELSE IF (@Max BETWEEN 10000 AND 99999)
            SET @NroCompra = 'COM00' + CONVERT(CHAR(5), @Max)
        ELSE IF (@Max BETWEEN 100000 AND 999999)
            SET @NroCompra = 'COM0' + CONVERT(CHAR(6), @Max)
        ELSE IF (@Max BETWEEN 1000000 AND 9999999)
            SET @NroCompra = 'COM' + CONVERT(CHAR(7), @Max)
    END

    -- Mostrar el código generado
    PRINT @NroCompra
END
GO




CREATE TYPE dbo.TipoDetalleCompra AS TABLE
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



CREATE PROCEDURE dbo.USP_Orden_Compra_Guardar
    @NroCompra CHAR(10),
    @CodEmpleado CHAR(5),
    @NroCotizacion CHAR(10),
    @Fec_Compra DATETIME,
    @Total NUMERIC(10,2),
    @Estado_Compra VARCHAR(20),
    @Detalle dbo.TipoDetalleCompra READONLY,
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
        BEGIN
            SET @Mensaje = 'Error: El código de empleado no existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM Tb_Cotizacion WHERE NroCotizacion = @NroCotizacion)
        BEGIN
            SET @Mensaje = 'Error: La cotización indicada no existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        IF EXISTS(SELECT 1 FROM Tb_Orden_Compra WHERE NroCompra = @NroCompra)
        BEGIN
            SET @Mensaje = 'Error: El Nro de Compra ya existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        INSERT INTO Tb_Orden_Compra(NroCompra, CodEmpleado, NroCotizacion, Fec_Compra, Total, Estado_Compra)
        VALUES(@NroCompra, @CodEmpleado, @NroCotizacion, @Fec_Compra, @Total, @Estado_Compra);

        INSERT INTO Tb_Detalle_Compra(
            NroCompra, CodSucursal, NroSerie_Producto, Prec_Compra, Cantidad, Importe, Porcentaje_Dscto, Dscto, SubTotal
        )
        SELECT @NroCompra, CodSucursal, NroSerie_Producto, Prec_Compra, Cantidad, (Prec_Compra * Cantidad), Porcentaje_Dscto, Dscto, SubTotal
        FROM @Detalle;

        COMMIT TRANSACTION;
        SET @Mensaje = 'OK: Orden registrada';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO



CREATE PROCEDURE dbo.USP_Listar_Orden_Compra
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        OC.NroCompra,
        OC.NroCotizacion,
        
        -- 1. Concatenamos Apellido y Nombre del Empleado
        E.Apellido + ', ' + E.Nombre AS NombreEmpleado,
        
        -- 2. Obtenemos la Razón Social de la Empresa Proveedora
        EP.Razon_Social AS EmpresaProveedor,
        
        OC.Fec_Compra,
        OC.Total,
        OC.Estado_Compra
    FROM Tb_Orden_Compra OC
    -- Unión para sacar datos del Empleado
    INNER JOIN Tb_Empleado E ON OC.CodEmpleado = E.CodEmpleado
    -- Unión con Cotización (Puente hacia el proveedor)
    INNER JOIN Tb_Cotizacion C ON OC.NroCotizacion = C.NroCotizacion
    -- Unión con Proveedor (Contacto)
    INNER JOIN Tb_Proveedor P ON C.CodProveedor = P.CodProveedor
    -- Unión con Empresa Proveedora (Para sacar la Razón Social)
    INNER JOIN Tb_Empresa_Proveedor EP ON P.CodEmpresa = EP.CodEmpresa
    WHERE 
        (@Filtro = '' OR 
         OC.NroCompra LIKE '%' + @Filtro + '%' OR 
         OC.NroCotizacion LIKE '%' + @Filtro + '%' OR
         EP.Razon_Social LIKE '%' + @Filtro + '%') -- Agregado: Buscar por nombre de empresa
    ORDER BY OC.Fec_Compra DESC;
END
GO



ALTER PROCEDURE USP_CargarDetalle_DesdeCotizacion_Compra
    @NroCotizacion CHAR(10),
    @CodSucursal CHAR(5),
    @MensajeAviso VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NroCotizacion = RTRIM(LTRIM(@NroCotizacion))

    -- =======================================================
    -- 1. VALIDAR SI EXISTE LA COTIZACIÓN
    -- =======================================================
    IF NOT EXISTS (SELECT 1 FROM Tb_Cotizacion WHERE RTRIM(NroCotizacion) = @NroCotizacion)
    BEGIN
        SET @MensajeAviso = 'ERROR: No se encontró la Cotización Nro ' + @NroCotizacion
        RETURN
    END

    -- =======================================================
    -- 2. VALIDAR ESTADO DE LA EMPRESA (A TRAVÉS DEL PROVEEDOR)
    -- =======================================================
    DECLARE @EstadoEmpresa VARCHAR(20)
    DECLARE @RazonSocial VARCHAR(100)
    DECLARE @NombreProveedor VARCHAR(100)

    SELECT 
        @EstadoEmpresa = E.Estado_Empresa,
        @RazonSocial = E.Razon_Social,
        @NombreProveedor = P.Apellidos + ', ' + P.Nombres
    FROM Tb_Cotizacion C
    -- 1. Unimos Cotización con el Proveedor (Persona) -> CHAR(6) con CHAR(6)
    INNER JOIN Tb_Proveedor P ON C.CodProveedor = P.CodProveedor
    -- 2. Unimos Proveedor con la Empresa -> CHAR(5) con CHAR(5)
    INNER JOIN Tb_Empresa_Proveedor E ON P.CodEmpresa = E.CodEmpresa
    WHERE RTRIM(C.NroCotizacion) = @NroCotizacion

    -- Si no se pudo hacer el cruce (quizás el proveedor existe pero no tiene empresa asignada o el código está mal)
    IF @EstadoEmpresa IS NULL
    BEGIN
        SET @MensajeAviso = 'ERROR: La cotización existe, pero no se pudo vincular con una Empresa Proveedora válida (Revisar Tb_Proveedor).'
        RETURN
    END

    -- Validamos si la EMPRESA está activa
    IF @EstadoEmpresa <> 'Activo'
    BEGIN
        SET @MensajeAviso = 'ERROR: La empresa ' + @RazonSocial + ' (del proveedor ' + @NombreProveedor + ') no está ACTIVA.'
        RETURN
    END

    SET @MensajeAviso = 'OK'

    -- =======================================================
    -- 3. RESULTSET 1: CABECERA (DATOS DE LA EMPRESA PARA LA ORDEN)
    -- =======================================================
    SELECT 
        C.NroCotizacion,
        C.CodProveedor, -- Aquí va el código de la persona (PRV001)
        E.CodEmpresa,   -- Aquí va el código de la empresa (EP001)
        E.Razon_Social AS ProveedorNombre, -- Mostramos el nombre de la empresa
        P.Apellidos + ' ' + P.Nombres AS Contacto, -- Dato extra útil
        C.Total AS TotalCotizado,
        C.CodEmpleado
    FROM Tb_Cotizacion C
    INNER JOIN Tb_Proveedor P ON C.CodProveedor = P.CodProveedor
    INNER JOIN Tb_Empresa_Proveedor E ON P.CodEmpresa = E.CodEmpresa
    WHERE RTRIM(C.NroCotizacion) = @NroCotizacion;

    -- =======================================================
    -- 4. RESULTSET 2: DETALLE (PRODUCTOS) - ESTO SE MANTIENE IGUAL
    -- =======================================================
    SELECT 
        DC.CodSucursal,
        P.Producto + ' - ' + M.Modelo + ' - ' + P.Color AS Producto,
        P.Descripcion_Producto AS ProductoDescripcion,
        DC.NroSerie_Producto,
        DC.Cantidad,
        DC.Prec_Compra,
        DC.Importe,
        DC.Porcentaje_Dscto,
        DC.Dscto,
        DC.SubTotal
    FROM Tb_Detalle_Cotizacion DC
    INNER JOIN Tb_Almacen_Sucursal A ON DC.NroSerie_Producto = A.NroSerie_Producto 
        AND A.CodSucursal = DC.CodSucursal 
    INNER JOIN Tb_Producto P ON A.CodProducto = P.CodProducto 
    INNER JOIN Tb_Modelo M ON P.CodModelo = M.CodModelo
    INNER JOIN Tb_Talla T ON P.CodTalla = T.CodTalla
    WHERE RTRIM(DC.NroCotizacion) = @NroCotizacion
      AND DC.CodSucursal = @CodSucursal
END
GO