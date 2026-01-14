<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Menu_Web.master" CodeFile="menu.aspx.cs" Inherits="menu" %>



<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server"> 
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
     
    <link href="Estilos/Estilo_Dashborad.css" rel="stylesheet" />
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
      
    
    <!-- SCRIPT PARA RENDERIZAR LOS GRÁFICOS -->
  <!-- Los datos "labels" y "data" vendrán desde C# -->
  <script>
      function renderizarGraficos(dataEmpleados, dataDiario, dataEstados) {

          // 1. Configuración Gráfico Empleados (Barras Horizontales)
          new Chart(document.getElementById('chartEmpleados'), {
              type: 'bar',
              data: {
                  labels: dataEmpleados.labels, // Ej: ['Juan', 'Maria']
                  datasets: [{
                      label: 'Ventas (S/.)',
                      data: dataEmpleados.values, // Ej: [1500, 2000]
                      backgroundColor: '#0d6efd',
                      borderRadius: 4,
                      barThickness: 20
                  }]
              },
              options: {
                  indexAxis: 'y', // Hace que las barras sean horizontales
                  maintainAspectRatio: false,
                  plugins: { legend: { display: false } },
                  scales: { x: { grid: { display: false } }, y: { grid: { display: false } } }
              }
          });

          // 2. Configuración Gráfico Diario (Barras Horizontales Azul Claro)
          new Chart(document.getElementById('chartDiario'), {
              type: 'bar',
              data: {
                  labels: dataDiario.labels,
                  datasets: [{
                      label: 'Total',
                      data: dataDiario.values,
                      backgroundColor: '#3b82f6',
                      borderRadius: 4,
                      barThickness: 20
                  }]
              },
              options: {
                  indexAxis: 'y',
                  maintainAspectRatio: false,
                  plugins: { legend: { display: false } },
                  scales: { x: { grid: { display: false } }, y: { grid: { display: false } } }
              }
          });

          // 3. Configuración Gráfico Estado (Pie / Pastel)
          new Chart(document.getElementById('chartEstado'), {
              type: 'pie',
              data: {
                  labels: dataEstados.labels,
                  datasets: [{
                      data: dataEstados.values,
                      backgroundColor: ['#0d6efd', '#6c757d', '#ffc107', '#198754', '#dc3545', '#0dcaf0'],
                      borderWidth: 0
                  }]
              },
              options: {
                  maintainAspectRatio: false,
                  plugins: {
                      legend: { position: 'right', labels: { boxWidth: 10, font: { size: 10 } } }
                  }
              }
          });
      }
  </script>

 </asp:Content>


<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 

