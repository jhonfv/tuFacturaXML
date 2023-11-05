using Microsoft.AspNetCore.Http;
using tuFactura.utilitarios.Modelos.DIAN;

namespace tuFacturaXML.negocio.Facturas
{
    public interface IFacturasNegocio
    {
        public Task<List<InvoiceType>> procesarFacturaAsync(List<IFormFile> files);
    }
}
