<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImprimirProforma.aspx.cs" Inherits="ImprimirProforma" %>
<!-- ... (resto del <head> y estilos se mantiene igual) ... -->
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Proforma de Cotización</title>
    <!-- Cargamos Bootstrap (el que tenías antes) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <!-- ¡CORRECCIÓN! Faltaba la etiqueta <style> -->
    <style>
        body {
            font-family: 'Arial', sans-serif;
            color: #333;
        }
        .proforma-box {
            max-width: 800px;
            margin: 20px auto;
            padding: 30px;
            border: 1px solid #eee;
            box-shadow: 0 0 10px rgba(0, 0, 0, .15);
        }
        .proforma-header {
            text-align: center;
            margin-bottom: 20px;
        }
        .section-header {
            font-weight: bold;
            border-bottom: 2px solid #333;
            padding-bottom: 5px;
            margin-top: 20px;
            margin-bottom: 10px;
        }
        .data-pair {
            margin-bottom: 5px;
        }
        .data-label {
            font-weight: bold;
            display: inline-block;
            width: 150px;
        }
        .data-value {
            display: inline-block;
        }
        .totals-table {
            float: right;
            width: 300px;
        }

        /* Estilos para impresión (oculta el botón) */
        @media print {
            .no-print {
                display: none;
            }
            body {
                margin: 0;
                padding: 0;
            }
            .proforma-box {
                box-shadow: none;
                border: none;
                margin: 0;
                padding: 0;
                max-width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container proforma-box">
            
            <div class="proforma-header">
                <h2>PROFORMA DE COTIZACIÓN</h2>
            </div>

            <div class="row">
                <div class="col-6">
                    <div class="data-pair">
                        <span class="data-label">N°:</span>
                        <span class="data-value"><asp:Label ID="LBL_NroProforma" runat="server"></asp:Label></span>
                    </div>
                    <div class="data-pair">
                        <span class="data-label">Fecha:</span>
                        <span class="data-value"><asp:Label ID="LBL_FechaEmision" runat="server"></asp:Label></span>
                    </div>
                </div>
                <div class="col-6 text-end">
                     <div class="data-pair">
                        <span class="data-label">Validez:</span>
                        <span class="data-value"><asp:Label ID="LBL_Validez" runat="server"></asp:Label></span>
                    </div>
                    <div class="data-pair">
                        <span class="data-label">Estado:</span>
                        <span class="data-value"><asp:Label ID="LBL_Estado" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>

            <div class="section-header">Datos del Proveedor</div>
            <div class="data-pair">
                <span class="data-label">Nombre/Razón Social:</span>
                <span class="data-value">G & G S. A.</span>
            </div>
            <div class="data-pair">
                <span class="data-label">RUC:</span>
                <span class="data-value">20377577729</span>
            </div>
            <div class="data-pair">
                <span class="data-label">Dirección:</span>
                <span class="data-value">MZ.M5 LT.69 ASENT.H. MANZANILLA II</span>
            </div>
             <div class="data-pair">
                <span class="data-label">Teléfono:</span>
                <span class="data-value">946-492-398</span>
            </div>
            <div class="data-pair">
                <span class="data-label">Correo electrónico:</span>
                <span class="data-value">--falta--</span>
            </div>
            <div class="data-pair">
                <span class="data-label">Vendedor:</span>
                <span class="data-value"><asp:Label ID="LBL_Vendedor" runat="server"></asp:Label></span>
            </div>

            <div class="section-header">Datos del Cliente</div>
            <div class="data-pair">
                <span class="data-label">Nombre/Razón Social:</span>
                <span class="data-value"><asp:Label ID="LBL_ClienteNombre" runat="server"></asp:Label></span>
            </div>

            <!-- --- ¡AQUÍ ESTÁ EL CAMBIO! --- -->
            <div class="data-pair">
                <!-- Ahora la etiqueta (ej: 'RUC:' o 'DNI:') es dinámica -->
                <asp:Label ID="LBL_ClienteDocTipo" runat="server" CssClass="data-label"></asp:Label>
                
                <!-- El valor solo muestra el número -->
                <span class="data-value"><asp:Label ID="LBL_ClienteDocNumero" runat="server"></asp:Label></span>
            </div>
            <!-- --- FIN DEL CAMBIO --- -->

            <div class="data-pair">
                <span class="data-label">Dirección:</span>
                <span class="data-value"><asp:Label ID="LBL_ClienteDireccion" runat="server"></asp:Label></span>
            </div>
             <div class="data-pair">
                <span class="data-label">Teléfono:</span>
                <span class="data-value"><asp:Label ID="LBL_ClienteTelefono" runat="server"></asp:Label></span>
            </div>
            <div class="data-pair">
                <span class="data-label">Correo electrónico:</span>
                <span class="data-value"><asp:Label ID="LBL_ClienteEmail" runat="server"></asp:Label></span>
            </div>

            <div class="section-header">Descripción de los Productos</div>
            <div class="table-responsive-sm mt-3">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th class="center">Cantidad</th>
                            <th>Descripción</th>
                            <th class="right">Precio Unitario (PEN)</th>
                            <th class="right">Total (PEN)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <%-- Usamos un Repeater para llenar la tabla dinámicamente --%>
                        <asp:Repeater ID="RPT_Detalle" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="center"><%# Eval("Cantidad") %></td>
                                    <td class="left strong"><%# Eval("ProductoDescripcion") %></td>
                                    <td class="right">S/. <%# Eval("Prec_Cotizado", "{0:F2}") %></td>
                                    <td class="right">S/. <%# Eval("Importe", "{0:F2}") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <div class="row">
                <div class="col-lg-7 col-sm-7">
                    <button onclick="window.print(); return false;" class="btn btn-primary no-print">
                        Imprimir / Guardar PDF
                    </button>
                </div>
                <div class="col-lg-5 col-sm-5 ms-auto">
                    <table class="table table-clear totals-table">
                        <tbody>
                            <tr>
                                <td class="left"><strong>Subtotal:</strong></td>
                                <td class="right"><asp:Label ID="LBL_Subtotal" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="left"><strong>IGV (18%):</strong></td>
                                <td class="right"><asp:Label ID="LBL_IGV" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="left"><strong>Total:</strong></td>
                                <td class="right"><strong><asp:Label ID="LBL_Total" runat="server"></asp:Label></strong></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>
    </form>
</body>
</html>