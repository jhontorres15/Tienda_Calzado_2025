

-- Procedimiento almacenado

CREATE PROCEDURE USP_Pedido_ObtenerDatos
    @NroPedido CHAR(12)
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- TABLA 0: CABECERA (Cliente + Totales Calculados)
    -- =============================================
    SELECT 
        P.NroPedido,
        P.CodCliente,
        ISNULL(RS.Razon_Social, C.Apellido + ', ' + C.Nombre) AS ClienteNombre,
        CASE 
            WHEN RS.Ruc IS NOT NULL THEN RS.Ruc 
            ELSE C.NroDoc_Identidad       
        END AS ClienteDocNumero,
        C.Direccion AS ClienteDireccion,
        
        -- CÁLCULOS PARA TUS TEXTBOX DE TOTALES
        -- Importe (Base Imponible) = Total / 1.18
        CAST((P.Total / 1.18) AS NUMERIC(10,2)) AS ImporteBase, 
        
        -- IGV = Total - Base
        CAST((P.Total - (P.Total / 1.18)) AS NUMERIC(10,2)) AS IGVCalculado,
        
        -- Total Neto
        P.Total
    FROM 
        Tb_Orden_Pedido P
        INNER JOIN Tb_Cliente C ON P.CodCliente = C.CodCliente
        LEFT JOIN Tb_Razon_Social RS ON C.CodCliente = RS.CodCliente
        LEFT JOIN Tb_Doc_Identidad TDI ON C.CodDoc_Identidad = TDI.CodDoc_Identidad
    WHERE 
        P.NroPedido = @NroPedido;

    -- =============================================
    -- TABLA 1: DETALLE (Mapeo exacto a tu GridView)
    -- =============================================
    SELECT 
        DP.NroSerie_Producto,   -- DataField="NroSerie_Producto"
        
        -- DataField="Producto" (Descripción completa)
        (PRO.Producto + ' ' + M.Modelo + ' ' + T.Color + ' T:' + CAST(TA.Talla AS VARCHAR)) AS Producto, 
        
        DP.CodSucursal,         -- DataField="CodSucursal"
        DP.Cantidad,            -- DataField="Cantidad"
        DP.Prec_Venta,          -- DataField="Prec_Venta"
        DP.Porcentaje_Dscto,    -- DataField="Porcentaje_Dscto"
        DP.Dscto,               -- DataField="Dscto"
        DP.SubTotal             -- DataField="SubTotal"
    FROM 
        Tb_Detalle_Pedido DP
        INNER JOIN Tb_Almacen_Sucursal ALS ON DP.NroSerie_Producto = ALS.NroSerie_Producto
        INNER JOIN Tb_Producto PRO ON ALS.CodProducto = PRO.CodProducto
        INNER JOIN Tb_Modelo M ON PRO.CodModelo = M.CodModelo
        INNER JOIN Tb_Talla TA ON PRO.CodTalla = TA.CodTalla
        INNER JOIN Tb_Producto T ON ALS.CodProducto = T.CodProducto 
    WHERE 
        DP.NroPedido = @NroPedido;
END
GO

CREATE PROCEDURE USP_Listar_TipoFacturacion
AS
BEGIN
    SELECT CodTipo_Facturacion,
           Tipo_Facturacion
    FROM Tb_Tipo_Facturacion
    ORDER BY Tipo_Facturacion;
END;
GO

