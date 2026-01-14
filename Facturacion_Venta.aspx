<%@ Page Title="Gestión de Facturación" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Facturacion_Venta.aspx.cs" Inherits="Facturacion_Venta" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Facturacion.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">
        <div class="row justify-content-end">
            <div class="col-md-9">

                <!-- Formulario de Facturación -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0">
                            <i class="fas fa-file-invoice-dollar me-2"></i>
                            Registro de Facturación
                        </h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="TXT_NroSerie" class="form-label">
                                    Nro. Serie
                                </label>
                                <asp:TextBox ID="TXT_NroSerie" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label for="DDL_Tipo" class="form-label">
                                    <i class="fas fa-receipt text-primary me-1"></i>
                                    Tipo Facturación
                                </label>
                                <asp:DropDownList ID="DDL_Tipo" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label for="DDL_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-info me-1"></i>
                                    Empleado
                                </label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control"></asp:TextBox>
                                   <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
                            </div>
                            <div class="col-md-3">
                                <label for="DDL_Pedido" class="form-label">
                                    <i class="fas fa-hashtag text-success me-1"></i>
                                    Nro. Pedido
                                </label>
                                <asp:TextBox ID="TXT_Pedido" runat="server" CssClass="form-control" AutoPostBack="true" ></asp:TextBox>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="TXT_FecEmision" class="form-label">
                                    <i class="fas fa-calendar-day text-secondary me-1"></i>
                                    Fecha Emisión
                                </label>
                                <asp:TextBox ID="TXT_FecEmision" runat="server" CssClass="form-control" TextMode="DateTimeLocal" ></asp:TextBox>
                            </div>
                           
                            <div class="col-md-6 d-flex align-items-end">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Cancelar_Click" />
                                <asp:Button ID="BTN_Facturar" runat="server" Text="Facturar" CssClass="btn btn-success" OnClick="BTN_Facturar_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="HF_CodCliente" runat="server" />
                    <asp:HiddenField ID="HF_ClienteNombre" runat="server" />
                    <asp:HiddenField ID="HF_ClienteDocNumero" runat="server" />
                    <asp:HiddenField ID="HF_ClienteDireccion" runat="server" />

                <!-- Detalle del Pedido -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-info text-white">
                        <h5 class="mb-0">
                            <i class="fas fa-list-check me-2"></i>
                            Detalle del Pedido
                        </h5>
                    </div>
                    <div class="card-body p-4">
                        <asp:GridView ID="GV_Detalle" runat="server" CssClass="table table-sm table-striped table-bordered" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto"  Visible="false" />
                                <asp:BoundField DataField="Producto" HeaderText="Producto"  />
                                <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal"  Visible="false" />
                                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                <asp:BoundField DataField="Prec_Venta" HeaderText="Precio" DataFormatString="{0:C}" />
                                <asp:BoundField DataField="Porcentaje_Dscto" HeaderText="% Dscto" DataFormatString="{0:N2}%" />
                                <asp:BoundField DataField="Dscto" HeaderText="Descuento" DataFormatString="{0:C}" />
                                <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" DataFormatString="{0:C}" />
                            </Columns>
                        </asp:GridView>

                        <div class="row g-3">
                            <div class="col-md-2">
                                <label class="form-label">Importe</label>
                                <asp:TextBox ID="TXT_Importe" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-2">
                                <label class="form-label">IGV (18%)</label>
                                <asp:TextBox ID="TXT_IGV" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-2">
                                <label class="form-label">Total</label>
                                <asp:TextBox ID="TXT_Total" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Listado de Comprobantes -->
                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0">
                            <i class="fas fa-file-invoice me-2"></i>
                            Listado de Comprobantes
                        </h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro Serie o Pedido" />
                            <asp:Button ID="BTN_Buscar" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_Buscar_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Facturas" runat="server" CssClass="table table-striped table-hover mb-0" AllowPaging="true" PageSize="5" AutoGenerateColumns="false" OnPageIndexChanging="GV_Facturas_PageIndexChanging" 
                                 OnRowCommand="GV_Facturas_RowCommand" >
                                <Columns>
                                   <%-- 1. NroSerie_Facturacion --%>
                                    <asp:BoundField DataField="NroSerie_Facturacion" HeaderText="Nro. Serie" >
                                        <HeaderStyle CssClass="col-nropedido" />
                                          <ItemStyle CssClass="col-nropedido" />
                                   </asp:BoundField>
                                     <%-- 2. Tipo_Facturacion --%>
                                        <asp:BoundField DataField="Tipo_Facturacion" HeaderText="Tipo Doc." />

                                        <%-- 3. Fec_Emision --%>
                                        <asp:BoundField DataField="Fec_Emision" HeaderText="Fecha de emisión" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />

                                        <%-- 4. DocumentoCliente --%>
                                        <asp:BoundField DataField="DocumentoCliente" HeaderText="RUC/DNI" />

                                        <%-- 5. Cliente --%>
                                        <asp:BoundField DataField="Cliente" HeaderText="Cliente" />

                                        <%-- 6. Vendedor (ESTA FALTABA) --%>
                                        <asp:BoundField DataField="Vendedor" HeaderText="Vendedor" />

                                        <%-- 7. Total --%>
                                        <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="S/ {0:N2}" ItemStyle-HorizontalAlign="Right" />
        
                                        <%-- 8. Estado --%>
                                        <asp:TemplateField HeaderText="Estado" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <span class='<%# Eval("Estado_Facturacion").ToString() == "Emitido" ? "badge bg-success" : "badge bg-danger" %>'>
                                                    <%# Eval("Estado_Facturacion") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:Button ID="Atender" runat="server" 
                                                Text="Registrar Pago"
                                                CommandName="Atender"
                                                CommandArgument='<%# Eval("NroSerie_Facturacion") %>'
                                                CssClass="btn btn-success btn-sm"
                                                CausesValidation="False"
                                          Visible='<%# Eval("Estado_Facturacion").ToString().Trim().ToLower() == "pendiente" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                        <%-- Botón Imprimir --%>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                        
                                                  <asp:Button ID="Imprimir" runat="server" 
                                                  Text="Imprimir"
                                                  CommandName="Imprimir"
                                                  CommandArgument='<%# Eval("NroSerie_Facturacion") %>'
                                                  CssClass="btn btn-success btn-sm"
                                                  CausesValidation="False"
                                                  Visible='<%# Eval("Estado_Facturacion").ToString() == "Emitido" %>' />
                                                
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