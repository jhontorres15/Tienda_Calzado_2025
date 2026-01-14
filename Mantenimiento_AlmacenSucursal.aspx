<%@ Page Title="" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="Mantenimiento_AlmacenSucursal.aspx.cs" Inherits="Mantenimiento_AlmacenSucursal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

       <link rel="stylesheet" href="Estilos/Estilo_AlmacenSucursal.css" type="text/css" />
   <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
   <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
   <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

   <script language="javascript" type="text/javascript">
       function ValidarFormulario() {
           var txtStockActual = document.getElementById('<%= txtStockActual.ClientID %>').value;
           var txtStockMaximo = document.getElementById('<%= txtStockMaximo.ClientID %>').value;
           var txtStockMinimo = document.getElementById('<%= txtStockMinimo.ClientID %>').value;
           var marca = document.getElementById('<%= TXT_NroSerie.ClientID %>').value;
           var DDL_Sucursal = document.getElementById('<%= DDL_Sucursal.ClientID %>').selectedIndex;
         
         
           if (txtStockMaximo.trim() === "") { alert('Ingrese el stock máximo'); return false; }
           if (categoria <= 0) { alert('Seleccione la Categoría'); return false; }
           if (marca <= 0) { alert('No hay número de Serie'); return false; }
           if (DDL_Sucursal <= 0) { alert('Seleccione la Sucursal'); return false; }
           if (isNaN(txtStockMinimo) || parseInt(stock) === "") { alert('Ingrese un Stock válido'); return false; }
           
       }
   </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <center><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

