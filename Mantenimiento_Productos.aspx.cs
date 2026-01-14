using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using static System.Net.Mime.MediaTypeNames;

public partial class Mantenimiento_Productos : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();


    //Declarar una variable que haga referencia al archivo:  LOGIC
    ProductoLOGIC ProductoLOGIC = new ProductoLOGIC();
    AlmacenLOGIC AlmacenLOGIC = new AlmacenLOGIC();

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
   //     this.DDL_Modelo.SelectedIndex = 0;
        this.DDL_Talla.SelectedIndex = 0;

        // Reiniciar la imagen y el mensaje del FileUpload
        this.IMG_Calzado.ImageUrl = "/IMG/LOGO.jpeg"; // Imagen por defecto
     

            //ubicar el cursor en el TXT_Producto
            this.TXT_Producto.Focus();
    }

    //CREAR METODO LISTAR CATEGORIAS
    public void Listar_Categorias()
    {

        DDL_Categoria.Items.Clear();
        DDL_Categoria.Items.Add("-- Seleccione --");

        var lista = ProductoLOGIC.ObtenerCategorias();

        foreach (string item in lista)
        {
            DDL_Categoria.Items.Add(item);
        }
    }

 

    public void Listar_Tallas()
    {
        // Limpiar el control
        DDL_Talla.Items.Clear();
        DDL_Talla.Items.Add("--Seleccione--");

        // Llamar a la capa Lógica
        var lista = ProductoLOGIC.ObtenerTallas();

        foreach (string item in lista)
        {
            // Agregar los nombres/valores de las tallas
            DDL_Talla.Items.Add(item);
        }

    }



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
        this.DDL_Color.Items.Add("Rosado");



    }




    public void Nuevo()
    {
        string codigo = ProductoLOGIC.ObtenerNuevoCodigo();
        Lb_CodProducto.Text = codigo;
    }

    protected void GV_Productos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // 1. Asigna el nuevo índice de página al GridView
        GV_Productos.PageIndex = e.NewPageIndex;

        // 2. Vuelve a cargar los datos del GridView
        Listar_Productos();
    }


    public void Guardar(string Tipo_Transaccion)
    {

        // Preparar cadenas numéricas (el LOGIC/DAO se encarga del TryParse seguro)
        string textoMenor = this.TXT_PrecVentaMenor.Text.Trim().Replace(",", ".");
        string textoMayor = this.TXT_PrecVentaMayor.Text.Trim().Replace(",", ".");

        // Preparar bytes de la foto
        byte[] imagenBytes = null;
        if (FileUpload1.HasFile)
        {
            using (var fs = FileUpload1.PostedFile.InputStream)
            {
                using (var br = new BinaryReader(fs))
                {
                    imagenBytes = br.ReadBytes((int)fs.Length);
                }
            }
        }



        // --- 2. LLAMADA A LA CAPA LÓGICA (Ejecución del Mantenimiento) ---

        string mensajeResultado = ProductoLOGIC.GuardarProducto(
            // Cadenas de Controles:
            this.Lb_CodProducto.Text.Trim(),
            this.TXT_Producto.Text,
            this.TXTDescripcion_Producto.Text,
            this.DDL_Talla.SelectedValue,
            this.DDL_Modelo.SelectedValue,
            this.DDL_Color.SelectedValue,

            // Cadenas Numéricas Limpias:
            textoMenor,
            textoMayor,
            this.TXT_Stock.Text,

            // Datos de Selección y Binarios:
            this.DDL_Categoria.SelectedValue,
            imagenBytes,
            this.DDL_Estado.SelectedValue,
            Tipo_Transaccion
        );

        // --- 3. RESPUESTA AL USUARIO ---

        // Determinar el tipo de mensaje a mostrar (ej. "success", "error", "warning")
        // Por defecto, usamos "info" si no sabemos si es éxito o error.
        string tipoMensaje = "success";

        // 🚀 Llamar al método MostrarMensaje para usar SweetAlert2 🚀
        MostrarMensaje(mensajeResultado, tipoMensaje);

        Session["MantenimientoProductos_CodProducto"] = "";
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }

  

   

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Listar_Productos();

        //Evaluar si la Págian web ha sido Recargada(Retrasada)
        if (!IsPostBack)
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
            this.Listar_Categorias();
          
           
         

           
                // ... (Tu código de carga de DropDownLists) ...

                // --- LÓGICA DE PRODUCTO PRINCIPAL ---
                string codProductoSession = (Session["MantenimientoProductos_CodProducto"] != null) ? Session["MantenimientoProductos_CodProducto"].ToString() : "";

                if (codProductoSession != "")
                {
                    // MODO EDICIÓN
                    // this.cargarDatos(); // Cargar datos del producto
                    Session["MantenimientoProducto_Tipo_Transaccion"] = "ACTUALIZAR";
                }
                else
                {
                    // 💡 MODO NUEVO: Inicialización directa 💡
                    this.limpiar();
                    this.Nuevo(); // Genera el CodProducto
                    Session["MantenimientoProducto_Tipo_Transaccion"] = "GUARDAR";
                }

 
            

        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        // 1. Reiniciar la sesión de código para forzar un nuevo registro
        Session["MantenimientoProductos_CodProducto"] = "";

        // 2. Limpiar la UI
        this.limpiar();

        // 3. Generar nuevo código
        this.Nuevo();

        // 4. Establecer el modo
        Session["MantenimientoProducto_Tipo_Transaccion"] = "GUARDAR";

    }

   

    protected void BTN_Guardar_Click(object sender, EventArgs e)
    {

        // Validar datos del lado del servidor
        if (string.IsNullOrEmpty(Lb_CodProducto.Text) )
        {
            MostrarMensaje("No hay codigo registrado.", "warning");
            return;
        }

        if (string.IsNullOrEmpty(TXTDescripcion_Producto.Text))
        {
            MostrarMensaje("Descripcion_Producto son obligatorios.", "warning");
            return;
        }

        if (string.IsNullOrEmpty(TXT_Stock.Text))
        {
            MostrarMensaje("La fecha de nacimiento es obligatoria.", "warning");
            return;
        }

        if (DDL_Modelo.SelectedValue == "")
        {
            MostrarMensaje("Debe seleccionar el sexo.", "warning");
            return;
        }
        //iNVOCAR EL METODO GUARDAR
        this.Guardar(Session["MantenimientoProducto_Tipo_Transaccion"].ToString());
       
       Listar_Productos();
        Session["MantenimientoProductos_CodProducto"] = "";
        limpiar();
        Nuevo();
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje",
         $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensaje}', confirmButtonText: 'Aceptar' }});", true);
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


    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        string filtro = TXT_BusquedaProducto.Text.Trim();

        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM V_Listar_Productos WHERE Producto LIKE @filtro OR CodProducto LIKE @filtro", Global.CN);
        da.SelectCommand.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
        da.Fill(dt);

        GV_Productos.DataSource = dt;
        GV_Productos.DataBind();
    }

    protected void GV_Productos_RowCommand(object source, GridViewCommandEventArgs e)
    {
        //Evaluar si el nombre del boton es : BTN_Ver
        if (e.CommandName == "BTN_Ver")
        {
            //obtener el indice del comandos seleccionado
            int indice = Convert.ToInt32(e.CommandArgument);

            // Acceso a datos de texto
            Session["MantenimientoProductos_CodProducto"] = this.GV_Productos.DataKeys[indice]["CodProducto"].ToString();
            Session["MantenimientoProductos_Producto"] = this.GV_Productos.DataKeys[indice]["Producto"].ToString();
            Session["MantenimientoProductos_Descripcion"] = this.GV_Productos.DataKeys[indice]["Descripcion_Producto"].ToString();
            Session["MantenimientoProductos_Categoria"] = this.GV_Productos.DataKeys[indice]["Categoria"].ToString();
            Session["MantenimientoProductos_Marca"] = this.GV_Productos.DataKeys[indice]["Marca"].ToString();
            Session["MantenimientoProductos_Modelo"] = this.GV_Productos.DataKeys[indice]["Modelo"].ToString();
            Session["MantenimientoProductos_Talla"] = this.GV_Productos.DataKeys[indice]["Talla"].ToString();
            Session["MantenimientoProductos_Color"] = this.GV_Productos.DataKeys[indice]["Color"].ToString();
            Session["MantenimientoProductos_Prec_Venta_Menor"] = this.GV_Productos.DataKeys[indice]["Prec_Venta_Menor"].ToString();
            Session["MantenimientoProductos_Prec_Venta_Mayor"] = this.GV_Productos.DataKeys[indice]["Prec_Venta_Mayor"].ToString();
            Session["MantenimientoProductos_Stock"] = this.GV_Productos.DataKeys[indice]["Stock_General"].ToString();
            Session["MantenimientoProductos_Estado"] = this.GV_Productos.DataKeys[indice]["Estado_Producto"].ToString();

            Session["MantenimientoProductos_Foto"] = this.GV_Productos.DataKeys[indice]["Foto"];

            Session["MantenimientoProducto_Tipo_Transaccion"] = "ACTUALIZAR";

            cargarDatos();


        }
    }

    public void cargarDatos()
    {

        // Llenar controles desde sesión
        this.Lb_CodProducto.Text = Session["MantenimientoProductos_CodProducto"].ToString();
        this.TXT_Producto.Text = Session["MantenimientoProductos_Producto"].ToString();
        this.TXTDescripcion_Producto.Text = Session["MantenimientoProductos_Descripcion"].ToString();

        this.DDL_Categoria.SelectedValue = Session["MantenimientoProductos_Categoria"]?.ToString();
        Listar_Marcas();
        this.DDL_Marca.SelectedValue = Session["MantenimientoProductos_Marca"].ToString();
        Listar_Modelos(DDL_Marca.SelectedValue);
        this.DDL_Modelo.SelectedValue = Session["MantenimientoProductos_Modelo"].ToString();
        this.DDL_Talla.Text = Session["MantenimientoProductos_Talla"].ToString();
        this.DDL_Color.Text = Session["MantenimientoProductos_Color"].ToString();
        this.TXT_PrecVentaMenor.Text = Session["MantenimientoProductos_Prec_Venta_Menor"].ToString();
        this.TXT_PrecVentaMayor.Text = Session["MantenimientoProductos_Prec_Venta_Mayor"].ToString();
        this.TXT_Stock.Text = Session["MantenimientoProductos_Stock"].ToString();
        this.DDL_Estado.Text = Session["MantenimientoProductos_Estado"].ToString();

        //byte[] fotoObj = (byte[])Session["MantenimientoProductos_Foto"];

        object fotoObj = Session["MantenimientoProductos_Foto"];

        if (fotoObj is byte[] fotoBytes && fotoBytes.Length > 0)
        {
            IMG_Calzado.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(fotoBytes);
        }
        else
        {
            IMG_Calzado.ImageUrl = "/IMG/LOGO.jpeg"; // Imagen por defecto
        }

    }

    public void Listar_Productos()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Nuevo DataTable
        DataTable DT = new DataTable();
        //Crear un Nuevo Adaptador de Datos
        SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM V_Listar_Productos", Global.CN);
        //Cargar Datos del Adaptador de Datos a un DataTable
        DA.Fill(DT);
        //Cargar Datos del DataTable en el Control: GV_Articulos
        GV_Productos.DataSource = DT;
        GV_Productos.DataBind();

        //Liberar Recursos del Adaptador de Datos
        DA.Dispose();
        //Cerrar Conexión con la Base de Datos

        Global.CN.Close();
    }



}

