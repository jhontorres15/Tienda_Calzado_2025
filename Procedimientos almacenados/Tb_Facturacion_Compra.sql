
IF OBJECT_ID(N'dbo.USP_Generar_NroSerie_FactCompra', N'P') IS NOT NULL DROP PROCEDURE dbo.USP_Generar_NroSerie_FactCompra;
GO
CREATE PROCEDURE dbo.USP_Generar_NroSerie_FactCompra
    @NroSerie_Facturacion CHAR(10) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Next INT = ISNULL((SELECT MAX(CAST(SUBSTRING(NroSerie_Facturacion, 4, 7) AS INT)) FROM Tb_Facturacion_Compra), 0) + 1;
    SET @NroSerie_Facturacion = 'FCO' + RIGHT('0000000' + CAST(@Next AS VARCHAR(7)), 7);
END
GO

IF OBJECT_ID(N'dbo.USP_Facturacion_Compra_Guardar', N'P') IS NOT NULL DROP PROCEDURE dbo.USP_Facturacion_Compra_Guardar;
GO
CREATE PROCEDURE dbo.USP_Facturacion_Compra_Guardar
    @NroSerie_Facturacion CHAR(10),
    @CodEmpresa CHAR(5),
    @CodTipo_Facturacion CHAR(3),
    @NroCompra CHAR(10),
    @Fec_Emision DATE,
    @SubTotal NUMERIC(10,2),
    @IGV NUMERIC(10,2),
    @Total NUMERIC(10,2),
    @Estado_Facturacion VARCHAR(20),
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Mensaje = '';
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS(SELECT 1 FROM Tb_Facturacion_Compra WHERE NroSerie_Facturacion = @NroSerie_Facturacion)
        BEGIN
            SET @Mensaje = 'Error: El Nro de Serie ya existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM Tb_Orden_Compra WHERE NroCompra = @NroCompra)
        BEGIN
            SET @Mensaje = 'Error: La orden de compra no existe.';
            ROLLBACK TRANSACTION; RETURN;
        END

        INSERT INTO Tb_Facturacion_Compra(NroSerie_Facturacion, CodEmpresa, CodTipo_Facturacion, NroCompra, Fec_Emision, SubTotal, IGV, Total, Estado_Facturacion)
        VALUES(@NroSerie_Facturacion, @CodEmpresa, @CodTipo_Facturacion, @NroCompra, @Fec_Emision, @SubTotal, @IGV, @Total, @Estado_Facturacion);

        COMMIT TRANSACTION;
        SET @Mensaje = 'OK: Factura de compra registrada';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

IF OBJECT_ID(N'dbo.USP_Listar_Facturacion_Compra', N'P') IS NOT NULL DROP PROCEDURE dbo.USP_Listar_Facturacion_Compra;
GO
CREATE PROCEDURE dbo.USP_Listar_Facturacion_Compra
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT NroSerie_Facturacion, CodEmpresa, CodTipo_Facturacion, NroCompra, Fec_Emision, SubTotal, IGV, Total, Estado_Facturacion
    FROM Tb_Facturacion_Compra
    WHERE (@Filtro = '' OR NroSerie_Facturacion LIKE '%' + @Filtro + '%' OR NroCompra LIKE '%' + @Filtro + '%')
    ORDER BY Fec_Emision DESC;
END
GO


ALTER PROCEDURE USP_Compra_ObtenerDatos_ParaFactura
    @NroCompra CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- TABLA 0: CABECERA (Proveedor + Totales)
    -- =============================================
    SELECT 
        OC.NroCompra,
        
        -- Datos de la Empresa Proveedora (Para llenar Tb_Facturacion_Compra)
        EP.CodEmpresa, 
        EP.Razon_Social AS ProveedorNombre,
        EP.Ruc AS ProveedorRuc,
        EP.Direccion AS ProveedorDireccion,
        
        -- CÁLCULOS FINANCIEROS
        -- Asumiendo que el Total de la Orden ya incluye IGV
        -- Base Imponible = Total / 1.18
        CAST((OC.Total / 1.18) AS NUMERIC(10,2)) AS ImporteBase, 
        
        -- IGV = Total - Base
        CAST((OC.Total - (OC.Total / 1.18)) AS NUMERIC(10,2)) AS IGVCalculado,
        
        -- Total Neto
        OC.Total
    FROM 
        Tb_Orden_Compra OC
        INNER JOIN Tb_Cotizacion C ON OC.NroCotizacion = C.NroCotizacion
        -- Del contacto obtenemos el código de empresa
        INNER JOIN Tb_Proveedor P ON C.CodProveedor = P.CodProveedor 
        -- De la empresa obtenemos Razón Social y RUC
        INNER JOIN Tb_Empresa_Proveedor EP ON P.CodEmpresa = EP.CodEmpresa 
    WHERE 
        OC.NroCompra = @NroCompra;

    -- =============================================
    -- TABLA 1: DETALLE (Productos de la compra)
    -- =============================================
    SELECT 
        DC.NroSerie_Producto,   -- Para DataField
        DC.CodSucursal,
        
        -- Descripción concatenada del producto
        (P.Producto + ' - ' + M.Modelo + ' - ' + P.Color + ' - Talla:' + CAST(T.Talla AS VARCHAR(10))) AS Producto, 
        
        DC.Cantidad,
        DC.Prec_Compra,       -- OJO: Aquí es Precio COMPRA
        DC.Porcentaje_Dscto,
        DC.Dscto,
        DC.SubTotal
    FROM 
        Tb_Detalle_Compra DC
        INNER JOIN Tb_Almacen_Sucursal A ON DC.NroSerie_Producto = A.NroSerie_Producto
            AND A.CodSucursal = DC.CodSucursal
        INNER JOIN Tb_Producto P ON A.CodProducto = P.CodProducto
        INNER JOIN Tb_Modelo M ON P.CodModelo = M.CodModelo
        INNER JOIN Tb_Talla T ON P.CodTalla = T.CodTalla
    WHERE 
        DC.NroCompra = @NroCompra;
END
GO



CREATE PROCEDURE dbo.USP_Listar_Facturas_Compra
    @filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FC.NroSerie_Facturacion,
        
        -- Datos del Proveedor (Empresa)
        EP.Razon_Social AS Proveedor,
        EP.Ruc,
        
        -- Datos de la Factura
        FC.NroCompra AS OrdenRelacionada, -- Para saber de qué orden vino
        FC.Fec_Emision,
        
        -- Importes
        FC.SubTotal,
        FC.IGV,
        FC.Total,
        
        -- Estado (Pendiente, Pagada, Anulada)
        FC.Estado_Facturacion
    FROM 
        Tb_Facturacion_Compra FC
        -- Unimos con la empresa para mostrar el nombre y no solo el código
        INNER JOIN Tb_Empresa_Proveedor EP ON FC.CodEmpresa = EP.CodEmpresa
    WHERE 
        (@filtro = '') OR 
        (FC.NroSerie_Facturacion LIKE '%' + @filtro + '%') OR
        (EP.Razon_Social LIKE '%' + @filtro + '%') OR
        (EP.Ruc LIKE '%' + @filtro + '%')
    ORDER BY 
        FC.Fec_Emision DESC; -- Las más recientes primero
END
GO