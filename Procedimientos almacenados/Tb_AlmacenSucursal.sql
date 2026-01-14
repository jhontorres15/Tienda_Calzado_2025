


-- Procedimiento almacenados : TB_Almacen_Sucursal


CREATE PROCEDURE USP_Almacen_Sucursal
    @Sucursal varchar(20),
    @NroSerie_Producto char(7),
    @Producto varchar(10), -- Asumo que este parámetro contiene el CodProducto (char(7))
    @Stock_Actual int,
    @Stock_Minimo int,
    @Stock_Maximo int,
    @Estado_Producto varchar(20),
    @Tipo_Transaccion VARCHAR(10), -- 'GUARDAR', 'ACTUALIZAR', 'ELIMINAR'
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN 
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION
        
        DECLARE @CodSucursal char(5);
        DECLARE @CodProducto char(7);

        -- 1. Obtener el CodSucursal a partir del nombre de la Sucursal
        SELECT @CodSucursal = CodSucursal FROM Tb_Sucursal
        WHERE Sucursal = @Sucursal;
        
        -- Asumo que el parámetro @Producto es en realidad el CodProducto
        SET @CodProducto = @Producto; 
        
        -- 2. Validación de existencia de Sucursal
        IF @CodSucursal IS NULL
        BEGIN 
            SET @Mensaje ='Error: La sucursal especificada no existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Lógica para GUARDAR (INSERTAR)
        IF UPPER(@Tipo_Transaccion) = 'GUARDAR'
        BEGIN 
            -- Verificar que el NroSerie_Producto no exista en la tabla (debe ser único)
            IF EXISTS(SELECT 1 FROM Tb_Almacen_Sucursal WHERE NroSerie_Producto = @NroSerie_Producto)
            BEGIN
                SET @Mensaje = 'Error: El número de serie del producto ya está registrado.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            INSERT INTO Tb_Almacen_Sucursal
            (CodSucursal, NroSerie_Producto, CodProducto, Stock_Actual, Stock_Minimo, Stock_Maximo, Estado_Producto )
            VALUES
            (@CodSucursal, @NroSerie_Producto, @CodProducto, @Stock_Actual, @Stock_Minimo, @Stock_Maximo, @Estado_Producto);

            SET @Mensaje = 'Inventario por sucursal registrado correctamente.';
        END

        -- 4. Lógica para ACTUALIZAR
        ELSE IF UPPER(@Tipo_Transaccion) = 'ACTUALIZAR'
        BEGIN 
            -- Verificar que el registro de inventario exista para actualizar
            IF NOT EXISTS(SELECT 1 FROM Tb_Almacen_Sucursal WHERE NroSerie_Producto = @NroSerie_Producto AND CodSucursal = @CodSucursal)
            BEGIN
                SET @Mensaje = 'Error: El número de serie o la sucursal no están registrados.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            UPDATE Tb_Almacen_Sucursal
            SET Stock_Actual = @Stock_Actual,
                Stock_Minimo = @Stock_Minimo,
                Stock_Maximo = @Stock_Maximo,
                Estado_Producto = @Estado_Producto
            -- NO actualizamos CodProducto, CodSucursal o NroSerie_Producto, ya que son claves.
            WHERE NroSerie_Producto = @NroSerie_Producto
              AND CodSucursal = @CodSucursal; 

            SET @Mensaje = 'Inventario por sucursal actualizado correctamente.';
        END

        -- 5. Lógica para ELIMINAR (DELETE)
        ELSE IF UPPER(@Tipo_Transaccion) = 'ELIMINAR'
        BEGIN
            -- Verificar que el registro exista para eliminar
            IF NOT EXISTS(SELECT 1 FROM Tb_Almacen_Sucursal WHERE NroSerie_Producto = @NroSerie_Producto AND CodSucursal = @CodSucursal)
            BEGIN
                SET @Mensaje = 'Error: El registro de inventario no existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            DELETE FROM Tb_Almacen_Sucursal
            WHERE NroSerie_Producto = @NroSerie_Producto
              AND CodSucursal = @CodSucursal;

            SET @Mensaje = 'Registro de inventario eliminado correctamente.';
        END

        -- 6. Manejo de Transacción No Válida
        ELSE
        BEGIN
            SET @Mensaje = 'Error: Tipo de transacción no válido. Use GUARDAR, ACTUALIZAR o ELIMINAR.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        COMMIT TRANSACTION
    END TRY
    
    BEGIN CATCH
        -- Asegurar el ROLLBACK en caso de cualquier error interno
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @Mensaje = 'Ocurrió un error en la base de datos: ' + ERROR_MESSAGE();
    END CATCH
