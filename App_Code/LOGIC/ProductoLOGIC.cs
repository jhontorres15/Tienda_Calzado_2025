using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ProductoLOGIC
/// </summary>
public class ProductoLOGIC
{

    
    ProductoDAO ProductoDAO = new ProductoDAO();


/// <summary>
    /// GENERA EL COD DEL PRODUCTO CON CADA REGISTRO
    /// </summary>
    public String ObtenerNuevoCodigo()
    {
         return ProductoDAO.GenerarCodigoProducto();
       
    }
    

    public List<string> ObtenerCategorias()
    {
        return ProductoDAO.ListarCategorias();
    }


    public List<string> ObtenerMarcas()
    {
        return ProductoDAO.ListarMarcas();
    }

    public List<string> ObtenerModelos(string Marca)
    {
        return ProductoDAO.ListarModelos(Marca);
    }

    public List<string> ObtenerTallas()
    {
        return ProductoDAO.ListarTallas();
    }



    public string GuardarProducto(
        string CodProducto, string Producto, string Descripcion_Producto, string Talla, string Modelo,
        string Color, string Prec_Venta_Menor, string Prec_Venta_Mayor, string Stock_General,
        string Categoria, byte[] FotoBytes, string Estado_Producto, string Tipo_Transaccion)
    {
        // === EJEMPLO DE REGLAS DE NEGOCIO / VALIDACIONES EN LA CAPA LOGIC ===

        // 1. Validación de datos obligatorios
        if (string.IsNullOrEmpty(Producto) || string.IsNullOrEmpty(Talla) || string.IsNullOrEmpty(Categoria))
        {
            return "Error: El Nombre del Producto, Talla y Categoría son obligatorios.";
        }

        // 3. Validación de longitud (opcional, aunque SQL lo restringe)
        if (Producto.Length > 20)
        {
            return "Error: El nombre del producto es demasiado largo (máx. 20 caracteres).";
        }

        // Si todas las validaciones pasan, se llama a la capa de datos.
        string mensaje = ProductoDAO.GuardarProducto(
            CodProducto, Producto, Descripcion_Producto, Talla, Modelo, Color,
            Prec_Venta_Menor, Prec_Venta_Mayor, Stock_General, Categoria,
            FotoBytes, Estado_Producto, Tipo_Transaccion
        );

        // Regla de negocio post-ejecución: Analizar el mensaje y devolver un resultado estándar.
        if (mensaje.StartsWith("Error"))
        {
            // Podrías registrar el error aquí
            return mensaje;
        }

        return mensaje;
    }
}