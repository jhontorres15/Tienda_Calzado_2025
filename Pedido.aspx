<%@ Page Title="Gestión de Pedidos" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Pedido.aspx.cs" Inherits="Pedido" ValidateRequest="false"%> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Pedido.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
     <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>


    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var nroPedido = document.getElementById('<%= TXT_NroPedido.ClientID %>').value;
           
            var codTipoPedido = document.getElementById('<%= DDL_TipoPedido.ClientID %>').selectedIndex;
            var codCliente = document.getElementById('<%= TXT_Cliente.ClientID %>').selectedIndex;
            var codEmpleado = document.getElementById('<%= TXT_Empleado.ClientID %>').selectedIndex;
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
       <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-10">
                <!-- Sección de Verificación de Proforma -->
                <div class="card shadow-lg border-0 mb-4 card-proforma-check">
                    <div class="card-header  bg-gradient-secondary text-white">
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
                                <asp:Button ID="BTN_CargarProforma" runat="server" Text="Cargar Datos" 
                                          CssClass="btn btn-success w-100" OnClick="BTN_CargarProforma_Click" 
                                          >
                                   
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
                    <div class="card-header  bg-gradient-secondary text-white">
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
                                <label for="DDL_Sucursal" class="form-label">
                                    <i class="fas fa-store text-success me-1"></i>
                                    Sucursal
                                </label>
                                <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select" ></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_TipoPedido" class="form-label">
                                    <i class="fas fa-tags text-warning me-1"></i>
                                    Tipo Pedido
                                </label>
                                <asp:DropDownList ID="DDL_TipoPedido" runat="server" CssClass="form-select">
                                    
                                 
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                                    
                                
                            <asp:UpdatePanel ID="UP_Cliente" runat="server" UpdateMode="Conditional">
                           <ContentTemplate>

                                <label for="TXT_Cliente" class="form-label">
                                    <i class="fas fa-user text-info me-1"></i>
                                    Cliente
                                </label>
                                <asp:TextBox ID="TXT_Cliente" runat="server" CssClass="form-control" placeholder="Buscar Cliente" ReadOnly="true">
                                    
                                </asp:TextBox>
                                 <asp:HiddenField ID="HFD_CodCliente" runat="server" Value="0" />
                                 </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-secondary me-1"></i>
                                    Empleado
                                </label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control" ReadOnly="true">                                   
                                </asp:TextBox>
                                 <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
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
                           
                        </div>

                        <!-- Detalle de Productos -->
                       
                        <h5 class="text-primary mb-3">
                            <i class="fas fa-list-ul me-2"></i>
                            Detalle de Productos
                        </h5>


                    <asp:UpdatePanel ID="UP_FormularioDetalle" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="DDL_Producto" class="form-label">
                                    <i class="fas fa-box text-success me-1"></i>
                                    Producto
                                </label>
                                
                            <asp:HiddenField ID="HFD_CodProducto" runat="server" />
                            <asp:HiddenField ID="HFD_NroSerie" runat="server" />
                            <asp:HiddenField ID="HFD_PrecioUnitario" runat="server" />
                            <asp:HiddenField ID="HFD_PrecioMayor" runat="server" />
                            <asp:HiddenField ID="HFD_StockDisponible" runat="server" />

                                <asp:TextBox ID="TXT_ProductoNombre" runat="server" CssClass="form-control" 
                                                AutoPostBack="true" placeholder="Buscar producto" readonly="true">
                                   
                                </asp:TextBox>
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
                                 CssClass="btn btn-success w-100" OnClick="BTN_AgregarDetalle_Click">
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
                             <div class="col-md-3">
                             <label class="form-label">
                                 <i class="fas fa-money-bill-wave text-success me-1"></i>
                                 Total Pedido
                             </label>
                             <asp:Label ID="LBL_TotalPedido" runat="server" CssClass="form-control bg-success text-white fw-bold text-center" 
                                      Text="S/. 0.00"></asp:Label>
                         </div>
                        </div>

                        <!-- GridView de Detalles -->
                        <div class="table-responsive">
                            <asp:GridView ID="GV_DetallePedido" runat="server" CssClass="table table-striped table-hover" 
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5" 
                                        OnPageIndexChanging="GV_DetallePedido_PageIndexChanging"
                                        OnRowCommand="GV_DetallePedido_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="NroSerie" />
                                    <asp:BoundField DataField="ProductoNombre" HeaderText="Producto"  />
                                    <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" Visible="false" />
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
                                          OnClick="BTN_Grabar_Click">
                                   
                                </asp:Button>
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary me-2" 
                                          OnClick="BTN_Cancelar_Click">
                                   
                                </asp:Button>
                                <asp:Button ID="BTN_Imprimir" runat="server" Text="Imprimir" CssClass="btn btn-info" 
                                          OnClick="BTN_Imprimir_Click">

                                  
                                </asp:Button>
                                  <asp:Button ID="BTN_Buscar_Cliente" runat="server" Text="Buscar Cliente" CssClass="btn btn-secondary me-2" 
                                          OnClientClick="$('#modalBuscarCliente').modal('show'); return false;" >
                                 </asp:Button> 
                                     <asp:Button ID="BTN_AbrirModalBuscar" runat="server" Text="Buscar Producto" CssClass="btn btn-secondary me-2" 
                            OnClick="BTN_AbrirModalBuscar_Click" >
                                      </asp:Button> 
                            </div>
                        </div>

                            
                              </ContentTemplate>
                            </asp:UpdatePanel>
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
                            <asp:GridView ID="GV_Pedidos" runat="server" 
                           CssClass="table table-striped table-hover mb-0 fix-table"
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="10" 
                                 OnRowCommand="GV_Pedidos_RowCommand"
                                >
                               <Columns>
                                <asp:BoundField DataField="NroPedido" HeaderText="Nro. Pedido" >
                                       <HeaderStyle CssClass="col-nropedido" />
                                         <ItemStyle CssClass="col-nropedido" />
                                  </asp:BoundField>
                                <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
                                <asp:BoundField DataField="Empleado" HeaderText="Vendedor" />
                                <asp:BoundField DataField="Sucursal" HeaderText="Sucursal" visible="false" />
                                <asp:BoundField DataField="Fec_Pedido" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="Total" HeaderText="Total (S/)" DataFormatString="{0:N2}" />
            
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstado" runat="server" 
                                               Text='<%# Eval("Estado_Pedido") %>'
                                                   CssClass='<%# ObtenerClaseEstado(Eval("Estado_Pedido").ToString()) %>'>
                                        </asp:Label>
               
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:ButtonField Text="Ver Detalle" CommandName="VerDetalle" ButtonType="Button" ControlStyle-CssClass="btn btn-info btn-sm" />
                               
                                   <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:Button ID="btnAtender" runat="server" 
                                        Text="Atender"
                                        CommandName="Atender"
                                        CommandArgument='<%# Eval("NroPedido") %>'
                                        CssClass="btn btn-success btn-sm"
                                        CausesValidation="False"
                                        Visible='<%# Eval("Estado_Pedido").ToString() == "Pendiente" %>' />
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




            <!-- MODAL PARA BUSCAR CLIENTE -->

