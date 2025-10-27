using System;
using System.Data;
using System.Data.SqlClient;

public partial class Login_Administrativo : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    //Crear el Método: Iniciar_Sesion()
    void Iniciar_Sesion(String CodUsuario, String Password)
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();
        //Declarar y Configurar un Nuevo Comando de Datos
        SqlCommand CMDIniciar_Sesion = new SqlCommand();
        CMDIniciar_Sesion.Connection = Global.CN;
        CMDIniciar_Sesion.CommandType = CommandType.StoredProcedure;
        CMDIniciar_Sesion.CommandText = "USP_Iniciar_Sesion_Empleado";
        //Declarar Parámetros de Salida de Datos
        SqlParameter ParamPerfil = new SqlParameter();
        SqlParameter ParamMensaje = new SqlParameter();
        //Establecer Nombre, Tipo de Dato y Longitud del Parámetro
        ParamPerfil.ParameterName = "@Perfil";
        ParamPerfil.SqlDbType = SqlDbType.VarChar;
        ParamPerfil.Size = 20;

        //Indicar el Tipo de Parámetro que representa: Salida de Datos (Output)
        ParamPerfil.Direction = ParameterDirection.Output;
        ParamMensaje.ParameterName = "@Mensaje";
        ParamMensaje.SqlDbType = SqlDbType.VarChar;
        ParamMensaje.Size = 100;
        //Indicar el Tipo de Parámetro que representa: Salida de Datos (Output)
        
ParamMensaje.Direction = ParameterDirection.Output;
        //Agregar Parámetros de Datos al Comando de Datos
        CMDIniciar_Sesion.Parameters.Add("@CodUsuario", SqlDbType.Char, 6).Value = CodUsuario ;
        CMDIniciar_Sesion.Parameters.Add("@Password", SqlDbType.VarChar, 20).Value = Password;
        CMDIniciar_Sesion.Parameters.Add(ParamPerfil);
        CMDIniciar_Sesion.Parameters.Add(ParamMensaje);
        //Ejecutar el Comando de Datos
        CMDIniciar_Sesion.ExecuteNonQuery();
        //Obtener el Perfil de Usuario
        this.Lb_Perfil.Text = ParamPerfil.Value.ToString();
        //Obtener el Mensaje del Procedimiento Almacenado
        this.Lb_Mensaje.Text = ParamMensaje.Value.ToString();
        //Liberar Recursos
        CMDIniciar_Sesion.Dispose();



        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }

    protected void Page_Load(Object sender, EventArgs e)
    {
        if (!IsPostBack == false)
        {
            //Evitar el recordatorio de datos en el control: Webform
            this.Form.Attributes.Add("autocomplete", "off");

            //invocar al metodo
            this.Iniciar_Sesion(this.TXT_Usuario.Text , this.TXT_Contraseña.Text);
        }
    }

    protected void BTN_InicioSesion_Click(object sender, EventArgs e)
    {
        //Evaluar si No se ha ingresado el Código de Usuario
        if (this.TXT_Usuario.Text.Trim().ToString() == "")
        {
            this.Lb_Mensaje.Text = "Ingrese el Código del Usuario";
            this.TXT_Usuario.Focus();
        }
        //Evaluar si No se ha ingresado el Password
        else if (this.TXT_Contraseña.Text.Trim().ToString() == "")
        {
            this.Lb_Mensaje.Text = "Ingrese Password";
            this.TXT_Contraseña.Focus();
        }
        else
        {
            //Invocar al Método: Iniciar_Sesion
            this.Iniciar_Sesion(this.TXT_Usuario.Text, this.TXT_Contraseña.Text);

            //Evaluar si el Passeord es Incorrecto
            if (this.Lb_Perfil.Text == "El Password es Incorrecto.")
            {
                //Ubicar Cursor en el Control
                this.TXT_Contraseña.Focus();
            }
            if (this.Lb_Mensaje.Text.Substring(1, 10).ToString() == "BIENVENIDO")
            {
                //Enviar al Formulario Web
                this.Response.Redirect("Menu_Web.master");
            }

        }


    }
}