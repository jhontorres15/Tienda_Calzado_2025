<%@ Page Title="Registro de Clientes" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="MantenimientoCliente.aspx.cs" Inherits="Cliente" ValidateRequest="false"%> 
  
<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <link rel="stylesheet" href="Estilos/Estilo_Cliente.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
     <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.js"></script>
    <script language="javascript" type="text/javascript"> 
        function ValidarFormulario() {
            var codCliente = document.getElementById('<%= TXT_CodCliente.ClientID %>').value;
            var apellidos = document.getElementById('<%= TXT_Apellidos.ClientID %>').value;
            var nombres = document.getElementById('<%= TXT_Nombres.ClientID %>').value;
            var fechaNac = document.getElementById('<%= TXT_FecNac.ClientID %>').value;
            var sexo = document.getElementById('<%= DDL_Sexo.ClientID %>').selectedIndex;
           
            var codDocIdentidad = document.getElementById('<%=  DDL_TipoDoc.ClientID %>').selectedIndex;
            var nroDocIdentidad = document.getElementById('<%= TXT_NroDocumento.ClientID %>').value;
           
            var direccion = document.getElementById('<%= TXT_Direccion.ClientID %>').value;
            var telefono = document.getElementById('<%= TXT_Telefono.ClientID %>').value;
            var email = document.getElementById('<%= TXT_Email.ClientID %>').value;

            if (codCliente.trim() === "") { alert("Ingrese código de cliente"); return false; }
            if (codCliente.length !== 8) { alert("El código de cliente debe tener 8 caracteres"); return false; }
            if (apellidos.trim() === "") { alert("Ingrese apellidos"); return false; }
            if (nombres.trim() === "") { alert("Ingrese nombres"); return false; }
            if (fechaNac.trim() === "") { alert("Seleccione fecha de nacimiento"); return false; }
            if (sexo <= 0) { alert("Seleccione sexo"); return false; }
            if (codPais.trim() === "") { alert("Ingrese código de país"); return false; }
            if (codPais.length !== 6) { alert("El código de país debe tener 6 caracteres"); return false; }
            if (codDocIdentidad.trim() === "") { alert("Ingrese código de documento de identidad"); return false; }
            if (codDocIdentidad.length !== 6) { alert("El código de documento debe tener 6 caracteres"); return false; }
            if (nroDocIdentidad.trim() === "") { alert("Ingrese número de documento de identidad"); return false; }
            if (cpostal.trim() === "") { alert("Ingrese código postal"); return false; }
            if (cpostal.length !== 6) { alert("El código postal debe tener 6 caracteres"); return false; }
            if (direccion.trim() === "") { alert("Ingrese dirección"); return false; }
            if (telefono.trim() === "") { alert("Ingrese teléfono"); return false; }
            if (email.trim() === "") { alert("Ingrese email"); return false; }

            // Validar formato de email
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) { alert("Ingrese un email válido"); return false; }

            return true;
        }
    </script> 

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


        function ValidarLongitudDocumento() {
            var ddlTipo = document.getElementById('<%= DDL_TipoDoc.ClientID %>');
            var txtDoc = document.getElementById('<%= TXT_NroDocumento.ClientID %>');
        var seleccion = ddlTipo.options[ddlTipo.selectedIndex].text.toUpperCase();

        // Limpiar el campo si cambia el tipo (opcional)
        // txtDoc.value = ""; 

        if (seleccion.indexOf("DNI") !== -1) {
            txtDoc.setAttribute("maxLength", "8");
            txtDoc.setAttribute("placeholder", "8 dígitos");
        } else if (seleccion.indexOf("RUC") !== -1) {
            txtDoc.setAttribute("maxLength", "11");
            txtDoc.setAttribute("placeholder", "11 dígitos");
        } else {
            txtDoc.setAttribute("maxLength", "20"); // Otros documentos
            txtDoc.setAttribute("placeholder", "");
        }
    }

    // Función para validar antes de enviar
    function ValidarFormulario() {
        var ddlTipo = document.getElementById('<%= DDL_TipoDoc.ClientID %>');
        var txtDoc = document.getElementById('<%= TXT_NroDocumento.ClientID %>');
        var seleccion = ddlTipo.options[ddlTipo.selectedIndex].text.toUpperCase();
        var longitud = txtDoc.value.length;

        if (seleccion.indexOf("DNI") !== -1 && longitud !== 8) {
            alert("El DNI debe tener exactamente 8 dígitos.");
            return false;
        }
        if (seleccion.indexOf("RUC") !== -1 && longitud !== 11) {
            alert("El RUC debe tener exactamente 11 dígitos.");
            return false;
        }
        return true;
    }

    </script>
