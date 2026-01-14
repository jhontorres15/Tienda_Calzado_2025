


CREATE PROCEDURE USP_Generar_NroEntrega
    @NroEntrega CHAR(12) OUTPUT
AS
BEGIN
    DECLARE @UltimoId VARCHAR(12)
    DECLARE @NuevoNumero INT

    -- 1. Buscamos el último código (Ej: E-0000000005)
    SELECT TOP 1 @UltimoId = NroEntrega 
    FROM Tb_Entrega 
    ORDER BY NroEntrega DESC

    -- 2. Si no hay registros, empezamos con el 1
    IF @UltimoId IS NULL
    BEGIN
        -- E- seguido de 9 ceros y un 1 (Total 12 caracteres)
        SET @NroEntrega = 'E-0000000001'
    END
    ELSE
    BEGIN
        -- 3. Quitamos los 2 primeros caracteres ('E-') y convertimos el resto a número
        -- SUBSTRING desde la posición 3, tomamos 10 caracteres
        SET @NuevoNumero = CAST(SUBSTRING(@UltimoId, 3, 10) AS INT) + 1

        -- 4. Formateamos: 'E-' + el número rellenado con ceros a la izquierda hasta llegar a 10 dígitos
        SET @NroEntrega = 'E-' + RIGHT('0000000000' + CAST(@NuevoNumero AS VARCHAR), 10)
    END
END
GO


CREATE PROCEDURE USP_Listar_TipoEntrega
AS
BEGIN
    SELECT CodTipo_Entrega, Tipo_Entrega 
    FROM Tb_Tipo_Entrega
    ORDER BY CodTipo_Entrega ASC
END
GO