<%@ Page Title="Gestión de Entregas" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Entrega.aspx.cs" Inherits="Entrega" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Facturacion.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4 facturacion-container">
        <div class="row justify-content-end">
            <div class="col-md-9">

                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h4 class="mb-0">
                            <i class="fas fa-truck me-2"></i>
                            Registro de Entrega
                        </h4>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="mb-0"></asp:Label>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="TXT_NroEntrega" class="form-label">Nro. Entrega</label>
                                <asp:TextBox ID="TXT_NroEntrega" runat="server" CssClass="form-control" MaxLength="12" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label for="DDL_TipoEntrega" class="form-label">
                                    <i class="fas fa-receipt text-primary me-1"></i>
                                    Tipo Entrega
                                </label>
                                <asp:DropDownList ID="DDL_TipoEntrega" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label for="TXT_Empleado" class="form-label">
                                    <i class="fas fa-user-tie text-info me-1"></i>
                                    Empleado
                                </label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="" />
                            </div>
                            <div class="col-md-3">
                                <label for="TXT_FecEntrega" class="form-label">
                                    <i class="fas fa-calendar-day text-secondary me-1"></i>
                                    Fecha Entrega
                                </label>
                                <asp:TextBox ID="TXT_FecEntrega" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col-md-3">
                                <label for="TXT_NroGuia" class="form-label">Nro. Guía Remisión</label>
                                <asp:TextBox ID="TXT_NroGuia" runat="server" CssClass="form-control" MaxLength="12"></asp:TextBox>
                            </div>

                         <div class="col-md-3">
                                <label class="form-label">Departamento</label>
                                <asp:DropDownList ID="DDL_Departamento" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Departamento_SelectedIndexChanged" >
                                </asp:DropDownList>
                          </div>
                         <div class="col-md-3">
                               <label class="form-label">Provincia</label>
                              <asp:DropDownList ID="DDL_Provincia" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Provincia_SelectedIndexChanged" >
                               </asp:DropDownList>
                            </div>
                           <div class="col-md-3">
                               <label class="form-label">Distrito</label>
                             <asp:DropDownList ID="DDL_Distrito" runat="server" CssClass="form-select"  AutoPostBack="true">
                            </asp:DropDownList>
                          </div>
                        </div>

                       <div class="row mb-3">

                            <div class="col-md-6">
                                <label for="TXT_Direccion" class="form-label">Dirección de Entrega</label>
                                <asp:TextBox ID="TXT_Direccion" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label for="DDL_Estado" class="form-label">Estado</label>
                                <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Pendiente" Value="Pendiente" />
                                    <asp:ListItem Text="En camino" Value="En camino" />
                                    <asp:ListItem Text="Entregado" Value="Entregado" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-9">
                                <label for="TXT_Obs" class="form-label">Observaciones</label>
                                <asp:TextBox ID="TXT_Obs" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="200"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12 d-flex align-items-end">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Cancelar_Click" />
                                <asp:Button ID="BTN_Registrar" runat="server" Text="Registrar Entrega" CssClass="btn btn-success" OnClick="BTN_Registrar_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card shadow-lg border-0">
                    <div class="card-header bg-gradient-primary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0">
                            <i class="fas fa-list me-2"></i>
                            Listado de Entregas
                        </h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por Nro Entrega o Guía" />
                            <asp:Button ID="BTN_Buscar" runat="server" CssClass="btn btn-primary btn-sm" Text="Buscar" OnClick="BTN_Buscar_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Entregas" runat="server" CssClass="table table-striped table-hover mb-0" AllowPaging="true" PageSize="5" AutoGenerateColumns="false" OnPageIndexChanging="GV_Entregas_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="NroEntrega" HeaderText="Nro. Entrega" />
                                    <asp:BoundField DataField="CodEmpleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="CodTipo_Entrega" HeaderText="Tipo" />
                                    <asp:BoundField DataField="Fec_Entrega" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                    <asp:BoundField DataField="NroGuia_Remision" HeaderText="Guía" />
                                    <asp:BoundField DataField="CPostal_Entrega" HeaderText="C. Postal" />
                                    <asp:BoundField DataField="Direccion_Entrega" HeaderText="Dirección" />
                                    <asp:BoundField DataField="Estado_Entrega" HeaderText="Estado" />
                                    <asp:BoundField DataField="Obs_Estado_Entrega" HeaderText="Observaciones" />
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

