<%@ Page Title="Registro de Pago" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Registro_Pago.aspx.cs" Inherits="Registro_Pago" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Facturacion.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link href="Estilos/Estilo_Pago.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-9">


                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0"><i class="fas fa-credit-card me-2"></i> Registro de Pago</h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="TXT_NroPago" class="form-label">
                                    <i class="fas fa-hashtag text-primary me-1"></i>
                                    Nro. Pago
                                </label>
                                <asp:TextBox ID="TXT_NroPago" runat="server" CssClass="form-control" />
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
                                <label for="TXT_NroSerie" class="form-label">
                                    <i class="fas fa-file-invoice text-success me-1"></i>
                                    Nro. Serie Facturación
                                </label>
                                <asp:TextBox ID="TXT_NroSerie" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-3">
                                <label for="TXT_Fecha" class="form-label">
                                    <i class="fas fa-calendar-day text-secondary me-1"></i>
                                    Fecha Pago
                                </label>
                                <asp:TextBox ID="TXT_Fecha" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                            </div>
                        </div>
                        
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="DDL_FormaPago" class="form-label">
                                    <i class="fas fa-wallet text-primary me-1"></i>
                                    Forma de Pago
                                </label>
                                <asp:DropDownList ID="DDL_FormaPago" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label for="TXT_Importe" class="form-label">
                                    <i class="fas fa-coins text-warning me-1"></i>
                                    Importe
                                </label>
                                <asp:TextBox ID="TXT_Importe" runat="server" CssClass="form-control" />
                            </div>
                     
                               <div class="col-md-3">
                                <label for="TXT_Referencia" class="form-label">
                                    <i class="fas fa-barcode text-dark me-1"></i>
                                    Código/Referencia
                                </label>
                                <asp:TextBox ID="TXT_Referencia" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="mt-3 d-flex align-items-end">
                            <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                            <asp:Button ID="BTN_Verificar" runat="server" CssClass="btn btn-primary me-2" Text="Verificar Pago" OnClick="BTN_Verificar_Click" />
                            <asp:Button ID="BTN_Registrar" runat="server" CssClass="btn btn-success" Text="Registrar Pago" OnClick="BTN_Registrar_Click" />
                        </div>
                    </div>
                </div>

                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0"><i class="fas fa-file-invoice me-2"></i> Pagos Registrados</h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_BuscarPago" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro Pago o Serie" />
                            <asp:Button ID="BTN_BuscarPago" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_BuscarPago_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Pagos" 
                                runat="server" 
                                    CssClass="table table-striped table-hover mb-0" 
                                    AllowPaging="true" 
                                    PageSize="10" 
                                    AutoGenerateColumns="false"
                                    OnPageIndexChanging="GV_Pagos_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="NroPago" HeaderText="NroPago" />
                                    <asp:BoundField DataField="CodEmpleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="NroSerie_Facturacion" HeaderText="Serie Fact" />
                                    <asp:BoundField DataField="Fec_Pago" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                    <asp:BoundField DataField="CodForma_Pago" HeaderText="Forma" />
                                    <asp:BoundField DataField="Importe" HeaderText="Importe" DataFormatString="{0:F2}" />
                                    <asp:BoundField DataField="Estado_Pago" HeaderText="Estado" />
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