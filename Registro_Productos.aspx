<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Registro_Productos.aspx.cs" Inherits="Registro_Productos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro de Productos</title>

    <!-- Fuentes y estilos -->
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" />
    <link href="Estilos/Estilo_Menu.css" rel="stylesheet" />

    <link rel="stylesheet" href="Estilos/Estilo_Articulos.css" type="text/css" />
    <script language="javascript" type="text/javascript">
        function ValidarFormulario() {
            var producto = document.getElementById('<%= TXT_Producto.ClientID %>').value;
            var descripcion = document.getElementById('<%= TXTDescripcion_Producto.ClientID %>').value;
            var marca = document.getElementById('<%= DDL_Marca.ClientID %>').selectedIndex;
            var modelo = document.getElementById('<%= DDL_Modelo.ClientID %>').selectedIndex;
            var talla = document.getElementById('<%= DDL_Talla.ClientID %>').selectedIndex;
            var color = document.getElementById('<%= DDL_Color.ClientID %>').selectedIndex;
            var precioVentaMenor = document.getElementById('<%= TXT_PrecVentaMenor.ClientID %>').value;
            var precioVentaMayor = document.getElementById('<%= TXT_PrecVentaMayor.ClientID %>').value;
            var stock = document.getElementById('<%= TXT_Stock.ClientID %>').value;
            var categoria = document.getElementById('<%= DDL_Categoria.ClientID %>').selectedIndex;
            var estado = document.getElementById('<%= DDL_Estado.ClientID %>').selectedIndex;

            if (producto.trim() === "") {
                alert('Ingrese Nombre del Producto');
                return false;
            }
            if (descripcion.trim() === "") { alert('Ingrese Descripción del Producto'); return false; }
            if (categoria <= 0) { alert('Seleccione la Categoría'); return false; }
            if (marca <= 0) { alert('Seleccione la Marca'); return false; }
            if (modelo <= 0) { alert('Seleccione el Modelo'); return false; }
            if (isNaN(stock) || parseInt(stock) <= 0) { alert('Ingrese un Stock válido'); return false; }
            if (isNaN(precioVentaMenor) || parseFloat(precioVentaMenor) <= 0) { alert('Ingrese Precio Venta Menor válido'); return false; }
            if (isNaN(precioVentaMayor) || parseFloat(precioVentaMayor) <= 0) { alert('Ingrese Precio Venta Mayor válido'); return false; }
            if (talla <= 0) { alert('Seleccione la Talla'); return false; }
            if (color <= 0) { alert('Seleccione el Color'); return false; }
            if (genero <= 0) { alert('Seleccione el Género'); return false; }
            if (estado <= 0) { alert('Seleccione el Estado'); return false; }
        }
    </script>
</head>


