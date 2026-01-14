using System;
using System.IO;
using System.Net;
using System.Text;

public static class PaymentVerificationService
{
    // Verificación sincrónica para simplificar integración con WebForms.
    // Retorna true/false y mensaje de detalle.
    public static bool Verify(string tipoPago, decimal importe, string referencia, out string mensaje)
    {
        mensaje = string.Empty;
        tipoPago = (tipoPago ?? string.Empty).Trim().ToUpperInvariant();

        // Efectivo: no requiere verificación externa.
        if (tipoPago == "EFE")
        {
            mensaje = "Pago en efectivo no requiere verificación.";
            return true;
        }

        // Tarjeta/Transferencia: aquí podrías integrar PSP/Bank API.
        if (tipoPago == "TAR" || tipoPago == "TRA")
        {
            // Simulación básica de verificación.
            if (!string.IsNullOrWhiteSpace(referencia) && importe > 0)
            {
                mensaje = "Verificación simulada exitosa.";
                return true;
            }
            mensaje = "Datos insuficientes para verificar pago (referencia/importe).";
            return false;
        }

        // Yape: integración vía API si está configurada, si no, simulación.
        if (tipoPago == "YAP")
        {
            var baseUrl = System.Configuration.ConfigurationManager.AppSettings["YapeApiBaseUrl"];
            var apiToken = System.Configuration.ConfigurationManager.AppSettings["YapeApiToken"];

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiToken))
            {
                // Fallback: simulación si no hay configuración.
                if (!string.IsNullOrWhiteSpace(referencia) && importe > 0)
                {
                    mensaje = "Yape API no configurada. Verificación simulada exitosa.";
                    return true;
                }
                mensaje = "Yape API no configurada y datos insuficientes para simular.";
                return false;
            }

            try
            {
                // Ejemplo de llamada POST JSON: { reference, amount }
                var url = baseUrl.TrimEnd('/') + "/payments/verify";
                var jsonBody = "{\"reference\":\"" + EscapeJson(referencia) + "\",\"amount\":" + importe.ToString("0.00") + "}";
                var data = Encoding.UTF8.GetBytes(jsonBody);

                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Headers["Authorization"] = "Bearer " + apiToken;

                using (var stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    var responseText = reader.ReadToEnd();
                    // Simplificación: si el status code es 200 asumimos verificado.
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        mensaje = "Verificación Yape exitosa.";
                        return true;
                    }
                    mensaje = "Verificación Yape fallida. Código: " + (int)resp.StatusCode;
                    return false;
                }
            }
            catch (WebException ex)
            {
                string detail = ex.Message;
                try
                {
                    using (var resp = (HttpWebResponse)ex.Response)
                    using (var reader = new StreamReader(resp.GetResponseStream()))
                    {
                        detail = reader.ReadToEnd();
                    }
                }
                catch { }
                mensaje = "Error llamando Yape API: " + detail;
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado: " + ex.Message;
                return false;
            }
        }

        mensaje = "Tipo de pago no soportado.";
        return false;
    }

    private static string EscapeJson(string s)
    {
        if (s == null) return string.Empty;
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}