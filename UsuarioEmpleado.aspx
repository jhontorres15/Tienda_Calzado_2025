<%@ Page Title="Gestión de Usuarios de Empleado" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true" CodeFile="UsuarioEmpleado.aspx.cs" Inherits="UsuarioEmpleado" ValidateRequest="false"%>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="Estilos/Estilo_Usuario.css" type="text/css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        function ValidarFormulario() {
            var codEmpleado = document.getElementById('<%= DDL_Empleado.ClientID %>').selectedIndex;
            var codUsuario = document.getElementById('<%= TXT_CodUsuario.ClientID %>').value;
            var password = document.getElementById('<%= TXT_Password.ClientID %>').value;
            var perfil = document.getElementById('<%= DDL_Perfil.ClientID %>').selectedIndex;
            var fecCreacion = document.getElementById('<%= TXT_FecCreacion.ClientID %>').value;
            var estado = document.getElementById('<%= DDL_EstadoUsuario.ClientID %>').selectedIndex;

            if (codEmpleado <= 0) { alert('Seleccione empleado'); return false; }
            if (codUsuario.trim() === '' || codUsuario.length > 20) { alert('CodUsuario es obligatorio (<=20)'); return false; }
            if (password.trim() === '' || password.length < 6 || password.length > 20) { alert('Password entre 6 y 20 caracteres'); return false; }
            if (perfil <= 0) { alert('Seleccione perfil'); return false; }
            if (fecCreacion.trim() === '') { alert('Seleccione fecha de creación'); return false; }
            if (estado <= 0) { alert('Seleccione estado de usuario'); return false; }
            return true;
        }

        function SoloNumeros(e) {
            var key = e.keyCode || e.which;
            var tecla = String.fromCharCode(key).toLowerCase();
            var letras = '0123456789.';
            var especiales = [8, 37, 39, 46];
            var tecla_especial = false;
            for (var i in especiales) { if (key == especiales[i]) { tecla_especial = true; break; } }
            if (letras.indexOf(tecla) == -1 && !tecla_especial) return false;
        }

        function SoloLetrasNumeros(e) {
            var key = e.keyCode || e.which;
            var tecla = String.fromCharCode(key).toLowerCase();
            var letras = 'abcdefghijklmnñopqrstuvwxyz0123456789-_.@';
            var especiales = [8, 37, 39, 46];
            var tecla_especial = false;
            for (var i in especiales) { if (key == especiales[i]) { tecla_especial = true; break; } }
            if (letras.indexOf(tecla) == -1 && !tecla_especial) return false;
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row justify-content-end">
            <div class="col-md-9">
                <!-- Registro de Usuario de Empleado -->
                <div class="card shadow-lg border-0 mb-4">
                    <div class="card-header bg-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-id-badge me-2"></i>
                            Registro de Usuario de Empleado
                        </h4>
                    </div>
                    <div class="card-body p-4">
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="DDL_CodEmpleado" class="form-label">
                                    <i class="fas fa-user-tie text-primary me-1"></i>
                                    Empleado
                                </label>
                                <asp:DropDownList ID="DDL_Empleado" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Empleado --</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_Usuario" class="form-label">
                                    <i class="fas fa-user text-success me-1"></i>
                                    Usuario
                                </label>
                                <asp:TextBox ID="TXT_CodUsuario" runat="server" CssClass="form-control" MaxLength="20" onkeypress="return SoloLetrasNumeros(event)" placeholder="Ej: jrodriguez"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_Password" class="form-label">
                                    <i class="fas fa-key text-warning me-1"></i>
                                    Password
                                </label>
                                <asp:TextBox ID="TXT_Password" runat="server" CssClass="form-control" TextMode="Password" MaxLength="20" onkeypress="return SoloLetrasNumeros(event)" placeholder="********"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="DDL_Perfil" class="form-label">
                                    <i class="fas fa-user-shield text-info me-1"></i>
                                    Perfil
                                </label>
                                <asp:DropDownList ID="DDL_Perfil" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Perfil --</asp:ListItem>
                                    <asp:ListItem Value="Administrador">Administrador</asp:ListItem>
                                    <asp:ListItem Value="Supervisor">Supervisor</asp:ListItem>
                                    <asp:ListItem Value="Vendedor">Vendedor</asp:ListItem>
                                    <asp:ListItem Value="Almacen">Almacén</asp:ListItem>
                                    <asp:ListItem Value="Auditor">Auditor</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="TXT_FecCreacion" class="form-label">
                                    <i class="fas fa-calendar-plus text-danger me-1"></i>
                                    Fec. Creación
                                </label>
                                <asp:TextBox ID="TXT_FecCreacion" runat="server" CssClass="form-control" READONLY="true" TextMode="DateTimeLocal"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="DDL_EstadoUsuario" class="form-label">
                                    <i class="fas fa-flag text-secondary me-1"></i>
                                    Estado
                                </label>
                                <asp:DropDownList ID="DDL_EstadoUsuario" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">-- Seleccionar Estado --</asp:ListItem>
                                    <asp:ListItem Value="Activo">Activo</asp:ListItem>
                                    <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                                    <asp:ListItem Value="Bloqueado">Bloqueado</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mt-2">
                            <div class="col-12">
                                <asp:Label ID="LBL_Mensaje" runat="server" CssClass="alert alert-info d-none" Text=""></asp:Label>
                            </div>
                        </div>

                        <!-- Botones de Acción -->
                        <div class="row mt-4">
                            <div class="col-12 text-center">
                                <asp:Button ID="BTN_Nuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary me-2" OnClick="BTN_Nuevo_Click"></asp:Button>
                                <asp:Button ID="BTN_Guardar" runat="server" Text="Guardar" CssClass="btn btn-success me-2" OnClick="BTN_Guardar_Click" OnClientClick="return ValidarFormulario()"></asp:Button>
                                <asp:Button ID="BTN_Cancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary me-2" OnClick="BTN_Cancelar_Click"></asp:Button>
                              
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Listado de Usuarios -->
                <div class="card shadow-lg border-0">
                    <div class="card-header bg-primary text-white">
                        <div class="row align-items-center">
                            <div class="col-md-6">
                                <h4 class="mb-0"><i class="fas fa-list me-2"></i>Listado de Usuarios</h4>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <asp:TextBox ID="TXT_Buscar" runat="server" CssClass="form-control" placeholder="Buscar por usuario, perfil..."></asp:TextBox>
                                    <asp:Button ID="BTN_Buscar" runat="server" Text="Buscar" CssClass="btn btn-outline-light" OnClick="BTN_Buscar_Click"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="GV_Usuarios" runat="server" CssClass="table table-striped table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="GV_Usuarios_PageIndexChanging" OnRowCommand="GV_Usuarios_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="CodEmpleado" HeaderText="Empleado" />
                                    <asp:BoundField DataField="Nombre_Completo" HeaderText="Nombre" />
                                    <asp:BoundField DataField="CodUsuario" HeaderText="Usuario" />
                                    <asp:BoundField DataField="Perfil" HeaderText="Perfil" />
                                    <asp:BoundField DataField="Password" HeaderText="Password" Visible="false" />
                                    <asp:BoundField DataField="Fec_Creacion" HeaderText="Fec. Creación" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                    <asp:BoundField DataField="Estado_Usuario" HeaderText="Estado" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="BTN_Editar" runat="server" Text="Editar" CssClass="btn btn-warning btn-sm me-1" CommandName="EditarUsuario" CommandArgument='<%# Eval("CodUsuario") %>' />
                                            <asp:Button ID="BTN_Eliminar" runat="server" Text="Eliminar" CommandName="EliminarUsuario" CommandArgument='<%# Eval("CodUsuario") %>'  CssClass="btn btn-danger btn-eliminar"/>
                                            <i class="fa fa-trash"></i> 
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
    </div>


    <script type="text/javascript">
        // Esperamos a que cargue la página
        document.addEventListener('DOMContentLoaded', function () {

            // Seleccionamos todos los botones con la clase 'btn-eliminar'
            const botones = document.querySelectorAll('.btn-eliminar');

            botones.forEach(boton => {
                boton.addEventListener('click', function (e) {
                    e.preventDefault(); // 1. Detenemos el envío al servidor temporalmente

                    const enlaceOriginal = this.getAttribute('href'); // Guardamos la acción del servidor (__doPostBack)

                    // 2. Mostramos la alerta bonita
                    Swal.fire({
                        title: '¿Estás seguro?',
                        text: "¡No podrás revertir esto! El usuario será eliminado.",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',     // Rojo para borrar
                        cancelButtonColor: '#3085d6',   // Azul para cancelar
                        confirmButtonText: 'Sí, eliminar',
                        cancelButtonText: 'Cancelar'
                    }).then((result) => {
                        // 3. Si el usuario confirma...
                        if (result.isConfirmed) {
                            // Ejecutamos la acción original del botón (ir al servidor)
                            window.location.href = enlaceOriginal;
                        }
                    });
                });
            });
        });
</script>
</asp:Content>