<div class="modal fade" id="modalBuscarCliente" tabindex="-1" aria-labelledby="modalBuscarClienteLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg"> <div class="modal-content">

            <div class="modal-header bg-primary text-white">
                <h5 class="modal-title" id="modalBuscarClienteLabel">
                    <i class="fas fa-search me-2"></i> Buscar Cliente
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>

            <div class="modal-body">

                <asp:UpdatePanel ID="UP_ModalCliente" runat="server" UpdateMode="Conditional">
                 <ContentTemplate>

                <div class="mb-3">
                    <label for="<%= txtBuscarNombre.ClientID %>" class="form-label fw-bold">
                        <i class="fas fa-signature me-1 text-primary"></i> Ingrese nombre o RUC:
                    </label>
                    <div class="input-group">
                        <asp:TextBox ID="txtBuscarNombre" CssClass="form-control" runat="server" placeholder="Nombre, RUC o DNI" />
                        <asp:Button ID="BTN_Buscar_Modal" runat="server" Text="Buscar" OnClick="BTN_Buscar_Modal_Click" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
                <div class="table-responsive mt-4">
                    <h6 class="mb-3 text-secondary">Resultados de Búsqueda</h6>
                    <asp:GridView ID="GV_Clientes_Modal" runat="server" 
                        AutoGenerateColumns="false" 
                        CssClass="table table-striped table-hover table-bordered"
                        OnRowCommand="GV_Clientes_Modal_RowCommand">
                         <Columns>
    
                        <asp:BoundField DataField="CodCliente" HeaderText="ID" ItemStyle-Width="50px" />
    
                        <asp:TemplateField HeaderText="Nombre">
                          
                            <ItemTemplate>
                                <asp:Label ID="LBL_NombreCompleto" runat="server" 
                                    Text='<%# Eval("Nombre") + " " + Eval("Apellido") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
    
                        <asp:BoundField DataField="Identificacion" HeaderText="Identificación" />
    
                        <asp:TemplateField HeaderText="Acción" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:Button ID="BTN_Seleccionar" runat="server" Text="Seleccionar" 
                                    CssClass="btn btn-success btn-sm w-100" 
                                    CommandName="SeleccionarCliente"
                                    CommandArgument='<%# Eval("CodCliente") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                  </asp:GridView>
                </div>


               </ContentTemplate>
           <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BTN_Buscar_Modal" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">
                    <i class="fas fa-times me-1"></i> Cerrar
                </button>
            </div>

        </div>
    </div>