END
GO

ALTER PROCEDURE USP_Generar_NroSerieProducto
    @NroSerie CHAR(7) OUTPUT
AS 
BEGIN
    -- DECLARAR VARIABELES AUXILIARES
    DECLARE @Total INT
    DECLARE @Max INT

    -- OBTENER TOTAL DE REGISTROS EN LA TABLA PRODUCTO
    -- Nota: Asumo que el NroSerie está en la tabla Tb_Producto
    SELECT @Total = COUNT(*) FROM Tb_Producto

    -- EVALUAR SI EL TOTAL DE REGISTROS ES CERO
    IF (@Total = 0)
    BEGIN
        -- ESTABLECER EL CÓDIGO DE SERIE INICIAL
        SET @NroSerie = 'SER0001'
    END
    ELSE
    BEGIN
        -- OBTENER EL VALOR MÁXIMO DE LA SERIE Y SUMAR 1
        -- Asumo que el formato es SER-XXX
        SELECT @Max = MAX(CAST(RIGHT(NroSerie_Producto, 4) AS INT)) + 1 FROM Tb_Almacen_Sucursal
        
        -- Si no hay series existentes (MAX devuelve NULL), iniciamos en 1
        IF @Max IS NULL
            SET @Max = 1;

        -- LÓGICA PARA RELLENAR CON CEROS
        IF @Max > 0 AND @Max < 10
        BEGIN
            SET @NroSerie = 'SER000' + CONVERT(CHAR(1), @Max)
        END
        ELSE IF @Max >= 10 AND @Max < 100
        BEGIN
            SET @NroSerie = 'SER00' + CONVERT(CHAR(2), @Max)
        END
        ELSE IF @Max >= 100 AND @Max < 1000
        BEGIN
            SET @NroSerie = 'SER0' + CONVERT(CHAR(3), @Max)
        END
		ELSE IF @Max >= 1000 AND @Max < 10000
        BEGIN
            SET @NroSerie = 'SER' + CONVERT(CHAR(4), @Max)
        END
        ELSE
        BEGIN
            -- Manejo de desbordamiento (más de 999 series)
            SET @NroSerie = 'ERROR' -- O maneja con un prefijo diferente
        END
    END
    
    PRINT @NroSerie
END
GO


CREATE PROCEDURE USP_Listar_Marcas_Filtradas
AS
BEGIN
    -- Listar solo las marcas que aparecen en Tb_Producto
    SELECT DISTINCT T3.Marca 
    FROM Tb_Producto AS T1
    INNER JOIN Tb_Modelo AS T2 ON T1.CodModelo = T2.CodModelo
    INNER JOIN Tb_Marca AS T3 ON T2.CodMarca = T3.CodMarca
    ORDER BY T3.Marca
END
GO

CREATE PROCEDURE USP_Listar_Modelos_Filtrados
    @Marca NVARCHAR(50)
