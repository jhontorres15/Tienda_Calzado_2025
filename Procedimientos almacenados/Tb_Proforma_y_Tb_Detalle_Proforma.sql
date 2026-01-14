
--Procedimientos almacenados para Tb_Proforma y Tb_Detalle_Proforma--


CREATE TYPE dbo.TipoDetalleProforma AS TABLE(
    CodSucursal char(5) NOT NULL,
    NroSerie_Producto char(7) NOT NULL,
    Prec_Venta_Menor numeric(10,2) NOT NULL,
    Prec_Venta_Mayor numeric(10,2) NOT NULL,
    Prec_Cotizado numeric(10,2) NOT NULL,
    Cantidad int NOT NULL,
    Importe numeric(10,2) NOT NULL
)
GO

CREATE PROCEDURE USP_Generar_NroProforma
@NroProforma CHAR(10) OUTPUT
AS
BEGIN
   -- Declarar variables auxiliares
   DECLARE @Total INT
   DECLARE @Max INT

   -- Obtener el total de registros en la tabla Tb_Empleado
   SELECT @Total = COUNT(NroProforma) FROM Tb_Proforma

   -- Evaluar si el total de registros es cero
   IF (@Total = 0)
   BEGIN
       -- Establecer el primer Numero de Proforma
       SET @NroProforma = 'PROF000001'
   END
   ELSE
   BEGIN
       -- Obtener el valor máximo numérico del código de empleado
       SELECT @Max = MAX(CAST(RIGHT(NroProforma, 4) AS INT)) + 1 FROM Tb_Proforma
       
       -- Generar el nuevo código con ceros a la izquierda según el valor
       IF (@Max BETWEEN 1 AND 9)
           SET @NroProforma = 'PROF00000' + CONVERT(CHAR(1), @Max)
       ELSE IF (@Max BETWEEN 10 AND 99)
           SET @NroProforma = 'PROF0000' + CONVERT(CHAR(2), @Max)
       ELSE IF (@Max BETWEEN 100 AND 999)
           SET @NroProforma = 'PROF000' + CONVERT(CHAR(3), @Max)
       ELSE IF (@Max BETWEEN 1000 AND 9999)
           SET @NroProforma = 'PROF00' + CONVERT(CHAR(4), @Max)
       ELSE IF (@Max BETWEEN 10000 AND 99999)
           SET @NroProforma = 'PROF0' + CONVERT(CHAR(4), @Max)
	   ELSE IF (@Max BETWEEN 100000 AND 999999)
           SET @NroProforma = 'PROF' + CONVERT(CHAR(4), @Max)
   END

   -- Mostrar el código generado
   PRINT @NroProforma
END
GO

CREATE PROCEDURE USP_Proforma_Venta
    @NroProforma char(10),
    @CodCliente char(8),          -- Recibimos el CÓDIGO del cliente
    @CodEmpleado char(5),        -- Recibimos el CÓDIGO del empleado
    @Fec_Emision datetime,
    @Fec_Caducidad date,
    @Total numeric(10,2),
    @Estado_Proforma varchar(20),
    
    -- El parámetro que recibe la tabla de detalles
    @Detalle dbo.TipoDetalleProforma READONLY, 
    
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- --- VALIDACIONES ---
        IF NOT EXISTS(SELECT 1 FROM Tb_Cliente WHERE CodCliente = @CodCliente)
        BEGIN
            SET @Mensaje ='Error: El código de cliente no existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        IF NOT EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
        BEGIN
            SET @Mensaje ='Error: El código de empleado no existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS(SELECT 1 FROM Tb_Proforma WHERE NroProforma = @NroProforma)
        BEGIN
            SET @Mensaje = 'Error: El código de Proforma ya existe. Presione "Nuevo" para generar otro.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- --- GUARDADO ---

        -- 1. Inserta la Cabecera
        INSERT INTO Tb_Proforma
        (NroProforma, CodCliente, CodEmpleado, Fec_Emision, Fec_Caducidad, Total, Estado_Proforma )
        VALUES
        (@NroProforma, @CodCliente, @CodEmpleado, @Fec_Emision, @Fec_Caducidad, @Total, @Estado_Proforma);

        -- 2. Inserta TODOS los Detalles
        INSERT INTO Tb_Detalle_Proforma
        (NroProforma, CodSucursal, NroSerie_Producto, Prec_Venta_Menor, Prec_Venta_Mayor, Prec_Cotizado, Cantidad, Importe)
        SELECT
            @NroProforma, -- El NroProforma de la cabecera
            CodSucursal, 
            NroSerie_Producto, 
            Prec_Venta_Menor, 
            Prec_Venta_Mayor, 
            Prec_Cotizado, 
            Cantidad, 
            Importe
        FROM @Detalle; -- Inserta todo desde nuestra "tabla" parámetro

        SET @Mensaje = 'Proforma registrada correctamente.';
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error al guardar: ' + ERROR_MESSAGE();
    END CATCH
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