</div>







    <div class="modal fade" id="modalBuscarProducto" tabindex="-1" aria-labelledby="modalBuscarProductoLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-xl"> <!-- modal-xl es 'Extra Grande' para que entren los filtros -->
        <div class="modal-content">

            <div class="modal-header bg-success text-white">
                <h5 class="modal-title" id="modalBuscarProductoLabel">
                    <i class="fas fa-search-plus me-2"></i> 
                    Buscar Producto en Sucursal: 
                    <asp:Label ID="LBL_Modal_Sucursal" runat="server" Text="[Seleccione Sucursal]" Font-Bold="true"></asp:Label>
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>

            <!-- 
                Este UpdatePanel es CRUCIAL. 
                Envuelve TODO el contenido del modal para que los filtros en cascada (AutoPostBack) 
                funcionen SIN cerrar el modal.
            -->
            <asp:UpdatePanel ID="UP_ModalBusqueda" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        
                    
                        <div class="row p-3 mb-3" style="background-color: #f8f9fa; border-radius: 8px;">
                            <div class="col-md-3 mb-3">
                                <label class="form-label fw-bold">Marca:</label>
                                <asp:DropDownList ID="DDL_FiltroMarca" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroMarca_Changed" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <label class="form-label fw-bold">Modelo:</label>
                                <asp:DropDownList ID="DDL_FiltroModelo" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroModelo_Changed" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <label class="form-label fw-bold">Talla:</label>
                                <asp:DropDownList ID="DDL_FiltroTalla" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroTalla_Changed" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <label class="form-label fw-bold">Color:</label>
                                <asp:DropDownList ID="DDL_FiltroColor" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroColor_Changed" />
                            </div>
                        </div>

                     
                        <div class="table-responsive" style="max-height: 400px; overflow-y: auto;">
                            <h6 class="mb-3 text-secondary">Productos Disponibles (Stock > 0)</h6>
                            
                            <asp:GridView ID="GV_ProductosSucursal" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="table table-sm table-hover table-striped"
                                Width="100%"
                                DataKeyNames="NroSerie_Producto, CodProducto, ProductoDescripcion, Stock_Actual, Prec_Venta_Menor, Prec_Venta_Mayor"
                                OnRowCommand="GV_ProductosSucursal_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="ProductoDescripcion" HeaderText="Producto" />
                                    <asp:BoundField DataField="Stock_Actual" HeaderText="Stock" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Prec_Venta_Menor" HeaderText="Precio" DataFormatString="{0:C2}" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                                 
                                   
                                    <asp:TemplateField HeaderText="Acción" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="BTN_Modal_Seleccionar" runat="server"
                                                Text="Seleccionar"
                                                CssClass="btn btn-success btn-sm w-100"
                                                CommandName="SeleccionarProducto"
                                                CommandArgument='<%# Container.DataItemIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="alert alert-info text-center">
                                        <i class="fas fa-info-circle me-2"></i>
                                        No se encontraron productos con los filtros seleccionados.
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
                
             
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DDL_FiltroMarca" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DDL_FiltroModelo" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DDL_FiltroTalla" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DDL_FiltroColor" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>

            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fas fa-times me-1"></i> Cerrar
                </button>
            </div>

        </div>
    </div>
</div>
</asp:Content>
