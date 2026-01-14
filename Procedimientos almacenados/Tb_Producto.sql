

--Procedimientos almacenados para la tabla Tb_Producto


CREATE PROCEDURE USP_Listar_Categorias
AS
BEGIN
    SELECT * FROM Tb_Categoria
    ORDER BY Categoria
END
GO
-------------------------------------------------------------------
CREATE PROCEDURE USP_Listar_Marcas
AS
BEGIN
    SELECT * 
    FROM Tb_Marca
    ORDER BY Marca
END
GO
--------------------------------------------------------------------
CREATE PROCEDURE USP_Listar_Tallas
AS
BEGIN
    SELECT * FROM Tb_Talla
    ORDER BY Talla
END
GO
------------------------------------------------------------------
CREATE PROCEDURE USP_Listar_Modelos
    @Marca NVARCHAR(50)
AS
BEGIN
    DECLARE @CodMarca CHAR(6);

    -- Obtener el c�digo de la marca seg�n el nombre
    SELECT @CodMarca = CodMarca
    FROM Tb_Marca
    WHERE Marca = @Marca;

    -- Traer los modelos que pertenecen a esa marca
    SELECT Modelo
    FROM Tb_Modelo
    WHERE CodMarca = @CodMarca
    ORDER BY Modelo;
END
GO


CREATE PROCEDURE USP_Generar_CodProducto
@CodProducto CHAR(7) OUTPUT
AS 
BEGIN
   ---DECLARAR VARIABELES AUXILIARES--
   DECLARE @Total int
   DECLARE @Max int

   --OBTENER TOTAL DE REGISTROS EN LAL TABAL PRODUCTO
   SELECT @Total = COUNT(*) FROM Tb_Producto

   --EVALUAR SI EL TOTAL DE REGISTROS ES CERO
    IF (@Total=0)
	  BEGIN
	  --ESTABLECER EL CODIGO DE PRODUTO--
	   SET @CodProducto = 'PRO-001'
	  END

	ELSE
	  BEGIN
	      --OBTENER EL VALRO MAXIMO DE COD PRODUCTO--
       SELECT @Max = MAX(RIGHT(CodProducto,3)) + 1 FROM Tb_Producto
	     	   
      if @Max>0 AND @Max<10
		  BEGIN
		   SET @CodProducto = 'PRO-00'+ CONVERT(CHAR(1),@Max)
		  END
	  IF @Max>10 AND @Max<99
		   BEGIN
		   SET @CodProducto = 'PRO-0'+ CONVERT(CHAR(2),@Max)
		   END
	  IF @Max>100 AND @Max<999
		   BEGIN
		   SET @CodProducto = 'PRO-'+ CONVERT(CHAR(3),@Max)
		   END
     END
	  PRINT @CodProducto
END
GO     

SELECT * FROM Tb_Producto
go

CREATE PROCEDURE USP_Mantenimiento_Producto
    @CodProducto CHAR(7),
    @Producto VARCHAR(20),
    @Descripcion_Producto VARCHAR(200),
    @Modelo VARCHAR(20),
    @Talla VARCHAR(5),
    @Color VARCHAR(20),
    @Prec_Venta_Menor NUMERIC(10,2),
    @Prec_Venta_Mayor NUMERIC(10,2),
    @Stock_General INT,
    @Categoria VARCHAR(20),
    @Foto VARBINARY(MAX),
    @Estado_Producto VARCHAR(20),
    @Tipo_Transaccion VARCHAR(10),  -- 'Guardar' = Insertar, 'Actualizar' = Modificar
    @Mensaje VARCHAR(255) OUTPUT
AS
BEGIN
    BEGIN TRANSACTION

    DECLARE @CodCategoria CHAR(5)
    DECLARE @CodTalla CHAR(5)
    DECLARE @CodModelo CHAR(5)

    -- Obtener códigos según nombres
    SELECT @CodCategoria = CodCategoria FROM Tb_Categoria 
	WHERE Categoria = @Categoria


    SELECT @CodTalla     = CodTalla     FROM Tb_Talla
    WHERE Talla = @Talla

    SELECT @CodModelo    = CodModelo    
	FROM Tb_Modelo    WHERE Modelo = @Modelo

    -- Validar que todos los códigos se obtuvieron
        -- Validar existencia de cada uno
    IF @CodCategoria IS NULL
    BEGIN
        SET @Mensaje = 'Error: La categoría especificada no existe.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @CodTalla IS NULL
    BEGIN
        SET @Mensaje = 'Error: La talla especificada no existe.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @CodModelo IS NULL
    BEGIN
        SET @Mensaje = 'Error: El modelo especificado no existe.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Insertar producto
    IF UPPER(@Tipo_Transaccion) = 'GUARDAR'
    BEGIN
        INSERT INTO Tb_Producto
        (CodProducto, Producto, Descripcion_Producto, CodModelo, CodTalla, Color,
         Prec_Venta_Menor, Prec_Venta_Mayor, Stock_General, CodCategoria, Foto, Estado_Producto)
        VALUES
        (@CodProducto, @Producto, @Descripcion_Producto, @CodModelo, @CodTalla, @Color,
         @Prec_Venta_Menor, @Prec_Venta_Mayor, @Stock_General, @CodCategoria, @Foto, @Estado_Producto)

        SET @Mensaje = 'Registro guardado con éxito'
    END

    -- Actualizar producto
    IF UPPER(@Tipo_Transaccion) = 'ACTUALIZAR'
    BEGIN
        UPDATE Tb_Producto
        SET Producto = @Producto,
            Descripcion_Producto = @Descripcion_Producto,
            CodModelo = @CodModelo,
            CodTalla = @CodTalla,
            Color = @Color,
            Prec_Venta_Menor = @Prec_Venta_Menor,
            Prec_Venta_Mayor = @Prec_Venta_Mayor,
            Stock_General = @Stock_General,
            CodCategoria = @CodCategoria,
			Foto = ISNULL(@Foto, Foto),
            Estado_Producto = @Estado_Producto
        WHERE CodProducto = @CodProducto

        SET @Mensaje = 'Registro actualizado con éxito'
    END

    -- Confirmar transacción
    IF @@ERROR = 0
        COMMIT TRANSACTION
    ELSE
    BEGIN
        ROLLBACK TRANSACTION
        SET @Mensaje = 'Ocurrió un error: ' + ERROR_MESSAGE()
    END

    PRINT @Mensaje
END
GO


CREATE VIEW V_Listar_Productos
AS
SELECT 
    P.CodProducto,
    P.Producto,
    P.Descripcion_Producto,
    C.Categoria        AS Categoria,
    M.Marca            AS Marca,
    Mo.Modelo          AS Modelo,
    T.Talla            AS Talla,
    P.Color,
    P.Prec_Venta_Menor,
    P.Prec_Venta_Mayor,
    P.Stock_General,
    P.Foto,  -- agregamos la foto
    P.Estado_Producto
FROM Tb_Producto P
INNER JOIN Tb_Categoria C ON P.CodCategoria = C.CodCategoria
INNER JOIN Tb_Talla T     ON P.CodTalla     = T.CodTalla
INNER JOIN Tb_Modelo Mo   ON P.CodModelo    = Mo.CodModelo
INNER JOIN Tb_Marca M     ON Mo.CodMarca    = M.CodMarca;
GO
