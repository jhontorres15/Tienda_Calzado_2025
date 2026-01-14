using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

public partial class Registro_Productos : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    //Global Global = new Global();

    //crear llimpiar 
    public void limpiar()
    {
        this.Lb_CodProducto.Text = "";
        this.TXTDescripcion_Producto.Text = "";
        this.TXT_PrecVentaMayor.Text = "";
        this.TXT_PrecVentaMenor.Text = "";
        this.TXT_Producto.Text = "";
        this.TXT_Stock.Text = "";

        //seleccionar el primer elemento de los controles
        this.DDL_Categoria.SelectedIndex = 0;
        this.DDL_Color.SelectedIndex = 0;
        this.DDL_Estado.SelectedIndex = 0;
        this.DDL_Marca.SelectedIndex = 0;
        this.DDL_Modelo.SelectedIndex = 0;
        this.DDL_Talla.SelectedIndex = 0;

        // Reiniciar la imagen y el mensaje del FileUpload
        this.IMG_Calzado.ImageUrl = "/IMG/LOGO.jpeg"; // Imagen por defecto


        //ubicar el cursor en el TXT_Producto
        this.TXT_Producto.Focus();
    }

    //CREAR METODO LISTAR CATEGORIAS
    public void Listar_Categorias()
    {
        //Abrir la Corjexion con la Base de Datos
        Global.CN.Open();
        //Crear un Lector de Datos
        SqlDataReader Lector;
        //Crear un Comando de Datos
        SqlCommand CMDCategoria = new SqlCommand();
        //Conectar Comando de Datos con la Base de Datos
        CMDCategoria.Connection = Global.CN;
        //Configurar el Comando de Datos
        CMDCategoria.CommandType = System.Data.CommandType.StoredProcedure;
        CMDCategoria.CommandText = "USP_Listar_Categorias";
        //Ejecutar el Procedimiento Almacenado
        Lector = CMDCategoria.ExecuteReader();
        //Limpiar los Valores del Control: DDL_Categoria
        this.DDL_Categoria.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Categoria
        this.DDL_Categoria.Items.Add(" -- Seleccione-");
        //Leer los Registros del Lector de Datos
        while (Lector.Read())
        {
            //Agregar los Datos al Control: DDL_Categoria
            this.DDL_Categoria.Items.Add(Lector.GetValue(1).ToString());
        }

        CMDCategoria.Dispose();

        Global.CN.Close();
    }


    public void Listar_Tallas()
    {
        Global.CN.Open();
        SqlDataReader Lector;
        SqlCommand CMDTalla = new SqlCommand();
        CMDTalla.Connection = Global.CN;
        CMDTalla.CommandType = CommandType.StoredProcedure;
        CMDTalla.CommandText = "USP_Listar_Tallas";

        Lector = CMDTalla.ExecuteReader();

        DDL_Talla.Items.Clear();
        DDL_Talla.Items.Add("--Seleccione--");

        while (Lector.Read())
        {
            // Text = Talla, Value = CodTalla
            DDL_Talla.Items.Add(Lector.GetValue(1).ToString());
        }

        CMDTalla.Dispose();
        Global.CN.Close();
    }

    //CREAR METODO LISTAR MARCAS
    public void Listar_Marcas()
    {
        Global.CN.Open();
        SqlDataReader Lector;
        SqlCommand CMDMarca = new SqlCommand();
        CMDMarca.Connection = Global.CN;
        CMDMarca.CommandType = CommandType.StoredProcedure;
        CMDMarca.CommandText = "USP_Listar_Marcas";

        Lector = CMDMarca.ExecuteReader();

        this.DDL_Marca.Items.Clear();
        this.DDL_Marca.Items.Add("--Seleccione--");

        while (Lector.Read())
        {
            // Usamos código como Value y nombre como texto (opcional)
            DDL_Marca.Items.Add(Lector.GetValue(1).ToString());
        }

        CMDMarca.Dispose();
        Global.CN.Close();
    }

    public void Listar_Modelos(string Marca)
    {
        Global.CN.Open();
        SqlDataReader Lector;
        SqlCommand CMDModelo = new SqlCommand();
        CMDModelo.Connection = Global.CN;
        CMDModelo.CommandType = CommandType.StoredProcedure;
        CMDModelo.CommandText = "USP_Listar_Modelos";

        // Pasamos el nombre de la marca como parámetro
        CMDModelo.Parameters.AddWithValue("@Marca", Marca);

        Lector = CMDModelo.ExecuteReader();

        DDL_Modelo.Items.Clear();
        DDL_Modelo.Items.Add("--Seleccione--");

        while (Lector.Read())
        {
            DDL_Modelo.Items.Add(Lector.GetValue(0).ToString()); // Nombre del modelo
        }

        CMDModelo.Dispose();
        Global.CN.Close();
    }

    //crear el metodo listar el estado del articulo
    public void listarEstado()
    {
        //Limpiar los Valores de Control :DDL_Categoria
        this.DDL_Estado.Items.Clear();

        //Agregar un primer elemento al control: DDL_Categoria 
        this.DDL_Estado.Items.Add("--Seleccione--");
        this.DDL_Estado.Items.Add("Disponible");
        this.DDL_Estado.Items.Add("Agotado");
        this.DDL_Estado.Items.Add("Retirado");
    }

    public void Listar_Colores()
    {
        // Limpiar los valores del DropDownList
        this.DDL_Color.Items.Clear();

        // Agregar un primer elemento al control
        this.DDL_Color.Items.Add("--Seleccione--");

        // Agregar los colores disponibles
        this.DDL_Color.Items.Add("Negro");
        this.DDL_Color.Items.Add("Blanco");
        this.DDL_Color.Items.Add("Rojo");
        this.DDL_Color.Items.Add("Azul");
        this.DDL_Color.Items.Add("Verde");
        this.DDL_Color.Items.Add("Amarillo");
        this.DDL_Color.Items.Add("Gris");
        this.DDL_Color.Items.Add("Marrón");
    }


    public void Nuevo()
    {
        //Abrir conexion con la base de datos 
        Global.CN.Open();

        //Crer un nuevo parametro de datos
        SqlParameter ParamCodigo = new SqlParameter();

        //Indicar el tipode Parametro de datos
        ParamCodigo.Direction = ParameterDirection.Output;

        //Crear nuevo comandod e datos
        SqlCommand CMDNuevo = new SqlCommand();

        //Configurar el comandod de datos 
        CMDNuevo.Connection = Global.CN;
        CMDNuevo.CommandType = CommandType.StoredProcedure;
        CMDNuevo.CommandText = "USP_Generar_CodProducto";

        //configurar el parametro de datos 
        ParamCodigo.ParameterName = "@CodProducto";
        ParamCodigo.SqlDbType = SqlDbType.Char;
        ParamCodigo.Size = 7;

        //agregar el parametro al comando de datos 
        CMDNuevo.Parameters.Add(ParamCodigo);

        //ejecutar el comando 
        CMDNuevo.ExecuteNonQuery();

        //capturar el valor del parametro de datos 
        this.Lb_CodProducto.Text = ParamCodigo.Value.ToString();

        //liberar recursos 
        CMDNuevo.Dispose();

        //Cerrar la conexion con la base de datos 
        Global.CN.Close();
    }

    //Crear el Método: Guardar()
    public void Guardar(string Tipo_Transaccion)
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();

        //Crear Parámetros
        SqlParameter ParamCodProducto = new SqlParameter();
        SqlParameter ParamProducto = new SqlParameter();
        SqlParameter ParamDescripcion_Producto = new SqlParameter();
        SqlParameter ParamMarca = new SqlParameter();
        SqlParameter ParamTalla = new SqlParameter();
        SqlParameter ParamModelo = new SqlParameter();
        SqlParameter ParamColor = new SqlParameter();
        SqlParameter ParamGenero = new SqlParameter();
        SqlParameter ParamPrec_Venta_Menor = new SqlParameter();
        SqlParameter ParamPrec_Venta_Mayor = new SqlParameter();
        SqlParameter ParamStock_General = new SqlParameter();
        SqlParameter ParamCategoria = new SqlParameter();
        SqlParameter ParamFoto = new SqlParameter();
        SqlParameter ParamEstado_Producto = new SqlParameter();
        SqlParameter ParamTipoTransaccion = new SqlParameter();
        SqlParameter ParamMensaje = new SqlParameter();

        //Indicar el Tipo de Parámetro de Datos
        ParamMensaje.Direction = ParameterDirection.Output;

        //Crear un Nuevo Comando de Datos
        SqlCommand CMDGuardar = new SqlCommand();

        //Configurar el Comando de Datos
        CMDGuardar.Connection = Global.CN;
        CMDGuardar.CommandType = CommandType.StoredProcedure;
        CMDGuardar.CommandText = "USP_Mantenimiento_Producto";



        //Configurar los Parámetros de Datos
        ParamCodProducto.ParameterName = "@CodProducto";
        ParamCodProducto.SqlDbType = SqlDbType.Char;
        ParamCodProducto.Size = 7;
        ParamCodProducto.Value = this.Lb_CodProducto.Text.ToUpper();

        ParamProducto.ParameterName = "@Producto";
        ParamProducto.SqlDbType = SqlDbType.VarChar;
        ParamProducto.Size = 20;
        ParamProducto.Value = this.TXT_Producto.Text;

        ParamDescripcion_Producto.ParameterName = "@Descripcion_Producto";
        ParamDescripcion_Producto.SqlDbType = SqlDbType.VarChar;
        ParamDescripcion_Producto.Size = 60;
        ParamDescripcion_Producto.Value = this.TXTDescripcion_Producto.Text;

        ParamTalla.ParameterName = "@Talla";
        ParamTalla.SqlDbType = SqlDbType.Char;
        ParamTalla.Size = 5;
        ParamTalla.Value = this.DDL_Talla.SelectedValue;

        ParamModelo.ParameterName = "@Modelo";
        ParamModelo.SqlDbType = SqlDbType.VarChar;
        ParamModelo.Size = 15;
        ParamModelo.Value = this.DDL_Modelo.SelectedValue;

        ParamColor.ParameterName = "@Color";
        ParamColor.SqlDbType = SqlDbType.VarChar;
        ParamColor.Size = 20;
        ParamColor.Value = this.DDL_Color.SelectedValue;



        ParamPrec_Venta_Menor.ParameterName = "@Prec_Venta_Menor";
        ParamPrec_Venta_Menor.SqlDbType = SqlDbType.Decimal;
        ParamPrec_Venta_Menor.Value = Convert.ToDecimal(this.TXT_PrecVentaMenor.Text);

        ParamPrec_Venta_Mayor.ParameterName = "@Prec_Venta_Mayor";
        ParamPrec_Venta_Mayor.SqlDbType = SqlDbType.Decimal;
        ParamPrec_Venta_Mayor.Value = Convert.ToDecimal(this.TXT_PrecVentaMayor.Text);

        ParamStock_General.ParameterName = "@Stock_General";
        ParamStock_General.SqlDbType = SqlDbType.Int;
        ParamStock_General.Value = Convert.ToInt32(this.TXT_Stock.Text);

        ParamCategoria.ParameterName = "@Categoria";
        ParamCategoria.SqlDbType = SqlDbType.VarChar;
        ParamCategoria.Size = 40;
        ParamCategoria.Value = this.DDL_Categoria.SelectedValue;

        byte[] imagenBytes = null;

        

        ParamFoto.ParameterName = "@Foto";
        ParamFoto.SqlDbType = SqlDbType.VarBinary;
        ParamFoto.Value = imagenBytes;

        ParamEstado_Producto.ParameterName = "@Estado_Producto";
        ParamEstado_Producto.SqlDbType = SqlDbType.VarChar;
        ParamEstado_Producto.Size = 20;
        ParamEstado_Producto.Value = this.DDL_Estado.SelectedValue;

        ParamTipoTransaccion.ParameterName = "@Tipo_Transaccion";
        ParamTipoTransaccion.SqlDbType = SqlDbType.VarChar;
        ParamTipoTransaccion.Size = 10;
        ParamTipoTransaccion.Value = Tipo_Transaccion;

        ParamMensaje.ParameterName = "@Mensaje";
        ParamMensaje.SqlDbType = SqlDbType.VarChar;
        ParamMensaje.Size = 255;


        //Agregar Parámetros al Comando de Datos
        CMDGuardar.Parameters.Add(ParamCodProducto);
        CMDGuardar.Parameters.Add(ParamProducto);
        CMDGuardar.Parameters.Add(ParamDescripcion_Producto);
        CMDGuardar.Parameters.Add(ParamTalla);
        CMDGuardar.Parameters.Add(ParamModelo);
        CMDGuardar.Parameters.Add(ParamColor);
        CMDGuardar.Parameters.Add(ParamPrec_Venta_Menor);
        CMDGuardar.Parameters.Add(ParamPrec_Venta_Mayor);
        CMDGuardar.Parameters.Add(ParamStock_General);
        CMDGuardar.Parameters.Add(ParamCategoria);
        CMDGuardar.Parameters.Add(ParamFoto);
        CMDGuardar.Parameters.Add(ParamEstado_Producto);
        CMDGuardar.Parameters.Add(ParamTipoTransaccion);
        CMDGuardar.Parameters.Add(ParamMensaje);

        //Ejecutar el comadno de Datos
        CMDGuardar.ExecuteNonQuery();

        //capturar ek parametro de datos: ParamMensaje
        //Mostrar mensaje de alerta 
        Response.Write("<script language='javascript'>alert('" + ParamMensaje.Value.ToString() + "'); </script>");

        //guardar 
        CMDGuardar.Dispose();

        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Evaluar si la Págian web ha sido Recargada(Retrasada)
        if (IsPostBack == false)
        {
            //Evitar el Recordatorio de Datos en formualrio Web
            this.Form.Attributes.Add("autocomplete", "of");

            //Invocar al metodo 
            this.limpiar();
            this.Listar_Categorias();
            this.listarEstado();

            this.Listar_Marcas();
            this.Listar_Tallas();
            this.Listar_Colores();


            this.BTN_Nuevo_Click(null, null);

            if (Session["MantenimientoProductos_CodProducto"].ToString() != "")
            {
                this.cargarDatos();

                Session["MantenimientoProducto_Tipo_Transaccion"] = "ACTUALIZAR";
            }
            else
            {
                this.BTN_Nuevo_Click(null, null);
            }
        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        //invocar el metodo limpiar
        this.limpiar();

        //invocar el metodo nuevo
        this.Nuevo();

        //cambiar el boto por actualizar 
        Session["MantenimientoProducto_Tipo_Transaccion"] = "GUARDAR";
    }




    protected void DDL_Marca_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_Marca.SelectedIndex > 0)
        {
            // Aquí usamos el Value (CodMarca) para filtrar
            Listar_Modelos(DDL_Marca.SelectedItem.Text);
        }
        else
        {
            DDL_Modelo.Items.Clear();
            DDL_Modelo.Items.Add("--Seleccione--");
        }
    }

    protected void BTN_Guardar_Click(object sender, EventArgs e)
    {
        //iNVOCAR EL METODO GUARDAR
        this.Guardar(Session["MantenimientoProducto_Tipo_Transaccion"].ToString());
    }


    protected void BTN_Reporte_Click(object sender, EventArgs e)
    {
        //Redireccionar hacia el formulario reporte productos
        Response.Redirect("Reporte_Productos.aspx");
    }



    protected void btnImprimirQR_Click(object sender, EventArgs e)
    {
        string codProducto = Lb_CodProducto.Text.Trim();
        if (string.IsNullOrEmpty(codProducto))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "sinCodigo", "alert('No hay código de producto para generar el QR.');", true);
            return;
        }

        var qrWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 250,
                Width = 250
            }
        };
        using (Bitmap bitmap = qrWriter.Write(codProducto))
        {
            string carpeta = Server.MapPath("~/Codigos/");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string filePath = Path.Combine(carpeta, codProducto + ".png");
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            Response.Clear();
            Response.ContentType = "image/png";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + codProducto + ".png");
            Response.TransmitFile(filePath);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }

    protected void btnImprimirBarras_Click(object sender, EventArgs e)
    {
        string codProducto = Lb_CodProducto.Text.Trim();

        if (string.IsNullOrEmpty(codProducto))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "sinCodigo", "alert('No hay código de producto para generar el código de barras.');", true);
            return;
        }

        try
        {
            // Configuramos el escritor de código de barras
            var barcodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 300,
                    Margin = 2,
                    PureBarcode = true
                }
            };

            // Generamos el código
            var pixelData = barcodeWriter.Write(codProducto);

            // Convertimos los píxeles en un Bitmap
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // Guardamos el código en una carpeta del servidor
                string carpeta = Server.MapPath("~/CodigosBarras/");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string filePath = Path.Combine(carpeta, codProducto + "_BARRAS.png");
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                // Lo descargamos
                Response.Clear();
                Response.ContentType = "image/png";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + codProducto + "_BARRAS.png");
                Response.TransmitFile(filePath);
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error al generar código de barras: {ex.Message}');", true);
        }
    }

    public void cargarDatos()
    {

        // Llenar controles desde sesión
        this.Lb_CodProducto.Text = Session["MantenimientoProductos_CodProducto"].ToString();
        this.TXT_Producto.Text = Session["MantenimientoProductos_Producto"].ToString();
        this.TXTDescripcion_Producto.Text = Session["MantenimientoProductos_Descripcion"].ToString();
        this.DDL_Categoria.Text = Session["MantenimientoProductos_Categoria"]?.ToString();
        Listar_Marcas();
        this.DDL_Marca.Text = Session["MantenimientoProductos_Marca"].ToString();
        Listar_Modelos(DDL_Marca.SelectedValue);
        this.DDL_Modelo.Text = Session["MantenimientoProductos_Modelo"].ToString();
        this.DDL_Talla.Text = Session["MantenimientoProductos_Talla"].ToString();
        this.DDL_Color.Text = Session["MantenimientoProductos_Color"].ToString();
        this.TXT_PrecVentaMenor.Text = Session["MantenimientoProductos_Prec_Venta_Menor"].ToString();
        this.TXT_PrecVentaMayor.Text = Session["MantenimientoProductos_Prec_Venta_Mayor"].ToString();
        this.TXT_Stock.Text = Session["MantenimientoProductos_Stock"].ToString();
        this.DDL_Estado.Text = Session["MantenimientoProductos_Estado"].ToString();

        byte[] foto = (byte[])Session["MantenimientoProductos_Foto"];
        if (foto != null && foto.Length > 0)
        {
            IMG_Calzado.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(foto);
        }
        else
        {
            IMG_Calzado.ImageUrl = "/IMG/LOGO.jpeg"; // Imagen por defecto
        }
    }
}