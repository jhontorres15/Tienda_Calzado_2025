<%@ Page Title="Facturación de Compra" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Facturacion_Compra.aspx.cs" Inherits="Facturacion_Compra" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Facturacion.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row justify-content-end">
            <div class="col-md-9">

                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0"><i class="fas fa-file-invoice me-2"></i>Registro de Facturación de Compra</h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-2">
                                <label class="form-label">Nro. Serie</label>
                                <asp:TextBox ID="TXT_NroSerie" runat="server" CssClass="form-control" MaxLength="10" ReadOnly="true" />
                            </div>
                           
                            <div class="col-md-3">
                                <label class="form-label fw-bold">Razón Social / Proveedor</label>
                                <asp:TextBox ID="TXT_ProveedorNombre" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                            </div>
            
                            <div class="col-md-2">
                                <label class="form-label fw-bold">R.U.C.</label>
                                <asp:TextBox ID="TXT_RUC" runat="server" CssClass="form-control bg-light" ReadOnly="true" />
                            </div>
                                <asp:HiddenField ID="HF_CodEmpresa" runat="server" />
                        
                            <div class="col-md-3">
                                <label class="form-label">Tipo</label>
                                <asp:DropDownList ID="DDL_TipoFact" runat="server" CssClass="form-select" />
                            </div>
                            <div class="col-md-2">
                                <label class="form-label">Fecha Emisión</label>
                                <asp:TextBox ID="TXT_FecEmision" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Orden de Compra</label>
                                <asp:TextBox ID="TXT_NroCompra" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                            <div class="col-md-3 d-flex align-items-end">
                                <asp:Button ID="BTN_CargarCompra" runat="server" Text="Cargar Datos" CssClass="btn btn-success w-100" OnClick="BTN_CargarCompra_Click" />
                            </div>
                    
                        </div>

                        <div class="table-responsive mb-3">
                            <asp:GridView ID="GV_Detalle" runat="server" CssClass="table table-sm table-striped table-bordered" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField DataField="CodSucursal" HeaderText="Sucursal" Visible="false" />
                                     <asp:BoundField DataField="Producto" HeaderText="Producto" />
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" Visible="false" />
                                    <asp:BoundField DataField="Prec_Compra" HeaderText="Precio" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                    <asp:BoundField DataField="Dscto" HeaderText="Descuento" DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" DataFormatString="{0:C}" />
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="row g-3">
                            <div class="col-md-3">
                                <label class="form-label">SubTotal</label>
                                <asp:TextBox ID="TXT_SubTotal" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">IGV</label>
                                <asp:TextBox ID="TXT_IGV" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Total</label>
                                <asp:TextBox ID="TXT_Total" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                                <asp:Button ID="BTN_Grabar" runat="server" Text="Grabar" CssClass="btn btn-success me-2" OnClick="BTN_Grabar_Click" />
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="BTN_Cancelar_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0"><i class="fas fa-list me-2"></i>Listado de Facturación de Compra</h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro Serie o Compra" />
                            <asp:Button ID="BTN_Buscar" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_Buscar_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Facturas" runat="server" CssClass="table table-striped table-hover mb-0" AllowPaging="true" PageSize="10" AutoGenerateColumns="false" OnPageIndexChanging="GV_Facturas_PageIndexChanging">
                                <Columns>
                                 <%-- 1. Serie de la Factura --%>
                <asp:BoundField DataField="NroSerie_Facturacion" HeaderText="Nro Serie" ItemStyle-Font-Bold="true" />
                
               
                <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                <asp:BoundField DataField="Ruc" HeaderText="RUC" />

               
                <asp:BoundField DataField="OrdenRelacionada" HeaderText="Ref. O/C" />

               
                <asp:BoundField DataField="Fec_Emision" HeaderText="Emisión" DataFormatString="{0:yyyy-MM-dd}" />

            
                <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="IGV" HeaderText="IGV" DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C2}" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" />

               
                <asp:BoundField DataField="Estado_Facturacion" HeaderText="Estado" />
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

