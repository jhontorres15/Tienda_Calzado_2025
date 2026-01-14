


ALTER PROCEDURE USP_Generar_CodEmpresaProveedor
    @CodEmpresa CHAR(5) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Max INT;

    -- Tomar los últimos 3 digitos, no 2
    SELECT @Max = MAX(CAST(RIGHT(CodEmpresa, 3) AS INT)) 
    FROM Tb_Empresa_Proveedor;

    -- Si no hay registros, inicia en 1
    SET @Max = ISNULL(@Max, 0) + 1;

    -- Aquí generamos: EP + 3 dígitos
    SET @CodEmpresa = 'EP' + RIGHT('000' + CAST(@Max AS VARCHAR(3)), 3);
END
GO


CREATE PROCEDURE USP_Mantenimiento_Empresa_Proveedor
    @CodEmpresa CHAR(5),
    @Razon_Social VARCHAR(100),
    @Ruc CHAR(11),
    @CodPais CHAR(6),
    @Direccion VARCHAR(100),
    @Telefono VARCHAR(20),
    @Email VARCHAR(60),
    @Estado_Empresa VARCHAR(20),
    @Tipo_Transaccion VARCHAR(10),
    @Mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Tipo_Transaccion = 'GUARDAR'
        BEGIN
            IF EXISTS(SELECT 1 FROM Tb_Empresa_Proveedor WHERE CodEmpresa = @CodEmpresa)
            BEGIN
                SET @Mensaje = 'El código de Empresa ya existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            INSERT INTO Tb_Empresa_Proveedor(CodEmpresa, Razon_Social, Ruc, CodPais, Direccion, Telefono, Email, Estado_Empresa)
            VALUES(@CodEmpresa, @Razon_Social, @Ruc, @CodPais, @Direccion, @Telefono, @Email, @Estado_Empresa);

            SET @Mensaje = 'Empresa registrada correctamente.';
        END
        ELSE IF @Tipo_Transaccion = 'ACTUALIZAR'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Empresa_Proveedor WHERE CodEmpresa = @CodEmpresa)
            BEGIN
                SET @Mensaje = 'El código de Empresa no existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            UPDATE Tb_Empresa_Proveedor
            SET Razon_Social = @Razon_Social,
                Ruc = @Ruc,
                CodPais = @CodPais,
                Direccion = @Direccion,
                Telefono = @Telefono,
                Email = @Email,
                Estado_Empresa = @Estado_Empresa
            WHERE CodEmpresa = @CodEmpresa;

            SET @Mensaje = 'Datos de la Empresa actualizados correctamente.';
        END
        ELSE IF @Tipo_Transaccion = 'DELETE'
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM Tb_Empresa_Proveedor WHERE CodEmpresa = @CodEmpresa)
            BEGIN
                SET @Mensaje = 'El código de Empresa no existe.';
                ROLLBACK TRANSACTION; RETURN;
            END

            DELETE FROM Tb_Empresa_Proveedor WHERE CodEmpresa = @CodEmpresa;
            SET @Mensaje = 'Empresa eliminada correctamente.';
        END
        ELSE
        BEGIN
            SET @Mensaje = 'Tipo de transacción no válido.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Mensaje = 'Ocurrió un error: ' + ERROR_MESSAGE();
    END CATCH
END
GO


CREATE PROCEDURE dbo.USP_Listar_Paises
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CodPais, Pais FROM Tb_Pais ORDER BY Nacionalidad;
END
GO


ALTER PROCEDURE USP_Listar_Empresas_Proveedor
    @Filtro VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EP.CodEmpresa, EP.Razon_Social, EP.Ruc, P.Pais, EP.Direccion, EP.Telefono, EP.Email, EP.Estado_Empresa
    FROM Tb_Empresa_Proveedor EP
    LEFT JOIN Tb_Pais P ON EP.CodPais = P.CodPais
    WHERE (@Filtro = '' OR EP.Razon_Social LIKE '%' + @Filtro + '%' OR EP.Ruc LIKE '%' + @Filtro + '%')
    ORDER BY EP.Razon_Social;
END
GO
