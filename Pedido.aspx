<%@ Page Title="Gestión de Pedidos" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Pedido.aspx.cs" Inherits="Pedido" ValidateRequest="false"%> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Pedido.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
      
    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var nroPedido = document.getElementById('<%= TXT_NroPedido.ClientID %>').value;
            var codSucursal = document.getElementById('<%= TXT_CodSucursal.ClientID %>').value;
            var codTipoPedido = document.getElementById('<%= DDL_TipoPedido.ClientID %>').selectedIndex;
            var codCliente = document.getElementById('<%= DDL_Cliente.ClientID %>').selectedIndex;
            var codEmpleado = document.getElementById('<%= DDL_Empleado.ClientID %>').selectedIndex;
            var fecPedido = document.getElementById('<%= TXT_FecPedido.ClientID %>').value;

            if (nroPedido.trim() === "") { alert("Ingrese número de pedido"); return false; }
            if (nroPedido.length !== 12) { alert("El número de pedido debe tener 12 caracteres"); return false; }
            if (codSucursal.trim() === "") { alert("Ingrese código de sucursal"); return false; }
            if (codSucursal.length !== 5) { alert("El código de sucursal debe tener 5 caracteres"); return false; }
            if (codTipoPedido <= 0) { alert("Seleccione tipo de pedido"); return false; }
            if (codCliente <= 0) { alert("Seleccione un cliente"); return false; }
            if (codEmpleado <= 0) { alert("Seleccione un empleado"); return false; }
            if (fecPedido.trim() === "") { alert("Seleccione fecha de pedido"); return false; }

            return true;
        }

        function ValidarDetalle() {
            var nroSerieProducto = document.getElementById('<%= DDL_Producto.ClientID %>').selectedIndex;
            var cantidad = document.getElementById('<%= TXT_Cantidad.ClientID %>').value;
            var precVenta = document.getElementById('<%= TXT_PrecVenta.ClientID %>').value;
            var porcentajeDscto = document.getElementById('<%= TXT_PorcentajeDscto.ClientID %>').value;

            if (nroSerieProducto <= 0) { alert("Seleccione un producto"); return false; }
            if (cantidad.trim() === "" || parseInt(cantidad) <= 0) { alert("Ingrese una cantidad válida"); return false; }
            if (precVenta.trim() === "" || parseFloat(precVenta) <= 0) { alert("Ingrese un precio de venta válido"); return false; }
            if (porcentajeDscto.trim() === "") { alert("Ingrese porcentaje de descuento (0 si no aplica)"); return false; }

            return true;
        }

        function CalcularDescuentoYSubtotal() {
            var cantidad = document.getElementById('<%= TXT_Cantidad.ClientID %>').value;
            var precVenta = document.getElementById('<%= TXT_PrecVenta.ClientID %>').value;
            var porcentajeDscto = document.getElementById('<%= TXT_PorcentajeDscto.ClientID %>').value;
            
            if (cantidad && precVenta && porcentajeDscto !== "") {
                var importe = parseFloat(cantidad) * parseFloat(precVenta);
                var descuento = importe * (parseFloat(porcentajeDscto) / 100);
                var subtotal = importe - descuento;
                
                document.getElementById('<%= LBL_Importe.ClientID %>').innerText = importe.toFixed(2);
                document.getElementById('<%= LBL_Descuento.ClientID %>').innerText = descuento.toFixed(2);
                document.getElementById('<%= LBL_SubTotal.ClientID %>').innerText = subtotal.toFixed(2);
            }
        }

        function ValidarProforma() {
            var nroProforma = document.getElementById('<%= TXT_NroProformaVerificar.ClientID %>').value;
            if (nroProforma.trim() === "") {
                alert("Ingrese número de proforma para verificar");
                return false;
            }
            if (nroProforma.length !== 10) {
                alert("El número de proforma debe tener 10 caracteres");
                return false;
            }
            return true;
        }
    </script> 

    <script language="javascript" type="text/javascript">
        function SoloNumeros(e) {
            key = e.keyCode || e.which;
            tecla = String.fromCharCode(key).toLowerCase();
            letras = "0123456789.";
            especiales = [8, 37, 39, 46];

            tecla_especial = false
            for (var i in especiales) {
                if (key == especiales[i]) {
                    tecla_especial = true;
                    break;
                }
            }

            if (letras.indexOf(tecla) == -1 && !tecla_especial)
                return false;
        }

        function SoloLetrasNumeros(e) {
            key = e.keyCode || e.which;
            tecla = String.fromCharCode(key).toLowerCase();
            letras = "abcdefghijklmnñopqrstuvwxyz0123456789-";
            especiales = [8, 37, 39, 46];

            tecla_especial = false
            for (var i in especiales) {
                if (key == especiales[i]) {
                    tecla_especial = true;
                    break;
                }
            }

            if (letras.indexOf(tecla) == -1 && !tecla_especial)
                return false;
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-9">
                <!-- Sección de Verificación de Proforma -->
                <div class="card shadow-lg border-0 mb-4 card-proforma-check">
                    <div class="card-header bg-gradient-info text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-search-plus me-2"></i>
                            Cargar desde Proforma
                        </h4>
                    </div>
                    <div class="card-body p-4">
                        <div class="row align-items-end">
                            <div class="col-md-4">
                                <label for="TXT_NroProformaVerificar" class="form-label">
                                    <i class="fas fa-file-invoice text-info me-1"></i>
                                    Nro. Proforma
                                </label>
                                <asp:TextBox ID="TXT_NroProformaVerificar" runat="server" CssClass="form-control" 
                                           MaxLength="10" onkeypress="return SoloLetrasNumeros(event)" 
                                           placeholder="Ej: PRO-000001"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <asp:Button ID="BTN_VerificarProforma" runat="server" Text="Verificar Proforma" 
                                          CssClass="btn btn-info w-100" OnClick="BTN_VerificarProforma_Click" 
                                          OnClientClick="return ValidarProforma()">
                                    
                                </asp:Button>
                            </div>
                            <div class="col-md-4">
                                <asp:Button ID="BTN_CargarProforma" runat="server" Text="Cargar Datos" 
                                          CssClass="btn btn-success w-100" OnClick="BTN_CargarProforma_Click" 
                                          Enabled="false">
                                   
                                </asp:Button>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <asp:Label ID="LBL_EstadoProforma" runat="server" CssClass="alert alert-info d-none" 
                                         Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Formulario de Pedido -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-shopping-cart me-2"></i>
                            Registro de Pedido
                        </h4>
                    </div>
                    <div class="card-body p-4">
                        <!-- Información del Pedido -->
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="TXT_NroPedido" class="form-label">
                                    <i class="fas fa-hashtag text-primary me-1"></i>
                                    Nro. Pedido
                                </label>
                                <asp:TextBox ID="TXT_NroPedido" runat="server" CssClass="form-control" 
                                           MaxLength="12" onkeypress="return SoloLetrasNumeros(event)" 
                                           placeholder="Ej: PED-00000001"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_CodSucursal" class="form-label">
                                    <i class="fas fa-store text-success me-1"></i>
                                    Cod. Sucursal
                                </label>
                                <asp:TextBox ID="TXT_CodSucursal" runat="server" CssClass="form-control" 
                                           MaxLength="5" onkeypress="return SoloLetrasNumeros(event)" 
                                           placeholder="Ej: SUC01"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_TipoPedido" class="form-label">
                                    <i class="fas fa-tags text-warning me-1"></i>
                                    Tipo Pedido
                                </label>
                                <asp:DropDownList ID="DDL_TipoPedido" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Tipo --</asp:ListItem>
                                    <asp:ListItem Value="NORMAL">Normal</asp:ListItem>
                                    <asp:ListItem Value="URGENT">Urgente</asp:ListItem>
                                    <asp:ListItem Value="ESPECI">Especial</asp:ListItem>
                                    <asp:ListItem Value="MAYOREO">Mayoreo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="DDL_Cliente" class="form-label">
                                    <i class="fas fa-user text-info me-1"></i>
                                    Cliente
                                </label>
                                <asp:DropDownList ID="DDL_Cliente" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Cliente --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-secondary me-1"></i>
                                    Empleado
                                </label>
                                <asp:DropDownList ID="DDL_Empleado" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Empleado --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_FecPedido" class="form-label">
                                    <i class="fas fa-calendar-alt text-danger me-1"></i>
                                    Fecha Pedido
                                </label>
                                <asp:TextBox ID="TXT_FecPedido" runat="server" CssClass="form-control" 
                                           TextMode="DateTimeLocal"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label for="DDL_EstadoPedido" class="form-label">
                                    <i class="fas fa-flag text-primary me-1"></i>
                                    Estado Pedido
                                </label>
                                <asp:DropDownList ID="DDL_EstadoPedido" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Pendiente">Pendiente</asp:ListItem>
                                    <asp:ListItem Value="En Proceso">En Proceso</asp:ListItem>
                                    <asp:ListItem Value="Completado">Completado</asp:ListItem>
                                    <asp:ListItem Value="Cancelado">Cancelado</asp:ListItem>
                                    <asp:ListItem Value="Entregado">Entregado</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">
                                    <i class="fas fa-money-bill-wave text-success me-1"></i>
                                    Total Pedido
                                </label>
                                <asp:Label ID="LBL_TotalPedido" runat="server" CssClass="form-control bg-success text-white fw-bold text-center" 
                                         Text="S/. 0.00"></asp:Label>
                            </div>
                        </div>

                        <!-- Detalle de Productos -->
                       
                        <h5 class="text-primary mb-3">
                            <i class="fas fa-list-ul me-2"></i>
                            Detalle de Productos
                        </h5>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="DDL_Producto" class="form-label">
                                    <i class="fas fa-box text-success me-1"></i>
                                    Producto
                                </label>
                                <asp:DropDownList ID="DDL_Producto" runat="server" CssClass="form-select" 
                                                AutoPostBack="true" OnSelectedIndexChanged="DDL_Producto_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-- Seleccionar Producto --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <label for="TXT_Cantidad" class="form-label">
                                    <i class="fas fa-sort-numeric-up text-info me-1"></i>
                                    Cantidad
                                </label>
                                <asp:TextBox ID="TXT_Cantidad" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularDescuentoYSubtotal()" 
                                           placeholder="0"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <label for="TXT_PrecVenta" class="form-label">
                                    <i class="fas fa-dollar-sign text-warning me-1"></i>
                                    Precio
                                </label>
                                <asp:TextBox ID="TXT_PrecVenta" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularDescuentoYSubtotal()" 
                                           placeholder="0.00"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <label for="TXT_PorcentajeDscto" class="form-label">
                                    <i class="fas fa-percentage text-danger me-1"></i>
                                    % Dscto
                                </label>
                                <asp:TextBox ID="TXT_PorcentajeDscto" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularDescuentoYSubtotal()" 
                                           placeholder="0" Text="0"></asp:TextBox>
                            </div>
                            <div class="col-md-2 d-flex align-items-end">
                                <asp:Button ID="BTN_AgregarDetalle" runat="server" Text="Agregar" 
                                          CssClass="btn btn-success w-100" OnClick="BTN_AgregarDetalle_Click" 
                                          OnClientClick="return ValidarDetalle()">
                                   
                                </asp:Button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-calculator text-secondary me-1"></i>
                                    Importe
                                </label>
                                <asp:Label ID="LBL_Importe" runat="server" CssClass="form-control bg-light" 
                                         Text="0.00"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-minus-circle text-danger me-1"></i>
                                    Descuento
                                </label>
                                <asp:Label ID="LBL_Descuento" runat="server" CssClass="form-control bg-warning text-dark" 
                                         Text="0.00"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-check-circle text-success me-1"></i>
                                    SubTotal
                                </label>
                                <asp:Label ID="LBL_SubTotal" runat="server" CssClass="form-control bg-success text-white fw-bold" 
                                         Text="0.00"></asp:Label>
                            </div>
                        </div>

                        <!-- GridView de Detalles -->
                        <div class="table-responsive">
                            <asp:GridView ID="GV_DetallePedido" runat="server" CssClass="table table-striped table-hover" 
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5" 
                                        OnPageIndexChanging="GV_DetallePedido_PageIndexChanging"
                                        OnRowCommand="GV_DetallePedido_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" />
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                    <asp:BoundField DataField="Prec_Venta" HeaderText="Precio" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Porcentaje_Dscto" HeaderText="% Dscto" DataFormatString="{0:N2}%" />
                                    <asp:BoundField DataField="Dscto" HeaderText="Descuento" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" DataFormatString="{0:C}" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="BTN_EliminarDetalle" runat="server" Text="Eliminar" 
                                                      CssClass="btn btn-danger btn-sm" 
                                                      CommandName="EliminarDetalle" 
                                                      CommandArgument='<%# Container.DataItemIndex %>'
                                                      OnClientClick="return confirm('¿Está seguro de eliminar este detalle?')">
                                                
                                            </asp:Button>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination justify-content-center" />
                            </asp:GridView>
                        </div>

                        <!-- Botones de Acción -->
                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary me-2" 
                                          OnClick="BTN_Nuevo_Click">
                                   
                                </asp:Button>
                                <asp:Button ID="BTN_Grabar" runat="server" Text="Grabar" CssClass="btn btn-success me-2" 
                                          OnClick="BTN_Grabar_Click" OnClientClick="return ValidarFormulario()">
                                   
                                </asp:Button>
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary me-2" 
                                          OnClick="BTN_Cancelar_Click">
                                   
                                </asp:Button>
                                <asp:Button ID="BTN_Imprimir" runat="server" Text="Imprimir" CssClass="btn btn-info" 
                                          OnClick="BTN_Imprimir_Click">
                                  
                                </asp:Button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Listado de Pedidos -->
                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-secondary text-white">
                        <div class="row align-items-center">
                            <div class="col-md-6">
                                <h4 class="mb-0">
                                    <i class="fas fa-list me-2"></i>
                                    Listado de Pedidos
                                </h4>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control" 
                                               placeholder="Buscar pedido..."></asp:TextBox>
                                    <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-outline-light" 
                                              OnClick="BTN_Buscar_Click">
                                       
                                    </asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Pedidos" runat="server" CssClass="table table-striped table-hover mb-0" 
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="10" 
                                        >
                                <Columns>
                                    <asp:BoundField DataField="NroPedido" HeaderText="Nro. Pedido" />
                                    <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
                                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="TipoPedido" HeaderText="Tipo" />
                                    <asp:BoundField DataField="Fec_Pedido" HeaderText="F. Pedido" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Estado_Pedido" HeaderText="Estado" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="BTN_Editar" runat="server" Text="Editar" 
                                                      CssClass="btn btn-warning btn-sm me-1" 
                                                      CommandName="EditarPedido" 
                                                      CommandArgument='<%# Eval("NroPedido") %>'>
                                            
                                            </asp:Button>
                                            <asp:Button ID="BTN_Eliminar" runat="server" Text="Eliminar" 
                                                      CssClass="btn btn-danger btn-sm" 
                                                      CommandName="EliminarPedido" 
                                                      CommandArgument='<%# Eval("NroPedido") %>'
                                                      OnClientClick="return confirm('¿Está seguro de eliminar este pedido?')">
                                               
                                            </asp:Button>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination justify-content-center" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
