using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Descripción breve de AlmacenLOGIC
/// </summary>
public class AlmacenLOGIC
{
    AlmacenDAO AlmacenDAO = new AlmacenDAO();
    public string ObtenerNuevoNroSerie()
    {
        return AlmacenDAO.GenerarNumeroDeSerie();
    }

    public List<string> ObtenerSucursales()
    {
        return AlmacenDAO.ListarSucursal();
    }

    public List<string> ObtenerMarcasParaFiltro()
    {
        List<string> lista = AlmacenDAO.ListarMarcasFiltradas();
        lista.Insert(0, "-- Seleccione Marca --");
        return lista;
    }

    public List<string> ObtenerModelosParaFiltro(string Marca)
    {
        List<string> lista = new List<string>();

        // Regla de negocio: Si no se selecciona una Marca válida, devuelve solo el placeholder.
        if (string.IsNullOrEmpty(Marca) || Marca.Contains("-- Seleccione"))
        {
            lista.Add("-- Seleccione Modelo --");
            return lista;
        }

        // Llama al DAO con el filtro
        lista = AlmacenDAO.ListarModelosFiltrados(Marca);
        lista.Insert(0, "-- Seleccione Modelo --");
        return lista;
    }

    public List<string> ObtenerTallasParaFiltro(string Marca, string Modelo)
    {
        List<string> lista = new List<string>();

        // Regla de negocio: Si no se selecciona Marca O Modelo válidos, devuelve solo el placeholder.
        if (string.IsNullOrEmpty(Marca) || string.IsNullOrEmpty(Modelo) ||
            Marca.Contains("-- Seleccione") || Modelo.Contains("-- Seleccione"))
        {
            lista.Add("-- Seleccione Talla --");
            return lista;
        }

        // 🛑 NOTA: Tu SP de Tallas no usa Marca/Modelo como filtro. 
        // Llama al DAO y el filtrado se hará si modificas el SP o implementas la lógica aquí.
        // Por ahora, solo llamamos al método DAO, asumiendo que el filtro es global:
        // Si necesitas el filtro de Talla por Modelo, modifica el SP en SQL y el DAO.
        lista = AlmacenDAO.ListarTallasFiltradas(Marca, Modelo);
        lista.Insert(0, "-- Seleccione Talla --");
        return lista;
    }

    public List<string> ObtenerColoresParaFiltro(string Marca, string Modelo, string Talla)
    {
        List<string> lista = new List<string>();

        // Regla de negocio: Si falta cualquiera de los filtros, devuelve solo el placeholder.
        if (string.IsNullOrEmpty(Marca) || string.IsNullOrEmpty(Modelo) || string.IsNullOrEmpty(Talla) ||
            Marca.Contains("-- Seleccione") || Modelo.Contains("-- Seleccione") || Talla.Contains("-- Seleccione"))
        {
            lista.Add("-- Seleccione Color --");
            return lista;
        }

        // Llama al DAO con los tres filtros
        lista = AlmacenDAO.ListarColoresFiltrados(Marca, Modelo, Talla);
        lista.Insert(0, "-- Seleccione Color --");
        return lista;
    }

    // --------------------------------------------------------------------------------
    // 2. OBTENER PRODUCTO FINAL (LLENAR DDL_Productos)
    // --------------------------------------------------------------------------------

    public List<ListItem> ObtenerProductosFinales(string Marca, string Modelo, string Talla, string Color)
    {
        // Validación de existencia de todos los filtros necesarios
        if (string.IsNullOrEmpty(Marca) || string.IsNullOrEmpty(Modelo) ||
            string.IsNullOrEmpty(Talla) || string.IsNullOrEmpty(Color) ||
            Marca.Contains("-- Seleccione") || Modelo.Contains("-- Seleccione") ||
            Talla.Contains("-- Seleccione") || Color.Contains("-- Seleccione"))
        {
            // Devuelve una lista con solo la opción de selección si faltan filtros.
            return new List<ListItem> { new ListItem("-- Seleccione Producto --", "") };
        }

        // Llama al DAO con todos los filtros para obtener el CodProducto y la descripción.
        return AlmacenDAO.ListarProductosFinales(Marca, Modelo, Talla, Color);
    }

    public string MantenimientoAlmacen(
        string sucursal,
        string nroSerieProducto,
        string codProducto,
        int stockActual,
        int stockMinimo,
        int stockMaximo,
        string estadoProducto,
        string tipoTransaccion)
    {
        // ----------------------------------------------------
        // 1. VALIDACIONES DE NEGOCIO (Lógica que NO va en el SP)
        // ----------------------------------------------------

        // Validación 1: Campos requeridos
        if (string.IsNullOrWhiteSpace(sucursal) ||
            string.IsNullOrWhiteSpace(nroSerieProducto) ||
            string.IsNullOrWhiteSpace(codProducto))
        {
            return "Error: Los campos Sucursal, Nro. de Serie y Código de Producto son obligatorios.";
        }

        // Validación 2: Lógica de stock
        if (stockMinimo < 0 || stockMaximo < 0 )
        {
            return "Error: Los valores de Stock no pueden ser negativos.";
        }

        // Validación 3: Stock Mínimo vs. Stock Máximo
        if (stockMinimo > stockMaximo)
        {
            return "Error: El Stock Mínimo no puede ser mayor que el Stock Máximo.";
        }

        // Validación 4: Coherencia de Stock
        if (stockActual > stockMaximo)
        {
            return "Advertencia: El Stock Actual es mayor que el Stock Máximo definido.";
            // Podrías devolver un mensaje de advertencia y continuar, o detener si es una regla estricta.
            // En este caso, lo detenemos para ser estrictos antes de llamar al DAO.
            // return "Error: El Stock Actual excede el Stock Máximo permitido.";
        }

        // Aquí podrías añadir validaciones de formato (si CodProducto es CHAR(7), etc.)

        // ----------------------------------------------------
        // 2. LLAMAR A LA CAPA DE DATOS (DAO)
        // ----------------------------------------------------

        // Si todas las validaciones pasan, se llama al DAO.
        try
        {
            return AlmacenDAO.MantenimientoAlmacen(
                sucursal,
                nroSerieProducto,
                codProducto,
                stockActual,
                stockMinimo,
                stockMaximo,
                estadoProducto,
                tipoTransaccion
            );
        }
        catch (Exception ex)
        {
            // Captura de errores inesperados de la capa de datos/conexión
            // Opcional: Registrar el error en un archivo o base de datos de logs.
            return "Error crítico de la base de datos: " + ex.Message;
        }
    }

    public DataTable ListarInventarioCompleto()
    {
        try
        {
            // Llama al nuevo método del DAO
            return AlmacenDAO.ListarInventarioCompleto();
        }
        catch (Exception ex)
        {
            // Captura y maneja errores críticos.
            Console.WriteLine("Error al listar inventario: " + ex.Message);
            return new DataTable();
        }
    }

    public DataRow ObtenerFiltrosPorCodProducto(string codProducto)
    {
        DataTable dt = AlmacenDAO.ObtenerFiltrosPorCodProducto(codProducto);

        if (dt.Rows.Count > 0)
        {
            return dt.Rows[0];
        }
        else
        {
            return null;
        }
    }
}