<body>
    <form id="form1" runat="server">       
            <div class="app">

            <!-- MENU LATERAL -->
            <aside class="sidebar">
                <header>&nbsp;Menú Principal</header>
                <nav class="sidebar-nav">
                    <ul>
                        <!-- EMPLEADOS -->
                        <li>
                            <a href="#"><i class="ion-person-stalker"></i><span> Empleados</span></a>
                            <ul class="nav-flyout">
                                <li><a href="RegistrarEmpleado.aspx"><i class="ion-person-add"></i> Registrar</a></li>
                                <li><a href="ListarEmpleados.aspx"><i class="ion-ios-list"></i> Listar</a></li>
                            </ul>
                        </li>

                        <!-- CLIENTES -->
                        <li>
                            <a href="#"><i class="ion-ios-people-outline"></i><span> Clientes</span></a>
                            <ul class="nav-flyout">
                                <li><a href="RegistrarCliente.aspx"><i class="ion-person-add"></i> Registrar</a></li>
                                <li><a href="ListarClientes.aspx"><i class="ion-ios-list"></i> Listar</a></li>
                                <li><a href="HistorialCompras.aspx"><i class="ion-ios-clock-outline"></i> Historial</a></li>
                            </ul>
                        </li>

                        <!-- VENTAS -->
                        <li>
                            <a href="#"><i class="ion-cash"></i><span> Ventas</span></a>
                            <ul class="nav-flyout">
                                <li><a href="RegistrarVenta.aspx"><i class="ion-compose"></i> Registrar</a></li>
                                <li><a href="ConsultarVentas.aspx"><i class="ion-search"></i> Consultar</a></li>
                                <li><a href="Proformas.aspx"><i class="ion-document-text"></i> Proformas</a></li>
                                <li><a href="ReporteVentas.aspx"><i class="ion-stats-bars"></i> Reporte</a></li>
                            </ul>
                        </li>

                        <!-- ALMACÉN -->
                        <li>
                            <a href="#"><i class="ion-cube"></i><span> Almacén</span></a>
                            <ul class="nav-flyout">
                                <li><a href="Mantenimiento_Productos.aspx"><i class="ion-pricetag"></i> Registrar Producto</a></li>
                                <li><a href="Kardex.aspx"><i class="ion-ios-pulse"></i> Kardex</a></li>
                                <li><a href="Proveedores.aspx"><i class="ion-briefcase"></i> Proveedores</a></li>
                                <li><a href="Categorias.aspx"><i class="ion-ios-pricetags-outline"></i> Categorías</a></li>
                            </ul>
                        </li>

                        <!-- REPORTES -->
                        <li>
                            <a href="#"><i class="ion-ios-paper-outline"></i><span> Reportes</span></a>
                            <ul class="nav-flyout">
                                <li><a href="ReporteMensual.aspx"><i class="ion-calendar"></i> Ventas por Mes</a></li>
                                <li><a href="ProductosVendidos.aspx"><i class="ion-trophy"></i> Más Vendidos</a></li>
                                <li><a href="StockBajo.aspx"><i class="ion-alert-circled"></i> Stock Bajo</a></li>
                                <li><a href="Utilidades.aspx"><i class="ion-social-usd"></i> Utilidades</a></li>
                            </ul>
                        </li>

                        <!-- CONFIGURACIÓN -->
                        <li>
                            <a href="#"><i class="ion-ios-gear-outline"></i><span> Configuración</span></a>
                            <ul class="nav-flyout">
                                <li><a href="Usuarios.aspx"><i class="ion-locked"></i> Usuarios</a></li>
                                <li><a href="Roles.aspx"><i class="ion-person"></i> Roles</a></li>
                                <li><a href="CerrarSesion.aspx"><i class="ion-log-out"></i> Cerrar Sesión</a></li>
                            </ul>
                        </li>
                    </ul>
                </nav>
            </aside>

        </div>
        <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <table class="Tabla_Articulo">
                <tr>
                    <td colspan="6" class="Titulo_Formulario">Registro de Productos</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Código:&nbsp;</td>
                    <td class="Fila_Etiqueta">
                        <asp:Label ID="Lb_CodProducto" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td></td>
                    <td class="Etiqueta_Articulo">Categoría:</td>
                    <td class="auto-style1">
                        <asp:DropDownList ID="DDL_Categoria" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList>
                    </td>
                    <td class="Fila_Boton" rowspan="9">
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="Boton" OnClick="BTN_Nuevo_Click" />
                        <br />
                        <br />
                        <asp:Button ID="BTN_Guardar" runat="server" Text="Guardar" CssClass="Boton" OnClick="BTN_Guardar_Click" OnClientClick="return ValidarFormulario();" />
                        <br />
                        <br />
                        <asp:Button ID="btnImprimirBarras" runat="server" Text="Imprimir BarCode" CssClass="Boton" OnClick="btnImprimirBarras_Click" />
                        <br />
                        <br />
                        <asp:Button ID="btnImprimirQR" runat="server" Text="Imprimir QR" CssClass="Boton" OnClick="btnImprimirQR_Click" />
                        <br />
                        <br />
                       
                        <asp:Button ID="BTN_Reporte" runat="server" Text="Ver Reporte" CssClass="Boton" OnClick="BTN_Reporte_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Nombre del Producto:&nbsp;</td>
                    <td class="auto-style2"><asp:TextBox ID="TXT_Producto" runat="server" CssClass="Control_Text" Width="188px" Style="text-transform: uppercase;"></asp:TextBox></td>
                    <td></td>
                    <td class="Etiqueta_Articulo">Foto:</td>
                    <td class="auto-style3">
                        <asp:Label ID="img_desc" runat="server"></asp:Label>
                        </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">DescrIpción del Producto:&nbsp;</td>
                    <td class="Fila"><asp:TextBox ID="TXTDescripcion_Producto" runat="server" CssClass="Control_Text" Width="188px" Style="text-transform: uppercase;"></asp:TextBox></td>
                    <td>&nbsp;</td>
                    <td class="Fila" colspan="2">
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="260px" />
                        </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">
                        Marca:&nbsp;</td>
                    <td class="Fila">
                        <asp:UpdatePanel ID="UpdatePanelMarca" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Marca" runat="server" AutoPostBack="true" CssClass="Control_Lista-Desplegable" OnSelectedIndexChanged="DDL_Marca_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        &nbsp;</td>
                    <td class="Fila">
                        &nbsp;</td>
                    <td rowspan="6">
                        <asp:Image ID="IMG_Calzado" runat="server" Width="150px" Height="150px" ImageUrl="~/IMG/LOGO.jpeg" />
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">
                        Modelo:&nbsp;</td>
                    <td class="Fila">
                        <asp:UpdatePanel ID="UpdatePanelModelo" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Modelo" runat="server" CssClass="Control_Lista-Desplegable">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        &nbsp;</td>
                    <td class="Fila">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Talla:&nbsp;</td>
                    <td class="Fila"><asp:DropDownList ID="DDL_Talla" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList></td>
                    <td>&nbsp;</td>
                    <td class="Fila">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Color:&nbsp;</td>
                    <td class="Fila"><asp:DropDownList ID="DDL_Color" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList></td>
                    <td>&nbsp;</td>
                    <td class="Fila">&nbsp;</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Precio Venta Menor:&nbsp;</td>
                    <td class="Fila"><asp:TextBox ID="TXT_PrecVentaMenor" placeholder="Soles" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox></td>
                    <td>&nbsp;</td>
                    <td class="Fila">&nbsp;</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Precio Venta Mayor:&nbsp;</td>
                    <td class="Fila"><asp:TextBox ID="TXT_PrecVentaMayor" placeholder="Soles" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox></td>
                    <td>&nbsp;</td>
                    <td class="Fila">&nbsp;</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Stock:&nbsp;</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXT_Stock" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                    <td class="Etiqueta_Articulo">Estado:</td>
                    <td><asp:DropDownList ID="DDL_Estado" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList></td>
                    <td class="auto-style1">&nbsp;</td>
                </tr>
                <tr>
                    <td class="Fila" colspan="6">&nbsp;</td>
                </tr>
                </table>
        </center>
     </form>
</body>
</html>
