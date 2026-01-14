

--Procedimientos almacenados para Tb_Orden_Pedido y Tb_Detalle_Pedido

CREATE PROCEDURE USP_Generar_NroPedido
    @NroPedido CHAR(12) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Declarar variable para el número
    DECLARE @Max INT

    -- 1. OBTENER EL ÚLTIMO NÚMERO
    -- Usamos RIGHT(..., 9) para leer TODOS los números después de 'PED'
    -- Si la tabla está vacía, MAX devolverá NULL
    SELECT @Max = MAX(CAST(RIGHT(NroPedido, 9) AS INT)) 
    FROM Tb_Orden_Pedido

    -- 2. LÓGICA DE INCREMENTO
    IF @Max IS NULL
    BEGIN
        -- Si es el primero, empezamos en 1
        SET @Max = 1
    END
    ELSE
    BEGIN
        -- Si ya existen, sumamos 1
        SET @Max = @Max + 1
    END

    -- 3. FORMATEAR EL CÓDIGO (PED + 9 dígitos)
    -- El truco: '000000000' + Numero, y luego cortamos los 9 de la derecha.
    -- Esto reemplaza a todos tus IF / ELSE IF y nunca falla.
    SET @NroPedido = 'PED' + RIGHT('000000000' + CAST(@Max AS VARCHAR(10)), 9)

    -- (Opcional) Imprimir para probar
    -- PRINT @NroPedido
END
GO

