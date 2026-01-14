


CREATE PROCEDURE USP_Generar_NroKardex
    @NroOperacion INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Max INT;

    -- Obtener el número máximo actual
    SELECT @Max = MAX(Nro_Operacion) FROM Tb_Kardex;

    -- Si no hay registros, iniciar en 1
    IF (@Max IS NULL)
        SET @NroOperacion = 1;
    ELSE
        SET @NroOperacion = @Max + 1;

END
GO

CREATE PROCEDURE USP_Listar_Operacion
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CodTipo_Operacion,
        Tipo_Operacion
    FROM Tb_Tipo_Operacion
    ORDER BY CodTipo_Operacion;
END
GO


CREATE TYPE TipoDetalleKardex AS TABLE
(
    NroSerie_Producto CHAR(7),
    Cantidad INT,
    Costo_Unitario NUMERIC(10,2),
    Costo_Total NUMERIC(10,2)
);
GO

ALTER PROCEDURE USP_Kardex_Guardar
    @NroOperacion INT,
    @CodEmpleado CHAR(5),
    @CodTipo_Operacion CHAR(5),
    @CodSucursal CHAR(5),
    @Fec_Operacion DATE,
    @CodTipo_Facturacion CHAR(3),
    @NroSerie_Facturacion CHAR(10),
    @Observaciones VARCHAR(200),
    @Detalle dbo.TipoDetalleKardex READONLY, -- Asegúrate que este TYPE exista en tu BD
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. VALIDACIÓN DE DUPLICIDAD DE OPERACIÓN
        IF EXISTS(SELECT 1 FROM Tb_Kardex WHERE Nro_Operacion = @NroOperacion)
        BEGIN
            SET @Mensaje = 'Error: El Nro de Operación ' + CAST(@NroOperacion AS VARCHAR) + ' ya existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. VALIDACIÓN DE FACTURA DUPLICADA
        IF EXISTS(SELECT 1 FROM Tb_Kardex 
                  WHERE NroSerie_Facturacion = @NroSerie_Facturacion 
                  AND CodTipo_Facturacion = @CodTipo_Facturacion)
        BEGIN
            SET @Mensaje = 'Error: El documento ' + RTRIM(@NroSerie_Facturacion) + ' ya se encuentra registrado.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. INSERTAR EN Tb_Kardex
        INSERT INTO Tb_Kardex
        (
            Nro_Operacion, 
            CodEmpleado, 
            CodTipo_Operacion, 
            Fec_Operacion, 
            CodSucursal, 
            NroSerie_Producto, 
            CodTipo_Facturacion, 
            NroSerie_Facturacion, 
            Costo_Unitario, 
            Cantidad, 
            Costo_Total, 
            Obs_Operacion
        )
        SELECT 
            @NroOperacion, 
            @CodEmpleado, 
            @CodTipo_Operacion, 
            @Fec_Operacion, 
            @CodSucursal, 
            D.NroSerie_Producto, 
            @CodTipo_Facturacion, 
            @NroSerie_Facturacion, 
            D.Costo_Unitario, 
            D.Cantidad, 
            D.Costo_Total, 
            @Observaciones
        FROM @Detalle D;

        -- ---------------------------------------------------------
        -- 4. ACTUALIZAR STOCK EN ALMACÉN (NUEVO)
        -- ---------------------------------------------------------
        -- Aquí cruzamos la tabla Almacen con el Detalle (@Detalle)
        -- usando el NroSerie_Producto y la Sucursal.
        
        UPDATE A
        SET A.Stock_Actual = A.Stock_Actual - D.Cantidad  -- SE RESTA EL STOCK
        FROM Tb_Almacen_Sucursal A
        INNER JOIN @Detalle D ON A.NroSerie_Producto = D.NroSerie_Producto
        WHERE A.CodSucursal = @CodSucursal;

        -- (Opcional) Validación: Si quieres evitar stock negativo, se haría aquí antes del COMMIT.

        COMMIT TRANSACTION;
        SET @Mensaje = 'OK: Kardex registrado y Stock actualizado correctamente.';

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'ERROR SQL: ' + ERROR_MESSAGE();
    END CATCH
