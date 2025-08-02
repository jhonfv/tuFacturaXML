using Microsoft.AspNetCore.Http;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFactura.utilitarios.Modelos.Factura;

namespace tuFacturaXML.negocio.Facturas
{
    public interface IFacturasNegocio
    {
        public Task<ResultadoProcesamiento> procesarFacturaAsync(List<IFormFile> files);
    }
}