CREATE PROCEDURE USP_CargarDetalle_DesdeProforma
    @NroProforma CHAR(10),
    @CodSucursal CHAR(5),
    @MensajeAviso VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- =======================================================
    -- 0. VALIDACIÓN DE ESTADO DEL CLIENTE (NUEVO BLOQUE)
    -- =======================================================
    DECLARE @EstadoCliente VARCHAR(20)
    DECLARE @NombreCliente VARCHAR(100)

    SELECT 
        @EstadoCliente = C.Estado_Cliente,
        @NombreCliente = ISNULL(RS.Razon_Social, C.Apellido + ', ' + C.Nombre)
    FROM Tb_Proforma P
    INNER JOIN Tb_Cliente C ON P.CodCliente = C.CodCliente
    LEFT JOIN Tb_Razon_Social RS ON C.CodCliente = RS.CodCliente
    WHERE P.NroProforma = @NroProforma

    -- Si el cliente NO está activo, detenemos todo y mandamos error.
    IF @EstadoCliente <> 'Activo'
    BEGIN
        SET @MensajeAviso = 'ERROR: El cliente ' + @NombreCliente + ' se encuentra en estado ' + UPPER(@EstadoCliente) + '. No se puede procesar pedidos.'
        RETURN -- Salimos del procedimiento aquí mismo
    END

    -- =======================================================
    -- 1. PRIMER RESULTSET: DATOS DE CABECERA (CLIENTE)
    -- =======================================================
    SELECT 
        P.CodCliente,
        ISNULL(RS.Razon_Social, C.Apellido + ', ' + C.Nombre) AS NombreCliente
    FROM Tb_Proforma P
    INNER JOIN Tb_Cliente C ON P.CodCliente = C.CodCliente
    LEFT JOIN Tb_Razon_Social RS ON C.CodCliente = RS.CodCliente
    WHERE P.NroProforma = @NroProforma;

    -- =======================================================
    -- 2. LÓGICA DE STOCK (Igual que antes)
    -- =======================================================
    DECLARE @DetalleCalculado TABLE (
        NroSerie_Producto CHAR(7),
        ProductoNombre VARCHAR(200),
        CodSucursal CHAR(5),
        CantidadSolicitada INT,
        StockActual INT,
        CantidadFinal INT,
        Prec_Venta NUMERIC(10,2),
        SubTotal NUMERIC(10,2),
        EstadoStock VARCHAR(20)
    )

    INSERT INTO @DetalleCalculado
    SELECT 
        DP.NroSerie_Producto,
        P.Producto + ' - ' + M.Modelo + ' - ' + T.Color,
        A.CodSucursal,
        DP.Cantidad,
        ISNULL(A.Stock_Actual, 0),
        CASE 
            WHEN ISNULL(A.Stock_Actual, 0) >= DP.Cantidad THEN DP.Cantidad
            ELSE ISNULL(A.Stock_Actual, 0)
        END,
        DP.Prec_Venta_Menor,
        (CASE 
            WHEN ISNULL(A.Stock_Actual, 0) >= DP.Cantidad THEN DP.Cantidad
            ELSE ISNULL(A.Stock_Actual, 0)
        END * DP.Prec_Venta_Menor),
        CASE 
            WHEN ISNULL(A.Stock_Actual, 0) = 0 THEN 'SIN STOCK'
            WHEN ISNULL(A.Stock_Actual, 0) < DP.Cantidad THEN 'INCOMPLETO'
            ELSE 'OK'
        END
    FROM Tb_Detalle_Proforma DP
    LEFT JOIN Tb_Almacen_Sucursal A ON DP.NroSerie_Producto = A.NroSerie_Producto AND A.CodSucursal = @CodSucursal
    INNER JOIN Tb_Producto P ON A.CodProducto = P.CodProducto 
    INNER JOIN Tb_Modelo M ON P.CodModelo = M.CodModelo
    INNER JOIN Tb_Talla TA ON P.CodTalla = TA.CodTalla
    INNER JOIN Tb_Producto T ON A.CodProducto = T.CodProducto
    WHERE DP.NroProforma = @NroProforma

    -- 3. CONSTRUIR MENSAJE
    SET @MensajeAviso = ''
    SELECT @MensajeAviso = @MensajeAviso + '- SIN STOCK: ' + ProductoNombre + CHAR(13) FROM @DetalleCalculado WHERE EstadoStock = 'SIN STOCK'
    SELECT @MensajeAviso = @MensajeAviso + '- INCOMPLETO (' + CAST(StockActual AS VARCHAR) + ' unid.): ' + ProductoNombre + CHAR(13) FROM @DetalleCalculado WHERE EstadoStock = 'INCOMPLETO'

    IF LEN(@MensajeAviso) > 0 SET @MensajeAviso = 'ADVERTENCIA DE STOCK:' + CHAR(13) + @MensajeAviso
    ELSE SET @MensajeAviso = 'OK'

    -- 4. SEGUNDO RESULTSET
    SELECT 
        NroSerie_Producto,
        ProductoNombre,
        CodSucursal,
        CantidadFinal AS Cantidad, 
        Prec_Venta,
        0.00 AS Porcentaje_Dscto, 
        0.00 AS Dscto,
        SubTotal
    FROM @DetalleCalculado
    WHERE CantidadFinal > 0
END
GO



CREATE PROCEDURE USP_Listar_TiposPedido
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CodTipo_Pedido, Tipo_Pedido
    FROM Tb_Tipo_Pedido
    ORDER BY Tipo_Pedido;
END
GO

CREATE PROCEDURE USP_Listar_Clientes_Busqueda
    @pBusqueda VARCHAR(50) 
AS
BEGIN
    SET NOCOUNT ON;

    -- Preparamos el término de búsqueda para LIKE (búsqueda parcial)
    -- Esto permite buscar nombres o apellidos aunque no se escriban completos.
    DECLARE @TerminoBusqueda VARCHAR(52) = '%' + @pBusqueda + '%';

    SELECT 
        -- Las columnas clave para mostrar en el GridView del modal y para seleccionar
        C.CodCliente,
        C.Apellido,
        C.Nombre,
        C.NroDoc_Identidad AS Identificacion,
        C.Estado_Cliente
    FROM 
        Tb_Cliente C
    WHERE 
        -- 1. Búsqueda EXACTA por Número de Documento (DNI o RUC)
        C.NroDoc_Identidad = @pBusqueda 
        OR 
        -- 2. Búsqueda PARCIAL por Apellido
        C.Apellido LIKE @TerminoBusqueda
        OR
        -- 3. Búsqueda PARCIAL por Nombre
        C.Nombre LIKE @TerminoBusqueda
        -- Si necesitas buscar el nombre completo concatenado, añade:
        -- OR (C.Nombre + ' ' + C.Apellido) LIKE @TerminoBusqueda