END
GO




CREATE PROCEDURE usp_listar_kardex
AS
BEGIN
    -- Se seleccionan las columnas exactas que pide tu GridView
    SELECT 
        Nro_Operacion,
        Fec_Operacion,
        CodTipo_Operacion, -- Necesario para tu lógica de colores (StartsWith "ENT")
        NroSerie_Producto,
        Cantidad,
        Costo_Unitario,
        Costo_Total
    FROM 
        Tb_Kardex
    ORDER BY 
        Fec_Operacion DESC, 
        Nro_Operacion DESC -- Para ver lo más reciente primero
END
GO


ALTER PROCEDURE USP_Kardex_Guardar
    @NroOperacion INT,              -- El número base que sugiere tu sistema
    @CodEmpleado CHAR(5),
    @CodTipo_Operacion CHAR(5),     
    @CodSucursal CHAR(5),
    @Fec_Operacion DATE,            
    @CodTipo_Facturacion CHAR(3),   
    @NroSerie_Facturacion CHAR(10),
    @Observaciones VARCHAR(200),
    @Detalle dbo.TipoDetalleKardex READONLY, 
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- ---------------------------------------------------------
        -- 1. ASEGURAR UN NÚMERO INICIAL ÚNICO
        -- ---------------------------------------------------------
        DECLARE @Inicio INT = @NroOperacion;
        DECLARE @MaximoActual INT;

        -- Buscamos cuál es el último número usado en la base de datos
        SELECT @MaximoActual = ISNULL(MAX(Nro_Operacion), 0) FROM Tb_Kardex WITH (UPDLOCK);

        -- Si el número que enviaste (@NroOperacion) ya está ocupado o es menor al actual,
        -- automáticamente saltamos al siguiente número libre para evitar errores.
        IF @Inicio <= @MaximoActual
        BEGIN
            SET @Inicio = @MaximoActual + 1;
        END

        -- ---------------------------------------------------------
        -- 2. VALIDACIÓN DE DUPLICADOS (DOCUMENTO)
        -- ---------------------------------------------------------
        IF @CodTipo_Facturacion <> 'OTR' AND EXISTS(SELECT 1 FROM Tb_Kardex 
                   WHERE NroSerie_Facturacion = @NroSerie_Facturacion 
                   AND CodTipo_Facturacion = @CodTipo_Facturacion)
        BEGIN
            SET @Mensaje = 'Error: El documento ' + RTRIM(@NroSerie_Facturacion) + ' ya se encuentra registrado.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ---------------------------------------------------------
        -- 3. INSERTAR (LA MAGIA DE SUMAR 1 POR 1)
        -- ---------------------------------------------------------
        INSERT INTO Tb_Kardex
        (
            Nro_Operacion,          -- Aquí guardaremos 4, 5, 6...
            CodEmpleado, 
            CodTipo_Operacion, 
            Fec_Operacion, 
            CodSucursal, 
            NroSerie_Producto, 
            CodTipo_Facturacion, 
            NroSerie_Facturacion, 
            Costo_Unitario, 
            Cantidad, 
            Costo_Total, 
            Obs_Operacion
        )
        SELECT 
            -- === EXPLICACIÓN DE ESTA FÓRMULA ===
            -- @Inicio: Es el número base (ej: 4).
            -- ROW_NUMBER(): Enumera las filas del detalle (1, 2, 3...).
            -- Al restar 1, hacemos que la primera fila sea: 4 + 0 = 4.
            -- La segunda fila será: 4 + 1 = 5.
            -- La tercera fila será: 4 + 2 = 6.
            @Inicio + (ROW_NUMBER() OVER (ORDER BY D.NroSerie_Producto) - 1),
            
            @CodEmpleado, 
            @CodTipo_Operacion, 
            @Fec_Operacion, 
            @CodSucursal, 
            D.NroSerie_Producto, 
            @CodTipo_Facturacion, 
            @NroSerie_Facturacion, 
            D.Costo_Unitario, 
            D.Cantidad, 
            D.Costo_Total, 
            @Observaciones
        FROM @Detalle D;

        -- ---------------------------------------------------------
        -- 4. ACTUALIZAR STOCK
        -- ---------------------------------------------------------
        UPDATE A
        SET A.Stock_Actual = A.Stock_Actual - D.Cantidad
        FROM Tb_Almacen_Sucursal A
        INNER JOIN @Detalle D ON A.NroSerie_Producto = D.NroSerie_Producto
        WHERE A.CodSucursal = @CodSucursal;

        COMMIT TRANSACTION;
        SET @Mensaje = 'OK: Kardex registrado correctamente.';

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'ERROR SQL: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE USP_Factura_ObtenerDatos
    @NroSerie_Facturacion CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- 1. BUSQUEDA EN VENTAS (Salidas de Kardex)
    -- =============================================
    IF EXISTS (SELECT 1 FROM Tb_Facturacion_Venta 
               WHERE NroSerie_Facturacion = @NroSerie_Facturacion 
               AND Estado_Facturacion = 'Emitida') -- Solo Emitidas
    BEGIN
        -- TABLA 0: CABECERA
        -- Retiramos CodSucursal ya que se toma del formulario
        SELECT 
            F.NroSerie_Facturacion, 
            F.CodTipo_Facturacion, 
            F.NroPedido, 
            F.Total AS Total_Facturacion,
            'VENTA' AS Origen_Datos
        FROM Tb_Facturacion_Venta F
        WHERE F.NroSerie_Facturacion = @NroSerie_Facturacion;

        -- TABLA 1: DETALLE
        SELECT 
            DP.NroSerie_Producto, 
            DP.Prec_Venta AS Costo_Unitario, 
            DP.Cantidad,
            (DP.Prec_Venta * DP.Cantidad) AS Costo_Total
        FROM Tb_Facturacion_Venta F
        INNER JOIN Tb_Orden_Pedido P ON F.NroPedido = P.NroPedido
        INNER JOIN Tb_Detalle_Pedido DP ON P.NroPedido = DP.NroPedido
        WHERE F.NroSerie_Facturacion = @NroSerie_Facturacion;
    END

    -- =============================================
    -- 2. BUSQUEDA EN COMPRAS (Entradas de Kardex)
    -- =============================================
    ELSE IF EXISTS (SELECT 1 FROM Tb_Facturacion_Compra 
                    WHERE NroSerie_Facturacion = @NroSerie_Facturacion
                    AND Estado_Facturacion = 'Emitida') -- Solo Emitidas
    BEGIN
        -- TABLA 0: CABECERA
        SELECT 
            F.NroSerie_Facturacion, 
            F.CodTipo_Facturacion, 
            F.NroCompra, 
            F.Total AS Total_Facturacion,
            'COMPRA' AS Origen_Datos
        FROM Tb_Facturacion_Compra F
        WHERE F.NroSerie_Facturacion = @NroSerie_Facturacion;

        -- TABLA 1: DETALLE
        SELECT 
            DC.NroSerie_Producto, 
            DC.Prec_Compra AS Costo_Unitario, 
            DC.Cantidad,
            DC.SubTotal AS Costo_Total 
        FROM Tb_Facturacion_Compra F
        INNER JOIN Tb_Orden_Compra OC ON F.NroCompra = OC.NroCompra
        INNER JOIN Tb_Detalle_Compra DC ON OC.NroCompra = DC.NroCompra
        WHERE F.NroSerie_Facturacion = @NroSerie_Facturacion;
    END

    -- =============================================
    -- 3. SI NO EXISTE O NO ESTÁ EMITIDA
    -- =============================================
    ELSE
    BEGIN
        -- Resultsets vacios
        SELECT TOP 0 NULL AS NroSerie_Facturacion;
        SELECT TOP 0 NULL AS NroSerie_Producto;
    END
END
GO