AS
BEGIN
    DECLARE @CodMarca CHAR(6);

    -- 1. Obtener el código de la marca
    SELECT @CodMarca = CodMarca
    FROM Tb_Marca
    WHERE Marca = @Marca;

    -- 2. Traer los modelos que pertenecen a esa marca Y que existen en Tb_Producto
    SELECT DISTINCT T1.Modelo
    FROM Tb_Modelo AS T1
    INNER JOIN Tb_Producto AS T2 ON T1.CodModelo = T2.CodModelo
    WHERE T1.CodMarca = @CodMarca
    ORDER BY T1.Modelo;
END
GO

CREATE PROCEDURE USP_Listar_Tallas_Filtradas_Por_Modelo
    @Marca NVARCHAR(50),
    @Modelo VARCHAR(20)
AS
BEGIN
    DECLARE @CodMarca CHAR(6);
    DECLARE @CodModelo CHAR(5);
    
    -- 1. Obtener CodMarca (Búsqueda robusta)
    SELECT @CodMarca = CodMarca 
    FROM Tb_Marca 
    WHERE LTRIM(RTRIM(Marca)) = LTRIM(RTRIM(@Marca)) COLLATE SQL_Latin1_General_CP1_CI_AS;
    
    -- 2. Obtener CodModelo (Búsqueda robusta)
    -- ESTA ES LA LÍNEA QUE FALLABA EN LA WEB POR INCONSISTENCIA DE ESPACIOS/MAYÚSCULAS
    SELECT @CodModelo = CodModelo 
    FROM Tb_Modelo 
    WHERE LTRIM(RTRIM(Modelo)) = LTRIM(RTRIM(@Modelo)) COLLATE SQL_Latin1_General_CP1_CI_AS;
    
    -- 3. Traer las tallas filtradas si el CodModelo fue resuelto
    SELECT DISTINCT T3.Talla
    FROM Tb_Producto AS T1
    INNER JOIN Tb_Talla AS T3 ON T1.CodTalla = T3.CodTalla
    WHERE T1.CodModelo = @CodModelo -- Filtra por el código resuelto
    ORDER BY T3.Talla
END
GO

CREATE PROCEDURE USP_Listar_Colores_Filtrados
    @Marca NVARCHAR(50),
    @Modelo VARCHAR(20), -- Asumiendo que Modelo es VARCHAR(20) basado en tu DDL_Modelo
    @Talla VARCHAR(5)    -- Asumiendo que Talla es VARCHAR(5) basado en tu DDL_Talla
AS
BEGIN
    DECLARE @CodMarca CHAR(6);
    DECLARE @CodTalla CHAR(5);
    DECLARE @CodModelo CHAR(5);
    
    -- 1. Obtener Códigos (Necesario para filtrar en Tb_Producto)
    SELECT @CodMarca = CodMarca FROM Tb_Marca WHERE Marca = @Marca;
    SELECT @CodModelo = CodModelo FROM Tb_Modelo WHERE Modelo = @Modelo;
    SELECT @CodTalla = CodTalla FROM Tb_Talla WHERE Talla = @Talla;

    -- 2. Traer los colores que existen en Tb_Producto para la combinación de IDs
    SELECT DISTINCT T1.Color
    FROM Tb_Producto AS T1
    WHERE T1.CodModelo = @CodModelo
      AND T1.CodTalla = @CodTalla
    ORDER BY T1.Color;
END
GO


CREATE VIEW V_Listar_Productos_Almacen
AS
SELECT TAS.CodSucursal, 
       TAS.NroSerie_Producto, 
       TAS.CodProducto, 
       TS.Sucursal, 
       TP.Producto, 
       TAS.Stock_Actual, 
       TAS.Stock_Minimo, 
       TAS.Stock_Maximo, 
       TAS.Estado_Producto
FROM dbo.Tb_Almacen_Sucursal AS TAS INNER JOIN
     dbo.Tb_Producto AS TP ON TAS.CodProducto = TP.CodProducto INNER JOIN
     dbo.Tb_Sucursal AS TS ON TAS.CodSucursal = TS.CodSucursal
GO