END
GO


CREATE PROCEDURE USP_Listar_Productos_Finales
    @Marca NVARCHAR(50),
    @Modelo VARCHAR(20),
    @Talla VARCHAR(5),
    @Color VARCHAR(20)
AS
BEGIN
    DECLARE @CodModelo CHAR(5);
    DECLARE @CodTalla CHAR(5);

    -- 1. Obtener Códigos
    SELECT @CodModelo = CodModelo FROM Tb_Modelo WHERE Modelo = @Modelo;
    SELECT @CodTalla = CodTalla FROM Tb_Talla WHERE Talla = @Talla;

    -- 2. Seleccionar el Código de Producto y la descripción para el DDL final
    SELECT 
        T1.CodProducto, 
        -- Concatenar la descripción final: CODIGO + NOMBRE + TALLA + COLOR
        T1.CodProducto + ' - ' + T1.Producto + ' - T:' + @Talla + ' - C:' + T1.Color AS DescripcionFinal
    FROM Tb_Producto AS T1
    WHERE T1.CodModelo = @CodModelo
      AND T1.CodTalla = @CodTalla
      AND T1.Color = @Color
    ORDER BY T1.CodProducto;
END
GO

CREATE PROCEDURE USP_Listar_Pedidos
AS
BEGIN
    SELECT * FROM V_Listar_Pedidos 
    ORDER BY Fec_Pedido DESC -- Muestra los más recientes primero
END

CREATE TYPE dbo.TipoDetallePedido AS TABLE(
    NroSerie_Producto VARCHAR(20),
    Prec_Venta DECIMAL(10,2),
    Cantidad INT,
    Porcentaje_Dscto DECIMAL(10,2),
    Dscto DECIMAL(10,2),
    SubTotal DECIMAL(10,2)
)
GO

