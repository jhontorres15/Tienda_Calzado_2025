using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class Catalogo : Page
{
    private const string VS_CATALOGO = "VS_CATALOGO";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarMarcas();
            InicializarCatalogo();
        }
    }

    // Carga marcas desde SP
    private void CargarMarcas()
    {
        var dt = EjecutarSP("SP_Listar_Marcas");
        DDL_Marca.DataSource = dt;
        DDL_Marca.DataTextField = dt.Columns.Contains("Marca") ? "Marca" : dt.Columns[1].ColumnName;
        DDL_Marca.DataValueField = dt.Columns.Contains("CodMarca") ? "CodMarca" : dt.Columns[0].ColumnName;
        DDL_Marca.DataBind();
        DDL_Marca.Items.Insert(0, new ListItem("-- Seleccione --", ""));

        DDL_Modelo.Items.Clear();
        DDL_Modelo.Items.Insert(0, new ListItem("-- Seleccione --", ""));
    }

    // Carga modelos dependientes de marca
    protected void DDL_Marca_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_Modelo.Items.Clear();
        DDL_Modelo.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        if (!string.IsNullOrEmpty(DDL_Marca.SelectedValue))
        {
            var dt = EjecutarSP("SP_Listar_Modelos_Por_Marca", new SqlParameter("@CodMarca", DDL_Marca.SelectedValue));
            DDL_Modelo.DataSource = dt;
            DDL_Modelo.DataTextField = dt.Columns.Contains("Modelo") ? "Modelo" : dt.Columns[1].ColumnName;
            DDL_Modelo.DataValueField = dt.Columns.Contains("CodModelo") ? "CodModelo" : dt.Columns[0].ColumnName;
            DDL_Modelo.DataBind();
            DDL_Modelo.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }
    }

    // Inicializa tabla en ViewState para tarjetas
    private void InicializarCatalogo()
    {
        var dt = new DataTable();
        dt.Columns.Add("CodModelo", typeof(string));
        dt.Columns.Add("CodMarca", typeof(string));
        dt.Columns.Add("Marca", typeof(string));
        dt.Columns.Add("Modelo", typeof(string));
        dt.Columns.Add("ImagenUrl", typeof(string));
        ViewState[VS_CATALOGO] = dt;
        RP_Catalogo.DataSource = dt;
        RP_Catalogo.DataBind();
    }

    // Agrega tarjeta del modelo seleccionado
    protected void BTN_Agregar_Click(object sender, EventArgs e)
    {
        LBL_Mensaje.Text = string.Empty;
        if (string.IsNullOrEmpty(DDL_Marca.SelectedValue))
        {
            LBL_Mensaje.Text = "Seleccione una marca.";
            return;
        }
        if (string.IsNullOrEmpty(DDL_Modelo.SelectedValue))
        {
            LBL_Mensaje.Text = "Seleccione un modelo.";
            return;
        }

        var dt = (DataTable)ViewState[VS_CATALOGO];
        var existe = dt.AsEnumerable().Any(r => r.Field<string>("CodModelo") == DDL_Modelo.SelectedValue);
        if (existe)
        {
            LBL_Mensaje.Text = "Este modelo ya está en el catálogo.";
            return;
        }

        // Obtiene el nombre de marca y modelo para mostrar
        string marcaTexto = DDL_Marca.SelectedItem.Text;
        string modeloTexto = DDL_Modelo.SelectedItem.Text;

        var row = dt.NewRow();
        row["CodModelo"] = DDL_Modelo.SelectedValue;
        row["CodMarca"] = DDL_Marca.SelectedValue;
        row["Marca"] = marcaTexto;
        row["Modelo"] = modeloTexto;
        // Imagen placeholder (puedes sustituir con url de DB)
        row["ImagenUrl"] = "IMG/3.png";
        dt.Rows.Add(row);

        RP_Catalogo.DataSource = dt;
        RP_Catalogo.DataBind();
    }

    // ItemDataBound: carga tallas por modelo y stock/colores iniciales
    protected void RP_Catalogo_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var ddlTalla = (DropDownList)e.Item.FindControl("DDL_Talla_Item");
            var txtStock = (TextBox)e.Item.FindControl("TXT_Stock_Item");
            var lblColores = (Label)e.Item.FindControl("LBL_Colores_Item");
            var hfModeloId = (HiddenField)e.Item.FindControl("HF_ModeloId");

            if (ddlTalla != null && txtStock != null && lblColores != null && hfModeloId != null)
            {
                // Cargar tallas
                var dtTallas = EjecutarSP("SP_Listar_Tallas_Por_Modelo", new SqlParameter("@CodModelo", hfModeloId.Value));
                ddlTalla.DataSource = dtTallas;
                ddlTalla.DataTextField = dtTallas.Columns.Contains("Talla") ? "Talla" : dtTallas.Columns[1].ColumnName;
                ddlTalla.DataValueField = dtTallas.Columns.Contains("CodTalla") ? "CodTalla" : dtTallas.Columns[0].ColumnName;
                ddlTalla.DataBind();

                if (ddlTalla.Items.Count > 0)
                {
                    ddlTalla.SelectedIndex = 0;
                    ActualizarStockYColores(hfModeloId.Value, ddlTalla.SelectedValue, txtStock, lblColores);
                }
                else
                {
                    txtStock.Text = "0";
                    lblColores.Text = "Sin colores disponibles";
                }
            }
        }
    }

    // Cambia talla y actualiza stock/colores
    protected void DDL_Talla_Item_SelectedIndexChanged(object sender, EventArgs e)
    {
        var ddl = (DropDownList)sender;
        var item = (RepeaterItem)ddl.NamingContainer;
        var hfModeloId = (HiddenField)item.FindControl("HF_ModeloId");
        var txtStock = (TextBox)item.FindControl("TXT_Stock_Item");
        var lblColores = (Label)item.FindControl("LBL_Colores_Item");

        if (hfModeloId != null && txtStock != null && lblColores != null)
        {
            ActualizarStockYColores(hfModeloId.Value, ddl.SelectedValue, txtStock, lblColores);
        }
    }

    // Eliminar tarjeta
    protected void RP_Catalogo_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Eliminar")
        {
            var dt = (DataTable)ViewState[VS_CATALOGO];
            var rows = dt.AsEnumerable().Where(r => r.Field<string>("CodModelo") == (string)e.CommandArgument).ToList();
            foreach (var r in rows) dt.Rows.Remove(r);
            RP_Catalogo.DataSource = dt;
            RP_Catalogo.DataBind();
        }
    }

    // Llama al SP que devuelve stock total y colores por talla
    private void ActualizarStockYColores(string codModelo, string codTalla, TextBox txtStock, Label lblColores)
    {
        var dt = EjecutarSP("SP_Obtener_Stock_Colores_Por_Talla",
            new SqlParameter("@CodModelo", codModelo),
            new SqlParameter("@CodTalla", codTalla));

        int stockTotal = 0;
        if (dt.Columns.Contains("Pares"))
        {
            stockTotal = dt.AsEnumerable().Sum(r => r.Field<int>("Pares"));
        }
        else if (dt.Columns.Contains("StockTotal"))
        {
            stockTotal = dt.AsEnumerable().Sum(r => r.Field<int>("StockTotal"));
        }
        txtStock.Text = stockTotal.ToString();

        // Indicador visual de stock bajo
        txtStock.CssClass = stockTotal < 5 ? "form-control form-control-sm mt-1 stock-low" : "form-control form-control-sm mt-1";

        // Colores disponibles con cantidad
        if (dt.Rows.Count > 0)
        {
            var partes = dt.AsEnumerable().Select(r =>
            {
                var color = dt.Columns.Contains("Color") ? r["Color"].ToString() : r[0].ToString();
                var pares = dt.Columns.Contains("Pares") ? r.Field<int>("Pares") : 0;
                return $"{color} ({pares})";
            });
            lblColores.Text = string.Join(", ", partes);
        }
        else
        {
            lblColores.Text = "Sin colores disponibles";
        }
    }

    // Ejecuta stored procedure y devuelve DataTable. Si falla, simula datos.
    private DataTable EjecutarSP(string spName, params SqlParameter[] parametros)
    {
        var dt = new DataTable();
        try
        {
            string connStr = ConfigurationManager.ConnectionStrings["CalzadoDB"]?.ConnectionString
                              ?? ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(spName, cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros);
                da.Fill(dt);
            }
        }
        catch
        {
            // Simulación para entorno sin BD
            dt = SimularSP(spName, parametros);
        }
        return dt;
    }

    // Simula datos para SPs definidos
    private DataTable SimularSP(string spName, SqlParameter[] parametros)
    {
        switch (spName)
        {
            case "SP_Listar_Marcas":
                var marcas = new DataTable();
                marcas.Columns.Add("CodMarca", typeof(string));
                marcas.Columns.Add("Marca", typeof(string));
                marcas.Rows.Add("NIKE01", "Nike");
                marcas.Rows.Add("ADID01", "Adidas");
                marcas.Rows.Add("PUMA01", "Puma");
                return marcas;

            case "SP_Listar_Modelos_Por_Marca":
                var codMarca = parametros?.FirstOrDefault(p => p.ParameterName == "@CodMarca")?.Value?.ToString();
                var modelos = new DataTable();
                modelos.Columns.Add("CodModelo", typeof(string));
                modelos.Columns.Add("Modelo", typeof(string));
                if (codMarca == "NIKE01")
                {
                    modelos.Rows.Add("M001", "Air Max 90");
                    modelos.Rows.Add("M002", "Air Force 1");
                }
                else if (codMarca == "ADID01")
                {
                    modelos.Rows.Add("M101", "Ultraboost");
                    modelos.Rows.Add("M102", "NMD R1");
                }
                else
                {
                    modelos.Rows.Add("M201", "Suede Classic");
                    modelos.Rows.Add("M202", "RS-X");
                }
                return modelos;

            case "SP_Listar_Tallas_Por_Modelo":
                var tallas = new DataTable();
                tallas.Columns.Add("CodTalla", typeof(string));
                tallas.Columns.Add("Talla", typeof(int));
                tallas.Rows.Add("T38", 38);
                tallas.Rows.Add("T40", 40);
                tallas.Rows.Add("T42", 42);
                return tallas;

            case "SP_Obtener_Stock_Colores_Por_Talla":
                var stock = new DataTable();
                stock.Columns.Add("Color", typeof(string));
                stock.Columns.Add("Pares", typeof(int));
                stock.Rows.Add("Negro", 3);
                stock.Rows.Add("Blanco", 2);
                stock.Rows.Add("Rojo", 1);
                return stock;
        }
        return new DataTable();
    }
}