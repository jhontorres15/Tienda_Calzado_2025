

--Procedimeintos almacenados: Tb_Cliente

CREATE PROCEDURE USP_Generar_CodCliente
@CodCliente CHAR(8) OUTPUT
AS
BEGIN
   -- Declarar variables auxiliares
   DECLARE @Total INT
   DECLARE @Max INT

   -- Obtener el total de registros en la tabla Tb_Empleado
   SELECT @Total = COUNT(CodCliente) FROM Tb_Cliente

   -- Evaluar si el total de registros es cero
   IF (@Total = 0)
   BEGIN
       -- Establecer el primer código de empleado
       SET @CodCliente = 'CLI-0001'
   END
   ELSE
   BEGIN
       -- Obtener el valor máximo numérico del código de empleado
       SELECT @Max = MAX(CAST(RIGHT(CodCliente, 4) AS INT)) + 1 FROM Tb_Cliente
       
       -- Generar el nuevo código con ceros a la izquierda según el valor
       IF (@Max BETWEEN 1 AND 9)
           SET @CodCliente = 'CLI-000' + CONVERT(CHAR(1), @Max)
       ELSE IF (@Max BETWEEN 10 AND 99)
           SET @CodCliente = 'CLI-00' + CONVERT(CHAR(2), @Max)
       ELSE IF (@Max BETWEEN 100 AND 999)
           SET @CodCliente = 'CLI-0' + CONVERT(CHAR(3), @Max)
       ELSE IF (@Max BETWEEN 1000 AND 9999)
           SET @CodCliente = 'CLI-' + CONVERT(CHAR(4), @Max)
   END

   -- Mostrar el código generado
   PRINT @CodCliente
END
GO

CREATE PROCEDURE USP_Mantenimiento_Cliente
    @CodCliente CHAR(8),
    @Apellido VARCHAR(40),
    @Nombre VARCHAR(30),
    @Fec_Nac DATE,
    @Sexo VARCHAR(10),
    @Nacionalidad VARCHAR(50),                
    @Doc_Identidad VARCHAR(50),       
    @NroDoc_Identidad VARCHAR(20),
    @Distrito VARCHAR(50),       
    @Direccion VARCHAR(100),
    @Telefono VARCHAR(12),
    @Email VARCHAR(40),
	@FecRegistro DATETIME,
    @Estado_Cliente VARCHAR(20),
    @Tipo_Transaccion VARCHAR(10),
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CodPais CHAR(6);
        DECLARE @CodDoc_Identidad CHAR(6);
        DECLARE @CPostal CHAR(6);

        -- Obtener códigos según nombres
        SELECT @CodPais = CodPais 
        FROM Tb_Pais 
        WHERE Nacionalidad = @Nacionalidad;

        SELECT @CodDoc_Identidad = CodDoc_Identidad 
        FROM Tb_Doc_Identidad
        WHERE Documento_Identidad = @Doc_Identidad;

        SELECT @CPostal = CPostal 
        FROM Tb_Distrito
        WHERE Distrito = @Distrito;

        -- Validar que todos los códigos existan antes de continuar
        IF @CodPais IS NULL OR @CodDoc_Identidad IS NULL

        BEGIN
            SET @Mensaje = 'Uno o más datos relacionados no existen en las tablas maestras.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @Tipo_Transaccion = 'GUARDAR'
        BEGIN
            IF EXISTS(SELECT 1 FROM Tb_Cliente WHERE CodCliente = @CodCliente)
            BEGIN
                SET @Mensaje = 'El código de Cliente ya existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            INSERT INTO Tb_Cliente
            (CodCliente, Apellido, Nombre, Fec_Nac, Sexo, CodPais, CodDoc_Identidad, NroDoc_Identidad,
             CPostal, Direccion, Telefono, Email, Fec_Registro, Estado_Cliente)
            VALUES
            (@CodCliente, @Apellido, @Nombre, @Fec_Nac, @Sexo, @CodPais, @CodDoc_Identidad, @NroDoc_Identidad, 
			 @CPostal, @Direccion, @Telefono, @Email,@FecRegistro, @Estado_Cliente);

            SET @Mensaje = 'Cliente registrado correctamente.';
        END

        ELSE IF @Tipo_Transaccion = 'ACTUALIZAR'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Cliente WHERE CodCliente = @CodCliente)
            BEGIN
                SET @Mensaje = 'El código de empleado no existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            UPDATE Tb_Cliente
            SET Apellido = @Apellido,
                Nombre = @Nombre,
                Fec_Nac = @Fec_Nac,
                Sexo = @Sexo,
                CodPais = @CodPais,
                CodDoc_Identidad = @CodDoc_Identidad,
                NroDoc_Identidad = @NroDoc_Identidad,
                CPostal = @CPostal,
                Direccion = @Direccion,
                Telefono = @Telefono,
                Email = @Email,
                Fec_Registro = @FecRegistro,            -- ✅ NUEVO: actualización de la foto
                Estado_Cliente = @Estado_Cliente 
            WHERE CodCliente = @CodCliente;

            SET @Mensaje = 'Datos del Cliente actualizados correctamente.';
        END

        ELSE IF @Tipo_Transaccion = 'DELETE'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Cliente WHERE CodCliente = @CodCliente)
            BEGIN
                SET @Mensaje = 'El código de Cliente no existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            DELETE FROM Tb_Cliente
            WHERE CodCliente = @CodCliente;

            SET @Mensaje = 'Cliente eliminado correctamente.';
        END

        ELSE
        BEGIN
            SET @Mensaje = 'Tipo de transacción no válido. Use INSERT, UPDATE o DELETE.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE USP_Listar_Documento_Identidad
