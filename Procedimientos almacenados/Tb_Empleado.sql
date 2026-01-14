

--Procedimeintos almacenados: Tb_Empleado

CREATE PROCEDURE USP_Mantenimiento_Empleado
    @CodEmpleado CHAR(5),
    @Apellido VARCHAR(40),
    @Nombre VARCHAR(30),
    @Fec_Nac DATE,
    @Sexo VARCHAR(10),
    @Estado_Civil VARCHAR(10),
    @Nro_Hijos INT,
    @Nacionalidad VARCHAR(50),                
    @Doc_Identidad VARCHAR(50),       
    @NroDoc_Identidad VARCHAR(20),
    @Fec_Contrato DATE,
    @Fec_Termino_Contrato DATE,
    @Sucursal VARCHAR(50),            
    @Area VARCHAR(50),                
    @Cargo VARCHAR(50),               
    @Sueldo NUMERIC(10,2),
    @Distrito VARCHAR(50),       
    @Direccion VARCHAR(100),
    @Telefono VARCHAR(12),
    @Email VARCHAR(40),
    @Foto VARBINARY(MAX),             -- ✅ NUEVO: Campo para la foto
    @Estado_Empleado VARCHAR(20),
    @Obs_Estado_Empleado VARCHAR(200),
    @Tipo_Transaccion VARCHAR(10),
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY -----------
        BEGIN TRANSACTION;

        DECLARE @CodPais CHAR(6);
        DECLARE @CodDoc_Identidad CHAR(6);
        DECLARE @CodSucursal CHAR(5);
        DECLARE @CodArea CHAR(4);
        DECLARE @CodCargo CHAR(5);
        DECLARE @CPostal CHAR(6);

        -- Obtener códigos según nombres
        SELECT @CodPais = CodPais FROM Tb_Pais 
        WHERE Nacionalidad = @Nacionalidad;

        SELECT @CodDoc_Identidad = CodDoc_Identidad 
        FROM Tb_Doc_Identidad
        WHERE Documento_Identidad = @Doc_Identidad;

        SELECT @CodSucursal = CodSucursal 
        FROM Tb_Sucursal 
        WHERE Sucursal = @Sucursal;

        SELECT @CodArea = CodArea 
        FROM Tb_Area 
        WHERE Area = @Area;

        SELECT @CodCargo = CodCargo 
        FROM Tb_Cargo 
        WHERE Cargo = @Cargo;

        SELECT @CPostal = CPostal 
        FROM Tb_Distrito
        WHERE Distrito = @Distrito;

        -- Validar que todos los códigos existan antes de continuar
        IF @CodPais IS NULL OR @CodDoc_Identidad IS NULL OR 
           @CodSucursal IS NULL OR @CodArea IS NULL OR 
           @CodCargo IS NULL OR @CPostal IS NULL
        BEGIN
            SET @Mensaje = 'Uno o más datos relacionados no existen en las tablas maestras.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @Tipo_Transaccion = 'GUARDAR'
        BEGIN
            IF EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
            BEGIN
                SET @Mensaje = 'El código de empleado ya existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            INSERT INTO Tb_Empleado
            (CodEmpleado, Apellido, Nombre, Fec_Nac, Sexo, Estado_Civil, Nro_Hijos,
             CodPais, CodDoc_Identidad, NroDoc_Identidad, Fec_Contrato, Fec_Termino_Contrato,
             CodSucursal, CodArea, CodCargo, Sueldo, CPostal, Direccion,
             Telefono, Email, Foto, Estado_Empleado, Obs_Estado_Empleado)
            VALUES
            (@CodEmpleado, @Apellido, @Nombre, @Fec_Nac, @Sexo, @Estado_Civil, @Nro_Hijos,
             @CodPais, @CodDoc_Identidad, @NroDoc_Identidad, @Fec_Contrato, @Fec_Termino_Contrato,
             @CodSucursal, @CodArea, @CodCargo, @Sueldo, @CPostal, @Direccion,
             @Telefono, @Email, @Foto, @Estado_Empleado, @Obs_Estado_Empleado);

            SET @Mensaje = 'Empleado registrado correctamente.';
        END

        ELSE IF @Tipo_Transaccion = 'ACTUALIZAR'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
            BEGIN
                SET @Mensaje = 'El código de empleado no existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            UPDATE Tb_Empleado
            SET Apellido = @Apellido,
                Nombre = @Nombre,
                Fec_Nac = @Fec_Nac,
                Sexo = @Sexo,
                Estado_Civil = @Estado_Civil,
                Nro_Hijos = @Nro_Hijos,
                CodPais = @CodPais,
                CodDoc_Identidad = @CodDoc_Identidad,
                NroDoc_Identidad = @NroDoc_Identidad,
                Fec_Contrato = @Fec_Contrato,
                Fec_Termino_Contrato = @Fec_Termino_Contrato,
                CodSucursal = @CodSucursal,
                CodArea = @CodArea,
                CodCargo = @CodCargo,
                Sueldo = @Sueldo,
                CPostal = @CPostal,
                Direccion = @Direccion,
                Telefono = @Telefono,
                Email = @Email,
                 Foto = ISNULL(@Foto, Foto),                   -- ✅ NUEVO: actualización de la foto
                Estado_Empleado = @Estado_Empleado,
                Obs_Estado_Empleado = @Obs_Estado_Empleado
            WHERE CodEmpleado = @CodEmpleado;

            SET @Mensaje = 'Datos del empleado actualizados correctamente.';
        END

        ELSE IF @Tipo_Transaccion = 'DELETE'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Empleado WHERE CodEmpleado = @CodEmpleado)
            BEGIN
                SET @Mensaje = 'El código de empleado no existe.';
                ROLLBACK TRANSACTION;
                RETURN;
            END

            DELETE FROM Tb_Empleado
            WHERE CodEmpleado = @CodEmpleado;

            SET @Mensaje = 'Empleado eliminado correctamente.';
        END

        ELSE
        BEGIN
            SET @Mensaje = 'Tipo de transacción no válido. Use INSERT, UPDATE o DELETE.';
        END

        COMMIT TRANSACTION;
    END TRY---------------
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error: ' + ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE USP_Generar_CodEmpleado
@CodEmpleado CHAR(5) OUTPUT
AS
BEGIN
   -- Declarar variables auxiliares
   DECLARE @Total INT
   DECLARE @Max INT

   -- Obtener el total de registros en la tabla Tb_Empleado
   SELECT @Total = COUNT(CodEmpleado) FROM Tb_Empleado

   -- Evaluar si el total de registros es cero
   IF (@Total = 0)
   BEGIN
       -- Establecer el primer código de empleado
       SET @CodEmpleado = 'E0001'
   END
   ELSE
   BEGIN
       -- Obtener el valor máximo numérico del código de empleado
       SELECT @Max = MAX(CAST(RIGHT(CodEmpleado, 4) AS INT)) + 1 FROM Tb_Empleado
       
       -- Generar el nuevo código con ceros a la izquierda según el valor
       IF (@Max BETWEEN 1 AND 9)
           SET @CodEmpleado = 'E000' + CONVERT(CHAR(1), @Max)
       ELSE IF (@Max BETWEEN 10 AND 99)
           SET @CodEmpleado = 'E00' + CONVERT(CHAR(2), @Max)
       ELSE IF (@Max BETWEEN 100 AND 999)
           SET @CodEmpleado = 'E0' + CONVERT(CHAR(3), @Max)
       ELSE IF (@Max BETWEEN 1000 AND 9999)
           SET @CodEmpleado = 'E' + CONVERT(CHAR(4), @Max)
   END

   -- Mostrar el código generado
   PRINT @CodEmpleado
