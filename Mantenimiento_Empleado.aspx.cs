using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Mantenimiento_Empleado : System.Web.UI.Page
{

    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();


    //Crear el Método Público: Listar_Articulos
    public void Listar_Empleados()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Nuevo DataTable
        DataTable DT = new DataTable();
        //Crear un Nuevo Adaptador de Datos
        SqlDataAdapter DA = new SqlDataAdapter("SELECT CodEmpleado, (Apellido + ' ' + Nombre) AS Empleado, Sucursal, Area, Cargo, Telefono, Email, Estado_Empleado FROM V_Listar_Empleados", Global.CN);
        //Cargar Datos del Adaptador de Datos a un DataTable
        DA.Fill(DT);
        //Cargar Datos del DataTable en el Control: GV_Articulos
        GV_Empleados.DataSource = DT;
        GV_Empleados.DataBind();

        //Liberar Recursos del Adaptador de Datos
        DA.Dispose();
        //Cerrar Conexión con la Base de Datos

        Global.CN.Close();
    }

    void Nuevo() { 
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
            //Declarar y Configurar un Nuevo Comando de Datos
            SqlCommand CMDNuevo = new SqlCommand();
        CMDNuevo.Connection = Global.CN;
            CMDNuevo.CommandType = CommandType.StoredProcedure;
            CMDNuevo.CommandText = "USP_Generar_CodEmpleado";
            //Declarar un Nuevo Parámetro de Salida de Datos
            SqlParameter ParamCodigo = new SqlParameter();
        //Establecer Nombre, Tipo de Dato y Longitud del Parámetro
        ParamCodigo.ParameterName = "@CodEmpleado";
            ParamCodigo.SqlDbType = SqlDbType.Char;
            ParamCodigo.Size = 5;
            //Indicar el Tipo de Parámetro que representa: Salida de Datos (Output)
            ParamCodigo.Direction = ParameterDirection.Output;
        
            //Agregar Parametro al Comando de Datos
            CMDNuevo.Parameters.Add(ParamCodigo);

        //Ejecutar Comando de Datos
        CMDNuevo.ExecuteNonQuery();
        //Obtener el Código Generado
        this.TXT_CodEmpleado.Text = ParamCodigo.Value.ToString();
        //Liberar Recursos
        CMDNuevo.Dispose();

        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
      }
    //Crear el Método: Guardar (String Tipo_Parámetro_Ejecutar)
    void Guardar(string Tipo_Transaccion) {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Nuevo Comando de Datos
        SqlCommand CMDGuardar = new SqlCommand();
        //Configurar Comando de Datos
        CMDGuardar.Connection = Global.CN;
        CMDGuardar.CommandType = CommandType.StoredProcedure;
        CMDGuardar.CommandText = "USP_Mantenimiento_Empleado";
        //Declarar Parámetros

        // ===== DECLARACIÓN DE PARÁMETROS PARA Tb_Empleado =====

        // 1️⃣ Declaración
        SqlParameter ParamCodEmpleado = new SqlParameter();
        SqlParameter ParamApellido = new SqlParameter();
        SqlParameter ParamNombre = new SqlParameter();
        SqlParameter ParamFec_Nac = new SqlParameter();
        SqlParameter ParamSexo = new SqlParameter();
        SqlParameter ParamEstado_Civil = new SqlParameter();
        SqlParameter ParamNro_Hijos = new SqlParameter();
        SqlParameter ParamCodPais = new SqlParameter();
        SqlParameter ParamCodDoc_Identidad = new SqlParameter();
        SqlParameter ParamNroDoc_Identidad = new SqlParameter();
        SqlParameter ParamFec_Contrato = new SqlParameter();
        SqlParameter ParamFec_Termino_Contrato = new SqlParameter();
        SqlParameter ParamCodSucursal = new SqlParameter();
        SqlParameter ParamCodArea = new SqlParameter();
        SqlParameter ParamCodCargo = new SqlParameter();
        SqlParameter ParamSueldo = new SqlParameter();
        SqlParameter ParamCPostal = new SqlParameter();
        SqlParameter ParamDireccion = new SqlParameter();
        SqlParameter ParamTelefono = new SqlParameter();
        SqlParameter ParamEmail = new SqlParameter();
        SqlParameter ParamEstado_Empleado = new SqlParameter();
        SqlParameter ParamObs_Estado_Empleado = new SqlParameter();
        SqlParameter ParamTipoTransaccion = new SqlParameter();
        SqlParameter ParamFoto = new SqlParameter();
        SqlParameter ParamMensaje = new SqlParameter();

        // 2️⃣ Configuración de parámetros
        ParamCodEmpleado.ParameterName = "@CodEmpleado";
        ParamCodEmpleado.SqlDbType = SqlDbType.Char;
        ParamCodEmpleado.Size = 5;
        ParamCodEmpleado.Value = this.TXT_CodEmpleado.Text;

        ParamApellido.ParameterName = "@Apellido";
        ParamApellido.SqlDbType = SqlDbType.VarChar;
        ParamApellido.Size = 40;
        ParamApellido.Value = this.txtApellido.Text.ToUpper().Trim();

        ParamNombre.ParameterName = "@Nombre";
        ParamNombre.SqlDbType = SqlDbType.VarChar;
        ParamNombre.Size = 30;
        ParamNombre.Value = this.txtNombre.Text.ToUpper().Trim();


        ParamFec_Nac.ParameterName = "@Fec_Nac";
        ParamFec_Nac.SqlDbType = SqlDbType.Date;
        ParamFec_Nac.Value = this.txtFecNac.Text.Replace("/", "-");

        ParamSexo.ParameterName = "@Sexo";
        ParamSexo.SqlDbType = SqlDbType.VarChar;
        ParamSexo.Size = 10;
        ParamSexo.Value = this.ddlSexo.SelectedValue.ToString();


        ParamEstado_Civil.ParameterName = "@Estado_Civil";
        ParamEstado_Civil.SqlDbType = SqlDbType.VarChar;
        ParamEstado_Civil.Size = 10; 
        ParamEstado_Civil.Value = this.DDL_EstadoCivil.SelectedValue;

       ParamNro_Hijos.ParameterName = "@Nro_Hijos";
       ParamNro_Hijos.SqlDbType = SqlDbType.Int;
       ParamNro_Hijos.Value= this.TXT_NroHijos.Text;


        ParamCodPais.ParameterName = "@Nacionalidad";
        ParamCodPais.SqlDbType = SqlDbType.Char;
        ParamCodPais.Size = 50;
        ParamCodPais.Value = this.DDL_Nacionalidad.SelectedValue;

        ParamCodDoc_Identidad.ParameterName = "@Doc_Identidad";
        ParamCodDoc_Identidad.SqlDbType = SqlDbType.Char;
        ParamCodDoc_Identidad.Size = 50;
        ParamCodDoc_Identidad.Value = this.DDL_TipoDoc.SelectedValue;

        ParamNroDoc_Identidad.ParameterName = "@NroDoc_Identidad";
        ParamNroDoc_Identidad.SqlDbType = SqlDbType.VarChar;
        ParamNroDoc_Identidad.Size = 20;
        ParamNroDoc_Identidad.Value = this.TXT_NroDocumento.Text;

        ParamFec_Contrato.ParameterName = "@Fec_Contrato";
        ParamFec_Contrato.SqlDbType = SqlDbType.Date;
        ParamFec_Contrato.Value = this.TXT_FecContrato.Text.Replace("/", "-");

        ParamFec_Termino_Contrato.ParameterName = "@Fec_Termino_Contrato";
        ParamFec_Termino_Contrato.SqlDbType = SqlDbType.Date;
        ParamFec_Termino_Contrato.Value = this.TXT_FecTermino.Text.Replace("/", "-");

        ParamCodSucursal.ParameterName = "@Sucursal";
        ParamCodSucursal.SqlDbType = SqlDbType.VarChar;
        ParamCodSucursal.Size = 50;
        ParamCodSucursal.Value = this.DDL_Sucursal.SelectedValue;

        ParamCodArea.ParameterName = "@Area";
        ParamCodArea.SqlDbType = SqlDbType.VarChar;
        ParamCodArea.Size = 50;
        ParamCodArea.Value = this.DDL_Area.SelectedValue;

        ParamCodCargo.ParameterName = "@Cargo";
        ParamCodCargo.SqlDbType = SqlDbType.VarChar;
        ParamCodCargo.Size = 50;
        ParamCodCargo.Value = this.DDL_Cargo.SelectedValue;

        ParamSueldo.ParameterName = "@Sueldo";
        ParamSueldo.SqlDbType = SqlDbType.Decimal;
        ParamSueldo.Value = Convert.ToDecimal(this.TXT_Sueldo.Text);

        ParamCPostal.ParameterName = "@Distrito";
        ParamCPostal.SqlDbType = SqlDbType.VarChar;
        ParamCPostal.Size = 50;
        ParamCPostal.Value = this.DDL_Distrito.SelectedValue;

        ParamDireccion.ParameterName = "@Direccion";
        ParamDireccion.SqlDbType = SqlDbType.VarChar;
        ParamDireccion.Size = 100;
        ParamDireccion.Value = this.TXT_Direccion.Text;

        ParamTelefono.ParameterName = "@Telefono";
        ParamTelefono.SqlDbType = SqlDbType.VarChar;
        ParamTelefono.Size = 12;
        ParamTelefono.Value = this.TXT_Telefono.Text;

        ParamEmail.ParameterName = "@Email";
        ParamEmail.SqlDbType = SqlDbType.VarChar;
        ParamEmail.Size = 40;
        ParamEmail.Value = this.TXT_Email.Text;

        // Procesar imagen
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

        ParamFoto.ParameterName = "@Foto";
        ParamFoto.SqlDbType = SqlDbType.VarBinary;
        ParamFoto.Value = imagenBytes;

        ParamEstado_Empleado.ParameterName = "@Estado_Empleado";
        ParamEstado_Empleado.SqlDbType = SqlDbType.VarChar;
        ParamEstado_Empleado.Size = 20;
        ParamEstado_Empleado.Value = this.DDL_Estado.SelectedValue;

        ParamObs_Estado_Empleado.ParameterName = "@Obs_Estado_Empleado";
        ParamObs_Estado_Empleado.SqlDbType = SqlDbType.VarChar;
        ParamObs_Estado_Empleado.Size = 200;
        ParamObs_Estado_Empleado.Value = this.TXT_ObsEmpleado.Text;

        ParamTipoTransaccion.ParameterName = "@Tipo_Transaccion";
        ParamTipoTransaccion.SqlDbType = SqlDbType.VarChar;
        ParamTipoTransaccion.Size = 10;
        ParamTipoTransaccion.Value = Tipo_Transaccion;

        ParamMensaje.ParameterName = "@Mensaje";
        ParamMensaje.SqlDbType = SqlDbType.VarChar;
        ParamMensaje.Size = 255;
        ParamMensaje.Direction = ParameterDirection.Output;  // <--- Falta esto

        // Agregar parámetros al comando
        CMDGuardar.Parameters.Add(ParamCodEmpleado);
        CMDGuardar.Parameters.Add(ParamApellido);
        CMDGuardar.Parameters.Add(ParamNombre);
        CMDGuardar.Parameters.Add(ParamFec_Nac);
        CMDGuardar.Parameters.Add(ParamSexo);
        CMDGuardar.Parameters.Add(ParamEstado_Civil);
        CMDGuardar.Parameters.Add(ParamNro_Hijos);
        CMDGuardar.Parameters.Add(ParamCodPais);
        CMDGuardar.Parameters.Add(ParamCodDoc_Identidad);
        CMDGuardar.Parameters.Add(ParamNroDoc_Identidad);
        CMDGuardar.Parameters.Add(ParamFec_Contrato);
        CMDGuardar.Parameters.Add(ParamFec_Termino_Contrato);
        CMDGuardar.Parameters.Add(ParamCodSucursal);
        CMDGuardar.Parameters.Add(ParamCodArea);
        CMDGuardar.Parameters.Add(ParamCodCargo);
        CMDGuardar.Parameters.Add(ParamSueldo);
        CMDGuardar.Parameters.Add(ParamCPostal);
        CMDGuardar.Parameters.Add(ParamDireccion);
        CMDGuardar.Parameters.Add(ParamTelefono);
        CMDGuardar.Parameters.Add(ParamEmail);
        CMDGuardar.Parameters.Add(ParamFoto);
        CMDGuardar.Parameters.Add(ParamEstado_Empleado);
        CMDGuardar.Parameters.Add(ParamObs_Estado_Empleado);
        CMDGuardar.Parameters.Add(ParamTipoTransaccion);
        CMDGuardar.Parameters.Add(ParamMensaje);

       
        // Ejecutar el comando
        CMDGuardar.ExecuteNonQuery();


 // Mostrar el mensaje devuelto por el procedimiento
        Response.Write("<script language='javascript'>alert('" + ParamMensaje.Value.ToString() + "');</script>");

        // Liberar recursos
        CMDGuardar.Dispose();

        // Cerrar conexión
        Global.CN.Close();

    }

    void Listar_Nacionalidad()
    {

        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Lector de Datos
        SqlDataReader Lector;
        //Crear un Comando de Datos
        SqlCommand CMDLista_Nac = new SqlCommand();
        //Conectar Comando de Datos con la Conexión de la BD
        CMDLista_Nac.Connection = Global.CN;
        //Establecer el Tipo de Comando de Datos a Ejecutar: Procedimiento Almacenado
        CMDLista_Nac.CommandType = CommandType.StoredProcedure;
        //Establecer el Nombre del Comando de Datos
        CMDLista_Nac.CommandText = "USP_Listar_Nacionalidad";
        //Ejecutar Comando de Datos
        Lector = CMDLista_Nac.ExecuteReader();
        //Limpiar Datos del Control: DDL_Nacionalidad
        this.DDL_Nacionalidad.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL] Nacionalidad
        this.DDL_Nacionalidad.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read())
        {
            //Agregar Valores al Control: DDL_Nacionalidad
            this.DDL_Nacionalidad.Items.Add(Lector.GetValue(0).ToString());
        }

        //LIberar Recursos
        CMDLista_Nac.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Sucursal()
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Lector de Datos
        SqlDataReader Lector;
        //Crear un Comando de Datos
        SqlCommand CMDLista_Suc = new SqlCommand();
        //Conectar Comando de Datos con la Conexión de la BD
        CMDLista_Suc.Connection = Global.CN;
        //Establecer el Tipo de Comando de Datos a Ejecutar: Procedimiento Almacenado
        CMDLista_Suc.CommandType = CommandType.StoredProcedure;
        //Establecer el Nombre del Comando de Datos
        CMDLista_Suc.CommandText = "USP_Listar_Sucursal";
        //Ejecutar Comando de Datos
        Lector = CMDLista_Suc.ExecuteReader();
        //Limpiar Datos del Control: DDL_Nacionalidad
        this.DDL_Sucursal.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL] Nacionalidad
        this.DDL_Sucursal.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read())
        {
            //Agregar Valores al Control: DDL_Nacionalidad
            this.DDL_Sucursal.Items.Add(Lector.GetValue(0).ToString());
        }

        //LIberar Recursos
        CMDLista_Suc.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Doc_Identidad(string Nacionalidad)
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();

        //Declarar Lector de Datos
        SqlDataReader Lector;

        //Crear un Nuevo Comando de Datos
        SqlCommand CMDListar_Documento = new SqlCommand();

        //Conectar Comando de Datos con la BD
        CMDListar_Documento.Connection = Global.CN;

        //Configurar Comando de Datos
        CMDListar_Documento.CommandType = CommandType.StoredProcedure;
        CMDListar_Documento.CommandText = "USP_Listar_Documento_Identidad";

        //Agregar Parámetro a Comando de Datos
        CMDListar_Documento.Parameters.Add("@Nacionalidad", SqlDbType.VarChar, 60).Value = Nacionalidad;
        //Ejecutar Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Documento.ExecuteReader();
        //Limpiar Elementos del Control:
        this.DDL_TipoDoc.Items.Clear();

        //Agregar un Primer Elemento al Control: DDL_TipoDoc
        this.DDL_TipoDoc.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read())
        {
            //Agregar Valores al Control: DDL_TipoDoc
            this.DDL_TipoDoc.Items.Add(Lector.GetValue(1).ToString());
        }

        CMDListar_Documento.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Area()
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();

        //Declarar Lector de Datos
        SqlDataReader Lector;

        //Crear un Nuevo Comando de Datos
        SqlCommand CMDListar_Area = new SqlCommand();

        //Conectar Comando de Datos con la BD
        CMDListar_Area.Connection = Global.CN;

        //Configurar Comando de Datos
        CMDListar_Area.CommandType = CommandType.StoredProcedure;
        CMDListar_Area.CommandText = "USP_Listar_Area";

        //Ejecutar Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Area.ExecuteReader();
        //Limpiar Elementos del Control:
        this.DDL_Area.Items.Clear();

        //Agregar un Primer Elemento al Control: DDL_TipoDoc
        this.DDL_Area.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read())
        {
            //Agregar Valores al Control: DDL_TipoDoc
            this.DDL_Area.Items.Add(Lector.GetValue(0).ToString());
        }

        CMDListar_Area.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Cargo()
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();

        //Declarar Lector de Datos
        SqlDataReader Lector;

        //Crear un Nuevo Comando de Datos
        SqlCommand CMDListar_Cargo = new SqlCommand();

        //Conectar Comando de Datos con la BD
        CMDListar_Cargo.Connection = Global.CN;

        //Configurar Comando de Datos
        CMDListar_Cargo.CommandType = CommandType.StoredProcedure;
        CMDListar_Cargo.CommandText = "USP_Listar_Cargo";

        //Ejecutar Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Cargo.ExecuteReader();
        //Limpiar Elementos del Control:
        this.DDL_Cargo.Items.Clear();

        //Agregar un Primer Elemento al Control: DDL_TipoDoc
        this.DDL_Cargo.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read())
        {
            //Agregar Valores al Control: DDL_TipoDoc
            this.DDL_Cargo.Items.Add(Lector.GetValue(0).ToString());
        }

        CMDListar_Cargo.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    //Crear el Método: Listar_Departamento()
