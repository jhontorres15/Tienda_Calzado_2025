

CREATE PROCEDURE USP_Listar_Documento_Identidad_Guia_Remision
AS
BEGIN
    SET NOCOUNT ON;

    -- Se elimina el filtro por nacionalidad y se listan todos los documentos
    SELECT CodDoc_Identidad, Documento_Identidad 
    FROM Tb_Doc_Identidad
END
GO

CREATE PROCEDURE USP_Generar_NroGuia
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NuevoNumero VARCHAR(12)
    DECLARE @UltimoNumero VARCHAR(12)

    -- 1. Obtener el último número registrado (Orden descendente)
    -- Formato esperado: GR01-0000001 (12 caracteres)
    SELECT TOP 1 @UltimoNumero = NroGuia_Remision 
    FROM Tb_Guia_Remision 
    ORDER BY NroGuia_Remision DESC

    -- 2. Calcular el nuevo número
    IF @UltimoNumero IS NULL
    BEGIN
        -- Si la tabla está vacía, iniciamos con el 1
        SET @NuevoNumero = 'GUIA-0000001'
    END
    ELSE
    BEGIN
        -- Desglozamos: 
        -- Parte Fija: 'GR01-' (5 caracteres)
        -- Parte Numérica: '0000001' (7 caracteres)
        
        DECLARE @Secuencia INT
        -- Tomamos los 7 digitos de la derecha y convertimos a Entero
        SET @Secuencia = CAST(RIGHT(@UltimoNumero, 7) AS INT) + 1

        -- Reconstruimos el formato rellenando con ceros a la izquierda
        SET @NuevoNumero = 'GUIA-' + RIGHT('0000000' + CAST(@Secuencia AS VARCHAR), 7)
    END

    -- 3. Devolver el resultado
    SELECT @NuevoNumero AS NroGenerado
END
GO

CREATE PROCEDURE USP_Listar_Tipo_Traslado
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CodTipo_Traslado, Motivo_Traslado 
    FROM Tb_Tipo_Traslado
END
GO

-- =============================================
-- 2. LISTAR TIPO DE DESTINATARIO
-- Devuelve: Código (CLIE, PROV...) y Descripción (Cliente, Proveedor...)
-- =============================================
CREATE PROCEDURE USP_Listar_Tipo_Destinatario
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CodTipo_Persona, Tipo_Persona 
    FROM Tb_Tipo_Persona
END
GO


ALTER PROCEDURE USP_Registrar_Guia_Remision
    @NroGuia_Remision CHAR(12),
    @CodEmpleado CHAR(5),
    @NroSerie_Facturacion CHAR(10),
    @Fec_Inicio_Traslado DATE,
    @Postal_Partida VARCHAR(20),
    @Direccion_Partida VARCHAR(60),
    @Postal_Llegada VARCHAR(20),
    @Direccion_Llegada VARCHAR(60),
    @CodTipo_Destinatario CHAR(4),
    @Destinatario VARCHAR(60),
    @CodDoc_Destinatario CHAR(6),
    @NroDoc_Identidad VARCHAR(20),
    @CodTipo_Traslado CHAR(5),
    @Observacion_Traslado VARCHAR(255),
    @Costo_Traslado NUMERIC(10,2),
    @RazonSocial_Transportista VARCHAR(20),
    @Ruc_Trasnportista CHAR(11), -- Ojo: Mantenemos tu nombre de columna original (con el error de tipeo si asi está en tu tabla)
    @Conductor VARCHAR(120),
    @CodDoc_Conductor CHAR(6),
    @NroDoc_Conductor VARCHAR(20),
    @NroPlaca_Vehiculo VARCHAR(10),
    @Observacion_GuiaRemision VARCHAR(255)
    -- Se ELIMINÓ el parámetro @Estado_GuiaRemision de aquí
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @CPostal_Partida CHAR(6);
        DECLARE @CPostal_Llegada CHAR(6);
      
        -- Obtener códigos postales internos basados en el nombre del distrito
        SELECT @CPostal_Partida = CPostal 
        FROM Tb_Distrito
        WHERE Distrito = @Postal_Partida;

       SELECT @CPostal_Llegada = CPostal 
        FROM Tb_Distrito
        WHERE Distrito = @Postal_Llegada;

        -- 1. Validar que la Guía no exista ya
        IF EXISTS (SELECT 1 FROM Tb_Guia_Remision WHERE NroGuia_Remision = @NroGuia_Remision)
        BEGIN
            RAISERROR('El Número de Guía de Remisión ya existe.', 16, 1);
            RETURN;
        END

        -- 2. Insertar el registro
        INSERT INTO Tb_Guia_Remision
        (
            NroGuia_Remision,
            CodEmpleado,
            NroSerie_Facturacion,
            Fec_Emision,
            Fec_Inicio_Traslado,
            CPostal_Partida,
            Direccion_Partida,
            CPostal_Llegada,
            Direccion_Llegada,
            CodTipo_Destinatario,
            Destinatario,
            CodDoc_Destinatario,
            NroDoc_Identidad,
            CodTipo_Traslado,
            Observacion_Traslado,
            Costo_Traslado,
            RazonSocial_Transportista,
            Ruc_Trasnportista,
            Conductor,
            CodDoc_Conductor,
            NroDoc_Conductor,
            NroPlaca_Vehiculo,
            Estado_GuiaRemision,      -- La columna en la tabla se mantiene
            Observacion_GuiaRemision
        )
        VALUES
        (
            @NroGuia_Remision,
            @CodEmpleado,
            @NroSerie_Facturacion,
            GETDATE(),               -- Fecha actual automática
            @Fec_Inicio_Traslado,
            @CPostal_Partida,        -- Usamos el código postal buscado arriba
            @Direccion_Partida,
            @CPostal_Llegada,        -- Usamos el código postal buscado arriba
            @Direccion_Llegada,
            @CodTipo_Destinatario,
            @Destinatario,
            @CodDoc_Destinatario,
            @NroDoc_Identidad,
            @CodTipo_Traslado,
            @Observacion_Traslado,
            @Costo_Traslado,
            @RazonSocial_Transportista,
            @Ruc_Trasnportista,
            @Conductor,
            @CodDoc_Conductor,
            @NroDoc_Conductor,
            @NroPlaca_Vehiculo,
            'Emitido',               -- <--- AQUI ESTA EL CAMBIO: Valor fijo automático
            @Observacion_GuiaRemision
        );

    END TRY
    BEGIN CATCH
        THROW; 
    END CATCH
END
GO


CREATE PROCEDURE USP_Listar_Guias_Remision
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        NroGuia_Remision,
        NroSerie_Facturacion,
        Fec_Emision,
        Fec_Inicio_Traslado,
        Destinatario,
        CodTipo_Traslado,
        Costo_Traslado,
        Estado_GuiaRemision
    FROM Tb_Guia_Remision
    ORDER BY Fec_Emision DESC; -- Muestra las guías más recientes primero
END
GO