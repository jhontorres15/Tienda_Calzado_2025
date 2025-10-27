<%@ Page Title="Reporte de Productos" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Reporte_Productos.aspx.cs" Inherits="Reporte_Productos" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">

 <link rel="stylesheet" href="Estilos/Estilo_Reporte_Producto.css" type="text/css" />
    <script src="https://unpkg.com/html5-qrcode"></script>
    <script src="js/efectos.js" type="text/javascript"></script>

        <link rel="stylesheet" href="Estilos/Estilo_Menu.css" type="text/css" />
    </asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

   

    
    <div class="Formulario_Reporte_Articulo">
        
            <left> 
            <table class="Tabla_Reporte">
                <tr>
                    <td class="Titulo_Formulario">Reportes de Artículos</td>
                </tr>
            </table>
</left>
            <br />

            <asp:TextBox ID="txtBuscar" runat="server" Placeholder="Buscar producto por nombre o código" CssClass="txtBuscar"></asp:TextBox>
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CssClass="btnBuscar" />

            <br /><br />

            <asp:GridView ID="GV_Productos" runat="server" AutoGenerateColumns="False" Width="1100px" OnRowCommand="GV_Productos_RowCommand">
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

                    <asp:TemplateField HeaderText="Foto">
                        <HeaderStyle CssClass="Cabecera_Etiqueta" />
                        <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" />
                        <ItemTemplate>
                        <asp:Image ID="IMG_Item" runat="server" 
                            ImageUrl='<%# "data:image/jpeg;base64," + Convert.ToBase64String((byte[])Eval("Foto")) %>' 
                            Width="50px" />
                          </ItemTemplate>
                        </asp:TemplateField>


                </Columns>
            </asp:GridView>

            
        
    
        </div>
</asp:Content>