void Listar_Departamento()
    {
     Global.CN.Open();

        SqlDataReader Lector;

        SqlCommand CMDListar_Dep = new SqlCommand();
        CMDListar_Dep.Connection = Global.CN;

        CMDListar_Dep.CommandType = CommandType.StoredProcedure;
        CMDListar_Dep.CommandText = "USP_Listar_Departamento";

        Lector = CMDListar_Dep.ExecuteReader();

        this.DDL_Departamento.Items.Clear();
        this.DDL_Departamento.Items.Add("<Seleccione>");
        while (Lector.Read()) { 
        this.DDL_Departamento.Items.Add(Lector.GetValue(0).ToString());

    } 
        CMDListar_Dep.Dispose();
        Global.CN.Close();
    }

    void Listar_Procincia(String Departamento)
    {
        Global.CN.Open();

        SqlDataReader Lector;
        SqlCommand CMDListar_Provincia = new SqlCommand();
         //Conectar y Configurar el Comando de Datos
         CMDListar_Provincia.Connection = Global.CN;
        CMDListar_Provincia.CommandType = CommandType.StoredProcedure;
        CMDListar_Provincia.CommandText = "USP_Listar_Provincia";
        //Agregar Parámetro de Datos
        CMDListar_Provincia.Parameters.Add("@Departamento", SqlDbType.VarChar, 40).Value = Departamento;
        //Ejecutar el Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Provincia.ExecuteReader();
        //Limpiar Datos del Control: DDL_Provincia
        this.DDL_Provincia.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Provincia.Items.Add("<Seleccione>");
        
        //Leer Datos
      
        while (Lector.Read())
  
        {

        //Agregar Datos al (/ Site
        this.DDL_Provincia.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos
      
        CMDListar_Provincia.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
}

    void Listar_Distrito(String Provincia)
    {
        Global.CN.Open();

        SqlDataReader Lector;
        SqlCommand CMDListar_Distrito = new SqlCommand();
        //Conectar y Configurar el Comando de Datos
        CMDListar_Distrito.Connection = Global.CN;
        CMDListar_Distrito.CommandType = CommandType.StoredProcedure;
        CMDListar_Distrito.CommandText = "USP_Listar_Distrito";
        //Agregar Parámetro de Datos
        CMDListar_Distrito.Parameters.Add("@Provincia", SqlDbType.VarChar, 40).Value = Provincia;

        //Ejecutar el Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Distrito.ExecuteReader();
        //Limpiar Datos del Control: DDL_Provincia
        this.DDL_Distrito.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Distrito.Items.Add("<Seleccione>");

        //Leer Datos

        while (Lector.Read())

        {

            //Agregar Datos al (/ Site
            this.DDL_Distrito.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos

        CMDListar_Distrito.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            //Evitar el recordatorio de datos en el control: Webform
            this.Form.Attributes.Add("autocomplete", "off");

            //listar metodo
            Listar_Nacionalidad();
            Listar_Sucursal();
            Listar_Area();
            Listar_Cargo();
            Listar_Departamento();
            Listar_Empleados();

            this.DDL_Distrito.Items.Add("<Seleccione>");
            this.DDL_Provincia.Items.Add("<Seleccione>");

            //Evaluar si la Variable de tipo Session "MantenimientoEmpleado_CodEmpleado" es diferente de vacio
            if (Session["MantenimientoEmpleado_CodEmpleado"].ToString() != "")
            {
                Session["MantenimientoEmpleado_Tipo_Transaccion"] = "ACTUALIZAR";
                //Invocar al Método: Cargar_Datos()
                this.CargarEmpleado(this.TXT_CodEmpleado.Text);
            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }



        }

    }

    protected void DDL_Nacionalidad_SelectedIndexChanged(object sender, EventArgs e)
    {
        //invocar al método Listar_Doc_Identidad
        this.Listar_Doc_Identidad(this.DDL_Nacionalidad.Text.ToString());

        this.DDL_TipoDoc.Focus();
    }



    protected void DDL_Departamento_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Invocar al método Listar_Provincia
        this.Listar_Procincia(this.DDL_Departamento.Text.ToString());

        //
        this.DDL_Provincia_SelectedIndexChanged(null,null);

        //Colocar el foco de atención en el control: DDL_Provincia
        this.DDL_Provincia.Focus();
    }

    protected void DDL_Provincia_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Listar_Distrito(this.DDL_Provincia.Text.ToString());

        //Colocar el foco de atención en el control: DDL_Distrito
        this.DDL_Distrito.Focus();
    }



    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        Nuevo();

        //Asignar Valor a la variable:
        Session["MantenimientoEmpleado_Tipo_Transaccion"] = "GUARDAR";

    }
    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        //Crear Controlador de Error
        try
        {
            //Invocar al Método: Guardar()
            this.Guardar(Session["MantenimientoEmpleado_Tipo_Transaccion"].ToString());
        }
        catch (Exception Error)
        {
         

        }
    }

    


    protected void GV_Empleados_RowCommand(object source, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Editar")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            // Obtener CodEmpleado desde DataKey
            string codEmpleado = GV_Empleados.DataKeys[index].Value.ToString();

            // Guardar CodEmpleado en sesión (opcional)
            Session["MantenimientoEmpleado_CodEmpleado"] = codEmpleado;

            // Cargar todos los datos del empleado
            CargarEmpleado(codEmpleado);
        }
    }

    private void CargarEmpleado(string codEmpleado)
    {
        // Conexión a la base de datos
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString))
        {
            con.Open();
            string query = "SELECT * FROM V_Listar_Empleados WHERE CodEmpleado = @CodEmpleado";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        TXT_CodEmpleado.Text = dr["CodEmpleado"].ToString();
                        txtNombre.Text = dr["Nombre"].ToString();
                        txtApellido.Text = dr["Apellido"].ToString();
                        txtFecNac.Text = Convert.ToDateTime(dr["Fec_Nac"]).ToString("yyyy-MM-dd");
                        ddlSexo.SelectedValue = dr["Sexo"].ToString();
                        DDL_EstadoCivil.SelectedValue = dr["Estado_Civil"].ToString();
                        TXT_NroHijos.Text = dr["Nro_Hijos"].ToString();
                        DDL_Nacionalidad.SelectedValue = dr["Nacionalidad"].ToString();
                            Listar_Doc_Identidad(DDL_Nacionalidad.SelectedValue);
                        DDL_TipoDoc.SelectedValue = dr["Documento_Identidad"].ToString();
                        TXT_NroDocumento.Text = dr["NroDoc_Identidad"].ToString();
                        TXT_FecContrato.Text = Convert.ToDateTime(dr["Fec_Contrato"]).ToString("yyyy-MM-dd");
                        TXT_FecTermino.Text = Convert.ToDateTime(dr["Fec_Termino_Contrato"]).ToString("yyyy-MM-dd");
                        DDL_Sucursal.SelectedValue = dr["Sucursal"].ToString();
                        DDL_Area.SelectedValue = dr["Area"].ToString();
                        DDL_Cargo.SelectedValue = dr["Cargo"].ToString();
                        TXT_Sueldo.Text = dr["Sueldo"].ToString();
                        TXT_Direccion.Text = dr["Direccion"].ToString();
                        TXT_Telefono.Text = dr["Telefono"].ToString();
                        TXT_Email.Text = dr["Email"].ToString();
                        DDL_Estado.SelectedValue = dr["Estado_Empleado"].ToString();
                        TXT_ObsEmpleado.Text = dr["Obs_Estado_Empleado"].ToString();

                        Listar_Departamento();
                        DDL_Departamento.SelectedValue = dr["Departamento"].ToString();

                        Listar_Procincia(dr["Departamento"].ToString());
                        DDL_Provincia.SelectedValue = dr["Provincia"].ToString();

                        Listar_Distrito(dr["Provincia"].ToString());
                        DDL_Distrito.SelectedValue = dr["Distrito"].ToString();


                        // Cargar foto si existe
                        if (dr["Foto"] != DBNull.Value)
                        {
                            byte[] fotoBytes = (byte[])dr["Foto"];
                            string base64 = Convert.ToBase64String(fotoBytes);
                            IMG_Calzado.ImageUrl = "data:image/png;base64," + base64;
                        }
                        else
                        {
                            IMG_Calzado.ImageUrl = "~/IMG/LOGO.jpeg";
                        }
                    }
                }
            }
        }
    }


}