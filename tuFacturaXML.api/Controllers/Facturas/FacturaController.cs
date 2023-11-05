using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using tuFacturaXML.negocio.Facturas;

namespace tuFacturaXML.api.Controllers.Facturas
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        readonly IFacturasNegocio _facturasNegocio;

        public FacturaController(IFacturasNegocio facturasNegocio)
        {
            _facturasNegocio = facturasNegocio;
        }

        [HttpPost("/cargarDocumentos")]
        public async Task<IActionResult> cargarDocumentosAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se enviaron archivos.");
            }

            var facturas= await _facturasNegocio.procesarFacturaAsync(files);
            

                return Ok(facturas);
        }
    }
}
