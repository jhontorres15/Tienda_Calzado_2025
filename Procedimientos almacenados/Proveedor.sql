

CREATE PROCEDURE dbo.USP_Generar_CodProveedor
    @CodProveedor CHAR(6) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Max INT;
    SELECT @Max = MAX(CAST(RIGHT(CodProveedor, 3) AS INT)) FROM Tb_Proveedor;
    SET @Max = ISNULL(@Max, 0) + 1;
    SET @CodProveedor = 'PRV' + RIGHT('000' + CAST(@Max AS VARCHAR(3)), 3);
END
GO



Alter PROCEDURE dbo.USP_Listar_Proveedores
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PR.CodProveedor, PR.Apellidos, PR.Nombres, PR.Telefono, PR.Email,dbo.Tb_Departamento.Departamento, dbo.Tb_Provincia.Provincia, dbo.Tb_Distrito.Distrito, PR.Direccion,
           EP.Razon_Social, PR.Estado_Proveedor
    FROM   dbo.Tb_Provincia INNER JOIN
                         dbo.Tb_Departamento ON dbo.Tb_Provincia.CodDepartamento = dbo.Tb_Departamento.CodDepartamento INNER JOIN
                         dbo.Tb_Distrito ON dbo.Tb_Provincia.CodProvincia = dbo.Tb_Distrito.CodProvincia,

    Tb_Proveedor PR
    LEFT JOIN Tb_Empresa_Proveedor EP ON PR.CodEmpresa = EP.CodEmpresa
    WHERE (@Filtro = '' OR PR.Apellidos LIKE '%' + @Filtro + '%' OR PR.Nombres LIKE '%' + @Filtro + '%' OR EP.Razon_Social LIKE '%' + @Filtro + '%')
    ORDER BY PR.Apellidos, PR.Nombres;
END
GO

CREATE PROCEDURE USP_Listar_Empresas_Proveedor
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CodEmpresa,
        Razon_Social
    FROM Tb_Empresa_Proveedor;
END
GO


CREATE PROCEDURE USP_Mantenimiento_Proveedor
    @CodProveedor CHAR(6),
    @Apellidos VARCHAR(40),
    @Nombres VARCHAR(30),
    @Telefono VARCHAR(12),
    @Email VARCHAR(30),
    @Distrito VARCHAR(30),
    @Direccion VARCHAR(100),
    @CodEmpresa CHAR(5),
    @Estado_Proveedor VARCHAR(20),
    @Tipo_Transaccion VARCHAR(10),
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @CPostal CHAR(6);

		SELECT @CPostal = CPostal 
        FROM Tb_Distrito
        WHERE Distrito = @Distrito;



        IF @Tipo_Transaccion = 'GUARDAR'
        BEGIN
            IF EXISTS(SELECT 1 FROM Tb_Proveedor WHERE CodProveedor = @CodProveedor)
            BEGIN
                SET @Mensaje = 'El código de Proveedor ya existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            IF NOT EXISTS(SELECT 1 FROM Tb_Empresa_Proveedor WHERE CodEmpresa = @CodEmpresa)
            BEGIN
                SET @Mensaje = 'La Empresa asociada no existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            INSERT INTO Tb_Proveedor(CodProveedor, Apellidos, Nombres, Telefono, Email, CPostal, Direccion, CodEmpresa, Estado_Proveedor)
            VALUES(@CodProveedor, @Apellidos, @Nombres, @Telefono, @Email, @CPostal, @Direccion, @CodEmpresa, @Estado_Proveedor);

            SET @Mensaje = 'Proveedor registrado correctamente.';
        END
        ELSE IF @Tipo_Transaccion = 'ACTUALIZAR'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Proveedor WHERE CodProveedor = @CodProveedor)
            BEGIN
                SET @Mensaje = 'El código de Proveedor no existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            UPDATE Tb_Proveedor
            SET Apellidos = @Apellidos,
                Nombres = @Nombres,
                Telefono = @Telefono,
                Email = @Email,
                CPostal = @CPostal,
                Direccion = @Direccion,
                CodEmpresa = @CodEmpresa,
                Estado_Proveedor = @Estado_Proveedor
            WHERE CodProveedor = @CodProveedor;

            SET @Mensaje = 'Datos del Proveedor actualizados correctamente.';
        END
        ELSE IF @Tipo_Transaccion = 'DELETE'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Proveedor WHERE CodProveedor = @CodProveedor)
            BEGIN
                SET @Mensaje = 'El código de Proveedor no existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            DELETE FROM Tb_Proveedor WHERE CodProveedor = @CodProveedor;
            SET @Mensaje = 'Proveedor eliminado correctamente.';
        END
        ELSE
        BEGIN
            SET @Mensaje = 'Tipo de transacción no válido.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLlBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error: ' + ERROR_MESSAGE();
    END CATCH
END
GO
