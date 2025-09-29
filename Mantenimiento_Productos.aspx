<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Mantenimiento_Productos.aspx.cs" Inherits="Mantenimiento_Articulos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro Productos</title>
    <link rel="stylesheet" href="Estilos/Estilo_Articulos.css" type="text/css" />
    <style type="text/css">
    
    </style>
</head>
    <script language="javascript" type="text/javascript">  
        function ValidarFormulario()
        {
            //capturar valores de los controles
            
            var producto = document.getElementById('TXT_Producto').value;
            var descripcion = document.getElementById('TXTDescripcion_Producto').value;
            var marca = document.getElementById('DDL_Marca').selectedIndex;
            var modelo = document.getElementById('DDL_Modelo').selectedIndex;
            var talla = document.getElementById('DDL_Talla').selectedIndex;
            var color = document.getElementById('DDL_Color').selectedIndex;
            var genero = document.getElementById('DDL_Genero').selectedIndex;
            var precioVentaMenor = document.getElementById('TXT_PrecVentaMenor').value;
            var precioVentaMayor = document.getElementById('TXT_PrecVentaMayor').value;
            var stock = document.getElementById('TXT_Stock').value;
            var categoria = document.getElementById('DDL_Categoria').selectedIndex;
            var estado = document.getElementById('DDL_Estado').selectedIndex;

            // Validar Nombre del Producto
            if (producto == null || producto.length == 0 || /^\s+$/.test(producto)) {
                alert('ERROR: Ingrese Nombre del Producto');
                document.getElementById('TXT_Producto').focus();
                return false;
            }

            // Validar Descripción
            if (descripcion == null || descripcion.length == 0 || /^\s+$/.test(descripcion)) {
                alert('ERROR: Ingrese Descripción del Producto');
                document.getElementById('TXTDescripcion_Producto').focus();
                return false;
            }

            // Validar Categoría
            if (categoria == null || categoria.length == 0 || document.getElementById('DDL_Categoria').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar la Categoría del Producto');
                document.getElementById('DDL_Categoria').focus();
                return false;
            }

            // Validar Marca
            if (marca == null || marca.length == 0 || document.getElementById('DDL_Marca').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar la Marca del Producto');
                document.getElementById('DDL_Marca').focus();
                return false;
            }

            // Validar Modelo
            if (modelo == null || modelo.length == 0 || document.getElementById('DDL_Modelo').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar el Modelo del Producto');
                document.getElementById('DDL_Modelo').focus();
                return false;
            }
            // Validar Stock (solo números enteros positivos)
            if (stock == null || stock.length == 0 || /^\s+$/.test(stock) || isNaN(stock) || parseInt(stock) <= 0) {
                alert('ERROR: Ingrese un Stock válido (solo números mayores a 0)');
                document.getElementById('TXT_Stock').focus();
                return false;
            }

            // Validar Precio Venta Menor (solo números positivos)
            if (precioVentaMenor == null || precioVentaMenor.length == 0 || /^\s+$/.test(precioVentaMenor) || isNaN(precioVentaMenor) || parseFloat(precioVentaMenor) <= 0) {
                alert('ERROR: Ingrese un Precio de Venta Menor válido (solo números mayores a 0)');
                document.getElementById('TXT_PrecVentaMenor').focus();
                return false;
            }

            // Validar Precio Venta Mayor (solo números positivos)
            if (precioVentaMayor == null || precioVentaMayor.length == 0 || /^\s+$/.test(precioVentaMayor) || isNaN(precioVentaMayor) || parseFloat(precioVentaMayor) <= 0) {
                alert('ERROR: Ingrese un Precio de Venta Mayor válido (solo números mayores a 0)');
                document.getElementById('TXT_PrecVentaMayor').focus();
                return false;
            }
            // Validar Talla
            if (talla == null || talla.length == 0 || document.getElementById('DDL_Talla').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar la Talla del Producto');
                document.getElementById('DDL_Talla').focus();
                return false;
            }
            // Validar Color
            if (color == null || color.length == 0 || document.getElementById('DDL_Color').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar el Color del Producto');
                document.getElementById('DDL_Color').focus();
                return false;
            }

            // Validar Género
            if (genero == null || genero.length == 0 || document.getElementById('DDL_Genero').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar el Género del Producto');
                document.getElementById('DDL_Genero').focus();
                return false;
            }
            // Validar Estado
            if (estado == null || estado.length == 0 || document.getElementById('DDL_Estado').value == '-Seleccione--') {
                alert('ERROR: Debe Seleccionar el Estado del Producto');
                document.getElementById('DDL_Estado').focus();
                return false;
            }
        }
    </script>

