using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

public partial class Mantenimiento_AlmacenSucursal : System.Web.UI.Page
{

    //Declarar una variable que haga referencia al archivo:  LOGIC
    ProductoLOGIC ProductoLOGIC = new ProductoLOGIC();
    AlmacenLOGIC AlmacenLOGIC = new AlmacenLOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Evitar el Recordatorio de Datos en formualrio Web
            this.Form.Attributes.Add("autocomplete", "of");

            //Invocar al metodo 
            this.listarEstado();
            Listar_Marcas();
            this.BTN_GuardarCambios.Text = "Guardar";
            this.GenerarNroSerie();
            this.Listar_Sucursal();
            ListarInventarioGrid();



            // --- LÓGICA DE ALMACÉN/MODAL ---
            string NroSerieProductoSession = (Session["MantenimientoAlmacenProducto_NroSerie_Producto"] != null) ? Session["MantenimientoAlmacenProducto_NroSerie_Producto"].ToString() : "";

            if (NroSerieProductoSession != "")
            {
                // MODO EDICIÓN
                // this.cargarDatosAlmacen(); // Cargar datos del almacén
                Session["MantenimientoAlmacenProducto_Tipo_Transaccion"] = "ACTUALIZAR";
            }
            else
            {
                // 💡 MODO NUEVO: Inicialización directa 💡
                this.LimpiarModal(); // Asumiendo que existe este método
                this.GenerarNroSerie();
                Session["MantenimientoAlmacenProducto_Tipo_Transaccion"] = "GUARDAR";
            }
        }
    }

    public void listarEstado()
    {
        //Limpiar los Valores de Control :DDL_Categoria
 
        this.DDL_Estado.Items.Clear();
        this.DDL_Estado.Items.Add("--Seleccione--");
        this.DDL_Estado.Items.Add("Disponible");
        this.DDL_Estado.Items.Add("Agotado");
        this.DDL_Estado.Items.Add("Retirado");
    }

    //CREAR METODO LISTAR MARCAS
    public void Listar_Marcas()
    {

        // Limpiar el control
        this.DDL_FiltroMarca.Items.Clear();
        this.DDL_FiltroMarca.Items.Add("--Seleccione--");

        // Llamar a la capa Lógica
        var lista3 = ProductoLOGIC.ObtenerMarcas();

        foreach (string item2 in lista3)
        {
            // Agregar los nombres de las marcas a la lista
            this.DDL_FiltroMarca.Items.Add(item2);
        }


    }
    void Listar_Sucursal()
    {

        var lista = AlmacenLOGIC.ObtenerSucursales();

        DDL_Sucursal.Items.Clear();
        DDL_Sucursal.Items.Add("--Seleccione--");

        foreach (var item in lista)
        {
            DDL_Sucursal.Items.Add(item);
        }
    }
    public void GenerarNroSerie()
    {
        // Obtener el valor desde la capa de servicio
        string serie = AlmacenLOGIC.ObtenerNuevoNroSerie();

        // Asignar a tu TextBox de Número de Serie (ej: TXT_NroSerie)
        this.TXT_NroSerie.Text = serie;
    }
    protected void BTN_Nuevo_Almacen_Click(object sender, EventArgs e)
    {

        // 1. Reiniciar la sesión de serie
        Session["MantenimientoAlmacenProducto_NroSerie_Producto"] = "";

        // 2. Limpiar la UI del modal
        this.LimpiarModal(); // Asumiendo que tienes este método

        // 3. Generar nueva serie
        this.GenerarNroSerie();

        // 4. Establecer el modo
        Session["MantenimientoAlmacenProducto_Tipo_Transaccion"] = "GUARDAR";

        this.BTN_GuardarCambios.Text = "Guardar";

    }

    public void ListarInventarioGrid() // 🛑 Parámetro filtroSucursal ELIMINADO 🛑
    {
        DataTable dtInventarioCompleto;

        try
        {

            dtInventarioCompleto = AlmacenLOGIC.ListarInventarioCompleto();

            // 2. Enlazar el resultado DIRECTAMENTE al GridView
            GV_InventarioSucursal.DataSource = dtInventarioCompleto; // No se necesita DataView
            GV_InventarioSucursal.DataBind();
        }
        catch (Exception ex)
        {
            // Manejo de errores
            // Opcional: Mostrar un mensaje SweetAlert de error si la conexión falla
            // MostrarMensaje("Error al cargar el inventario: " + ex.Message, "error");

            // Asignar null o new DataTable() para evitar errores de enlace
            GV_InventarioSucursal.DataSource = null;
            GV_InventarioSucursal.DataBind();
        }

        // 3. Actualizar el UpdatePanel
        //    UP_AlmacenGestor.Update();
    }

    protected void DDL_FiltroMarca_Changed(object sender, EventArgs e)
    {
        // Limpiar DDLs dependientes
        DDL_FiltroModelo.Items.Clear();
        DDL_FiltroTalla.Items.Clear();
        DDL_FiltroColor.Items.Clear();
        DDL_Productos.Items.Clear();

        string marca = DDL_FiltroMarca.SelectedValue;

        // Llamar al LOGIC para obtener los Modelos filtrados
        var listaModelos = AlmacenLOGIC.ObtenerModelosParaFiltro(marca);

        // Llenar DDL_FiltroModelo
        foreach (string modelo in listaModelos)
        {
            DDL_FiltroModelo.Items.Add(modelo);
        }

        // 💡 IMPORTANTE: Llamar al siguiente evento en cascada para precargar la Talla.
        // Usamos el mismo patrón DDL_FiltroModelo_Changed para precargar la Talla.
        DDL_FiltroModelo_Changed(null, null);
    }

    protected void DDL_FiltroModelo_Changed(object sender, EventArgs e)
    {
        // Limpiar DDLs dependientes
        DDL_FiltroTalla.Items.Clear();
        DDL_FiltroColor.Items.Clear();
        DDL_Productos.Items.Clear();

        // 1. Obtener filtros
        string marca = DDL_FiltroMarca.SelectedValue;
        string modelo = DDL_FiltroModelo.SelectedValue;

        // 2. Llamar al LOGIC para obtener las Tallas filtradas
        var listaTallas = AlmacenLOGIC.ObtenerTallasParaFiltro(marca, modelo);

        // 3. Llenar DDL_FiltroTalla
        foreach (string talla in listaTallas)
        {
            DDL_FiltroTalla.Items.Add(talla);
        }

        // 💡 IMPORTANTE: Llamar al siguiente evento en cascada para precargar el Color.
        DDL_FiltroTalla_Changed(null, null);
    }

    protected void DDL_FiltroTalla_Changed(object sender, EventArgs e)
    {
        // Limpiar DDLs dependientes
        DDL_FiltroColor.Items.Clear();
        DDL_Productos.Items.Clear();

        // 1. Obtener filtros
        string marca = DDL_FiltroMarca.SelectedValue;
        string modelo = DDL_FiltroModelo.SelectedValue;
        string talla = DDL_FiltroTalla.SelectedValue;

        // 2. Llamar al LOGIC para obtener los Colores filtrados
        var listaColores = AlmacenLOGIC.ObtenerColoresParaFiltro(marca, modelo, talla);

        // 3. Llenar DDL_FiltroColor
        foreach (string color in listaColores)
        {
            DDL_FiltroColor.Items.Add(color);
        }

        // 💡 IMPORTANTE: Llamar al evento final para precargar la lista de Productos.
        DDL_FiltroColor_Changed(null, null);
    }

    protected void DDL_FiltroColor_Changed(object sender, EventArgs e)
    {
        // Limpiar DDL de resultado final
        DDL_Productos.Items.Clear();

        // 1. Obtener todos los filtros
        string marca = DDL_FiltroMarca.SelectedValue;
        string modelo = DDL_FiltroModelo.SelectedValue;
        string talla = DDL_FiltroTalla.SelectedValue;
        string color = DDL_FiltroColor.SelectedValue;

        // 2. Llamar al LOGIC para obtener la lista de productos finales (CodProducto + Descripción)
        var listaProductos = AlmacenLOGIC.ObtenerProductosFinales(marca, modelo, talla, color);

        // 3. Llenar DDL_Productos
        foreach (ListItem item in listaProductos)
        {
            DDL_Productos.Items.Add(item);
        }
    }

    protected void BTN_Registrar_Almacen_Click(object sender, EventArgs e)
    {
        GuardarAlmacen(Session["MantenimientoAlmacenProducto_Tipo_Transaccion"].ToString());
        HttpContext.Current.ApplicationInstance.CompleteRequest();
        LimpiarModal();
        ListarInventarioGrid();
        GenerarNroSerie();

        
    }

 


    public void GuardarAlmacen(string Tipo_Transaccion)
    {
        // 1. Capturar datos de la UI y convertirlos (igual que antes)
        string sucursal = DDL_Sucursal.SelectedValue.ToString();
        string nroSerie = TXT_NroSerie.Text;
        string textoCompleto = DDL_Productos.Text;
        string codProducto = textoCompleto.Substring(0, 7);

        // Usamos TryParse para evitar errores si el usuario ingresa texto en lugar de números
        int.TryParse(txtStockActual.Text, out int stockActual);
        int.TryParse(txtStockMinimo.Text, out int stockMinimo);
        int.TryParse(txtStockMaximo.Text, out int stockMaximo);

        string estado = DDL_Estado.SelectedValue.ToString();
        string transaccion = Tipo_Transaccion;

        // 2. Llamar a la BLL

        string mensajeResultado = AlmacenLOGIC.MantenimientoAlmacen(
            sucursal,
            nroSerie,
            codProducto,
            stockActual,
            stockMinimo,
            stockMaximo,
            estado,
            transaccion
        );

        // 3. Mostrar el resultado (usando tu método SweetAlert)
        string tipoAlerta = mensajeResultado.Contains("Error") ? "error" : "success";
        MostrarMensaje(mensajeResultado, tipoAlerta);
        this.BTN_GuardarCambios.Text = "Guardar";
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje",
         $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensaje}', confirmButtonText: 'Aceptar' }});", true);
    }

    protected void btnImprimirQR_Click(object sender, EventArgs e)
    {
        string codProducto = TXT_NroSerie.Text.Trim();
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
        string codProducto = TXT_NroSerie.Text.Trim();

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

    private void LimpiarModal()
    {
        this.DDL_FiltroMarca.SelectedIndex = 0;
        this.DDL_Sucursal.SelectedIndex = 0;
        this.DDL_Estado.SelectedIndex = 0;
        DDL_FiltroModelo.Items.Clear();
        DDL_FiltroTalla.Items.Clear();
        DDL_FiltroColor.Items.Clear();
        DDL_Productos.Items.Clear();
        txtStockMaximo.Text = "";
        txtStockMaximo.Text = "";

    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarModal();
    }

    protected void GV_InventarioSucursal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // 1. Verificamos que el comando sea el de "Editar"
        if (e.CommandName == "CMD_EDITAR")
        {
            // 2. Obtenemos el índice de la fila desde el CommandArgument
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            // 3. Obtenemos los DataKeys de esa fila (¡la forma más segura!)
            DataKey key = GV_InventarioSucursal.DataKeys[rowIndex];

            string nroSerie = key.Values["NroSerie_Producto"].ToString();
        
            string codProducto = key.Values["CodProducto"].ToString();

            // 4. Obtenemos la fila completa para leer los campos BoundField
            GridViewRow row = GV_InventarioSucursal.Rows[rowIndex];
            string nombreSucursal = System.Web.HttpUtility.HtmlDecode(row.Cells[1].Text);
            // 5. Leemos los valores de las celdas (usamos HtmlDecode por seguridad)
            // NOTA: El índice de la celda depende del orden en tu GridView
            string stockActual = System.Web.HttpUtility.HtmlDecode(row.Cells[3].Text);
            string stockMinimo = System.Web.HttpUtility.HtmlDecode(row.Cells[4].Text);
            string stockMaximo = System.Web.HttpUtility.HtmlDecode(row.Cells[5].Text);
            string estado = System.Web.HttpUtility.HtmlDecode(row.Cells[6].Text);

            // --- 6. LLENAMOS EL FORMULARIO ---

            // A. Campos de Texto Simples
            TXT_NroSerie.Text = nroSerie;
            txtStockActual.Text = stockActual;
            txtStockMinimo.Text = stockMinimo;
            txtStockMaximo.Text = stockMaximo;

            // B. DropDownLists Simples (Sucursal y Estado)
            DDL_Sucursal.ClearSelection();

            // Busca por el NOMBRE que leíste de la celda
            ListItem itemSucursal = DDL_Sucursal.Items.FindByValue(nombreSucursal);

            // (Opcional pero seguro) Si tu DDL usa el Texto y no el Valor:
            if (itemSucursal == null)
            {
                itemSucursal = DDL_Sucursal.Items.FindByText(nombreSucursal);
            }

            // ¡Comprobamos que no sea null ANTES de seleccionar!
            if (itemSucursal != null)
            {
                itemSucursal.Selected = true;
            }

            DDL_Estado.ClearSelection();
            // Si usa un ID (ej. 1, 0), deberías tener ese ID en tus DataKeyNames
            DDL_Estado.Items.FindByText(estado).Selected = true;



            // C. LOS DROPDOWNS EN CASCADA
            DataRow filtros = AlmacenLOGIC.ObtenerFiltrosPorCodProducto(codProducto);

            if (filtros != null)
            {
                // 1. Obtenemos los NOMBRES de los filtros
                string nombreMarca = filtros["Marca"].ToString();
                string nombreModelo = filtros["Modelo"].ToString();
                string nombreTalla = filtros["Talla"].ToString();
                string nombreColor = filtros["Color"].ToString();

                // 2. Llenar y seleccionar MARCA
                DDL_FiltroMarca.ClearSelection();
                // Tus SPs usan el NOMBRE como VALOR
                ListItem itemMarca = DDL_FiltroMarca.Items.FindByValue(nombreMarca);
                if (itemMarca != null)
                {
                    itemMarca.Selected = true;
                }

                // 3. Forzar la carga de MODELOS y seleccionar
                DDL_FiltroMarca_Changed(null, null); // Ejecuta el evento para cargar Modelos
                DDL_FiltroModelo.ClearSelection();
                // Tu SP 'USP_Listar_Productos_Finales' confirma que usas el NOMBRE del modelo
                ListItem itemModelo = DDL_FiltroModelo.Items.FindByValue(nombreModelo);
                if (itemModelo != null)
                {
                    itemModelo.Selected = true;
                }

                // 4. Forzar la carga de TALLAS y seleccionar
                DDL_FiltroModelo_Changed(null, null); // Ejecuta el evento para cargar Tallas
                DDL_FiltroTalla.ClearSelection();
                // Tu SP 'USP_Listar_Productos_Finales' confirma que usas el NOMBRE de la talla
                ListItem itemTalla = DDL_FiltroTalla.Items.FindByValue(nombreTalla);
                if (itemTalla != null)
                {
                    itemTalla.Selected = true;
                }

                // 5. Forzar la carga de COLORES y seleccionar
                DDL_FiltroTalla_Changed(null, null); // Ejecuta el evento para cargar Colores
                DDL_FiltroColor.ClearSelection();
                // Tu SP 'USP_Listar_Productos_Finales' confirma que usas el NOMBRE del color
                ListItem itemColor = DDL_FiltroColor.Items.FindByValue(nombreColor);
                if (itemColor != null)
                {
                    itemColor.Selected = true;
                }

                // 6. Forzar la carga de PRODUCTOS y seleccionar
                DDL_FiltroColor_Changed(null, null); // Ejecuta el evento para cargar Productos
                DDL_Productos.ClearSelection();
                ListItem itemProducto = DDL_Productos.Items.FindByValue(codProducto);
                if (itemProducto != null)
                {
                    itemProducto.Selected = true;
                }
            }
        }

        Session["MantenimientoAlmacenProducto_Tipo_Transaccion"] = "ACTUALIZAR";
        this.BTN_GuardarCambios.Text = "Actualizar";
    }
}