CREATE PROCEDURE USP_Listar_Proforma
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        P.NroProforma, 
        
        -- Unimos Nombre y Apellido de Cliente
        (C.Apellido + ', ' + C.Nombre) AS Cliente,
        
        -- Unimos Nombre y Apellido de Empleado
        (E.Apellido + ', ' + E.Nombre) AS Empleado,
        
        P.Fec_Emision, 
        P.Fec_Caducidad, 
        P.Total, 
        P.Estado_Proforma
    FROM 
        Tb_Proforma AS P
        INNER JOIN Tb_Cliente AS C ON P.CodCliente = C.CodCliente
        INNER JOIN Tb_Empleado AS E ON P.CodEmpleado = E.CodEmpleado
    ORDER BY
        P.Fec_Emision DESC; -- Ordena por las m�s nuevas primero
END
GO

CREATE PROCEDURE USP_Proforma_ObtenerDatos
    @NroProforma CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Obtener la Cabecera
    SELECT 
        P.NroProforma,
        P.Fec_Emision,
        P.Fec_Caducidad,
        P.Total,
        P.Estado_Proforma,
        
        ISNULL(RS.Razon_Social, C.Nombre + ' ' + C.Apellido) AS ClienteNombre,
        
       
        -- Usamos CASE para comprobar si existe una Raz�n Social
        CASE 
            WHEN RS.Ruc IS NOT NULL THEN 'RUC' -- Si hay Raz�n Social, es RUC
            ELSE TDI.Documento_Identidad      -- Si no, es DNI (o lo que sea)
        END AS ClienteDocTipo,
        
        CASE
            WHEN RS.Ruc IS NOT NULL THEN RS.Ruc -- Si hay Raz�n Social, usa el RUC
            ELSE C.NroDoc_Identidad             -- Si no, usa el DNI del cliente
        END AS ClienteDocNumero,
        -- --- FIN DEL CAMBIO ---
        
        C.Direccion AS ClienteDireccion,
        C.Telefono AS ClienteTelefono,
        C.Email AS ClienteEmail,
        (E.Apellido + ', ' + E.Nombre) AS EmpleadoNombre
    FROM 
        Tb_Proforma AS P
        LEFT JOIN Tb_Cliente AS C ON P.CodCliente = C.CodCliente
        LEFT JOIN Tb_Empleado AS E ON P.CodEmpleado = E.CodEmpleado
        LEFT JOIN Tb_Razon_Social AS RS ON C.CodCliente = RS.CodCliente
        LEFT JOIN Tb_Doc_Identidad AS TDI ON C.CodDoc_Identidad = TDI.CodDoc_Identidad
    WHERE 
        P.NroProforma = @NroProforma;

    -- 2. Obtener el Detalle
    SELECT 
        D.NroSerie_Producto,
        (
            P.Producto + ' - ' + 
            ISNULL(MA.Marca, 'S/M') + ' - ' +
            ISNULL(M.Modelo, 'S/M') + ' - ' + 
            ISNULL(CONVERT(VARCHAR(10), T.Talla), 'S/T') + ' - ' + 
            P.Color
        ) AS ProductoDescripcion,
        D.Cantidad,
        D.Prec_Cotizado,
        D.Importe
    FROM 
        Tb_Detalle_Proforma AS D
        LEFT JOIN Tb_Almacen_Sucursal AS A ON D.NroSerie_Producto = A.NroSerie_Producto
        LEFT JOIN Tb_Producto AS P ON A.CodProducto = P.CodProducto
        LEFT JOIN Tb_Modelo AS M ON P.CodModelo = M.CodModelo
        LEFT JOIN Tb_Talla AS T ON P.CodTalla = T.CodTalla
        LEFT JOIN Tb_Marca AS MA ON M.CodMarca = MA.CodMarca 
    WHERE 
        D.NroProforma = @NroProforma;
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

