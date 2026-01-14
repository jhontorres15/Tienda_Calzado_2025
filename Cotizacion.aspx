<%@ Page Title="Gestión de Cotizaciones" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Cotizacion.aspx.cs" Inherits="Cotizacion" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Proforma.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

   
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>


      <script language="javascript" type="text/javascript"> 
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
            document.getElementById('<%= TXT_Importe.ClientID %>').value = importe.toFixed(2);
            document.getElementById('<%= TXT_Dscto.ClientID %>').value = descuento.toFixed(2);
            document.getElementById('<%= TXT_SubTotal.ClientID %>').value = subtotal.toFixed(2);
          }
      </script>


</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container mt-4">
        <div class="row justify-content-end">
            <div class="col-md-9">

                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-secondary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0">
                            <i class="fas fa-file-signature me-2"></i>
                            Registro de Cotización
                        </h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label">Nro. Cotización</label>
                                <asp:TextBox ID="TXT_NroCotizacion" runat="server" CssClass="form-control" MaxLength="10" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Empleado</label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="HFD_CodEmpleado" runat="server" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Proveedor</label>
                                <asp:DropDownList ID="DDL_Proveedor" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Fecha</label>
                                <asp:TextBox ID="TXT_FecCotizacion" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label">Estado</label>
                                <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Pendiente" Value="Pendiente" />
                                    <asp:ListItem Text="Aprobada" Value="Aprobada" />
                                    <asp:ListItem Text="Rechazada" Value="Rechazada" />
                                </asp:DropDownList>
                            </div>
                    
                        </div>

                        <hr />
                        <asp:UpdatePanel ID="UP_FormularioDetalle" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                        <div class="row mb-2">
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
                                <asp:TextBox ID="TXT_ProductoNombre" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                              <asp:HiddenField ID="HFD_NroSerie" runat="server" />
                                  <asp:HiddenField ID="HDF_ProductoDescripcion" runat="server" />
                                  <div class="col-md-2">
                                 <label class="form-label">Cantidad</label>
                                 <asp:TextBox ID="TXT_Cantidad" runat="server" CssClass="form-control" placeholder="0"
                                    onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()"  ></asp:TextBox>
                             </div>
                             <div class="col-md-2">
                                 <label class="form-label">Precio Compra</label>
                                 <asp:TextBox ID="TXT_PrecCompra" runat="server" CssClass="form-control" placeholder="0.00"
                                   onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()" ></asp:TextBox>
                             </div>
                            
                        </div>

                  

                       

                      <div class="row mb-3">

                          
                           <div class="col-md-3">
                             <label class="form-label">Importe</label>
                             <asp:TextBox ID="TXT_Importe" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                         </div>
                               <div class="col-md-2">
                                <label for="TXT_PorcentajeDscto" class="form-label">
                                    <i class="fas fa-percentage text-danger me-1"></i>
                                    % Dscto
                                </label>
                                <asp:TextBox ID="TXT_PorcentajeDscto" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularDescuentoYSubtotal()" onchange="CalcularDescuentoYSubtotal()"
                                           placeholder="0" Text="0"></asp:TextBox>
                            </div>
                         <div class="col-md-2">
                             <label class="form-label">Descuento</label>
                             <asp:TextBox ID="TXT_Dscto" runat="server" CssClass="form-control bg-warning text-dark" ReadOnly="false" />
                         </div>
                          
                              <div class="col-md-3">
                                  <label class="form-label">SubTotal</label>
                                  <asp:TextBox ID="TXT_SubTotal" runat="server" CssClass="form-control bg-success text-white fw-bold" ReadOnly="true" />
                              </div>

                                <div class="col-md-2 d-flex align-items-end">
                                <asp:Button ID="BTN_AgregarDetalle" runat="server" Text="Agregar" CssClass="btn btn-primary w-100" OnClick="BTN_AgregarDetalle_Click" />
                            </div>
                    </div>

                         <div class="table-responsive mb-3">
                             <asp:GridView ID="GV_Detalle" runat="server" CssClass="table table-sm table-striped table-bordered"
                                 AutoGenerateColumns="false"
                                 OnRowCommand="GV_DetalleCotizacion_RowCommand"  >
                                 <Columns>
                                     <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" visible="false" />
                                     <asp:BoundField DataField="Producto" HeaderText="Producto" />
                                     <asp:BoundField DataField="NroSerie_Producto" HeaderText="nro serie"  visible="true" />
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
                             </asp:GridView>
                         </div>


                      <!-- Total de la Proforma -->
                      <div class="row mt-3">
                          <div class="col-md-8"></div>
                          <div class="col-md-4">
                              <div class="card bg-primary text-white">
                                  <div class="card-body text-center">
                                      <h5 class="mb-1">
                                          <i class="fas fa-money-bill-wave me-2"></i>
                                          Total Cotizacion
                                      </h5>
                                      <h3 class="mb-0">
                                          <asp:Label ID="LBL_Total" runat="server" Text="S/. 0.00"></asp:Label>
                                      </h3>
                                  </div>
                              </div>
                          </div>
                      </div>  

                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                                <asp:Button ID="BTN_Grabar" runat="server" Text="Grabar" CssClass="btn btn-success me-2" OnClick="BTN_Grabar_Click" />
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="BTN_Cancelar_Click" />
                            
                               
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
                    <div class="card-header bg-gradient-secondary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0"><i class="fas fa-list me-2"></i>Listado de Cotizaciones</h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro o Proveedor" />
                            <asp:Button ID="BTN_Buscar" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_Buscar_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Cotizaciones" runat="server" CssClass="table table-striped table-hover mb-0" 
                                AllowPaging="true" PageSize="10" AutoGenerateColumns="false" 
                                OnPageIndexChanging="GV_Cotizaciones_PageIndexChanging"
                                 OnRowCommand="GV_Cotizacion_RowCommand" >
                                <Columns>
                                    <asp:BoundField DataField="NroCotizacion" HeaderText="Nro." />
                                    <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="Fec_Cotizacion" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="S/ {0:N2}" />
                                    <asp:BoundField DataField="Estado_Cotizacion" HeaderText="Estado" />

                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                             <asp:Button ID="BTN_Atender" runat="server" Text="Atender"
                                                        CssClass="btn btn-success btn-sm me-1" CommandName="Atender"
                                                        CommandArgument='<%# Eval("NroCotizacion") %>'
                                                        Visible='<%# Eval("Estado_Cotizacion").ToString() == "Pendiente" %>'>
                                                    </asp:Button>
                                           
                                            <asp:Button ID="BTN_Anular" runat="server" Text="Anular" 
                                                      CssClass="btn btn-danger btn-sm" 
                                                      CommandName="AnularCotizacion" 
                                                      CommandArgument='<%# Eval("NroCotizacion") %>'
                                                      OnClientClick="return confirm('¿Está seguro de que desea anular esta Cotizacion?')">
          
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
                            <h6 class="mb-3 text-secondary">Productos</h6>
                            
                           <asp:GridView ID="GV_Productos" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="table table-sm table-hover table-striped"
                            Width="100%"
                            DataKeyNames="CodProducto, NroSerie_Producto,  ProductoDescripcion, NombreComercial, Stock_Actual, Prec_Venta_Mayor"
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

