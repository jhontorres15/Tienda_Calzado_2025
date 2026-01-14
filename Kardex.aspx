<%@ Page Title="Sistema de Kardex" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Kardex.aspx.cs" Inherits="Kardex" %> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Kardex.css" type="text/css" /> 
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
     
    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var nroOperacion = document.getElementById('<%= TXT_NroOperacion.ClientID %>').value;
            var codEmpleado = document.getElementById('<%= TXT_Empleado.ClientID %>').value;
            var tipoOperacion = document.getElementById('<%= DDL_TipoOperacion.ClientID %>').selectedIndex;
            var fecha = document.getElementById('<%= TXT_Fecha.ClientID %>').value;
            var codSucursal = document.getElementById('<%= DDL_Sucursal.ClientID %>').selectedIndex;
           
            var tipoFacturacion = document.getElementById('<%= DDL_Tipo.ClientID %>').selectedIndex;
       
        
          
            var observaciones = document.getElementById('<%= TXT_Observaciones.ClientID %>').value;
 
            if (nroOperacion.trim() === "") { alert("Ingrese número de operación"); return false; } 
            if (codEmpleado.trim() === "") { alert("Ingrese código de empleado"); return false; }
            if (codEmpleado.length !== 5) { alert("El código de empleado debe tener 5 caracteres"); return false; }
            if (tipoOperacion <= 0) { alert("Seleccione un tipo de operación"); return false; } 
            if (fecha.trim() === "") { alert("Seleccione una fecha"); return false; }
            if (codSucursal.trim() === "") { alert("Ingrese código de sucursal"); return false; }
            if (codSucursal.length !== 5) { alert("El código de sucursal debe tener 5 caracteres"); return false; }
            if (producto.trim() === "") { alert("Ingrese número de serie del producto"); return false; }
            if (producto.length !== 7) { alert("El número de serie del producto debe tener 7 caracteres"); return false; }
            if (tipoFacturacion.trim() === "") { alert("Ingrese tipo de facturación"); return false; }
            if (tipoFacturacion.length !== 3) { alert("El tipo de facturación debe tener 3 caracteres"); return false; }
            if (nroSerieFacturacion.trim() === "") { alert("Ingrese número de serie de facturación"); return false; }
            if (nroSerieFacturacion.length > 10) { alert("El número de serie de facturación no debe exceder 10 caracteres"); return false; }
            if (isNaN(cantidad) || parseInt(cantidad) <= 0) { alert("Ingrese una cantidad válida"); return false; } 
            if (isNaN(costo) || parseFloat(costo) <= 0) { alert("Ingrese un costo válido"); return false; }
            if (observaciones.trim() === "") { alert("Ingrese observaciones"); return false; }
            if (observaciones.length > 200) { alert("Las observaciones no deben exceder 200 caracteres"); return false; }
            
            return true;
        }

     
    </script> 
</asp:Content> 
 
