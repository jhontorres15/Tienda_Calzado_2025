<%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Catalogo.aspx.cs" Inherits="Catalogo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Estilos/Estilo_Catalogo.css" rel="stylesheet" />
    <script src="Scripts/jquery-3.4.1.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container catalogo-container mt-4">
        <div class="row justify-content-center">
            <div class="col-md-10">
                <div class="card shadow catalogo-config-card">
                    <div class="card-header d-flex align-items-center">
                        <i class="fa fa-tags mr-2"></i>
                        <h5 class="mb-0">Agregar Modelo al Catálogo</h5>
                    </div>
                    <div class="card-body">
                        <div class="form-row">
                            <div class="form-group col-md-4">
                                <label for="DDL_Marca">Marca</label>
                                <asp:DropDownList ID="DDL_Marca" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DDL_Marca_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="form-group col-md-5">
                                <label for="DDL_Modelo">Modelo</label>
                                <asp:DropDownList ID="DDL_Modelo" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="form-group col-md-3 d-flex align-items-end">
                                <asp:Button ID="BTN_Agregar" runat="server" CssClass="btn btn-gradient btn-block" Text="Agregar" OnClick="BTN_Agregar_Click" />
                            </div>
                        </div>
                        <asp:Label ID="LBL_Mensaje" runat="server" CssClass="text-danger"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-12">
                <h5 class="section-title">Catálogo de Zapatillas</h5>
                <asp:Repeater ID="RP_Catalogo" runat="server" OnItemDataBound="RP_Catalogo_ItemDataBound" OnItemCommand="RP_Catalogo_ItemCommand">
                    <ItemTemplate>
                        <div class="col-md-4 mb-4 d-inline-block">
                            <div class="card shadow catalogo-card  col-md-6">
                                <asp:HiddenField ID="HF_ModeloId" runat="server" Value='<%# Eval("CodModelo") %>' />
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-7 order-md-1">
                                            <h6 class="card-title mb-3 fw-bold"><%# Eval("Marca") %> - <%# Eval("Modelo") %></h6>
                                            <div class="talla-block mb-2">
                                                <span class="label">Talla</span>
                                                <asp:DropDownList ID="DDL_Talla_Item" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_Talla_Item_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="stock-block mb-2">
                                                <span class="label">Stock total</span>
                                                <asp:TextBox ID="TXT_Stock_Item" runat="server" CssClass="form-control form-control-sm mt-1" ReadOnly="true" />
                                            </div>
                                            <div class="colors-block mb-2">
                                                <span class="label">Colores disponibles</span>
                                                <asp:Label ID="LBL_Colores_Item" runat="server" CssClass="colors-list"></asp:Label>
                                            </div>
                                            <asp:LinkButton ID="BTN_Eliminar" runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Eliminar" CommandArgument='<%# Eval("CodModelo") %>'>Eliminar tarjeta</asp:LinkButton>
                                        </div>
                                        <div class="col-5 d-flex align-items-center order-md-2">
                                            <asp:Image ID="IMG_Modelo" runat="server" CssClass="img-fluid rounded modelo-img" ImageUrl='<%# Eval("ImagenUrl") %>' AlternateText='<%# Eval("Modelo") %>' />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>