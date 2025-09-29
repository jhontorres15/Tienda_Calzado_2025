<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login_Administrativo.aspx.cs" Inherits="Login_Administrativo" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
       <title>Acceso al sistema Web</title>
        <link rel="stylesheet" href="Estilos/Estilo_LoginAdministrativo.css" type="text/css" />

    <style type="text/css">
        .auto-style3 {
            border-radius: 20px !important;
            border: 1px solid #c0c0c0;
            outline: 0;
            box-sizing: border-box;
            padding: 12px 15px;
            margin-left: 0px;
        }
        .auto-style4 {
            text-align: center;
            height: 24px;
        }
    </style>

</head>
<body class="body">
    <form id="form1" runat="server" class="Formulario_Login">
        <div>

            <table align="center" class="Tabla_Login">
                <tr>
                    <td class="Titulo_Formulario">Inicio de Sesión</td>
                </tr>
                <tr>
                    <td class="Titulo_Formulario">&nbsp;</td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        &nbsp;<asp:TextBox ID="TXT_Uusario" placeholder="✉️ Email o nombre de usuario" runat="server" CssClass="auto-style3" Width="299px" required></asp:TextBox>
                     </td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        &nbsp;<asp:TextBox ID="TXT_Contraseña" placeholder="🔐 Ingrese su contraseña" runat="server" CssClass="input" TextMode="Password" Width="294px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="Boton" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BTN_InicioSesion" runat="server" Text="Iniciar Sesión" CssClass="Boton" />
                    </td>
                </tr>
                <tr>
                    <td class="Center-Content">
                        &nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

