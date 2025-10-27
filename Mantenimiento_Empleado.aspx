<%@ Page Title="Registro de Empleados" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="Mantenimiento_Empleado.aspx.cs" Inherits="Mantenimiento_Empleado" ValidateRequest="false"%>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Cliente.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script  language="javascript" type="text/javascript">
    function SoloLetras(e) {
        key = e.keyCode || e.which;
        tecla = String.fromCharCode(key).toLowerCase();
        letras = " áéíóúabcdefghijklmnñopqrstuvwxyz";
        especiales = [8, 37, 39, 46];

        tecla_especial = false;

        for (var i in especiales) { 
            if (key == especiales[i]) {
                
                tecla_especial = true;
                break;
            }
    
        }
    if (letras.indexOf(tecla) == - 1 && !tecla_especial)
        return false;
    }
    </script>
    <script>
        function SoloNumeros(e) {
         //El Código (variable code) es la representación decimal ASCII de la tecla presionada.
         var code = (e.which) ? e.which : e.keyCode;

        if (code == 8) { //Backspace
                return true;
         }
        
        else if (code >= 48 && code <= 57) //El Código de Tecla es un Número
        {
            return true;
        }
        else {  //Otras Teclas

            return false;
        }
        }
   

    </script>

</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container mt-4">
    <h1 class="text-center mb-4 text-primary">Registro de Empleados</h1>

    <div class="row derecha1">
        <!-- Formulario de registro -->
        <div class="col-md-9">
            <div class="card mb-4">
                <div class="card-header bg-primary text-white">
                    <h5>Datos del Empleado</h5>
                </div>
                <div class="card-body">
                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label for="<%= TXT_CodEmpleado.ClientID %>" class="form-label">Código Empleado:</label>
                            <asp:TextBox ID="TXT_CodEmpleado" runat="server" CssClass="form-control" MaxLength="60" Width="150px"  readonly/>
                        </div>
                         <div class="col-md-4">
                            <label for="<%= txtNombre.ClientID %>" class="form-label">Nombres:</label>
                            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="60" Width="260px" onkeypress="return SoloLetras(event);"/>
                        </div>
                        <div class="col-md-4">
                            <label for="<%= txtApellido.ClientID %>" class="form-label">Apellidos:</label>
                            <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" MaxLength="60"  Width="260px"  onkeypress="return SoloLetras(event);"/>
            

                       </div>
                        
                        <br />

                    <div class="row mb-3">
                        
                         <div class="col-md-4">
                         <label for="<%= txtFecNac.ClientID %>" class="form-label">Fecha Nacimiento:</label>
                         <asp:TextBox ID="txtFecNac" runat="server" CssClass="form-control" TextMode="Date" />
                     </div>
                        <div class="col-md-4">
                            <label for="<%= ddlSexo.ClientID %>" class="form-label">Sexo:</label>
                            <asp:DropDownList ID="ddlSexo" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Seleccione..." Value="" />
                                <asp:ListItem Text="Masculino" Value="M" />
                                <asp:ListItem Text="Femenino" Value="F" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Nacionalidad:</label>
                            <asp:DropDownList ID="DDL_Nacionalidad" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DDL_Nacionalidad_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
    
                    </div>
                    <div class="row mb-3">
                       <div class="col-md-3">
                        <label class="form-label">Tipo de Documento:</label>
                        <asp:DropDownList ID="DDL_TipoDoc" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label for="<%= TXT_NroDocumento.ClientID %>" class="form-label">Nro. Documento:</label>
                        <asp:TextBox ID="TXT_NroDocumento" runat="server" CssClass="form-control" MaxLength="20" Width="200px" onkeypress="return SoloNumeros(event);"/>
                    </div>
                        <div class="col-md-3">
                        <label for="<%= TXT_FecContrato.ClientID %>" class="form-label">Fecha Contrato:</label>
                        <asp:TextBox ID="TXT_FecContrato" runat="server" CssClass="form-control" TextMode="Date" Width="200px" />
                    </div>
                    <div class="col-md-3">
                        <label for="<%= TXT_FecTermino.ClientID %>" class="form-label">Término Contrato:</label>
                        <asp:TextBox ID="TXT_FecTermino" runat="server" CssClass="form-control" TextMode="Date" Width="200px"/>
                    </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label class="form-label">Departamento</label>
                            <asp:DropDownList ID="DDL_Departamento" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DDL_Departamento_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Provincia</label>
                            <asp:DropDownList ID="DDL_Provincia" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DDL_Provincia_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Distrito</label>
                            <asp:DropDownList ID="DDL_Distrito" runat="server" CssClass="form-select" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>

                        
                    <div class="row mb-3">
                         <div class="col-md-5">
                        <label for="<%= TXT_Direccion.ClientID %>" class="form-label">Dirección:</label>
                        <asp:TextBox ID="TXT_Direccion" runat="server" CssClass="form-control" MaxLength="80" Width="350px"/>
                    </div>
                      <div class="col-md-4">
                       <label class="form-label">Estado Civil:</label>
                      <asp:DropDownList ID="DDL_EstadoCivil" runat="server" CssClass="form-select" Width="250px">
                             <asp:ListItem Text="Seleccione..." Value="" />
                               <asp:ListItem Text="Divorciado" Value="D" />
                               <asp:ListItem Text="Viudo" Value="V" />
                               <asp:ListItem Text="Casado" Value="C" />
                       </asp:DropDownList> 
                   </div>
                         <div class="col-md-3">
                         <label  class="form-label">Nro. De Hijos:</label>
                         <asp:TextBox ID="TXT_NroHijos" runat="server" CssClass="form-control" MaxLength="20" Width="120px" onkeypress="return SoloNumeros(event);"/>
                     </div>

                        </div>

                    <div class="row mb-3">
                        
                        <div class="col-md-3">
                            <label class="form-label">Sucursal:</label>
                           <asp:DropDownList ID="DDL_Sucursal" runat="server" CssClass="form-select">
                            </asp:DropDownList> 
                        </div>

                        <div class="col-md-3">
                        <label class="form-label">Area:</label>
                       <asp:DropDownList ID="DDL_Area" runat="server" CssClass="form-select">
                        </asp:DropDownList> 
                                </div>
                             <div class="col-md-3">
                         <label class="form-label">Cargo:</label>
                        <asp:DropDownList ID="DDL_Cargo" runat="server" CssClass="form-select">
                         </asp:DropDownList> 
                                 </div>
                        <div class="col-md-3">
                         <label for="<%= TXT_Sueldo.ClientID %>" class="form-label">Sueldo:</label>
                         <asp:TextBox ID="TXT_Sueldo" runat="server" CssClass="form-control" TextMode="Number" />
                     </div>
                    </div>
                       
                    </div>

                    <div class="row mb-3">
      
                        <div class="col-md-3">
                            <label for="<%= TXT_Telefono.ClientID %>" class="form-label">Teléfono:</label>
                            <asp:TextBox ID="TXT_Telefono" runat="server" CssClass="form-control" MaxLength="12" onkeypress="return SoloNumeros(event);"/>
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_Email.ClientID %>" class="form-label">Email:</label>
                            <asp:TextBox ID="TXT_Email" runat="server" CssClass="form-control" MaxLength="40" TextMode="Email" />
                        </div>

                     <div class="col-md-3">
                            <label class="form-label">Estado Empleado:</label>
                            <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Activo" />
                                <asp:ListItem Text="Inactivo" />
                                <asp:ListItem Text="Vacaciones" />
                                <asp:ListItem Text="Licencia" />
                            </asp:DropDownList>
                        </div>
                     <div class="col-md-3">
                            <label class="form-label">Foto:</label>
                            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">

                        <div class="col-md-3  center">
                       <label  class="form-label">Foto:</label>
                      <asp:Image ID="IMG_Calzado" runat="server" Width="150px" Height="150px" ImageUrl="~/IMG/LOGO.jpeg" />
                        </div>
                         
                       <div class="col-md-3">
                       <label  class="form-label">Observacion Empleado:</label>
                       <asp:TextBox ID="TXT_ObsEmpleado" runat="server" CssClass="form-control" MaxLength="20" Width="500px" EnableTheming="True" TextMode="MultiLine" height="55px"/>
                      </div>

                  </div>


                    <div class="text-center mt-4">
                        <asp:Button ID="BTN_Registrar" runat="server" Text="Registrar" CssClass="btn btn-primary" OnClick="BTN_Registrar_Click" />
                        &nbsp;
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="BTN_Nuevo_Click" />
                        &nbsp;
                        <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-info" />
                    </div>
                    </div>
                </div>
            </div>
        </div>
   

    <!-- Tabla de empleados -->
    <div class="row derecha">
        <div class="col-md-9">
            <div class="card">
                <div class="card-header bg-success text-white d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Listado de Empleados</h5>
                    <div>
                        <asp:TextBox ID="TXT_BusquedaEmpleado" runat="server" CssClass="form-control form-control-sm d-inline-block" placeholder="Buscar empleado..." Width="200px" />
                        <asp:Button ID="BTN_BuscarEmpleado" runat="server" Text="Buscar" CssClass="btn btn-sm btn-light ms-2" />
                        <asp:Button ID="BTN_ExportarEmpleado" runat="server" Text="Exportar" CssClass="btn btn-sm btn-light ms-2" />
                    </div>
                </div>
                <div class="card-body">
                    <asp:GridView ID="GV_Empleados" runat="server" CssClass="table table-striped table-hover"
                        AutoGenerateColumns="False" DataKeyNames="CodEmpleado"
                        AllowPaging="True" PageSize="10"  OnRowCommand="GV_Empleados_RowCommand">
                        <Columns>
                             <asp:BoundField DataField="CodEmpleado" HeaderText="Código" />
                            <asp:BoundField DataField="Empleado" HeaderText="Empleado" />  <%-- Apellido + Nombre --%>
                            <asp:BoundField DataField="Sucursal" HeaderText="Sucursal" />
                            <asp:BoundField DataField="Area" HeaderText="Área" />
                            <asp:BoundField DataField="Cargo" HeaderText="Cargo" />
                            <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                            <asp:BoundField DataField="Email" HeaderText="Correo Electrónico" />
                            <asp:BoundField DataField="Estado_Empleado" HeaderText="Estado" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:Button ID="BTN_EditarEmpleado" runat="server" Text="Editar" CssClass="btn btn-primary btn-sm" CommandName="Editar" CommandArgument="<%# Container.DataItemIndex %>" />
                                    <asp:Button ID="BTN_EliminarEmpleado" runat="server" Text="Eliminar" CssClass="btn btn-danger btn-sm" CommandName="Eliminar"  />
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