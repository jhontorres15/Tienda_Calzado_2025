<%@ Page Title="Orden de Compra" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Orden_Compra.aspx.cs" Inherits="Orden_Compra" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Pedido.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>


    <script type="text/javascript">
        function SoloNumeros(e) {
            var key = e.keyCode || e.which;
            var tecla = String.fromCharCode(key).toLowerCase();
            var letras = "0123456789.";
            var especiales = [8, 37, 39, 46];
            var tecla_especial = false;
            for (var i in especiales) { if (key == especiales[i]) { tecla_especial = true; break; } }
            if (letras.indexOf(tecla) == -1 && !tecla_especial) return false;
        }

        function CalcularDescuentoYSubtotal() {
            // 1. Obtener valores (Usamos || 0 para evitar errores si el campo está vacío)
            var cantStr = document.getElementById('<%= TXT_Cantidad.ClientID %>').value;
           var precStr = document.getElementById('<%= TXT_PrecCompra.ClientID %>').value;
           var porcStr = document.getElementById('<%= TXT_PorcentajeDscto.ClientID %>').value;

               var cantidad = parseFloat(cantStr) || 0;
               var precVenta = parseFloat(precStr) || 0;
               var porcentajeDscto = parseFloat(porcStr) || 0;

               // 2. Realizar cálculos
               var importe = cantidad * precVenta;
               var descuento = importe * (porcentajeDscto / 100);
               var subtotal = importe - descuento;

               // 3. Asignar resultados (USAR .value, NO .innerText)
               // .toFixed(2) asegura que se muestren siempre 2 decimales
               document.getElementById('<%= LBL_Importe.ClientID %>').value = importe.toFixed(2);
               document.getElementById('<%= LBL_Descuento.ClientID %>').value = descuento.toFixed(2);
                       document.getElementById('<%= LBL_SubTotal.ClientID %>').value = subtotal.toFixed(2);
       }
    </script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-10">

                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-secondary text-white">
                        <h4 class="mb-0"><i class="fas fa-search-plus me-2"></i>Cargar desde Cotización</h4>
                    </div>
                    <div class="card-body p-4">
                        <div class="row align-items-end">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Cotización</label>
                                <asp:TextBox ID="TXT_NroCotizacionOrigen" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="BTN_CargarCotizacion" runat="server" Text="Cargar Datos" CssClass="btn btn-success w-100" OnClick="BTN_CargarCotizacion_Click" />
                            </div>
                        </div>
                       
                    </div>
                </div>

                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-secondary text-white">
                        <h4 class="mb-0"><i class="fas fa-clipboard-list me-2"></i>Registro de Orden de Compra</h4>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Compra</label>
                                <asp:TextBox ID="TXT_NroCompra" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Empleado</label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control" ReadOnly="true" />
                                <asp:HiddenField ID="HFD_CodEmpleado" runat="server" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Fecha Compra</label>
                                <asp:TextBox ID="TXT_FecCompra" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Cotización</label>
                                <asp:TextBox ID="TXT_NroCotizacion" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                         
                         
                        </div>

                        <h5 class="text-primary mb-3"><i class="fas fa-list-ul me-2"></i>Detalle de Productos</h5>
                        <asp:UpdatePanel ID="UP_FormularioDetalle" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mb-3">
                                      <div class="col-md-3">
                                           <label for="DDL_Sucursal" class="form-label">
                                               <i class="fas fa-store text-primary me-1"></i>
                                               Sucursal
                                           </label>
                                           <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select" 
                                                      MaxLength="5" ></asp:DropDownList>
                                       </div>
                                    <div class="col-md-5">
                                         <label class="form-label">Producto</label>
                                         <asp:TextBox ID="TXT_ProductoNombre" runat="server" CssClass="form-control" ReadOnly="true" ></asp:TextBox>
                                     </div>
                                       <asp:HiddenField ID="HFD_NroSerie" runat="server" />
                                    <asp:HiddenField ID="HDF_ProductoDescripcion" runat="server" Visible="true" />
                                    <div class="col-md-2">
                                        <label class="form-label">Cantidad</label>
                                        <asp:TextBox ID="TXT_Cantidad" runat="server" CssClass="form-control" onkeypress="return SoloNumeros(event)" 
                                            onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()" />
                                    </div>
                                    <div class="col-md-2">
                                        <label class="form-label">Precio Compra</label>
                                        <asp:TextBox ID="TXT_PrecCompra" runat="server" CssClass="form-control" onkeypress="return SoloNumeros(event)" 
                                            onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()" />
                                    </div>
                                            </div>  
                                <div class="row mb-3">
                                    <div class="col-md-2">
                                               <label for="TXT_PorcentajeDscto" class="form-label">
                                                   <i class="fas fa-percentage text-danger me-1"></i>
                                                   % Dscto
                                               </label>
                                               <asp:TextBox ID="TXT_PorcentajeDscto" runat="server" CssClass="form-control" 
                                                          onkeypress="return SoloNumeros(event)" onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()"
                                                          placeholder="0" Text="0"></asp:TextBox>
                                           </div>
                       

                                                          <div class="col-md-3">
                                        <label class="form-label">Importe</label>
                                        <asp:TextBox ID="LBL_Importe" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                                    </div>
                                          
                                    <div class="col-md-2">
                                        <label class="form-label">Descuento</label>
                                        <asp:TextBox ID="LBL_Descuento" runat="server" CssClass="form-control bg-warning text-dark " ReadOnly="false" />
                                    </div>
                     
                                         <div class="col-md-3">
                                             <label class="form-label">SubTotal</label>
                                             <asp:TextBox ID="LBL_SubTotal" runat="server" CssClass="form-control bg-success text-white fw-bold" ReadOnly="true" />
                                         </div>



                                   <div class="col-md-2 d-flex align-items-end">
                         <asp:Button ID="BTN_AgregarDetalle" runat="server" Text="Agregar" CssClass="btn btn-success w-100" OnClick="BTN_AgregarDetalle_Click" />
                                 </div>
                                </div>


                                <div class="table-responsive">
                                    <asp:GridView ID="GV_DetalleCompra" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" AllowPaging="true" PageSize="5" OnPageIndexChanging="GV_DetalleCompra_PageIndexChanging"
                                          OnRowCommand="GV_DetalleCompra_RowCommand"   >
                                        <Columns>
                                            <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" Visible="false" />
                                            <asp:BoundField DataField="Producto" HeaderText="Producto" />
                                            <asp:BoundField DataField="NroSerie_Producto" HeaderText="Nro Producto" Visible="false" />
                                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                            <asp:BoundField DataField="Prec_Compra" HeaderText="Precio" DataFormatString="{0:C}" />
                                            <asp:BoundField DataField="Importe" HeaderText="Importe" DataFormatString="{0:C}" />
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

                                
                                <div class="row mb-3">

                                     
                                    <div class="col-md-3">
                                        <label class="form-label">Total Compra</label>
                                        <asp:Label ID="LBL_TotalCompra" runat="server" CssClass="form-control bg-success text-white fw-bold text-center" Text="S/. 0.00"></asp:Label>
                                    </div>

                                </div>

                                <div class="row mt-4">
                                    <div class="col-12 text-center">
                                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary me-2" OnClick="BTN_Nuevo_Click" />
                                        <asp:Button ID="BTN_Grabar" runat="server" Text="Grabar" CssClass="btn btn-success me-2" OnClick="BTN_Grabar_Click" />
                                        <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary me-2" OnClick="BTN_Cancelar_Click" />
                                    
                                             <asp:Button ID="BTN_AbrirModalBuscar" runat="server" Text="Buscar Producto" CssClass="btn btn-secondary me-2" 
                                                    OnClick="BTN_AbrirModalBuscar_Click" >
                                                           </asp:Button>            
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-secondary text-white">
                        <div class="row align-items-center">
                            <div class="col-md-6"><h4 class="mb-0"><i class="fas fa-list me-2"></i>Listado de Compras</h4></div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control" placeholder="Buscar compra..." />
                                    <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-outline-light" OnClick="BTN_Buscar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Compras" runat="server" CssClass="table table-striped table-hover mb-0 fix-table" 
                                AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                                  OnRowCommand="GV_Compras_RowCommand"   >
                                <Columns>
                                   <asp:BoundField DataField="NroCompra" HeaderText="Nro. Compra" />
        
                                 <asp:BoundField DataField="EmpresaProveedor" HeaderText="Empresa" /> 
                                    <asp:BoundField DataField="NombreEmpleado" HeaderText="Empleado" /> 
                                    <asp:BoundField DataField="Fec_Compra" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                       <asp:BoundField DataField="Total" HeaderText="Total (S/)" DataFormatString="{0:N2}" />
        
                                  <asp:BoundField DataField="Estado_Compra" HeaderText="Estado" />

                                          <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Button ID="btnAtender" runat="server" 
                                            Text="Atender"
                                            CommandName="Atender"
                                            CommandArgument='<%# Eval("NroCompra") %>'
                                            CssClass="btn btn-success btn-sm"
                                            CausesValidation="False"
                                            Visible='<%# Eval("Estado_Compra").ToString() == "Pendiente" %>' />
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




          
    <div class="modal fade" id="modalBuscarProducto" tabindex="-1" aria-labelledby="modalBuscarProductoLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-xl"> <!-- modal-xl es 'Extra Grande' para que entren los filtros -->
        <div class="modal-content">

            <div class="modal-header bg-success text-white">
                <h5 class="modal-title" id="modalBuscarProductoLabel">
                    <i class="fas fa-search-plus me-2"></i> 
                    Buscar Producto : 
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
                            <h6 class="mb-3 text-secondary">Productos</h6>
                            
                           <asp:GridView ID="GV_Productos" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="table table-sm table-hover table-striped"
                            Width="100%"
                            DataKeyNames="CodProducto, ProductoDescripcion, NroSerie_Producto, NombreComercial, Stock_Actual, Prec_Venta_Mayor"
                            OnRowCommand="GV_Productos_RowCommand">
                            <Columns>
                              
                                <asp:BoundField DataField="NombreComercial" HeaderText="Producto" />
                                <asp:BoundField DataField="ProductoDescripcion" HeaderText="Detalles" />

            
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
