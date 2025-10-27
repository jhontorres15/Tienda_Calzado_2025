<%@ Page Title="Gestión de Proformas" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Proforma.aspx.cs" Inherits="Proforma" ValidateRequest="false"%> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Proforma.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
      
    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var nroProforma = document.getElementById('<%= TXT_NroProforma.ClientID %>').value;
            var codCliente = document.getElementById('<%= DDL_Cliente.ClientID %>').selectedIndex;
            var codEmpleado = document.getElementById('<%= DDL_Empleado.ClientID %>').selectedIndex;
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

        function ValidarDetalle() {
            var codSucursal = document.getElementById('<%= TXT_CodSucursal.ClientID %>').value;
            var nroSerieProducto = document.getElementById('<%= DDL_Producto.ClientID %>').selectedIndex;
            var cantidad = document.getElementById('<%= TXT_Cantidad.ClientID %>').value;
            var precCotizado = document.getElementById('<%= TXT_PrecCotizado.ClientID %>').value;

            if (codSucursal.trim() === "") { alert("Ingrese código de sucursal"); return false; }
            if (codSucursal.length !== 5) { alert("El código de sucursal debe tener 5 caracteres"); return false; }
            if (nroSerieProducto <= 0) { alert("Seleccione un producto"); return false; }
            if (cantidad.trim() === "" || parseInt(cantidad) <= 0) { alert("Ingrese una cantidad válida"); return false; }
            if (precCotizado.trim() === "" || parseFloat(precCotizado) <= 0) { alert("Ingrese un precio cotizado válido"); return false; }

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
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-9">
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
                                <label for="DDL_Cliente" class="form-label">
                                    <i class="fas fa-user text-success me-1"></i>
                                    Cliente
                                </label>
                                <asp:DropDownList ID="DDL_Cliente" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Cliente --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-info me-1"></i>
                                    Empleado
                                </label>
                                <asp:DropDownList ID="DDL_Empleado" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Empleado --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="TXT_FecEmision" class="form-label">
                                    <i class="fas fa-calendar-plus text-warning me-1"></i>
                                    Fecha Emisión
                                </label>
                                <asp:TextBox ID="TXT_FecEmision" runat="server" CssClass="form-control" 
                                           TextMode="DateTimeLocal"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_FecCaducidad" class="form-label">
                                    <i class="fas fa-calendar-times text-danger me-1"></i>
                                    Fecha Caducidad
                                </label>
                                <asp:TextBox ID="TXT_FecCaducidad" runat="server" CssClass="form-control" 
                                           TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_EstadoProforma" class="form-label">
                                    <i class="fas fa-flag text-secondary me-1"></i>
                                    Estado
                                </label>
                                <asp:DropDownList ID="DDL_EstadoProforma" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Pendiente">Pendiente</asp:ListItem>
                                    <asp:ListItem Value="Aprobada">Aprobada</asp:ListItem>
                                    <asp:ListItem Value="Rechazada">Rechazada</asp:ListItem>
                                    <asp:ListItem Value="Vencida">Vencida</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Detalle de Productos -->
                        <hr class="my-4">
                        <h5 class="text-primary mb-3">
                            <i class="fas fa-shopping-cart me-2"></i>
                            Detalle de Productos
                        </h5>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="TXT_CodSucursal" class="form-label">
                                    <i class="fas fa-store text-primary me-1"></i>
                                    Cod. Sucursal
                                </label>
                                <asp:TextBox ID="TXT_CodSucursal" runat="server" CssClass="form-control" 
                                           MaxLength="5" onkeypress="return SoloLetrasNumeros(event)" 
                                           placeholder="Ej: SUC01"></asp:TextBox>
                            </div>
                            <div class="col-md-5">
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
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" />
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

                <!-- Listado de Proformas -->
                <div class="card shadow-lg border-0">
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
                                    <asp:BoundField DataField="NroProforma" HeaderText="Nro. Proforma" />
                                    <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
                                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="Fec_Emision" HeaderText="F. Emisión" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="Fec_Caducidad" HeaderText="F. Caducidad" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Estado_Proforma" HeaderText="Estado" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="BTN_Editar" runat="server" Text="Editar" 
                                                      CssClass="btn btn-warning btn-sm me-1" 
                                                      CommandName="EditarProforma" 
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
</asp:Content>