CREATE PROCEDURE USP_Buscar_Stock_Proforma
    @SucursalNombre VARCHAR(100),  -- �CAMBIO 1: Recibe el NOMBRE!
    @Marca VARCHAR(100),
    @Modelo VARCHAR(100),
    @Talla VARCHAR(20),
    @Color VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- �CAMBIO 2: Declarar variable para el c�digo!
    DECLARE @CodSucursal CHAR(5);

    -- �CAMBIO 3: Buscar el C�DIGO usando el NOMBRE!
    SELECT @CodSucursal = CodSucursal 
    FROM Tb_Sucursal 
    WHERE RTRIM(LTRIM(Sucursal)) = RTRIM(LTRIM(@SucursalNombre));

    -- Si no encontramos un c�digo (ej. nombre incorrecto), salimos.
    IF @CodSucursal IS NULL
    BEGIN
        -- Devolvemos 5 tablas vac�as para que la aplicaci�n no se caiga
        SELECT 1 WHERE 1=0;
        SELECT 1 WHERE 1=0;
        SELECT 1 WHERE 1=0;
        SELECT 1 WHERE 1=0;
        SELECT 1 WHERE 1=0;
        RETURN;
    END

    -- 1. Crear la tabla temporal (ESTA L�GICA NO CAMBIA)
    SELECT
        A.NroSerie_Producto,
        A.CodSucursal,
        A.Stock_Actual -
		ISNULL((
		SELECT SUM(DP.Cantidad)
		FROM Tb_Detalle_Pedido DP INNER JOIN Tb_Orden_Pedido OP
		ON DP.NroPedido = OP.NroPedido
		WHERE OP.Estado_Pedido = 'Pendiente'
		AND DP.NroSerie_Producto = A.NroSerie_Producto), 0) as Stock_Actual,
        P.CodProducto,
        (P.Producto + ' - T:' + CAST(T.Talla AS VARCHAR(10)) + ' - C:' + P.Color) AS ProductoDescripcion,
        P.Prec_Venta_Menor,
        P.Prec_Venta_Mayor,
        M.Marca,
        MOD.Modelo,
        T.Talla,
        P.Color
    INTO
        #ProductosFiltrados
    FROM
        Tb_Almacen_Sucursal AS A
    INNER JOIN
        Tb_Producto AS P ON A.CodProducto = P.CodProducto
    INNER JOIN
        Tb_Modelo AS MOD ON P.CodModelo = MOD.CodModelo
    INNER JOIN
        Tb_Marca AS M ON MOD.CodMarca = M.CodMarca
    INNER JOIN
        Tb_Talla AS T ON P.CodTalla = T.CodTalla
    WHERE
        -- Usamos la variable @CodSucursal que acabamos de encontrar
        A.CodSucursal = @CodSucursal 
        AND A.Stock_Actual > 0
        AND (@Marca = '' OR M.Marca = @Marca)
        AND (@Modelo = '' OR MOD.Modelo = @Modelo)
        AND (@Talla = '' OR T.Talla = @Talla)
        AND (@Color = '' OR P.Color = @Color);

    
    -- DEVOLVER LOS 5 RESULT SETS (TABLAS)

    -- Resultado 1: GridView
    SELECT NroSerie_Producto, CodProducto, ProductoDescripcion, Stock_Actual, 
           Prec_Venta_Menor, Prec_Venta_Mayor
    FROM #ProductosFiltrados
    ORDER BY ProductoDescripcion;

    -- Resultado 2: Marcas
    SELECT DISTINCT Marca FROM #ProductosFiltrados ORDER BY Marca;

    -- Resultado 3: Modelos
    SELECT DISTINCT Modelo FROM #ProductosFiltrados ORDER BY Modelo;

    -- Resultado 4: Tallas
    SELECT DISTINCT Talla FROM #ProductosFiltrados ORDER BY Talla;

    -- Resultado 5: Colores
    SELECT DISTINCT Color FROM #ProductosFiltrados ORDER BY Color;

    DROP TABLE #ProductosFiltrados;
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