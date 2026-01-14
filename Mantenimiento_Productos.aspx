<%@ Page Title="Registro Productos" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true"  EnableEventValidation="false" CodeFile="Mantenimiento_Productos.aspx.cs" Inherits="Mantenimiento_Productos" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Cliente.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
     <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
     <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

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
            if (isNaN(stock) || parseInt(stock) === "") { alert('Ingrese un Stock válido'); return false; }
            if (isNaN(precioVentaMenor) || parseFloat(precioVentaMenor) <= 0) { alert('Ingrese Precio Venta Menor válido'); return false; }
            if (isNaN(precioVentaMayor) || parseFloat(precioVentaMayor) <= 0) { alert('Ingrese Precio Venta Mayor válido'); return false; }
            if (talla <= 0) { alert('Seleccione la Talla'); return false; }
            if (color <= 0) { alert('Seleccione el Color'); return false; }
            if (estado <= 0) { alert('Seleccione el Estado'); return false; }
        }
    </script>
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<div class="container mt-4">
    <h1 class="text-center mb-4 text-primary">Registro de Productos</h1>

    <div class="row derecha1">
        <!-- Formulario de registro -->
        <div class="col-md-9">
            <div class="card mb-4">
                <div class="card-header bg-primary text-white">
                    <h5>Datos del Producto</h5>
                </div>
                <div class="card-body">
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label for="<%= Lb_CodProducto.ClientID %>" class="form-label">Código Producto:</label>
                            <asp:Label ID="Lb_CodProducto" runat="server" CssClass="form-control" Style="font-weight: bold; color: #2c3e50;"></asp:Label>
                        </div>
                        <div class="col-md-5">
                            <label for="<%= TXT_Producto.ClientID %>" class="form-label">Nombre del Producto:</label>
                            <asp:TextBox ID="TXT_Producto" runat="server" CssClass="form-control" Style="text-transform: uppercase;" />
                        </div>
                        <div class="col-md-4">
                            <label for="<%= DDL_Categoria.ClientID %>" class="form-label">Categoría:</label>
                            <asp:DropDownList ID="DDL_Categoria" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-8">
                            <label for="<%= TXTDescripcion_Producto.ClientID %>" class="form-label">Descripción del Producto:</label>
                            <asp:TextBox ID="TXTDescripcion_Producto" runat="server" CssClass="form-control" Style="text-transform: uppercase;" TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="col-md-4">
                            <label for="<%= DDL_Estado.ClientID %>" class="form-label">Estado:</label>
                            <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label for="<%= DDL_Marca.ClientID %>" class="form-label">Marca:</label>
                          <asp:UpdatePanel ID="UpdatePanelMarca" runat="server">
                                <ContentTemplate>
                                    <asp:DropDownList 
                                        ID="DDL_Marca" 
                                        runat="server" 
                                        AutoPostBack="true" 
                                        CssClass="form-select" 
                                     OnSelectedIndexChanged="DDL_Marca_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </ContentTemplate>
                               
                            </asp:UpdatePanel>
                        </div>
                        <div class="col-md-3">
                            <label for="<%= DDL_Modelo.ClientID %>" class="form-label">Modelo:</label>
                          <asp:UpdatePanel ID="UpdatePanelModelo" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Modelo" runat="server"  CssClass="form-select"  >
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        </div>
                        <div class="col-md-3">
                            <label for="<%= DDL_Talla.ClientID %>" class="form-label">Talla:</label>
                            <asp:DropDownList ID="DDL_Talla" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label for="<%= DDL_Color.ClientID %>" class="form-label">Color:</label>
                            <asp:DropDownList ID="DDL_Color" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label for="<%= TXT_PrecVentaMenor.ClientID %>" class="form-label">Precio Venta Menor:</label>
                            <asp:TextBox ID="TXT_PrecVentaMenor" placeholder="Soles"  runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_PrecVentaMayor.ClientID %>" class="form-label">Precio Venta Mayor:</label>
                            <asp:TextBox ID="TXT_PrecVentaMayor" placeholder="Soles" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_Stock.ClientID %>" class="form-label">Stock:</label>
                            <asp:TextBox ID="TXT_Stock" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Foto:</label>
                            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label">Imagen Actual:</label>
                            <asp:Image ID="IMG_Calzado" runat="server" Width="150px" Height="150px" ImageUrl="~/IMG/LOGO.jpeg" CssClass="img-thumbnail" />
                        </div>
                        <div class="col-md-9 d-flex align-items-end">
                            <div class="text-center">
                                <asp:Label ID="img_desc" runat="server" CssClass="form-text text-muted"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="text-center mt-4">
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary" OnClick="BTN_Nuevo_Click"/>
                        &nbsp;
                        <asp:Button ID="BTN_Guardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClientClick="return ValidarFormulario();" OnClick="BTN_Guardar_Click" />
                        &nbsp;
                    </div>
                </div>
            </div>
        </div>
    </div>


    
    <!-- Tabla de productos -->
    <div class="row right">
        <div class="col-md-10 offset-md-2">
            <div class="card">
                <div class="card-header bg-success text-white d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Listado de Productos</h5>
                    <div>
                        <asp:TextBox ID="TXT_BusquedaProducto" runat="server" CssClass="form-control form-control-sm d-inline-block" placeholder="Buscar producto..." Width="200px" />
                        <asp:Button ID="BTN_BuscarProducto" runat="server" Text="Buscar" CssClass="btn btn-sm btn-light ms-2" />
                        <asp:Button ID="BTN_ExportarProducto" runat="server" Text="Exportar" CssClass="btn btn-sm btn-light ms-2" />
                    </div>
                </div>
                <div class="card-body">
                  <asp:GridView ID="GV_Productos" runat="server"  AutoGenerateColumns="False"
                    CssClass="table table-striped table-hover"
                    OnRowCommand="GV_Productos_RowCommand" Width="100%" 
                      DataKeyNames="CodProducto,Producto,Descripcion_Producto,Categoria,Marca,Modelo,Talla,Color,Prec_Venta_Menor,Prec_Venta_Mayor,Stock_General,Estado_Producto,Foto"
                            EnableViewState="false"
                      AllowPaging="true" PageSize="10"
                      OnPageIndexChanging="GV_Productos_PageIndexChanging" >
                    <Columns>

                        <asp:BoundField DataField="CodProducto" HeaderText="Código">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="80px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Producto" HeaderText="Producto">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="150px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Descripcion_Producto" HeaderText="Descripción"  Visible="False">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="250px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Categoria" HeaderText="Categoría"  Visible="True">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="120px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Marca" HeaderText="Marca" >
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="80px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Modelo" HeaderText="Modelo">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="90px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Talla" HeaderText="Talla">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="50px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Color" HeaderText="Color">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="100px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Prec_Venta_Menor" HeaderText="Prec. Venta Menor">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="80px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Prec_Venta_Mayor" HeaderText="Prec. Venta Mayor">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="80px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Stock_General" HeaderText="Stock">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="80px" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Estado_Producto" HeaderText="Estado Producto" >
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" Width="100px" />
                        </asp:BoundField>

                        <asp:BoundField HeaderText="Foto" Visible="False">
                            <HeaderStyle CssClass="Cabecera_Etiqueta" />
                            <ItemStyle CssClass="Fila_Dato" HorizontalAlign="Center" />
                          
                        </asp:BoundField>

                         <asp:ButtonField ButtonType="Button" CommandName="BTN_Ver" Text="Editar">
                         <ItemStyle CssClass="btn btn-primary btn-sm text-white" Width="90px" />
                     </asp:ButtonField>

                    </Columns>

                    <PagerStyle CssClass="pagination justify-content-center" />
                </asp:GridView>
                   
                </div>
            </div>
        </div>
    </div>



</div>

</asp:Content>