END
GO

CREATE PROCEDURE USP_Listar_Cargo
AS
BEGIN
SELECT Cargo FROM Tb_Cargo
order by Cargo
END
GO

CREATE PROCEDURE USP_Listar_Area
AS
BEGIN
SELECT Area FROM Tb_Area
END
GO

CREATE PROCEDURE USP_Listar_Empleado
AS
BEGIN
SELECT CodEmpleado + ' - ' + Nombre + ' '+ Apellido AS Empleado 
FROM Tb_Empleado
order by Nombre
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

CREATE PROCEDURE USP_Listar_Sucursal_con_codigo
AS
BEGIN
    SET NOCOUNT ON;
    
    -- ¡CORRECCIÓN!
    -- Ahora seleccionamos el CÓDIGO (CodSucursal) y el NOMBRE (Sucursal)
    SELECT 
        CodSucursal, -- Asumo que esta es tu columna de ID, ej: 'S0001'
        Sucursal     -- Esta es la columna del nombre, ej: 'gygsports'
    FROM 
        Tb_Sucursal
    ORDER BY 
        Sucursal
END
GO

CREATE PROCEDURE USP_Listar_Sucursal
AS
BEGIN
SELECT Sucursal FROM Tb_Sucursal
ORDER BY Sucursal
END
GO

CREATE PROCEDURE USP_Listar_Nacionalidad
AS
BEGIN
SELECT Nacionalidad FROM Tb_Pais
ORDER BY Nacionalidad
END
GO

