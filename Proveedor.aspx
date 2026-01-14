<%@ Page Title="Proveedor" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Proveedor.aspx.cs" Inherits="Proveedor" ValidateRequest="false" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Cliente.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row derecha1">
            <div class="col-md-10 offset-md-1">

                <div class="card shadow border-0 mb-4">
                    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                        <h5 class="mb-0"><i class="fa fa-user-tie me-2"></i>Mantenimiento de Proveedor</h5>
                        <asp:Label ID="LBL_Mensaje" runat="server"></asp:Label>
                    </div>
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-md-3">
                                <label class="form-label">Código</label>
                                <asp:TextBox ID="TXT_CodProveedor" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Apellidos</label>
                                <asp:TextBox ID="TXT_Apellidos" runat="server" CssClass="form-control" MaxLength="40" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Nombres</label>
                                <asp:TextBox ID="TXT_Nombres" runat="server" CssClass="form-control" MaxLength="30" />
                            </div>
                            <div class="col-md-2">
                                <label class="form-label">Estado</label>
                                <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Activo">Activo</asp:ListItem>
                                    <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row g-3 mt-2">
                            <div class="col-md-3">
                                <label class="form-label">Teléfono</label>
                                <asp:TextBox ID="TXT_Telefono" runat="server" CssClass="form-control" MaxLength="12" />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Email</label>
                                <asp:TextBox ID="TXT_Email" runat="server" CssClass="form-control" MaxLength="30" />
                            </div>
                              <div class="col-md-2">
                                      <label class="form-label">Departamento</label>
                                      <asp:DropDownList ID="DDL_Departamento" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Departamento_SelectedIndexChanged" >
                                      </asp:DropDownList>
                                </div>
                               <div class="col-md-2">
                                     <label class="form-label">Provincia</label>
                                    <asp:DropDownList ID="DDL_Provincia" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Provincia_SelectedIndexChanged" >
                                     </asp:DropDownList>
                                  </div>
                                 <div class="col-md-2">
                                     <label class="form-label">Distrito</label>
                                   <asp:DropDownList ID="DDL_Distrito" runat="server" CssClass="form-select"  AutoPostBack="true">
                                  </asp:DropDownList>
                                </div>
                              </div>
                            </div>
                            
                       

                        <div class="row g-3 mt-2">
                                <div class="col-md-5">
                                <label class="form-label">Dirección</label>
                                <asp:TextBox ID="TXT_Direccion" runat="server" CssClass="form-control" MaxLength="100" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Empresa</label>
                                <asp:DropDownList ID="DDL_Empresa" runat="server" CssClass="form-select" />
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-outline-secondary me-2" OnClick="BTN_Nuevo_Click" />
                                <asp:Button ID="BTN_Guardar" runat="server" Text="Guardar" CssClass="btn btn-success me-2" OnClick="BTN_Guardar_Click" />
                                <asp:Button ID="BTN_Actualizar" runat="server" Text="Actualizar" CssClass="btn btn-primary me-2" OnClick="BTN_Actualizar_Click" />
                             
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="BTN_Cancelar_Click" />
                            </div>
                        </div>
                    </div>
                </div>


                <div class="card shadow border-0 mb-4 ms-auto" style="width: 86%;">
                    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                        <h5 class="mb-0"><i class="fa fa-list me-2"></i>Listado de Proveedores</h5>
                        <div class="d-flex align-items-center">
                            <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control form-control-sm me-2" placeholder="Buscar por nombre o RUC de empresa" />
                            <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-light btn-sm" OnClick="BTN_Buscar_Click" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Proveedores" runat="server" CssClass="table table-striped table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="GV_Proveedores_PageIndexChanging"
                                   onRowCommand="GV_Proveedores_RowCommand"          >
                                <Columns>
                                    <asp:BoundField DataField="CodProveedor" HeaderText="Código" visible="false" />
                                    <asp:BoundField DataField="Apellidos" HeaderText="Apellidos" />
                                    <asp:BoundField DataField="Nombres" HeaderText="Nombres" />
                                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" />
                                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" />
                                    <asp:BoundField DataField="Provincia" HeaderText="Provincia" />
                                    <asp:BoundField DataField="Distrito" HeaderText="Distrito" />
                                    <asp:BoundField DataField="CPostal" HeaderText="C. Postal" visible="false"/>
                                    <asp:BoundField DataField="Direccion" HeaderText="Dirección" visible="false"/>
                                    <asp:BoundField DataField="Razon_Social" HeaderText="Empresa" />
                                    <asp:BoundField DataField="Estado_Proveedor" HeaderText="Estado" />
                                   <asp:TemplateField HeaderText="Acciones">
                                     <ItemTemplate>
                                         <asp:LinkButton ID="Editar" runat="server" CssClass="btn btn-sm btn-warning me-1"
                                             CommandName="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" >
                                             <i class="fas fa-edit"></i>
                                         </asp:LinkButton>
                                         
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
    
</asp:Content>

