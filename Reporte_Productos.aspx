<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reporte_Productos.aspx.cs" Inherits="Reporte_Productos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Reporte de Productos</title>
    <link rel="stylesheet" href="Estilos/Estilo_Reporte_Producto.css" type="text/css" />
    <style type="text/css">

    </style>
</head>
<body class="body">
    <form id="form1" runat="server" class="Formulario_Reporte_Articulo">
        <div>
            <center> 
            <table class="auto-style1">
                <tr>
                    <td class="Titulo_Formulario">Reportes de Articulos</td>
                </tr>
            </table>
                <br />
                <asp:GridView ID="GV_Productos" runat="server" AutoGenerateColumns="False" Width="1100px">
                    <Columns>
                        <asp:ButtonField ButtonType="Button" CommandName="BTN_Ver" Text="Ver Detalle" >
                        <ItemStyle CssClass="BotonVerDetalle" Width="50px" />
                        </asp:ButtonField>
                        <asp:BoundField DataField="CodProducto" HeaderText="Código">
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Producto" HeaderText="Producto" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descripcion_Producto" HeaderText="Descripcion">
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Categoria" HeaderText="Categoría" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Marca" HeaderText="Marca" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Talla" HeaderText="Talla" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="30px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Color" HeaderText="Color" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Genero" HeaderText="Género" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Prec_Venta_Menor" HeaderText="Prec. Venta Menor" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Prec_Venta_Mayor" HeaderText="Prec. Venta Mayor" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Stock_General" HeaderText="Stock" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Estado_Producto" HeaderText="Estado Producto" >
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" VerticalAlign="Middle" Width="60px" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <br /> 
            </center>
        </div>
    </form>
</body>
</html>
