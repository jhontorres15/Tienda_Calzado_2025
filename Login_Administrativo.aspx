<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login_Administrativo.aspx.cs" Inherits="Login_Administrativo" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
       <title>Acceso al sistema Web</title>
        <link rel="stylesheet" href="Estilos/Estilo_LoginAdministrativo.css" type="text/css" />

</head>
<body class="body">
    <form id="form1" runat="server" class="Formulario_Login">
        <div>

            <table align="center" class="Tabla_Login">
                <tr>
                    <td class="Titulo_Formulario">Inicio de Sesión</td>
                </tr>

                 <tr>
                    <td class="auto-style1">
                        <asp:Label ID="Lb_Perfil" runat="server"></asp:Label>
                     </td>
                </tr>

                <tr>
                    <td class="Center-Content">
                                      
                        <asp:TextBox ID="TXT_Usuario" placeholder="Ingrese su Usuario    ✉️" runat="server" CssClass="input" Width="220px"></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="TXT_Usuario"
                            CssClass="text-danger" ErrorMessage="El campo de nombre de usuario es obligatorio."/>
                    </td>
                 </tr>
                <tr>
                    <td class="text-center">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        <asp:TextBox ID="TXT_Contraseña" placeholder="Ingrese su Contraseña   🔐" runat="server" CssClass="input" TextMode="Password" Width="220px"></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="TXT_Contraseña"
                            CssClass="text-danger" ErrorMessage="El campo de contraseña es obligatorio." />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="Boton" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BTN_InicioSesion" runat="server" Text="Iniciar Sesión" CssClass="Boton" OnClick="BTN_InicioSesion_Click" />
                            <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
                            <asp:HiddenField ID="HFD_Nombre" runat="server" Value="0" />
                    </td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        <asp:Label ID="Lb_Mensaje" runat="server"></asp:Label>
                       
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

