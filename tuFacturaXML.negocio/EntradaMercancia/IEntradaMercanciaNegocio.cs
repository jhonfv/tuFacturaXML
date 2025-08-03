using System.Threading.Tasks;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFactura.utilitarios.Modelos.Factura;

namespace tuFacturaXML.negocio.EntradaMercancia
{
    public interface IEntradaMercanciaNegocio
    {
        Task<ResultadoEntradaMercancia> ProcesarEntradaMercanciaAsync(InvoiceType factura);
        Task<XEntradaDeMercancia?> ObtenerEntradaMercanciaAsync(int entradaId);
        Task<bool> EliminarEntradaMercanciaAsync(int entradaId);
        Task<bool> ActualizarEntradaMercanciaAsync(XEntradaDeMercancia entrada);
    }

    public class ResultadoEntradaMercancia
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int? EntradaId { get; set; }
        public XEntradaDeMercancia? Entrada { get; set; }
        public List<XEntradaDeMercanciaDetalle> Detalles { get; set; } = new();
    }
} 