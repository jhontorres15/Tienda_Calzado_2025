using System;
using System.Data;

public class EmpresaProveedorLOGIC
{
    private readonly EmpresaProveedorDAO dao = new EmpresaProveedorDAO();

    public string Generar_CodEmpresa() => dao.Generar_CodEmpresa();

    public DataTable Listar_Paises() => dao.Listar_Paises();

    public string Mantener(string codEmpresa, string razon, string ruc, string codPais, string direccion, string telefono, string email, string estado, string tipo)
    {
        if (string.IsNullOrWhiteSpace(codEmpresa)) return "ERROR: Código es obligatorio";
        if (string.IsNullOrWhiteSpace(razon)) return "ERROR: Razón social es obligatoria";
        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11) return "ERROR: RUC inválido";
        if (string.IsNullOrWhiteSpace(codPais)) return "ERROR: País es obligatorio";
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@")) return "ERROR: Email inválido";
        return dao.Mantener(codEmpresa, razon, ruc, codPais, direccion, telefono, email, estado, tipo);
    }

    public DataTable Listar_Empresas(string filtro) => dao.Listar_Empresas(filtro);
}