<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
    <div class="container mt-4">
        <h1 class="text-center mb-4 text-primary">Sistema de Kardex</h1>

        <div class="row derecha1">
            <!-- Formulario de registro -->
            <div class="col-md-9">
                <div class="card mb-4">
                    <div class="card-header bg-primary text-white">
                        <h5>Registro de Operaciones</h5>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Operación:</label>
                                <asp:TextBox ID="TXT_NroOperacion" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Empleado:</label>
                                <asp:TextBox ID="TXT_Empleado" runat="server" CssClass="form-control" MaxLength="5" />
                                  <asp:HiddenField ID="HFD_CodEmpleado" runat="server" Value="0" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Tipo de Operación:</label>
                                <asp:DropDownList ID="DDL_TipoOperacion" runat="server" CssClass="form-select"> </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Fecha Operación:</label>
                                <asp:TextBox ID="TXT_Fecha" runat="server" CssClass="form-control" TextMode="Date"  />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Sucursal:</label>
                                 <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select" ></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                 <label class="form-label">Tipo Facturación:</label>
                                  <asp:DropDownList ID="DDL_Tipo" runat="server" CssClass="form-select" ></asp:DropDownList>
                             </div>
                             

                        </div>

                   

                        <div class="row mb-3">
                             <div class="col-md-4">
                              <label class="form-label">Nro. Serie Facturación</label>
                            <asp:TextBox ID="TXT_NroSerieFacturacion" runat="server" CssClass="form-control" MaxLength="10" Enabled="false" />
                              </div>
        
                            <div class="col-md-4">
                                <label class="form-label">Observaciones:</label>
                                <asp:TextBox ID="TXT_Observaciones" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="200" />
                            </div>
                        </div>

                         <div> 
                            
                            <asp:Button ID="BTN_Limpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" />
                        </div>
                    
                </div>
                           <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-gradient-secondary text-white d-flex align-items-center justify-content-between">
                        <h5 class="mb-0"><i class="fas fa-search-plus me-2"></i>Cargar desde Factura/Boleta</h5>
                    </div>
                    <div class="card-body p-4">
                        <div class="row align-items-end mb-3">
                            <div class="col-md-4">
                                <label class="form-label">Nro. Serie Facturación</label>
                                <asp:TextBox ID="TXT_NroSerie_Cargar" runat="server" CssClass="form-control" MaxLength="10" />
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="BTN_CargarFactura" runat="server" Text="Cargar Datos" CssClass="btn btn-success w-100" OnClick="BTN_CargarFactura_Click" />
                            </div>
                        </div>
                        <div class="table-responsive">
                            <asp:GridView ID="GV_DetalleKardex" runat="server" CssClass="table table-sm table-striped table-bordered" AutoGenerateColumns="false">
                                <Columns>
                                    
                                    <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" />
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                    <asp:BoundField DataField="Costo_Unitario" HeaderText="Costo Unit." DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Costo_Total" HeaderText="Costo Total" DataFormatString="{0:C}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-3">
                                <label class="form-label">Total Costo</label>
                                <asp:TextBox ID="TXT_TotalCosto" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-9 d-flex align-items-end justify-content-end">
                                <asp:Button ID="BTN_GuardarKardex" runat="server" Text="Guardar Kardex" CssClass="btn btn-success" OnClick="BTN_GuardarKardex_Click" />
                            </div>
                        </div>
                    </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    

            <div class="row derecha">
                <div class="col-md-9"> 
            <div class="card1 mb-4">
                <div class="card-header1 bg-info text-white mb-2 justify-content-between" >
                    <h5>Resumen de Inventario</h5>
                </div>
                <div class="card-body1">
                    <div class="row text-center">
                        <div class="col-md-3"><div class="card bg-light"><div class="card-body"><h6>Total Entradas</h6><h3><asp:Label ID="LBL_TotalEntradas" runat="server" Text="0" /></h3></div></div></div>
                        <div class="col-md-3"><div class="card bg-light"><div class="card-body"><h6>Total Salidas</h6><h3><asp:Label ID="LBL_TotalSalidas" runat="server" Text="0" /></h3></div></div></div>
                        <div class="col-md-3"><div class="card bg-light"><div class="card-body"><h6>Stock Actual</h6><h3><asp:Label ID="LBL_StockActual" runat="server" Text="0" /></h3></div></div></div>
                  
                    </div>
                </div>
                </div>
              </div> 
           

          


      
        
            <div class="row derecha1">
                <div class="col-md-9"> 
               <div class="card"> 
                <div class="card-header bg-success text-white d-flex justify-content-between align-items-center"> 
                    <h5 class="mb-0">Movimientos de Kardex</h5> 
                    <div>
                        <asp:Button ID="BTN_Exportar" runat="server" Text="Exportar" CssClass="btn btn-sm btn-light" />
                        <asp:Button ID="BTN_Filtrar" runat="server" Text="Filtrar" CssClass="btn btn-sm btn-light ms-2"/>
                    </div>
                </div> 
                <div class="card-body">
                    <asp:GridView ID="GV_Kardex" runat="server" CssClass="table table-striped table-hover" 
                        AutoGenerateColumns="False" DataKeyNames="Nro_Operacion" >
                        <Columns>
                            <asp:BoundField DataField="Nro_Operacion" HeaderText="Nro. Op." />
                            <asp:BoundField DataField="Fec_Operacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:TemplateField HeaderText="Tipo">
                              <ItemTemplate>
                            <span class='<%# Eval("CodTipo_Operacion").ToString() == "TOP01" ? "entrada" : (Eval("CodTipo_Operacion").ToString() == "TOP02" ? "salida" : "devolucion") %>'>
                                <%# Eval("CodTipo_Operacion").ToString() == "TOP01" ? "Entrada" : (Eval("CodTipo_Operacion").ToString() == "TOP02" ? "Salida" : "Devolución") %>
                            </span>
                        </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="NroSerie_Producto" HeaderText="Producto" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="Costo_Unitario" HeaderText="Costo Unit." DataFormatString="{0:C2}" />
                            <asp:BoundField DataField="Costo_Total" HeaderText="Costo Total" DataFormatString="{0:C2}" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LNK_Editar" runat="server" CssClass="btn btn-sm btn-warning me-1"
                                        CommandName="Editar" CommandArgument='<%# Eval("Nro_Operacion") %>'>
                                        <i class="fas fa-edit"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LNK_Eliminar" runat="server" CssClass="btn btn-sm btn-danger"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("Nro_Operacion") %>'
                                        OnClientClick="return confirm('¿Está seguro de eliminar este registro?');">
                                        <i class="fas fa-trash"></i> Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
                </div>
            </div>
        </div>
    

</asp:Content>
