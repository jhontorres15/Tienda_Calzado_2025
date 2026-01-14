using System;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Empresa_Proveedor : System.Web.UI.Page
{
    EmpresaProveedorLOGIC logic = new EmpresaProveedorLOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GenerarCodigo();
            CargarPaises();
            CargarListado();

           
            BTN_Actualizar.Visible = false;

            //Evaluar si la Variable de tipo Session "MantenimientoEmpleado_CodEmpleado" es diferente de vacio
            if (Session["MantenimientoEmpresa_CodEmpresa"].ToString() != "")
            {

                //    cargarDatos();
                Session["MantenimientoEmpresa_Tipo_Transaccion"] = "ACTUALIZAR";

            }
            else
            {

                //    Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }
        }
    }

    void GenerarCodigo()
    {
        TXT_CodEmpresa.Text = logic.Generar_CodEmpresa();
        TXT_CodEmpresa.Enabled = false;
    }

    void CargarPaises()
    {
        var dt = logic.Listar_Paises();
        DDL_Pais.DataSource = dt;
        DDL_Pais.DataTextField = dt.Columns.Contains("Nacionalidad") ? "Nacionalidad" : dt.Columns[1].ColumnName;
        DDL_Pais.DataValueField = dt.Columns.Contains("CodPais") ? "CodPais" : dt.Columns[0].ColumnName;
        DDL_Pais.DataBind();
    }

    void Mostrar(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalEmpresaProveedor", script, true);
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        Limpiar();
        GenerarCodigo();
    }


    void Limpiar()
    {
        TXT_RazonSocial.Text = "";
        TXT_Ruc.Text = "";
        TXT_Direccion.Text = "";
        TXT_Telefono.Text = "";
        TXT_Email.Text = "";
        DDL_Pais.SelectedIndex = 0;
        DDL_Estado.SelectedIndex = 0;
    }

    protected void BTN_Guardar_Click(object sender, EventArgs e)
    {
        string mensaje = logic.Mantener(TXT_CodEmpresa.Text.Trim(), TXT_RazonSocial.Text.Trim(), TXT_Ruc.Text.Trim(), DDL_Pais.SelectedValue,
            TXT_Direccion.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Estado.SelectedValue, "GUARDAR");
        if (mensaje.StartsWith("Cliente") || mensaje.StartsWith("OK") || mensaje.StartsWith("Empresa"))
        {
            Mostrar("Registro guardado.", "success");
            Limpiar();
            CargarListado();
            Session["MantenimientoEmpresa_Tipo_Transaccion"] = "";
        }
        else Mostrar(mensaje, "error");
    }

    protected void BTN_Actualizar_Click(object sender, EventArgs e)
    {
        string mensaje = logic.Mantener(TXT_CodEmpresa.Text.Trim(), TXT_RazonSocial.Text.Trim(), TXT_Ruc.Text.Trim(), DDL_Pais.SelectedValue,
            TXT_Direccion.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Estado.SelectedValue, "ACTUALIZAR");
        if (mensaje.StartsWith("Datos") || mensaje.StartsWith("OK"))
        {
            Mostrar("Actualización correcta.", "success");
            CargarListado();

            BTN_Guardar.Visible = false;
        }
        else Mostrar(mensaje, "error");

    }

    protected void BTN_Eliminar_Click(object sender, EventArgs e)
    {
        string mensaje = logic.Mantener(TXT_CodEmpresa.Text.Trim(), TXT_RazonSocial.Text.Trim(), TXT_Ruc.Text.Trim(), DDL_Pais.SelectedValue,
            TXT_Direccion.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Estado.SelectedValue, "DELETE");
        if (mensaje.StartsWith("Cliente") || mensaje.StartsWith("OK") || mensaje.StartsWith("Empresa"))
        {
            Mostrar("Eliminación correcta.", "success");
            Limpiar();
            CargarListado();
        }
        else Mostrar(mensaje, "error");
    }

    void CargarListado()
    {
        var dt = logic.Listar_Empresas(TXT_Buscar.Text.Trim());
        GV_Empresas.DataSource = dt;
        GV_Empresas.DataBind();
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        CargarListado();
    }

    protected void GV_Empresas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Empresas.PageIndex = e.NewPageIndex;
        CargarListado();
    }

    protected void GV_Empresas_RowCommand(object source, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Editar")
            {
                //obtener el indice del comandos seleccionado
                int indice = Convert.ToInt32(e.CommandArgument);


                if (e.CommandName == "Editar")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = GV_Empresas.Rows[index];

                    // Cargar cada campo en los TextBox correspondientes
                    TXT_CodEmpresa.Text = row.Cells[0].Text;
                    TXT_RazonSocial.Text = row.Cells[1].Text;
                    TXT_Ruc.Text = row.Cells[2].Text;
                    DDL_Pais.SelectedValue = row.Cells[3].Text;
                    TXT_Direccion.Text = row.Cells[4].Text;
                    TXT_Telefono.Text = row.Cells[5].Text;
                    TXT_Email.Text = row.Cells[6].Text;
                    DDL_Estado.SelectedValue = row.Cells[7].Text;

                    LBL_Mensaje.Text = "Modo edición activado";
                    LBL_Mensaje.ForeColor = System.Drawing.Color.Blue;

                    BTN_Guardar.Visible = false;
                    BTN_Actualizar.Visible = true;
                }
                Session["MantenimientoEmpresa_CodEmpresa"] = TXT_CodEmpresa.ToString();
                Session["MantenimientoEmpresaProveedor_Tipo_Transaccion"] = "ACTUALIZAR";

 
            }

            else if (e.CommandName == "Eliminar")
            {
                //string codCliente = e.CommandArgument.ToString();
                //EliminarCliente(codCliente);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar comando: " + ex.Message, "danger");
        }
    }


    private void MostrarMensaje(string mensaje, string tipo)
    {
        // Reemplazamos las comillas simples por su código HTML ('&#39;') para evitar errores de sintaxis JS
        string mensajeLimpio = mensaje.Replace("'", "&#39;");

        // El 'tipo' debe ser una cadena válida de SweetAlert2 (ej: 'success', 'error', 'warning', 'info', 'question')
        string script = $@"
        Swal.fire({{ 
            icon: '{tipo}', 
            title: 'Información', 
            text: '{mensajeLimpio}', 
            confirmButtonText: 'Aceptar' 
        }});";

        // Registra el script en la página. El 'true' final significa que se incluyen las etiquetas <script>.
        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertScript", script, true);
    }

}

