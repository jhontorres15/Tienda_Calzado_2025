<%@ Page Title="Libro de Reclamaciones" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Libro_Reclamacion.aspx.cs" Inherits="Libro_Reclamacion" ValidateRequest="false"%>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Pedido.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <script language="javascript" type="text/javascript">
        function ValidarBusquedaFactura() {
            var nroFactura = document.getElementById('<%= TXT_NroFacturaBuscar.ClientID %>').value;
            if (nroFactura.trim() === "") {
                alert("Ingrese número de factura para buscar");
                return false;
            }
            return true;
        }

        function ValidarFormulario() {
            var numReclamo = document.getElementById('<%= txtNumReclamo.ClientID %>').value;
            var codCliente = document.getElementById('<%= ddlCliente.ClientID %>').selectedIndex;
            var codTipoReclamo = document.getElementById('<%= ddlTipoReclamo.ClientID %>').selectedIndex;
            var descripcion = document.getElementById('<%= txtDescripcion.ClientID %>').value;

            if (numReclamo.trim() === "") { alert("Ingrese número de reclamo"); return false; }
            if (codCliente <= 0) { alert("Seleccione un cliente"); return false; }
            if (codTipoReclamo <= 0) { alert("Seleccione tipo de reclamo"); return false; }
            if (descripcion.trim() === "") { alert("Ingrese descripción del reclamo"); return false; }

            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-9">
                <!-- Sección de Búsqueda de Factura -->
                <div class="card shadow-lg border-0 mb-4 card-proforma-check">
                    <div class="card-header bg-gradient-info text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-search me-2"></i>
                            Buscar Detalles de Factura
                        </h4>
                    </div>
                    <div class="card-body p-4">
                        <div class="row align-items-end">
                            <div class="col-md-4">
                                <label for="TXT_NroFacturaBuscar" class="form-label">
                                    <i class="fas fa-file-invoice text-info me-1"></i>
                                    Nro. Factura
                                </label>
                                <asp:TextBox ID="TXT_NroFacturaBuscar" runat="server" CssClass="form-control" 
                                           MaxLength="15" placeholder="Ej: FAC-000001"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <asp:Button ID="BTN_BuscarFactura" runat="server" Text="Buscar Factura" 
                                          CssClass="btn btn-info w-100" 
                                          OnClientClick="return ValidarBusquedaFactura()">
                                  
                                </asp:Button>
                            </div>
                            <div class="col-md-4">
                                <asp:Button ID="BTN_LimpiarBusqueda" runat="server" Text="Limpiar" 
                                          CssClass="btn btn-outline-secondary w-100" >
                               
                                </asp:Button>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <asp:Label ID="LBL_DetallesFactura" runat="server" CssClass="alert alert-info d-none" 
                                         Text="Los detalles de la factura aparecerán aquí"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Formulario de Libro de Reclamaciones -->
                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-info text-white">
                        <h3 class="mb-0">
                            <i class="fas fa-book me-2"></i>
                            Registro de Reclamo
                        </h3>
                    </div>
                <div class="card-body">
                    <!-- Fila 1 -->
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-hashtag me-1"></i> Nº Reclamo</label>
                            <asp:TextBox ID="txtNumReclamo" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-user me-1"></i> Cliente</label>
                            <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-list me-1"></i> Tipo Reclamo</label>
                            <asp:DropDownList ID="ddlTipoReclamo" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-store me-1"></i> Sucursal</label>
                            <asp:DropDownList ID="ddlSucursal" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <!-- Fila 2 -->
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-receipt me-1"></i> Serie Facturación</label>
                            <asp:TextBox ID="txtSerie" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label"><i class="fa-solid fa-calendar-day me-1"></i> Fecha Reclamo</label>
                            <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label"><i class="fa-solid fa-align-left me-1"></i> Descripción</label>
                            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Fila 3 -->
                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label class="form-label"><i class="fa-solid fa-file-lines me-1"></i> Detalle Reclamo</label>
                            <asp:TextBox ID="txtDetalle" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label"><i class="fa-solid fa-bullhorn me-1"></i> Exigencia/Solicitud</label>
                            <asp:TextBox ID="txtExigencia" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label"><i class="fa-solid fa-flag me-1"></i> Estado</label>
                            <asp:TextBox ID="txtEstado" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Fila 4 -->
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label"><i class="fa-solid fa-note-sticky me-1"></i> Observación Estado</label>
                            <asp:TextBox ID="txtObsEstado" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Botones -->
                    <div class="d-flex gap-2 mb-3">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" OnClick="btnLimpiar_Click" />
                    </div>

                    <!-- Buscador -->
                    <div class="input-group mb-3">
                        <span class="input-group-text"><i class="fa-solid fa-magnifying-glass"></i></span>
                        <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Buscar por Nº Reclamo o Cliente"></asp:TextBox>
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success" OnClick="btnBuscar_Click" />
                    </div>

                    <!-- Listado -->
                    <div class="table-responsive">
                        <asp:GridView ID="gvReclamos" runat="server" CssClass="table table-hover align-middle" AutoGenerateColumns="false" AllowPaging="true" PageSize="8" OnPageIndexChanging="gvReclamos_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="NumReclamo" HeaderText="Nº Reclamo" />
                                <asp:BoundField DataField="CodCliente" HeaderText="Cliente" />
                                <asp:BoundField DataField="CodTipo_Reclamo" HeaderText="Tipo" />
                                <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" />
                                <asp:BoundField DataField="NroSerie_Facturacion" HeaderText="Serie" />
                                <asp:BoundField DataField="Fec_Reclamo" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="Descripcion_Reclamo" HeaderText="Descripción" />
                                <asp:BoundField DataField="Estado_Reclamo" HeaderText="Estado" />
                            </Columns>
                            <PagerStyle CssClass="pagination" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>


    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
 </asp:Content>