<%@ Page Title="Enviar Mensaje WhatsApp" Language="C#" MasterPageFile="~/Menu_Web.master" AutoEventWireup="true"
    CodeFile="WhatsAppSender.aspx.cs" Inherits="WhatsAppSender" Async="true" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
     
         <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" /> 
        <style>
            .wa-card {
                border: none;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }

            .wa-header {
                background-color: #075E54;
                color: #fff;
                padding: 15px;
                border-top-left-radius: 5px;
                border-top-right-radius: 5px;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
        <div class="container mt-4 " >
            <div class="row justify-content-end">
                <div class="col-md-8 pull-right">
                    <div class="card wa-card">
                        <div class="card-header wa-header">
                            <h4 class="mb-0"><i class="fa fa-whatsapp"></i> Enviar WhatsApp a Cliente</h4>
                        </div>
                        <div class="card-body">

                            <asp:Panel ID="PNL_ConfigError" runat="server" Visible="false"
                                CssClass="alert alert-warning">
                                <strong>Atención:</strong> Debes configurar el TOKEN y URL en el archivo
                                <code>App_Code/WhapiService.cs</code> para que esto funcione.
                            </asp:Panel>

                            <asp:Panel ID="PNL_Result" runat="server" Visible="false" CssClass="alert" role="alert">
                                <asp:Label ID="LBL_Result" runat="server"></asp:Label>
                            </asp:Panel>

                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Teléfono:</label>
                                <div class="col-sm-9">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text"><i class="fa fa-phone"></i></span>
                                        </div>
                                        <asp:TextBox ID="TXT_Telefono" runat="server" CssClass="form-control"
                                            placeholder="Ej: 51987654321" TextMode="Phone"></asp:TextBox>
                                    </div>
                                    <small class="text-muted">Incluir código de país sin el signo + (Ej: 51 para
                                        Perú)</small>
                                </div>
                            </div>

                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Mensaje:</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="TXT_Mensaje" runat="server" CssClass="form-control"
                                        TextMode="MultiLine" Rows="5" placeholder="Escribe tu mensaje aquí...">
                                    </asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Adjuntar Archivo:</label>
                                <div class="col-sm-9">
                                    <asp:FileUpload ID="FU_Archivo" runat="server" CssClass="form-control-file" />
                                    <small class="form-text text-muted">Soporta PDF, JPG, PNG.</small>
                                </div>
                            </div>

                            <hr />

                            <div class="text-right">
                                <asp:Button ID="BTN_Enviar" runat="server" Text="Enviar Mensaje"
                                    CssClass="btn btn-success btn-lg" OnClick="BTN_Enviar_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>