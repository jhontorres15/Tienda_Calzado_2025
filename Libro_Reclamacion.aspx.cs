using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class Libro_Reclamacion : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarCombos();
            ListarReclamos(null);
        }
    }

    private void CargarCombos()
    {
        // Datos simulados; puedes reemplazar por SPs reales
        ddlCliente.Items.Clear();
        ddlCliente.Items.Add("CLI00001");
        ddlCliente.Items.Add("CLI00002");
        ddlCliente.Items.Add("CLI00003");

        ddlTipoReclamo.Items.Clear();
        ddlTipoReclamo.Items.Add("TR001");
        ddlTipoReclamo.Items.Add("TR002");
        ddlTipoReclamo.Items.Add("TR003");

        ddlSucursal.Items.Clear();
        ddlSucursal.Items.Add("S0001");
        ddlSucursal.Items.Add("S0002");
        ddlSucursal.Items.Add("S0003");
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        try
        {
            GuardarReclamo();
            LimpiarFormulario();
            ListarReclamos(null);
        }
        catch (Exception ex)
        {
            // En un proyecto real: loguear y mostrar mensaje amigable
            Response.Write($"<script>console.error('Error: {ex.Message.Replace("'", "\'")}');</script>");
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        ListarReclamos(txtBuscar.Text.Trim());
    }

    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    protected void gvReclamos_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvReclamos.PageIndex = e.NewPageIndex;
        ListarReclamos(txtBuscar.Text.Trim());
    }

    private void GuardarReclamo()
    {
        var connStr = ConfigurationManager.ConnectionStrings["Conn"]?.ConnectionString;
        if (!string.IsNullOrEmpty(connStr))
        {
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SP_Insertar_Libro_Reclamacion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumReclamo", txtNumReclamo.Text.Trim());
                cmd.Parameters.AddWithValue("@CodCliente", ddlCliente.SelectedValue);
                cmd.Parameters.AddWithValue("@CodTipo_Reclamo", ddlTipoReclamo.SelectedValue);
                cmd.Parameters.AddWithValue("@CodSucursal", ddlSucursal.SelectedValue);
                cmd.Parameters.AddWithValue("@NroSerie_Facturacion", txtSerie.Text.Trim());
                cmd.Parameters.AddWithValue("@Fec_Reclamo", DateTime.Parse(txtFecha.Text));
                cmd.Parameters.AddWithValue("@Descripcion_Reclamo", txtDescripcion.Text.Trim());
                cmd.Parameters.AddWithValue("@Detalle_Reclamo", txtDetalle.Text.Trim());
                cmd.Parameters.AddWithValue("@Exigencia_Solicitud", txtExigencia.Text.Trim());
                cmd.Parameters.AddWithValue("@Estado_Reclamo", txtEstado.Text.Trim());
                cmd.Parameters.AddWithValue("@Obs_Estado_Reclamo", txtObsEstado.Text.Trim());
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Simulación en memoria (Session) cuando no hay conexión
            var dt = Session["RECLAMOS_DT"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("NumReclamo");
                dt.Columns.Add("CodCliente");
                dt.Columns.Add("CodTipo_Reclamo");
                dt.Columns.Add("CodSucursal");
                dt.Columns.Add("NroSerie_Facturacion");
                dt.Columns.Add("Fec_Reclamo", typeof(DateTime));
                dt.Columns.Add("Descripcion_Reclamo");
                dt.Columns.Add("Estado_Reclamo");
                Session["RECLAMOS_DT"] = dt;
            }

            var row = dt.NewRow();
            row["NumReclamo"] = txtNumReclamo.Text.Trim();
            row["CodCliente"] = ddlCliente.SelectedValue;
            row["CodTipo_Reclamo"] = ddlTipoReclamo.SelectedValue;
            row["CodSucursal"] = ddlSucursal.SelectedValue;
            row["NroSerie_Facturacion"] = txtSerie.Text.Trim();
            row["Fec_Reclamo"] = string.IsNullOrWhiteSpace(txtFecha.Text) ? DateTime.Now : DateTime.Parse(txtFecha.Text);
            row["Descripcion_Reclamo"] = txtDescripcion.Text.Trim();
            row["Estado_Reclamo"] = txtEstado.Text.Trim();
            dt.Rows.Add(row);
        }
    }

    private void ListarReclamos(string term)
    {
        var connStr = ConfigurationManager.ConnectionStrings["Conn"]?.ConnectionString;
        DataTable dt = new DataTable();

        if (!string.IsNullOrEmpty(connStr))
        {
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SP_Listar_Libro_Reclamacion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Term", string.IsNullOrWhiteSpace(term) ? (object)DBNull.Value : term);
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        else
        {
            // Simulación desde Session
            var dtSession = Session["RECLAMOS_DT"] as DataTable;
            if (dtSession != null)
            {
                dt = dtSession.Copy();
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var rows = dt.Select($"NumReclamo LIKE '%{term.Replace("'", "''")}%' OR CodCliente LIKE '%{term.Replace("'", "''")}%' ");
                    if (rows.Length > 0)
                    {
                        dt = rows.CopyToDataTable();
                    }
                    else
                    {
                        dt.Rows.Clear();
                    }
                }
            }
        }

        gvReclamos.DataSource = dt;
        gvReclamos.DataBind();
    }

    private void LimpiarFormulario()
    {
        txtNumReclamo.Text = string.Empty;
        ddlCliente.SelectedIndex = 0;
        ddlTipoReclamo.SelectedIndex = 0;
        ddlSucursal.SelectedIndex = 0;
        txtSerie.Text = string.Empty;
        txtFecha.Text = string.Empty;
        txtDescripcion.Text = string.Empty;
        txtDetalle.Text = string.Empty;
        txtExigencia.Text = string.Empty;
        txtEstado.Text = string.Empty;
        txtObsEstado.Text = string.Empty;
        txtBuscar.Text = string.Empty;
    }
}