CREATE PROCEDURE USP_Registrar_Pedido
    -- Parámetros de Cabecera (Tb_Orden_Pedido)
    @NroPedido CHAR(12),
    @CodSucursal CHAR(5),        -- Se usará para todos los ítems del detalle
    @CodTipo_Pedido CHAR(6),
    @CodCliente CHAR(8),
    @CodEmpleado CHAR(5),
    @Fec_Pedido DATETIME,
    @Total NUMERIC(10,2),
    @Estado_Pedido VARCHAR(20),
    
    -- Parámetro de Detalle (Tu tabla personalizada)
    @Detalle dbo.TipoDetallePedido READONLY,
    
    -- Parámetro de Salida (Mensaje)
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- --- 1. VALIDACIONES BÁSICAS ---
        
        -- Validar Cliente
        IF NOT EXISTS(SELECT 1 FROM Tb_Cliente WHERE CodCliente = @CodCliente)
        BEGIN
            SET @Mensaje ='Error: El código de cliente no existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Validar Empleado
        IF NOT EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
        BEGIN
            SET @Mensaje ='Error: El código de empleado no existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar Duplicado
        IF EXISTS(SELECT 1 FROM Tb_Orden_Pedido WHERE NroPedido = @NroPedido)
        BEGIN
            SET @Mensaje = 'Error: El Nro de Pedido ya existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- --- 2. GUARDADO ---

        -- A) Insertar la Cabecera
        INSERT INTO Tb_Orden_Pedido
        (
            NroPedido, CodSucursal, CodTipo_Pedido, CodCliente, 
            CodEmpleado, Fec_Pedido, Total, Estado_Pedido
        )
        VALUES
        (
            @NroPedido, @CodSucursal, @CodTipo_Pedido, @CodCliente, 
            @CodEmpleado, @Fec_Pedido, @Total, @Estado_Pedido
        );

        -- B) Insertar los Detalles
        INSERT INTO Tb_Detalle_Pedido
        (
            NroPedido, 
            CodSucursal, 
            NroSerie_Producto, 
            Prec_Venta, 
            Cantidad, 
            Importe,          -- <--- La columna destino en la tabla real
            Porcentaje_Dscto, 
            Dscto, 
            SubTotal
        )
        SELECT
            @NroPedido,       -- Asignamos el NroPedido a todos los items
            @CodSucursal,     -- Asignamos la Sucursal de la cabecera
            NroSerie_Producto, 
            Prec_Venta, 
            Cantidad, 
            
            -- ¡SOLUCIÓN AQUÍ! Calculamos el importe al vuelo
            (Prec_Venta * Cantidad), -- En vez de seleccionar una columna 'Importe' que no existe
            
            Porcentaje_Dscto, 
            Dscto, 
            SubTotal
        FROM @Detalle; 

        -- --- 3. CONFIRMACIÓN ---
        SET @Mensaje = 'Pedido registrado correctamente.';
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        -- En caso de error, deshacer todo
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error al guardar el pedido: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE USP_Listar_Clientes_Busqueda
    @pBusqueda VARCHAR(50),
    @Mensaje VARCHAR(200) OUTPUT -- Parámetro de salida para avisar a C#
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TerminoBusqueda VARCHAR(52) = '%' + @pBusqueda + '%';

    -- 1. Intentamos seleccionar los clientes ACTIVOS que coincidan
    SELECT 
        C.CodCliente,
        C.Apellido,
        C.Nombre,
        C.NroDoc_Identidad AS Identificacion,
        C.Estado_Cliente
    FROM 
        Tb_Cliente C
    WHERE 
        C.Estado_Cliente = 'Activo' -- FILTRO PRINCIPAL: Solo Activos
        AND (
            C.NroDoc_Identidad = @pBusqueda 
            OR C.Apellido LIKE @TerminoBusqueda
            OR C.Nombre LIKE @TerminoBusqueda
        );

    -- 2. Verificamos si se encontró algo
    -- @@ROWCOUNT nos dice cuántas filas devolvió el SELECT anterior
    IF @@ROWCOUNT = 0 
    BEGIN
        -- Si no salió nada, verificamos si el cliente EXISTE pero está BLOQUEADO/INACTIVO
        -- Esto es útil principalmente cuando buscas por DNI exacto
        IF EXISTS (
            SELECT 1 
            FROM Tb_Cliente 
            WHERE (NroDoc_Identidad = @pBusqueda OR Apellido LIKE @TerminoBusqueda)
            AND Estado_Cliente IN ('Inactivo', 'Bloqueado')
        )
        BEGIN
            -- Encontramos al cliente, pero no está activo
            SET @Mensaje = 'El cliente existe pero se encuentra BLOQUEADO o INACTIVO';
        END
        ELSE
        BEGIN
            -- No existe ni activo ni inactivo
            SET @Mensaje = ''; -- O 'No se encontraron resultados'
        END
    END
    ELSE
    BEGIN
        -- Si se encontraron clientes activos, el mensaje va limpio
        SET @Mensaje = '';
    END
END
GO

CREATE PROCEDURE usp_RegistrarPedido_ActualizarProforma
    @NroProforma char(10),
    @NroPedido char(12)
AS
BEGIN
    -- Iniciamos una transacción para asegurar que ambos pasos se cumplan o ninguno
    BEGIN TRANSACTION;

    BEGIN TRY
        -- PASO 1: Insertar en la tabla intermedia
        INSERT INTO Tb_Proforma_Pedido (NroProforma, NroPedido)
        VALUES (@NroProforma, @NroPedido);

        -- PASO 2: Actualizar el estado de la Proforma a 'Aprobada'
        UPDATE Tb_Proforma
        SET Estado_Proforma = 'Aprobada'
        WHERE NroProforma = @NroProforma;

        -- Si todo salió bien, guardamos los cambios permanentemente
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Si hubo error, deshacemos todo
        ROLLBACK TRANSACTION;
        
        -- Lanzamos el error para que C# lo capture
        DECLARE @MensajeError NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@MensajeError, 16, 1);
    END CATCH
END
GO