@Nacionalidad VARCHAR(60)
AS
BEGIN
    IF UPPER(@Nacionalidad) = 'Peruana'
     BEGIN
        SELECT * FROM Tb_Doc_Identidad
        WHERE LEFT(Documento_Identidad, 3) IN ('DNI', 'RUC')
        END

        ELSE
        BEGIN
        SELECT * FROM Tb_Doc_Identidad
        WHERE LEFT(Documento_Identidad, 3) NOT LIKE '%DNI%' AND
        LEFT(Documento_Identidad,3) NOT LIKE '%RUC%'
    END
END
GO

CREATE PROCEDURE USP_Listar_Departamento
AS
BEGIN
SELECT Departamento FROM Tb_Departamento
order by Departamento
END
GO

CREATE PROCEDURE USP_Listar_Provincia
@Departamento VARCHAR(40)
AS
BEGIN
DECLARE @CodDepartamento CHAR(2)
SELECT @CodDepartamento = CodDepartamento
FROM Tb_Departamento
WHERE Departamento = @Departamento
SELECT Provincia FROM Tb_Provincia
WHERE CodDepartamento = @CodDepartamento
ORDER BY Provincia
END
GO

CREATE PROCEDURE USP_Listar_Distrito
    @Provincia VARCHAR(40)
AS
BEGIN
    DECLARE @CodProvincia CHAR(4)

    -- Obtener el código de la provincia según el nombre
    SELECT @CodProvincia = CodProvincia
    FROM Tb_Provincia
    WHERE Provincia = @Provincia

    -- Listar los distritos asociados a esa provincia
    SELECT Distrito
    FROM Tb_Distrito
    WHERE CodProvincia = @CodProvincia
    ORDER BY Distrito
END
GO

CREATE PROCEDURE USP_Listar_Nacionalidad
AS
BEGIN
SELECT Nacionalidad FROM Tb_Pais
ORDER BY Nacionalidad
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

CREATE VIEW V_Listar_Clientes
AS
SELECT dbo.Tb_Cliente.CodCliente,
	   dbo.Tb_Cliente.Apellido, 
	   dbo.Tb_Cliente.Nombre, 
	   dbo.Tb_Cliente.Fec_Nac, 
	   dbo.Tb_Cliente.Sexo, 
	   dbo.Tb_Pais.Nacionalidad, 
	   dbo.Tb_Doc_Identidad.Documento_Identidad, 
       dbo.Tb_Cliente.NroDoc_Identidad, 
	   dbo.Tb_Distrito.Distrito, 
	   dbo.Tb_Provincia.Provincia, 
	   dbo.Tb_Departamento.Departamento, 
	   dbo.Tb_Cliente.Direccion, 
	   dbo.Tb_Cliente.Telefono, 
	   dbo.Tb_Cliente.Email, 
       dbo.Tb_Cliente.Fec_Registro,
	   dbo.Tb_Cliente.Estado_Cliente
FROM            dbo.Tb_Provincia INNER JOIN
                dbo.Tb_Departamento ON dbo.Tb_Provincia.CodDepartamento = dbo.Tb_Departamento.CodDepartamento INNER JOIN
                dbo.Tb_Distrito ON dbo.Tb_Provincia.CodProvincia = dbo.Tb_Distrito.CodProvincia INNER JOIN
                dbo.Tb_Cliente ON dbo.Tb_Distrito.CPostal = dbo.Tb_Cliente.CPostal INNER JOIN
                dbo.Tb_Doc_Identidad ON dbo.Tb_Cliente.CodDoc_Identidad = dbo.Tb_Doc_Identidad.CodDoc_Identidad INNER JOIN
                dbo.Tb_Pais ON dbo.Tb_Cliente.CodPais = dbo.Tb_Pais.CodPais;


CREATE VIEW V_Listar_Clientes
AS
SELECT        dbo.Tb_Cliente.CodCliente, dbo.Tb_Cliente.Apellido, dbo.Tb_Cliente.Nombre, dbo.Tb_Cliente.Fec_Nac, dbo.Tb_Cliente.Sexo, dbo.Tb_Pais.Nacionalidad, dbo.Tb_Doc_Identidad.Documento_Identidad, 
                         dbo.Tb_Cliente.NroDoc_Identidad, dbo.Tb_Departamento.Departamento, dbo.Tb_Provincia.Provincia, dbo.Tb_Distrito.Distrito, dbo.Tb_Cliente.Direccion, dbo.Tb_Cliente.Telefono, dbo.Tb_Cliente.Email, 
                         dbo.Tb_Cliente.Fec_Registro, dbo.Tb_Cliente.Estado_Cliente
FROM            dbo.Tb_Provincia INNER JOIN
                         dbo.Tb_Departamento ON dbo.Tb_Provincia.CodDepartamento = dbo.Tb_Departamento.CodDepartamento INNER JOIN
                         dbo.Tb_Distrito ON dbo.Tb_Provincia.CodProvincia = dbo.Tb_Distrito.CodProvincia INNER JOIN
                         dbo.Tb_Cliente INNER JOIN
                         dbo.Tb_Doc_Identidad ON dbo.Tb_Cliente.CodDoc_Identidad = dbo.Tb_Doc_Identidad.CodDoc_Identidad ON dbo.Tb_Distrito.CPostal = dbo.Tb_Cliente.CPostal INNER JOIN
                         dbo.Tb_Pais ON dbo.Tb_Cliente.CodPais = dbo.Tb_Pais.CodPais

