<%@ Page Title="Guía de Remisión" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Guia_Remision.aspx.cs" Inherits="Guia_Remision" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Facturacion.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link href="Estilos/Estilo_Pago.css" rel="stylesheet" />
    
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-9">
                <!-- Formulario Guía de Remisión -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0"><i class="fas fa-truck-moving me-2"></i> Registro de Guía de Remisión</h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-hashtag text-primary me-1"></i> Nro. Guía</label>
                                <asp:TextBox ID="TXT_NroGuia" runat="server" CssClass="form-control" MaxLength="12" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-user-tie text-info me-1"></i> Empleado</label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control" MaxLength="5" />
                                <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-file-invoice text-success me-1"></i> Serie Facturación</label>
                                <asp:TextBox ID="TXT_NroSerie" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                           
                        </div>

                        <div class="row mb-4">
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-calendar-check text-secondary me-1"></i> Inicio Traslado</label>
                                <asp:TextBox ID="TXT_FecInicio" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                       
                                  <div class="col-md-3">
                                  <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Departamento de Partida</label>
                                  <asp:DropDownList ID="DDL_Departamento" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Departamento_SelectedIndexChanged" >
                                  </asp:DropDownList>
                            </div>
                           <div class="col-md-3">
                                 <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Provincia</label>
                                <asp:DropDownList ID="DDL_Provincia" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Provincia_SelectedIndexChanged" >
                                 </asp:DropDownList>
                              </div>
                             <div class="col-md-3">
                                 <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Distrito</label>
                               <asp:DropDownList ID="DDL_Distrito" runat="server" CssClass="form-select"  AutoPostBack="true">
                              </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label"><i class="fas fa-location-dot text-danger me-1"></i> Dirección Partida</label>
                                <asp:TextBox ID="TXT_DireccionPartida" runat="server" CssClass="form-control" MaxLength="60" />
                            </div>
                        </div>

                        <div class="row mb-3">
                          
                                   <div class="col-md-4">
                                      <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Departamento de Destino</label>
                                      <asp:DropDownList ID="DDL_Departamento_Destino" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Departamento_Destino_SelectedIndexChanged" >
                                      </asp:DropDownList>
                                </div>
                               <div class="col-md-4">
                                     <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Provincia</label>
                                    <asp:DropDownList ID="DDL_Provincia_Destino" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Provincia_Destino_SelectedIndexChanged" >
                                     </asp:DropDownList>
                                  </div>
                                 <div class="col-md-4">
                                     <label class="form-label"><i class="fas fa-mail-bulk text-primary me-1"></i>Distrito</label>
                                   <asp:DropDownList ID="DDL_Distrito_Destino" runat="server" CssClass="form-select"  AutoPostBack="true">
                                  </asp:DropDownList>
                                </div>
                            <div class="col-md-6">
                                <label class="form-label"><i class="fas fa-location-dot text-success me-1"></i> Dirección Llegada</label>
                                <asp:TextBox ID="TXT_DireccionLlegada" runat="server" CssClass="form-control" MaxLength="60" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-user-tag text-primary me-1"></i> Tipo Destinatario</label>
                                <asp:DropDownList ID="DDL_TipoDestinatario" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-user text-info me-1"></i> Destinatario</label>
                                <asp:TextBox ID="TXT_Destinatario" runat="server" CssClass="form-control" MaxLength="60" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-id-card text-secondary me-1"></i> Doc. Destinatario</label>
                                <asp:DropDownList ID="DDL_DocDestinatario" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-barcode text-dark me-1"></i> Nro. Doc. Identidad</label>
                                <asp:TextBox ID="TXT_NroDocIdentidad" runat="server" CssClass="form-control" MaxLength="20" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-route text-primary me-1"></i> Tipo Traslado</label>
                                <asp:DropDownList ID="DDL_TipoTraslado" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-comment text-secondary me-1"></i> Observación Traslado</label>
                                <asp:TextBox ID="TXT_ObsTraslado" runat="server" CssClass="form-control" MaxLength="255" TextMode="MultiLine" Rows="2" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-coins text-warning me-1"></i> Costo Traslado</label>
                                <asp:TextBox ID="TXT_CostoTraslado" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-building text-info me-1"></i> Razón Social Transportista</label>
                                <asp:TextBox ID="TXT_RazonTransportista" runat="server" CssClass="form-control" MaxLength="20" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-id-card text-secondary me-1"></i> RUC Transportista</label>
                                <asp:TextBox ID="TXT_RucTransportista" runat="server" CssClass="form-control" MaxLength="11" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label"><i class="fas fa-person text-primary me-1"></i> Conductor</label>
                                <asp:TextBox ID="TXT_Conductor" runat="server" CssClass="form-control" MaxLength="120" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-id-card-clip text-secondary me-1"></i> Doc. Conductor</label>
                                <asp:DropDownList ID="DDL_DocConductor" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-barcode text-dark me-1"></i> Nro. Doc. Conductor</label>
                                <asp:TextBox ID="TXT_NroDocConductor" runat="server" CssClass="form-control" MaxLength="20" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label"><i class="fas fa-car-side text-success me-1"></i> Nro. Placa Vehículo</label>
                                <asp:TextBox ID="TXT_NroPlaca" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                            
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <label class="form-label"><i class="fas fa-comment-dots text-secondary me-1"></i> Observación Guía</label>
                                <asp:TextBox ID="TXT_ObsGuia" runat="server" CssClass="form-control" MaxLength="255" TextMode="MultiLine" Rows="2" />
                            </div>
                        </div>

                        <div class="mt-3 d-flex align-items-end">
                            <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                            <asp:Button ID="BTN_Registrar" runat="server" Text="Registrar Guía" CssClass="btn btn-success" OnClick="BTN_Registrar_Click" />
                        </div>
                    </div>
                </div>

                <!-- Listado de Guías -->
                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0"><i class="fas fa-file-invoice me-2"></i> Listado de Guías de Remisión</h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_BuscarGuia" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro Guía o Serie" />
                            <asp:Button ID="BTN_BuscarGuia" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_BuscarGuia_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Guias" runat="server" CssClass="table table-striped table-hover mb-0" AllowPaging="true" PageSize="5" AutoGenerateColumns="false" OnPageIndexChanging="GV_Guias_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="NroGuia_Remision" HeaderText="Guía" />
                                    <asp:BoundField DataField="NroSerie_Facturacion" HeaderText="Serie" />
                                    <asp:BoundField DataField="Fec_Emision" HeaderText="Emisión" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="Fec_Inicio_Traslado" HeaderText="Inicio" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="Destinatario" HeaderText="Destinatario" />
                                    <asp:BoundField DataField="CodTipo_Traslado" HeaderText="Traslado" />
                                    <asp:BoundField DataField="Costo_Traslado" HeaderText="Costo" DataFormatString="{0:N2}" />
                                    <asp:BoundField DataField="Estado_GuiaRemision" HeaderText="Estado" />
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