</asp:Content> 
 

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
   <div class="container mt-4"> 
    <h1 class="text-center mb-4 text-primary">Registro de Clientes</h1>

   <div class="row derecha1"> 
      
    <div class="col-md-9">
            <div class="card mb-5">
                <div class="card-header bg-primary text-white">
                    <h5>Datos del Cliente</h5> 
                </div> 
                <div class="card-body"> 
                     
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label for="<%= TXT_CodCliente.ClientID %>" class="form-label">Código Cliente:</label>
                            <asp:TextBox ID="TXT_CodCliente" runat="server" CssClass="form-control" MaxLength="8" readonly="true"/>
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_FecNac.ClientID %>" class="form-label">Fecha Nacimiento:</label>
                            <asp:TextBox ID="TXT_FecNac" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_Apellidos.ClientID %>" class="form-label">Apellidos:</label>
                            <asp:TextBox ID="TXT_Apellidos" runat="server" CssClass="form-control" MaxLength="40" onkeypress="return SoloLetras(event);" AutoPostBack="true" OnTextChanged="TXT_NombreApellido_TextChanged" />
                        </div>
                        <div class="col-md-3">
                            <label for="<%= TXT_Nombres.ClientID %>" class="form-label">Nombres:</label>
                            <asp:TextBox ID="TXT_Nombres" runat="server" CssClass="form-control" MaxLength="30" onkeypress="return SoloLetras(event);" AutoPostBack="true" OnTextChanged="TXT_NombreApellido_TextChanged" />
                        </div>
                    </div>



                     <asp:UpdatePanel ID="UP_TipoDocumento" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="row mb-3"> 
            <div class="col-md-2">
                <label for="<%= DDL_Sexo.ClientID %>" class="form-label">Sexo:</label>
                <asp:DropDownList ID="DDL_Sexo" runat="server" CssClass="form-control" Width="140px">
                    <asp:ListItem Text="Seleccione..." Value="" />
                    <asp:ListItem Text="Masculino" Value="M" />
                    <asp:ListItem Text="Femenino" Value="F" />
                </asp:DropDownList>
            </div>
            
            <div class="col-md-3">
                <label class="form-label">Nacionalidad:</label>
                <asp:DropDownList ID="DDL_Nacionalidad" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DDL_Nacionalidad_SelectedIndexChanged" width="200px" >
                </asp:DropDownList>
            </div>
             
            <div class="col-md-3">
                <label class="form-label">Tipo de Documento:</label>
                <asp:DropDownList ID="DDL_TipoDoc" runat="server" CssClass="form-select" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="DDL_TipoDoc_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label for="<%= TXT_NroDocumento.ClientID %>" class="form-label">Nro. Documento:</label>
                <asp:TextBox ID="TXT_NroDocumento" runat="server" CssClass="form-control" MaxLength="20" Width="200px" onkeypress="return SoloNumeros(event);" />
            </div>
            
            </div>

        <asp:Panel ID="Panel_DatosEmpresa" runat="server" Visible="false">
            <div class="row mb-3"> 
                <div class="col-md-3">
                     <label class="form-label"><i class="fas fa-user-tag text-primary me-1"></i> Tipo Destinatario</label>
                     <asp:DropDownList ID="DDL_TipoDestinatario" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-md-6">
                     <label class="form-label">Razón Social:</label>
                     <asp:TextBox ID="TXT_RazonSocial" runat="server" CssClass="form-control" MaxLength="100" placeholder="Ej: IMPORTACIONES S.A.C." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Estado Contribuyente:</label>
                    <asp:DropDownList ID="DDL_EstadoRuc" runat="server" CssClass="form-select">
                        <asp:ListItem Text="ACTIVO" Value="ACTIVO" />
                        <asp:ListItem Text="BAJA" Value="BAJA" />
                        <asp:ListItem Text="SUSPENSION" Value="SUSPENSION" />
                    </asp:DropDownList>
                </div>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
                



                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label class="form-label">Departamento</label>
                            <asp:DropDownList ID="DDL_Departamento" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Departamento_SelectedIndexChanged" >
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Provincia</label>
                            <asp:DropDownList ID="DDL_Provincia" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DDL_Provincia_SelectedIndexChanged" >
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Distrito</label>
                            <asp:DropDownList ID="DDL_Distrito" runat="server" CssClass="form-select"  AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="mb-3">
                        <label for="<%= TXT_Direccion.ClientID %>" class="form-label">Dirección:</label>
                        <asp:TextBox ID="TXT_Direccion" runat="server" CssClass="form-control" MaxLength="100"  Width="500px"/>
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
                            <label for="<%= DDL_EstadoCliente.ClientID %>" class="form-label">Estado Cliente:</label>
                            <asp:DropDownList ID="DDL_EstadoCliente" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Activo" Value="Activo" />
                                <asp:ListItem Text="Inactivo" Value="Inactivo" />
                                <asp:ListItem Text="Bloqueado" Value="Bloqueado" />
                            </asp:DropDownList>
                        </div>
                    </div>
                                
                    <div class="text-center mt-4">
                        <asp:Button ID="BTN_Registrar" runat="server" Text="Registrar" CssClass="btn btn-primary" OnClick="BTN_Registrar_Click" OnClientClick="return ValidarFormulario();" /> 
                        &nbsp; 
                        <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="BTN_Nuevo_Click" />
                    </div>

                                     
                </div> 

            </div> 
         </div>
        </div>




    </div> 
  <!-- Estadísticas de clientes -->