<body class="body">
    <form id="form1" runat="server" class="Formulario_Articulo">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div>
            <center> 

            <table class="Tabla_Articulo">
                <tr>
                    <td colspan="2" class="Titulo_Formulario">Registro de Productos</td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Código:</td>
                    <td class="Fila">
                        <asp:Label ID="Lb_CodProducto" runat="server" Text="Lb_CodProducto" CssClass="Etiqueta_Articulo"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Nombre:</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXT_Producto" runat="server" CssClass="Control_Text" Width="188px" Style="text-transform: uppercase;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Descripción del producto:</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXTDescripcion_Producto" runat="server" CssClass="Control_Text" Width="188px" Style="text-transform: uppercase;" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Marca:</td>
                    <td class="Fila">
                         <asp:UpdatePanel ID="UpdatePanelMarca" runat="server">
                           <ContentTemplate>
                        <asp:DropDownList ID="DDL_Marca" runat="server" AutoPostBack="true"  CssClass="Control_Lista-Desplegable" OnSelectedIndexChanged="DDL_Marca_SelectedIndexChanged">
                        </asp:DropDownList>
                     </ContentTemplate>
                   </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Modelo:</td>
                    <td class="Fila">
                    <asp:UpdatePanel ID="UpdatePanelModelo" runat="server">
                       <ContentTemplate>
                        <asp:DropDownList ID="DDL_Modelo" runat="server" CssClass="Control_Lista-Desplegable">
                        </asp:DropDownList>
                         </ContentTemplate>
                   </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Talla:</td>
                    <td class="Fila">
                        <asp:DropDownList ID="DDL_Talla" runat="server" CssClass="Control_Lista-Desplegable">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Color:</td>
                    <td class="Fila">
                        <asp:DropDownList ID="DDL_Color" runat="server" CssClass="Control_Lista-Desplegable">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Genero:</td>
                    <td class="Fila">
                        <asp:DropDownList ID="DDL_Genero" runat="server" CssClass="Control_Lista-Desplegable" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Precio Venta menor:</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXT_PrecVentaMenor" placeholder="Soles" runat="server"  CssClass="Control_Text" Width="80px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Precio Venta mayor:</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXT_PrecVentaMayor" placeholder="Soles" runat="server" CssClass="Control_Text" Width="80px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Stock General:</td>
                    <td class="Fila">
                        <asp:TextBox ID="TXT_Stock"  runat="server" CssClass="Control_Text" Width="80px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Etiqueta_Articulo">Categoria:</td>
                    <td class="Fila">
                        <asp:DropDownList ID="DDL_Categoria" runat="server" CssClass="Control_Lista-Desplegable">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4">Foto:</td>
                    <td class="Fila">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style6">Estado:</td>
                    <td class="Fila">
                        <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="Control_Lista-Desplegable">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" class="Center-Content">
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="Boton" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BTN_Guardar" runat="server" Text="Guardar" CssClass="Boton" OnClick="BTN_Guardar_Click"      
                            OnClientClick ="ValidarFormulario();"/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BTN_Reporte" runat="server" Text="Ver Reporte" CssClass="Boton" OnClick="BTN_Reporte_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1"></td>
                    <td class="auto-style1"></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
          </center>
        </div>
    </form>
</body>
</html>

