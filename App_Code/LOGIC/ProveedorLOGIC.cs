using System;
using System.Data;

public class ProveedorLOGIC
{
    private readonly ProveedorDAO dao = new ProveedorDAO();

    public string Generar_CodProveedor() => dao.Generar_CodProveedor();

    public DataTable Listar_Empresas() => dao.Listar_Empresas();

    public string Mantener(string codProveedor, string apellidos, string nombres, string telefono, string email, string cpostal, string direccion, string codEmpresa, string estado, string tipo)
    {
        if (string.IsNullOrWhiteSpace(codProveedor)) return "ERROR: Código es obligatorio";
        if (string.IsNullOrWhiteSpace(apellidos)) return "ERROR: Apellidos es obligatorio";
        if (string.IsNullOrWhiteSpace(nombres)) return "ERROR: Nombres es obligatorio";
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@")) return "ERROR: Email inválido";
        if (string.IsNullOrWhiteSpace(codEmpresa)) return "ERROR: Empresa es obligatoria";
        return dao.Mantener(codProveedor, apellidos, nombres, telefono, email, cpostal, direccion, codEmpresa, estado, tipo);
    }

    public DataTable Listar_Proveedores(string filtro) => dao.Listar_Proveedores(filtro);
}

