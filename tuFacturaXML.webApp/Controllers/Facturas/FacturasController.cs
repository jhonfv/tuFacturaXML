using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tuFacturaXML.negocio.Facturas;

namespace tuFacturaXML.webApp.Controllers.Facturas
{
    public class FacturasController : Controller
    {

        private readonly IFacturasNegocio _facturasNegocio;

        public FacturasController(IFacturasNegocio facturasNegocio)
        {
            _facturasNegocio = facturasNegocio;
        }

        // GET: FacturasController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CargarFactura(List<IFormFile> file)
        {
            if (file == null || file.Count == 0)
            {
                //return NotFound("El archivo es nulo o está vacío.");
                return View("Details", null);
            }
            var facturas = await _facturasNegocio.procesarFacturaAsync(file);
            return View("Details", facturas);
        }

        // GET: FacturasController/Details/5
        public ActionResult Details()
        {
            return View();
        }        
    }
}