CREATE PROCEDURE USP_Registrar_Facturacion
    -- Parámetros de entrada (lo que envía tu C#)
    @NroSerie_Facturacion CHAR(10),
    @CodTipo_Facturacion CHAR(3),
    @CodEmpleado CHAR(5),
    @NroPedido CHAR(12),
    @SubTotal NUMERIC(10,2),
    @IGV NUMERIC(10,2),
    @Total NUMERIC(10,2)
AS
BEGIN
    -- Iniciamos una Transacción para asegurar integridad
    -- (O se guardan los dos, o no se guarda ninguno)
    BEGIN TRANSACTION

    BEGIN TRY
        -- 1. INSERTAR LA FACTURA (Nace con estado 'EMITIDO')
        INSERT INTO Tb_Facturacion_Venta
        (
            NroSerie_Facturacion,
            CodTipo_Facturacion,
            CodEmpleado,
            NroPedido,
            Fec_Emision,
            SubTotal,
            IGV,
            Total,
            Estado_Facturacion
        )
        VALUES
        (
            @NroSerie_Facturacion,
            @CodTipo_Facturacion,
            @CodEmpleado,
            @NroPedido,
            GETDATE(), -- Fecha y hora actual del servidor
            @SubTotal,
            @IGV,
            @Total,
            'Pendiente'  -- Estado inicial automático
        )

        -- 2. ACTUALIZAR EL ESTADO DEL PEDIDO
        -- Pasamos el pedido de 'Pendiente' a 'Facturado' para cerrar el ciclo
        UPDATE Tb_Orden_Pedido
        SET Estado_Pedido = 'Facturado'
        WHERE NroPedido = @NroPedido

        -- Si todo salió bien, confirmamos los cambios
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        -- Si hubo error, deshacemos todo
        ROLLBACK TRANSACTION
        -- Opcional: Mostrar el error
        SELECT ERROR_MESSAGE() AS MensajeError
    END CATCH
END
GO

ALTER PROCEDURE USP_Generar_NroSerie_Facturacion
@NroSerie CHAR(10) OUTPUT
AS
BEGIN
    -- Declarar variables auxiliares
    DECLARE @Total INT
    DECLARE @Max INT

    -- Obtener el total de registros en la tabla Tb_Facturacion_Venta
    SELECT @Total = COUNT(*) FROM Tb_Facturacion_Venta

    -- Evaluar si el total de registros es cero
    IF (@Total = 0)
    BEGIN
        -- Establecer el primer Numero de Serie (Prefijo FACT + 6 dígitos)
        SET @NroSerie = 'FACT000001'
    END
    ELSE
    BEGIN
        -- Obtener el valor máximo numérico actual (tomando los 6 de la derecha) y sumar 1
        SELECT @Max = MAX(CAST(RIGHT(NroSerie_Facturacion, 6) AS INT)) + 1 
        FROM Tb_Facturacion_Venta
        
        -- Generar el nuevo código con ceros a la izquierda según el valor
        IF (@Max BETWEEN 1 AND 9)
            SET @NroSerie = 'FACT00000' + CONVERT(CHAR(1), @Max)
        ELSE IF (@Max BETWEEN 10 AND 99)
            SET @NroSerie = 'FACT0000' + CONVERT(CHAR(2), @Max)
        ELSE IF (@Max BETWEEN 100 AND 999)
            SET @NroSerie = 'FACT000' + CONVERT(CHAR(3), @Max)
        ELSE IF (@Max BETWEEN 1000 AND 9999)
            SET @NroSerie = 'FACT00' + CONVERT(CHAR(4), @Max)
        ELSE IF (@Max BETWEEN 10000 AND 99999)
            SET @NroSerie = 'FACT0' + CONVERT(CHAR(5), @Max)
        ELSE IF (@Max BETWEEN 100000 AND 999999)
            SET @NroSerie = 'FACT' + CONVERT(CHAR(6), @Max)
    END

    -- Mostrar el código generado
    PRINT @NroSerie
END
GO


CREATE PROCEDURE USP_Facturacion_ObtenerDatos_Impresion
    @NroSerie_Facturacion CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- =======================================================
    -- 1. CABECERA (Datos del Cliente y Totales)
    -- =======================================================
    SELECT 
        F.NroSerie_Facturacion,
        F.Fec_Emision,
        F.CodTipo_Facturacion,
        
        -- Totales
        F.SubTotal,
        F.IGV,
        F.Total,

        -- DATOS DEL CLIENTE (Lógica Inteligente Factura vs Boleta)
        CASE 
            WHEN F.CodTipo_Facturacion = 'F01' THEN RS.Razon_Social 
            ELSE C.Nombre + ' ' + C.Apellido 
        END AS ClienteNombre,

        CASE 
            WHEN F.CodTipo_Facturacion = 'F01' THEN 'RUC'
            ELSE ISNULL(TDI.Documento_Identidad, 'DNI') 
        END AS ClienteDocTipo,

        CASE 
            WHEN F.CodTipo_Facturacion = 'F01' THEN RS.Ruc 
            ELSE C.NroDoc_Identidad 
        END AS ClienteDocNumero,

        C.Direccion AS ClienteDireccion,
        
        -- Título del Documento
        CASE 
            WHEN F.CodTipo_Facturacion = 'F01' THEN 'FACTURA ELECTRÓNICA'
            ELSE 'BOLETA DE VENTA ELECTRÓNICA'
        END AS TituloDocumento

    FROM Tb_Facturacion_Venta F
    INNER JOIN Tb_Orden_Pedido OP ON F.NroPedido = OP.NroPedido
    INNER JOIN Tb_Cliente C ON OP.CodCliente = C.CodCliente
    LEFT JOIN Tb_Razon_Social RS ON C.CodCliente = RS.CodCliente
    LEFT JOIN Tb_Doc_Identidad TDI ON C.CodDoc_Identidad = TDI.CodDoc_Identidad
    WHERE 
        F.NroSerie_Facturacion = @NroSerie_Facturacion;

    -- =======================================================
    -- 2. DETALLE (Obtenido desde Tb_Detalle_Pedido)
    -- =======================================================
    SELECT 
        DP.Cantidad,
        
        -- Construcción de la Descripción (Producto + Marca + Modelo + Talla + Color)
        (
            P.Producto + 
            ISNULL(' - ' + MA.Marca, '') + 
            ISNULL(' - ' + M.Modelo, '') + 
            ISNULL(' - Talla:' + CAST(T.Talla AS VARCHAR), '') + 
            ISNULL(' - ' + P.Color, '')
        ) AS ProductoDescripcion,

        DP.Prec_Venta,  -- Precio Unitario
        DP.SubTotal     -- Importe Total por línea
    FROM 
        Tb_Facturacion_Venta F
        -- 1. Unimos Factura con el Detalle del Pedido
        INNER JOIN Tb_Detalle_Pedido DP ON F.NroPedido = DP.NroPedido
        
        -- 2. Del Detalle vamos al Almacén (usando Serie y Sucursal)
        INNER JOIN Tb_Almacen_Sucursal A ON DP.NroSerie_Producto = A.NroSerie_Producto 
                                         AND DP.CodSucursal = A.CodSucursal
        
        -- 3. Del Almacén sacamos los datos del Producto
        INNER JOIN Tb_Producto P ON A.CodProducto = P.CodProducto
        
        -- 4. Datos opcionales para descripción bonita
        LEFT JOIN Tb_Modelo M ON P.CodModelo = M.CodModelo
        LEFT JOIN Tb_Marca MA ON M.CodMarca = MA.CodMarca
        LEFT JOIN Tb_Talla T ON P.CodTalla = T.CodTalla
    WHERE 
        F.NroSerie_Facturacion = @NroSerie_Facturacion;
END
GO