<div class="container-fluid mt-4">
    <div class="row justify-content-end">
        <!-- Asumiendo que hay un menú lateral en col-md-3, usamos col-md-9 -->
        <div class="col-md-10">
       
            <!-- Título del Reporte -->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h3 class="text-dark fw-bold">
                    <i class="fas fa-chart-line text-primary me-2"></i>
                    Reporte de Ventas - Noviembre
                </h3>
                <div class="date-badge">
                    <i class="far fa-calendar-alt"></i> Noviembre 2023
                </div>
            </div>

            <!-- Fila Superior: KPIs y Filtros -->
            <div class="row mb-4">
                <!-- Columna Izquierda: Tarjetas de KPI (Los números grandes) -->
                <div class="col-lg-5">
                    <div class="row g-3">
                        <!-- KPI 1 -->
                        <div class="col-md-6">
                            <div class="card kpi-card shadow-lg border-0">
                                <div class="card-body text-center">
                                    <h3 class="fw-bold text-primary">160.00</h3>
                                    <p class="text-muted small mb-0">Mín. de Venta_Minima</p>
                                </div>
                            </div>
                        </div>
                        <!-- KPI 2 -->
                        <div class="col-md-6">
                            <div class="card kpi-card shadow-lg border-0">
                                <div class="card-body text-center">
                                    <h3 class="fw-bold text-success">3.00 mil</h3>
                                    <p class="text-muted small mb-0">Máx. de Venta_Max</p>
                                </div>
                            </div>
                        </div>
                        <!-- KPI 3 -->
                        <div class="col-md-6">
                            <div class="card kpi-card shadow-lg border-0">
                                <div class="card-body text-center">
                                    <h3 class="fw-bold text-dark">3.46 mill.</h3>
                                    <p class="text-muted small mb-0">Suma de Venta_Bruta</p>
                                </div>
                            </div>
                        </div>
                        <!-- KPI 4 -->
                        <div class="col-md-6">
                            <div class="card kpi-card shadow-lg border-0">
                                <div class="card-body text-center">
                                    <h3 class="fw-bold text-danger">127.77 mil</h3>
                                    <p class="text-muted small mb-0">Descuentos</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Columna Derecha: Panel de Filtros (Estado Pedido) -->
                <div class="col-lg-7">
                    <div class="card shadow-lg border-0 h-100 bg-dark-gradient text-white">
                        <div class="card-header bg-transparent border-0">
                            <h6 class="mb-0"><i class="fas fa-filter me-1"></i> Estado del Pedido</h6>
                        </div>
                       <div class="card-body d-flex align-items-center">
                             <div class="row w-100 g-2">
                                <div class="col-md-4">
                                     <label class="small text-white-50 mb-1">Mes</label>
                                     <asp:DropDownList ID="DDL_Mes" runat="server" CssClass="form-select form-select-sm shadow-none" AutoPostBack="true">
                                         <asp:ListItem Text="Enero" Value="1" />
                                         <asp:ListItem Text="Febrero" Value="2" />
                                         <asp:ListItem Text="Marzo" Value="3" />
                                         <asp:ListItem Text="Abril" Value="4" />
                                         <asp:ListItem Text="Mayo" Value="5" />
                                         <asp:ListItem Text="Junio" Value="6" />
                                         <asp:ListItem Text="Julio" Value="7" />
                                         <asp:ListItem Text="Agosto" Value="8" />
                                         <asp:ListItem Text="Septiembre" Value="9" />
                                         <asp:ListItem Text="Octubre" Value="10" />
                                         <asp:ListItem Text="Noviembre" Value="11" Selected="True" />
                                         <asp:ListItem Text="Diciembre" Value="12" />
                                     </asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                     <label class="small text-white-50 mb-1">Año</label>
                                     <asp:DropDownList ID="DDL_Anio" runat="server" CssClass="form-select form-select-sm shadow-none" AutoPostBack="true">
                                         <asp:ListItem Text="2022" Value="2022" />
                                         <asp:ListItem Text="2023" Value="2023" Selected="True" />
                                         <asp:ListItem Text="2024" Value="2024" />
                                     </asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                     <label class="small text-white-50 mb-1">Estado</label>
                                     <asp:DropDownList ID="DDL_Estado" runat="server" CssClass="form-select form-select-sm shadow-none" AutoPostBack="true">
                                         <asp:ListItem Text="Todos" Value="%" Selected="True"></asp:ListItem>
                                         <asp:ListItem Text="Aprobado" Value="Aprobado"></asp:ListItem>
                                         <asp:ListItem Text="En Proceso" Value="En Proceso"></asp:ListItem>
                                         <asp:ListItem Text="Enviado" Value="Enviado"></asp:ListItem>
                                         <asp:ListItem Text="Pendiente" Value="Pendiente"></asp:ListItem>
                                         <asp:ListItem Text="Cancelado" Value="Cancelado"></asp:ListItem>
                                         <asp:ListItem Text="Entregado" Value="Entregado"></asp:ListItem>
                                         <asp:ListItem Text="Facturado" Value="Facturado"></asp:ListItem>
                                     </asp:DropDownList>
                                </div>
                             </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Fila Media: Tablas de Resumen -->
            <div class="row mb-4">
                <!-- Tabla Modelos -->
                <div class="col-md-5">
                    <div class="card shadow-sm border-0 h-100">
                        <div class="card-header bg-white border-bottom-0 pt-3">
                            <h6 class="fw-bold text-secondary">Modelo vs Cantidad</h6>
                        </div>
                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <table class="table table-hover align-middle mb-0">
                                    <thead class="bg-light text-secondary small">
                                        <tr>
                                            <th>Modelo</th>
                                            <th class="text-end">Total</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr><td>TOP FLEX</td><td class="text-end fw-bold">4,473</td></tr>
                                        <tr><td>MUNDIAL</td><td class="text-end fw-bold">3,947</td></tr>
                                        <tr><td>FS REACTIVE</td><td class="text-end fw-bold">2,584</td></tr>
                                        <tr><td>LIGA 5</td><td class="text-end fw-bold">1,365</td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Tabla Pivot Colores (Simplificada para vista) -->
                <div class="col-md-7">
                    <div class="card shadow-sm border-0 h-100">
                        <div class="card-header bg-white border-bottom-0 pt-3">
                            <h6 class="fw-bold text-secondary">Matriz Color vs Talla</h6>
                        </div>
                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <table class="table table-sm table-bordered text-center small mb-0">
                                    <thead class="bg-light">
                                        <tr>
                                            <th>Color</th>
                                            <th>39</th>
                                            <th>40</th>
                                            <th>41</th>
                                            <th>42</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr><td class="text-start">Ámbar</td><td>222</td><td>277</td><td>272</td><td>324</td></tr>
                                        <tr><td class="text-start">Azul</td><td>265</td><td>223</td><td>488</td><td>611</td></tr>
                                        <tr><td class="text-start">Blanco</td><td>263</td><td>333</td><td>469</td><td>756</td></tr>
                                        <tr><td class="text-start">Negro</td><td>834</td><td>751</td><td>1242</td><td>1364</td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

                    <!-- Fila Inferior: Gráficos -->
               <div class="row">
                <!-- Gráfico 1: Ventas por Empleado -->
                <div class="col-md-4">
                    <div class="card shadow-lg border-0 h-100">
                        <div class="card-body">
                            <h6 class="card-title text-secondary small fw-bold">Ventas por Empleado</h6>
                            <div style="height: 250px; position: relative;">
                                <!-- Canvas para Chart.js -->
                                <canvas id="chartEmpleados"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Gráfico 2: Ventas Diarias -->
                <div class="col-md-4">
                    <div class="card shadow-lg border-0 h-100">
                        <div class="card-body">
                            <h6 class="card-title text-secondary small fw-bold">Ventas Diarias</h6>
                            <div style="height: 250px; position: relative;">
                                <canvas id="chartDiario"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Gráfico 3: Estado de Ventas -->
                <div class="col-md-4">
                    <div class="card shadow-sm border-0 h-100">
                        <div class="card-body">
                            <h6 class="card-title text-secondary small fw-bold">Estado de Ventas</h6>
                            <div style="height: 250px; position: relative;">
                                <canvas id="chartEstado"></canvas>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
      
        </div>
 
</div>
   
   

   
</asp:Content>
