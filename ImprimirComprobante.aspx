<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImprimirComprobante.aspx.cs" Inherits="ImprimirComprobante" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Impresión de Comprobante</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 13px;
            color: #000;
            background-color: #fff;
        }
        .comprobante-container {
            max-width: 850px;
            margin: 20px auto;
            padding: 30px;
            border: 1px solid #ccc; /* Borde visible en pantalla */
        }
        /* Cuadro del RUC (Estilo SUNAT) */
        .ruc-box {
            border: 2px solid #000;
            padding: 15px;
            text-align: center;
            border-radius: 8px;
        }
        .ruc-number {
            font-size: 18px;
            font-weight: bold;
        }
        .doc-type {
            font-size: 20px;
            font-weight: bold;
            background-color: #eee;
            padding: 5px;
            margin: 10px 0;
            display: block;
        }
        .company-logo {
            max-height: 100px;
            margin-bottom: 10px;
        }
        .company-name {
            font-size: 22px;
            font-weight: bold;
            text-transform: uppercase;
        }
        .section-title {
            font-weight: bold;
            border-bottom: 1px solid #000;
            margin-bottom: 5px;
            margin-top: 15px;
            text-transform: uppercase;
            font-size: 12px;
        }
        .table-items thead th {
            background-color: #eee;
            border-bottom: 2px solid #000;
            text-align: center;
            font-size: 12px;
        }
        .table-items tbody td {
            vertical-align: middle;
            font-size: 12px;
        }
        .total-letters {
            background-color: #f8f9fa;
            padding: 5px 10px;
            font-weight: bold;
            border: 1px solid #dee2e6;
            margin-top: 10px;
        }
        .qr-placeholder {
            width: 120px;
            height: 120px;
            border: 1px dashed #ccc;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #aaa;
            margin: 0 auto;
        }
        .footer-sunat {
            margin-top: 30px;
            text-align: center;
            font-size: 11px;
            border-top: 1px solid #ccc;
            padding-top: 10px;
        }

        /* Estilos exclusivos para impresión */
        @media print {
            body { margin: 0; padding: 0; }
            .comprobante-container {
                border: none;
                margin: 0;
                padding: 0;
                max-width: 100%;
            }
            .no-print { display: none !important; }
            .ruc-box { border: 2px solid #000 !important; }
            .doc-type { background-color: #eee !important; -webkit-print-color-adjust: exact; }
            .table-items thead th { background-color: #eee !important; -webkit-print-color-adjust: exact; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="container mt-3 mb-3 no-print text-center">
            <button onclick="window.print(); return false;" class="btn btn-primary btn-lg shadow">
                <i class="fas fa-print"></i> Imprimir Comprobante
            </button>
            <button onclick="window.close();" class="btn btn-secondary btn-lg shadow ms-2">
                Cerrar
            </button>
        </div>

        <div class="comprobante-container">
            
            <div class="row align-items-center mb-4">
                <div class="col-7">
                    <h1 class="company-name">G & G S. A.</h1>
                    <p class="mb-0"><strong>Dirección:</strong> MZ.M5 LT.69 ASENT.H. MANZANILLA II, CALLAO - PERÚ</p>
                    <p class="mb-0"><strong>Teléfono:</strong> 946-492-398</p>
                    <p class="mb-0"><strong>Email:</strong> ventas@gyg.com.pe</p>
                </div>

                <div class="col-5">
                    <div class="ruc-box">
                        <div class="ruc-number">R.U.C. 20377577729</div>
                        
                        <span class="doc-type">
                            <asp:Label ID="LBL_TipoComprobante" runat="server" Text="FACTURA ELECTRÓNICA"></asp:Label>
                        </span>

                        <div class="ruc-number">
                            N° <asp:Label ID="LBL_SerieCorrelativo" runat="server" Text="F001-00000001"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-4">
                <div class="col-12">
                    <div class="section-title">Datos del Adquirente</div>
                    <table class="table table-sm table-borderless mb-0">
                        <tr>
                            <td style="width: 15%;"><strong>Señor(es):</strong></td>
                            <td><asp:Label ID="LBL_ClienteNombre" runat="server"></asp:Label></td>
                            <td style="width: 15%;"><strong>Fecha Emisión:</strong></td>
                            <td><asp:Label ID="LBL_FechaEmision" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><strong><asp:Label ID="LBL_TipoDocIdentidad" runat="server" Text="RUC:"></asp:Label></strong></td>
                            <td><asp:Label ID="LBL_ClienteDoc" runat="server"></asp:Label></td>
                            <td><strong>Moneda:</strong></td>
                            <td>SOLES</td>
                        </tr>
                        <tr>
                            <td><strong>Dirección:</strong></td>
                            <td colspan="3"><asp:Label ID="LBL_ClienteDireccion" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>
            </div>

            <div class="table-responsive mb-4">
                <table class="table table-bordered table-sm table-items">
                    <thead>
                        <tr>
                            <th style="width: 50px;">Cant.</th>
                            <th style="width: 80px;">Unidad</th>
                            <th>Descripción</th>
                            <th style="width: 100px;">P. Unitario</th>
                            <th style="width: 100px;">Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="RPT_Detalle" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="text-center"><%# Eval("Cantidad") %></td>
                                    <td class="text-center">NIU</td> <td><%# Eval("ProductoDescripcion") %></td>
                                    <td class="text-end"><%# Eval("Prec_Venta", "{0:N2}") %></td>
                                    <td class="text-end"><%# Eval("SubTotal", "{0:N2}") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>

            <div class="row">
                <div class="col-8">
                    <div class="total-letters mb-3">
                         <asp:Label ID="LBL_TotalLetras" runat="server" Text="CERO CON 00/100 SOLES"></asp:Label>
                    </div>
                    
                    <div class="row">
                        <div class="col-4 text-center">
                            <div class="qr-placeholder">
                                <small>[Código QR]</small>
                            </div>
                        </div>
                        <div class="col-8">
                            <p class="small text-muted mb-1">
                                Representación Impresa del Comprobante Electrónico.
                                <br />
                                Autorizado mediante Resolución de Intendencia N° 034-005-0005315.
                                <br />
                                Consulte su documento en <strong>www.gyg.com.pe</strong>
                            </p>
                        </div>
                    </div>
                </div>

                <div class="col-4">
                    <table class="table table-sm table-bordered">
                        <tr>
                            <td class="text-end bg-light"><strong>Op. Gravada:</strong></td>
                            <td class="text-end"><asp:Label ID="LBL_OpGravada" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="text-end bg-light"><strong>I.G.V. (18%):</strong></td>
                            <td class="text-end"><asp:Label ID="LBL_IGV" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="text-end bg-light"><strong>Importe Total:</strong></td>
                            <td class="text-end fw-bold"><asp:Label ID="LBL_Total" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>
            </div>

            <div class="footer-sunat">
                Este documento ha sido emitido electrónicamente. Gracias por su compra.
            </div>

        </div>
    </form>
</body>
</html>