<div class="container mt-4">
    <h1 class="text-center mb-4 text-primary">Registro de Productos Almacén</h1>

    <asp:UpdatePanel ID="UP_Formulario" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>


     <div class="row right">
                <div class="col-md-10 offset-md-2">
                        <div class="card mb-4">
                            <div class="card-header bg-secondary text-white">
                                <h5>1. Identificación</h5>
                            </div>
                            <div class="card-body">
                                
                                <div class="row mb-3">
                                    <div class="col-md-4">
                                        <label for="<%= TXT_NroSerie.ClientID %>" class="form-label fw-bold">
                                               <i class="fas fa-barcode me-1 text-primary"></i> Número de Serie:</label>
                                        <asp:TextBox ID="TXT_NroSerie" CssClass="form-control" runat="server" ReadOnly="true" />
                                    </div>

                                    <div class="col-md-4">
                                        <label for="<%= DDL_Sucursal.ClientID %>" class="form-label fw-bold">
                                       <i class="fas fa-map-marker-alt me-1 text-primary"></i> Sucursal/Almacén:</label>
                                        <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select"  />
                                    </div>
                                    
                                    <div class="col-md-4">
                                        <label for="<%= DDL_Estado.ClientID %>" class="form-label fw-bold">Estado:</label>
                                        <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                                                           
                               <hr class="my-4"/>
             
                              <h6 class="mb-3 text-secondary">Seleccionar Producto</h6>
                  
                              <div class="row col-12">
                                  <div class="col-md-3 mb-3">
                                        <label class="form-label fw-bold">
                                            <i class="fas fa-map-marker-alt me-1 text-primary"></i> Marca:
                                        </label>

                                       <asp:DropDownList ID="DDL_FiltroMarca" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroMarca_Changed" />
                                  </div>

                                  <div class="col-md-3 mb-3">
                                         <label class="form-label fw-bold">
                                            <i class="fas fa-map-marker-alt me-1 text-primary"></i> Modelo:
                                        </label>

                                      <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                          <ContentTemplate>
                                              <asp:DropDownList ID="DDL_FiltroModelo" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroModelo_Changed" />
                                          </ContentTemplate>
                                          <Triggers>
                                              <asp:AsyncPostBackTrigger ControlID="DDL_FiltroMarca" EventName="SelectedIndexChanged" />
                                          </Triggers>
                                      </asp:UpdatePanel>
                                  </div>

                                  <div class="col-md-3 mb-3">
                                          <label class="form-label fw-bold">
                                            <i class="fas fa-map-marker-alt me-1 text-primary"></i> Talla:
                                        </label>

                                      <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                          <ContentTemplate>
                                              <asp:DropDownList ID="DDL_FiltroTalla" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroTalla_Changed" />
                                          </ContentTemplate>
                                          <Triggers>
                                              <asp:AsyncPostBackTrigger ControlID="DDL_FiltroModelo" EventName="SelectedIndexChanged" />
                                          </Triggers>
                                      </asp:UpdatePanel>
                                  </div>

                                  <div class="col-md-3 mb-3">
                                          <label class="form-label fw-bold">
                                           <i class="fas fa-map-marker-alt me-1 text-primary"></i> Color:
                                         </label>

                                      <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                          <ContentTemplate>
                                              <asp:DropDownList ID="DDL_FiltroColor" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_FiltroColor_Changed" />
                                          </ContentTemplate>
                                          <Triggers>
                                              <asp:AsyncPostBackTrigger ControlID="DDL_FiltroTalla" EventName="SelectedIndexChanged"  />
                                          </Triggers>
                                      </asp:UpdatePanel>
                                  </div>
                              </div>

                              <div class="row">
                                  <div class="col-md-8 mb-3">
                                       <label for="<%= DDL_Productos.ClientID %>" class="form-label fw-bold">
                                        <i class="fas fa-box me-1 text-primary"></i> Seleccione el Producto:
                                          </label>

                                      <asp:UpdatePanel ID="UP_DDL_Productos" runat="server" UpdateMode="Conditional">
                                          <ContentTemplate>
                                              <asp:DropDownList ID="DDL_Productos"  runat="server" CssClass="form-select" AutoPostBack="True" />
                                          </ContentTemplate>
                                          <Triggers>
                                              <asp:AsyncPostBackTrigger ControlID="DDL_FiltroColor" EventName="SelectedIndexChanged"  />
                                          </Triggers>


                              
                                      </asp:UpdatePanel>
                                  </div>
                              </div>
           
                                                <div class="row">
                                    <div class="col-md-4 mb-3">
                                        <label for="<%= txtStockActual.ClientID %>" class="form-label fw-bold">
                                            <i class="fas fa-sort-numeric-up me-1 text-primary"></i> Stock a Ingresar:
                                        </label>
                                        <asp:TextBox ID="txtStockActual" CssClass="form-control" runat="server" TextMode="Number" Text="0" />
                                    </div>

                                    <div class="col-md-4 mb-3">
                                        <label for="<%= txtStockMinimo.ClientID %>" class="form-label fw-bold">
                                            <i class="fas fa-bell me-1 text-warning"></i> Stock Mínimo:
                                        </label>
                                        <asp:TextBox ID="txtStockMinimo" CssClass="form-control" runat="server" TextMode="Number" />
                                        <%-- Este campo debe precargarse al seleccionar el producto --%>
                                    </div>

                                    <div class="col-md-4 mb-3">
                                        <label for="<%= txtStockMaximo.ClientID %>" class="form-label fw-bold">
                                            <i class="fas fa-plus-circle me-1 text-success"></i> Stock Máximo:
                                        </label>
                                        <asp:TextBox ID="txtStockMaximo" CssClass="form-control" runat="server" TextMode="Number" />
                                        <%-- Este campo debe precargarse al seleccionar el producto --%>
                                    </div>
                                </div>
                                </div>


               </div>     
                       

                        <div class="d-flex justify-content-end gap-2 mb-5">
                           <asp:Button ID="BTN_Nuevo_Almacen" runat="server" Text="Nuevo" 
                                CssClass="btn btn-success btn-lg" OnClick="BTN_Nuevo_Almacen_Click" />
                            <asp:Button ID="BTN_GuardarCambios" runat="server" Text="Guardar" 
                                CssClass="btn btn-success btn-lg" OnClick="BTN_Registrar_Almacen_Click" />

                         
                        </div>
                    </div>
                   </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="DDL_FiltroMarca" EventName="SelectedIndexChanged" />
                        </Triggers>

                    
                </asp:UpdatePanel>
        
            
    
   

   <div class="row right">
       <div class="col-md-10 offset-md-2">
           <div class="card">
               <div class="card-header bg-success text-white d-flex justify-content-between align-items-center">
                   <h5 class="mb-0">Listado de Productos</h5>
                   <div>
                       <asp:TextBox ID="TXT_BusquedaProducto" runat="server" CssClass="form-control form-control-sm d-inline-block" placeholder="Buscar producto..." Width="200px" />
                       <asp:Button ID="BTN_BuscarProducto" runat="server" Text="Buscar" CssClass="btn btn-sm btn-light ms-2" />
                       <asp:Button ID="BTN_ExportarProducto" runat="server" Text="Exportar" CssClass="btn btn-sm btn-light ms-2" />
                   </div>
               </div>
           
                 <asp:GridView ID="GV_InventarioSucursal" runat="server" 
                     AutoGenerateColumns="False"
                     CssClass="table table-sm table-hover table-striped"
                    OnRowCommand="GV_InventarioSucursal_RowCommand"
                     DataKeyNames="CodSucursal, NroSerie_Producto, CodProducto" 
                     Width="100%">
                     <Columns>
                         <asp:BoundField DataField="NroSerie_Producto" HeaderText="Serie" ItemStyle-Width="80px" />
                         <asp:BoundField DataField="Sucursal" HeaderText="Sucursal" ItemStyle-Width="80px" />
                         <asp:BoundField DataField="Producto" HeaderText="Producto" ItemStyle-Width="50px"  />
                         
                         <asp:BoundField DataField="Stock_Actual" HeaderText="Stock Actual" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px" />
                         <asp:BoundField DataField="Stock_Minimo" HeaderText="Stock Minimo" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px" />
                         <asp:BoundField DataField="Stock_Maximo" HeaderText="Stock Maximo" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px" />

                         <asp:BoundField DataField="Estado_Producto" HeaderText="Estado" ItemStyle-Width="100px" />
                          <asp:BoundField DataField="CodProducto" HeaderText="CodProducto" ItemStyle-Width="50px" />
                         
                         <asp:TemplateField HeaderText="Acción" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                             <ItemTemplate>
                                 <asp:Button ID="btnImprimirBarras" runat="server" Text="Imprimir BarCode" CssClass="btn btn-info" />
                                  
                                  <asp:Button ID="btnImprimirQR" runat="server" Text="Imprimir QR" CssClass="btn btn-warning" />
                                  
                                 <asp:LinkButton ID="LKB_EditarStock" runat="server" CommandName="CMD_EDITAR"
                                   CommandArgument='<%# Container.DataItemIndex %>'
                                     CssClass="btn btn-warning btn-sm">
                                     <i class="fas fa-edit"></i> Editar
                                 </asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                     </Columns>
                 </asp:GridView>
                               
                </div>
            </div>
        </div>

     </div>

     
</asp:Content>

