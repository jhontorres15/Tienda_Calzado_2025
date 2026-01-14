<%@ Page Title="Gestión de Proformas" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Mantenimiento_Proforma.aspx.cs" Inherits="Mantenimiento_Proforma" ValidateRequest="false"%> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Proforma.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script type="text/javascript">
        // Ejecuta este código después de que la página ha cargado
        $(document).ready(function () {
        
        // Repite para cualquier otro TextBox de fecha/hora que necesites bloquear:
         $('#<%= TXT_FecEmision.ClientID %>').attr('readonly', 'readonly');
    });
    </script>
      
    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var nroProforma = document.getElementById('<%= TXT_NroProforma.ClientID %>').value;
            var codCliente = document.getElementById('<%= TXT_Cliente.ClientID %>').value;
            var codEmpleado = document.getElementById('<%= TXT_Empleado.ClientID %>').value;
            var fecEmision = document.getElementById('<%= TXT_FecEmision.ClientID %>').value;
            var fecCaducidad = document.getElementById('<%= TXT_FecCaducidad.ClientID %>').value;

            if (nroProforma.trim() === "") { alert("Ingrese número de proforma"); return false; }
            if (nroProforma.length !== 10) { alert("El número de proforma debe tener 10 caracteres"); return false; }
            if (codCliente <= 0) { alert("Seleccione un cliente"); return false; }
            if (codEmpleado <= 0) { alert("Seleccione un empleado"); return false; }
            if (fecEmision.trim() === "") { alert("Seleccione fecha de emisión"); return false; }
            if (fecCaducidad.trim() === "") { alert("Seleccione fecha de caducidad"); return false; }

            // Validar que la fecha de caducidad sea posterior a la de emisión
            var fechaEmision = new Date(fecEmision);
            var fechaCaducidad = new Date(fecCaducidad);
            if (fechaCaducidad <= fechaEmision) {
                alert("La fecha de caducidad debe ser posterior a la fecha de emisión");
                return false;
            }

            return true;
        }

        function CalcularImporte() {
            var cantidad = document.getElementById('<%= TXT_Cantidad.ClientID %>').value;
            var precCotizado = document.getElementById('<%= TXT_PrecCotizado.ClientID %>').value;
            
            if (cantidad && precCotizado) {
                var importe = parseFloat(cantidad) * parseFloat(precCotizado);
                document.getElementById('<%= LBL_Importe.ClientID %>').innerText = importe.toFixed(2);
            }
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
                <!-- Formulario de Proforma -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-file-invoice-dollar me-2"></i>
                            Registro de Proforma
                        </h4>
                    </div>
                    <div class="card-body p-4">
                        <!-- Información de la Proforma -->
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="TXT_NroProforma" class="form-label">
                                    <i class="fas fa-hashtag text-primary me-1"></i>
                                    Nro. Proforma
                                </label>
                                <asp:TextBox ID="TXT_NroProforma" runat="server" CssClass="form-control" 
                                           MaxLength="10" onkeypress="return SoloLetrasNumeros(event)" 
                                           placeholder="Ej: PRO-000001"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                
                        <asp:UpdatePanel ID="UP_Cliente" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>

                                <label for="TXT_Cliente" class="form-label">
                                    <i class="fas fa-user text-success me-1"></i>
                                    Cliente
                                </label>
                               <asp:TextBox ID="TXT_Cliente" runat="server" CssClass="form-control" placeholder="Buscar Cliente"
                                         TextMode="SingleLine" ReadOnly="true"></asp:TextBox>

                                <asp:HiddenField ID="HFD_CodCliente" runat="server" Value="0" />
                                 </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-info me-1"></i>
                                    Empleado
                                </label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control" ReadOnly="true">
                                </asp:TextBox>
                                  <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="TXT_FecEmision" class="form-label">
                                    <i class="fas fa-calendar-plus text-warning me-1"></i>
                                    Fecha Emisión
                                </label>
                                <asp:TextBox ID="TXT_FecEmision" runat="server" CssClass="form-control" 
                                           TextMode="DateTimeLocal" ></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_FecCaducidad" class="form-label">
                                    <i class="fas fa-calendar-times text-danger me-1"></i>
                                    Fecha Caducidad
                                </label>
                                <asp:TextBox ID="TXT_FecCaducidad" runat="server" CssClass="form-control" 
                                           TextMode="Date"></asp:TextBox>
                            </div>
                          

                        <!-- Detalle de Productos -->
                        <hr class="my-4"/>

                        <asp:UpdatePanel ID="UP_FormularioDetalle" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>


                        <h5 class="text-primary mb-3">
                            <i class="fas fa-shopping-cart me-2"></i>
                            Detalle de Productos
                        </h5>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="DDL_Sucursal" class="form-label">
                                    <i class="fas fa-store text-primary me-1"></i>
                                    Cod. Sucursal
                                </label>
                                <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select" 
                                           MaxLength="5" ></asp:DropDownList>
                            </div>
                            <div class="col-md-5">
                                <label for="DDL_Producto" class="form-label">
                                    <i class="fas fa-box text-success me-1"></i>
                                    Producto
                                </label>
                         
                                <asp:TextBox ID="TXT_ProductoNombre" runat="server" CssClass="form-control" 
                                  readonly="true"
                                 placeholder="Buscar Producto"></asp:TextBox>
                            </div>


                            <asp:HiddenField ID="HFD_CodProducto" runat="server" />
                            <asp:HiddenField ID="HFD_NroSerie" runat="server" />
                            <asp:HiddenField ID="HFD_PrecioUnitario" runat="server" />
                            <asp:HiddenField ID="HFD_PrecioMayor" runat="server" />
                            <asp:HiddenField ID="HFD_StockDisponible" runat="server" />
                            <div class="col-md-2">
                                <label for="TXT_Cantidad" class="form-label">
                                    <i class="fas fa-sort-numeric-up text-info me-1"></i>
                                    Cantidad
                                </label>
                                <asp:TextBox ID="TXT_Cantidad" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularImporte()" 
                                           placeholder="0"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <label for="TXT_PrecCotizado" class="form-label">
                                    <i class="fas fa-dollar-sign text-warning me-1"></i>
                                    Precio
                                </label>
                                <asp:TextBox ID="TXT_PrecCotizado" runat="server" CssClass="form-control" 
                                           onkeypress="return SoloNumeros(event)" onkeyup="CalcularImporte()" 
                                           placeholder="0.00"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-tag text-secondary me-1"></i>
                                    Precio Menor
                                </label>
                                <asp:Label ID="LBL_PrecMenor" runat="server" CssClass="form-control bg-light" 
                                         Text="0.00"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-tags text-secondary me-1"></i>
                                    Precio Mayor
                                </label>
                                <asp:Label ID="LBL_PrecMayor" runat="server" CssClass="form-control bg-light" 
                                         Text="0.00"></asp:Label>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-calculator text-success me-1"></i>
                                    Importe
                                </label>
                                <asp:Label ID="LBL_Importe" runat="server" CssClass="form-control bg-success text-white fw-bold" 
                                         Text="0.00"></asp:Label>
                            </div>
                            <div class="col-md-3 d-flex align-items-end">
                                <asp:Button ID="BTN_AgregarDetalle" runat="server" Text="Agregar" 
                                          CssClass="btn btn-success w-100" OnClick="BTN_AgregarDetalle_Click" 
                                          OnClientClick="return ValidarDetalle()">
                                   
                                </asp:Button>
                            </div>
                        </div>

                        <!-- GridView de Detalles -->
                        <div class="table-responsive">
                            <asp:GridView ID="GV_DetalleProforma" runat="server" CssClass="table table-striped table-hover" 
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5" 
                                        OnPageIndexChanging="GV_DetalleProforma_PageIndexChanging"
                                        OnRowCommand="GV_DetalleProforma_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" visible="false" />
                                    <asp:BoundField DataField="ProductoDescripcion" HeaderText="Producto" />
                                    <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" visible="false"/>
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                    <asp:BoundField DataField="Prec_Cotizado" HeaderText="Precio" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Importe" HeaderText="Importe" DataFormatString="{0:C}" />
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

                        
                        <!-- Total de la Proforma -->
                        <div class="row mt-3">
                            <div class="col-md-8"></div>
                            <div class="col-md-4">
                                <div class="card bg-primary text-white">
                                    <div class="card-body text-center">
                                        <h5 class="mb-1">
                                            <i class="fas fa-money-bill-wave me-2"></i>
                                            Total Proforma
                                        </h5>
                                        <h3 class="mb-0">
                                            <asp:Label ID="LBL_Total" runat="server" Text="S/. 0.00"></asp:Label>
                                        </h3>
                                    </div>
                                </div>
                            </div>
                        </div>  


                           

                        <!-- Botones de Acción -->
                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary me-2" 
                                          OnClick="BTN_Nuevo_Click">
                                </asp:Button>

                                <asp:Button ID="BTN_Grabar" runat="server" Text="Guardar" CssClass="btn btn-success me-2" 
                                          OnClick="BTN_Grabar_Click" >
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



                <!-- Listado de Proformas -->
                <div class="card shadow-lg border-0" >
                    <div class="card-header bg-gradient-secondary text-white">
                        <div class="row align-items-center">
                            <div class="col-md-6">
                                <h4 class="mb-0">
                                    <i class="fas fa-list me-2"></i>
                                    Listado de Proformas
                                </h4>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control" 
                                               placeholder="Buscar proforma..."></asp:TextBox>
                                    <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-outline-light" 
                                              OnClick="BTN_Buscar_Click">
                                        
                                    </asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Proformas" runat="server" CssClass="table table-striped table-hover mb-0" 
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="10" 
                                        OnPageIndexChanging="GV_Proformas_PageIndexChanging"
                                        OnRowCommand="GV_Proformas_RowCommand">
                                <Columns>
                                              
                                   <asp:BoundField DataField="NroProforma" HeaderText="Nro.Proforma"  >
                                    <HeaderStyle CssClass="col-codigo" />
                                    <ItemStyle CssClass="col-codigo" />
                                  </asp:BoundField>
                                 <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
                   

                                  <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                                  <asp:BoundField DataField="Fec_Emision" HeaderText="F. Emisión" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                  <asp:BoundField DataField="Fec_Caducidad" HeaderText="F. Caducidad" DataFormatString="{0:dd/MM/yyyy}" />
                                  <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                                  
                                              <asp:TemplateField HeaderText="Estado">
                                         <ItemTemplate>
                                             <asp:Label ID="Estado_Proforma" runat="server" 
                                                    Text='<%# Eval("Estado_Proforma") %>'
                                                        CssClass='<%# ObtenerClaseEstado(Eval("Estado_Proforma").ToString()) %>'>
                                             </asp:Label>
               
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                             <asp:Button ID="BTN_Atender" runat="server" Text="Atender"
                                                        CssClass="btn btn-success btn-sm me-1" CommandName="Atender"
                                                        CommandArgument='<%# Eval("NroProforma") %>'
                                                        Visible='<%# Eval("Estado_Proforma").ToString() == "Pendiente" %>'>
                                                    </asp:Button>
                                            <asp:Button ID="BTN_Imprimir" runat="server" Text="Imprimir" 
                                                      CssClass="btn btn-warning btn-sm me-1" 
                                                      CommandName="Imprimir" 
                                                      CommandArgument='<%# Eval("NroProforma") %>'>
                                               
                                            </asp:Button>
                                            <asp:Button ID="BTN_Eliminar" runat="server" Text="Eliminar" 
                                                      CssClass="btn btn-danger btn-sm" 
                                                      CommandName="EliminarProforma" 
                                                      CommandArgument='<%# Eval("NroProforma") %>'
                                                      OnClientClick="return confirm('¿Está seguro de eliminar esta proforma?')">
                                              
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
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Nro. Serie" />
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


      