<div class="row derecha">
    <div class="col-md-9"> 
   <div class="card mb-3">
       <div class="card-header1 bg-info text-white">
           <h5>Estadísticas de Clientes</h5>
       </div>
       <div class="card-body1">
           <div class="col-md-3">
               <div class="card bg-light">
                   <div class="card-body text-center">
                       <h6>Total Clientes</h6>
                       <h3><asp:Label ID="LBL_TotalClientes" runat="server" Text="0" /></h3>
                   </div>
               </div>
           </div>
           <div class="col-md-3">
               <div class="card bg-light">
                   <div class="card-body text-center">
                       <h6>Clientes Activos</h6>
                       <h3><asp:Label ID="LBL_ClientesActivos" runat="server" Text="0" /></h3>
                   </div>
               </div>
           </div>
           <div class="col-md-3">
               <div class="card bg-light">
                   <div class="card-body text-center">
                       <h6>Clientes Inactivos</h6>
                       <h3><asp:Label ID="LBL_ClientesInactivos" runat="server" Text="0" /></h3>
                   </div>
               </div>
           </div>
           <div class="col-md-3">
               <div class="card bg-light">
                   <div class="card-body text-center">
                       <h6>Nuevos (Mes)</h6>
                       <h3><asp:Label ID="LBL_ClientesNuevos" runat="server" Text="0" /></h3>
                   </div>
               </div>
           </div>
       </div>
   </div>
        </div>
</div>


    <!-- Tabla de clientes -->
    <div class="row derecha"> 
        <div class="col-md-9">
            <div class="card">
                <div class="card-header bg-success text-white d-flex justify-content-between align-items-center"> 
            <h5 class="mb-0">Listado de Clientes</h5> 
            <div>
                <asp:TextBox ID="TXT_Busqueda" runat="server" CssClass="form-control form-control-sm d-inline-block" placeholder="Buscar cliente..." Width="200px" />
                <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-sm btn-light ms-2" OnClick="BTN_Buscar_Click" />
               
            </div>
        </div> 
        <div class="card-body">
            <asp:GridView ID="GV_Clientes" runat="server" CssClass="table table-striped table-hover" 
                AutoGenerateColumns="False" DataKeyNames="CodCliente,Apellido,Nombre,Fec_Nac,Sexo,Nacionalidad,Documento_Identidad,NroDoc_Identidad,Departamento,Provincia,Distrito,Direccion,Telefono,Email,Fec_Registro,Estado_Cliente" OnRowCommand="GV_Clientes_RowCommand"
                AllowPaging="True" PageSize="10" OnPageIndexChanging="GV_Clientes_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="CodCliente" HeaderText="Código" />
                    <asp:BoundField DataField="Apellido" HeaderText="Apellidos" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombres" />
                    <asp:BoundField DataField="Fec_Nac" HeaderText="Fec_Nac" Visible="False"/>
                    <asp:BoundField DataField="Sexo" HeaderText="Sexo" Visible="False"/>
                    <asp:BoundField DataField="Nacionalidad" HeaderText="Nacionalidad" Visible="False"/>
                    <asp:BoundField DataField="Documento_Identidad" HeaderText="Documento" />
                    <asp:BoundField DataField="NroDoc_Identidad" HeaderText="N° documento" />
                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" Visible="False"/>
                    <asp:BoundField DataField="Provincia" HeaderText="Provincia" Visible="False"/>
                    <asp:BoundField DataField="Distrito" HeaderText="Distrito" Visible="False"/>
                    <asp:BoundField DataField="Direccion" HeaderText="Direccion" Visible="False"/>
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="Fec_Registro" HeaderText="Fec_registro" Visible="False"/>
                    <asp:BoundField DataField="Estado_Cliente" HeaderText="Estado" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton ID="Editar" runat="server" CssClass="btn btn-sm btn-warning me-1"
                                CommandName="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" >
                                <i class="fas fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="LNK_Eliminar" runat="server" CssClass="btn btn-sm btn-danger"
                                CommandName="Eliminar" CommandArgument='<%# Eval("CodCliente") %>'
                                OnClientClick="return confirm('¿Está seguro de eliminar este cliente?');">
                                <i class="fas fa-trash"></i>
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


<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>