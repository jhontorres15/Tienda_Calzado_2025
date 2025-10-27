<%@ Page Title="Registro Productos" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Mantenimiento_Productos.aspx.cs" Inherits="Mantenimiento_Productos" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
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

        <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap" rel="stylesheet" />
   
    <link href="Estilos/Estilo_Menu.css" rel="stylesheet" />



    

</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="app">   
    <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <table class="Tabla_Articulo">
        <tr>
            <td colspan="6" class="Titulo_Formulario">Registro de Productos</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Código del Producto:&nbsp;&nbsp;</td>
            <td class="Fila">
                <asp:Label ID="Lb_CodProducto" runat="server" Text="Label"></asp:Label>
            </td>
            <td class="auto-style6"></td>
            <td class="Etiqueta_Articulo">Categoría:</td>
            <td colspan="2" class="Fila">
                <asp:DropDownList ID="DDL_Categoria" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Nombre del Producto:&nbsp;&nbsp;&nbsp;</td>
            <td class="Fila">
                <asp:TextBox ID="TXT_Producto" runat="server" CssClass="Control_Text" Width="188px" Style="text-transform: uppercase;"></asp:TextBox>
            </td>
            <td></td>
            <td class="Etiqueta_Articulo">Estado:</td>
            <td class="Fila">
                <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList>
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
            <td class="Etiqueta_Articulo">Descripción del Producto:</td>
            <td class="Fila"><asp:TextBox ID="TXTDescripcion_Producto" runat="server" CssClass="Control_Text" Width="199px" Style="text-transform: uppercase;" Height="41px" TextMode="MultiLine"></asp:TextBox></td>
            <td class="auto-style6"></td>
            <td class="Etiqueta_Articulo">Foto:</td>
            <td class="Etiqueta_Articulo">
                <asp:Label ID="img_desc" runat="server"></asp:Label>
                </td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Marca:&nbsp;&nbsp;</td>
            <td class="Fila">
                <asp:UpdatePanel ID="UpdatePanelMarca" runat="server">
                    <ContentTemplate>
                        <asp:DropDownList ID="DDL_Marca" runat="server" AutoPostBack="true" CssClass="Control_Lista-Desplegable" OnSelectedIndexChanged="DDL_Marca_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td class="auto-style2"></td>
            <td class="auto-style1" colspan="2">
                <asp:FileUpload ID="FileUpload1" runat="server" Width="260px" />
                </td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">
                Modelo:&nbsp;&nbsp;</td>
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
            <td rowspan="6">
                <asp:Image ID="IMG_Calzado" runat="server" Width="150px" Height="150px" ImageUrl="~/IMG/LOGO.jpeg" />
            </td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">
                Talla:&nbsp;&nbsp;</td>
            <td class="Fila">
                <asp:DropDownList ID="DDL_Talla" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList>
            </td>
            <td>
                &nbsp;</td>
            <td class="Fila">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Color:</td>
            <td class="Fila"><asp:DropDownList ID="DDL_Color" runat="server" CssClass="Control_Lista-Desplegable"></asp:DropDownList></td>
            <td>&nbsp;</td>
            <td class="Fila">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Precio Venta Menor:&nbsp;&nbsp;</td>
            <td class="Fila"><asp:TextBox ID="TXT_PrecVentaMenor" placeholder="Soles" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox></td>
            <td>&nbsp;</td>
            <td class="Fila">&nbsp;</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Precio Venta Mayor:</td>
            <td class="Fila"><asp:TextBox ID="TXT_PrecVentaMayor" placeholder="Soles" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox></td>
            <td>&nbsp;</td>
            <td class="Fila">&nbsp;</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo">Stock:&nbsp;&nbsp;</td>
            <td class="Fila">
                <asp:TextBox ID="TXT_Stock" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
            <td class="Fila">&nbsp;</td>
        </tr>
        <tr>
            <td class="Etiqueta_Articulo"></td>
            <td class="auto-style4">
            </td>
            <td class="auto-style2"></td>
            <td class="Etiqueta_Articulo">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style1"></td>
        </tr>
        <tr>
            <td class="Fila" colspan="6">&nbsp;</td>
        </tr>
        </table>
</center> </div>
</asp:Content>
