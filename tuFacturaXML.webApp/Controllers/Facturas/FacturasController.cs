using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tuFactura.utilitarios.Herramientas.Logging;
using tuFactura.utilitarios.Modelos.Factura;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFacturaXML.negocio.EntradaMercancia;
using tuFacturaXML.negocio.Facturas;
using System.Text.Json;

namespace tuFacturaXML.webApp.Controllers.Facturas
{
    public class FacturasController : Controller
    {
        private readonly IFacturasNegocio _facturasNegocio;
        private readonly IEntradaMercanciaNegocio _entradaMercanciaNegocio;
        private readonly ILoggerService _logger;
        private static readonly Dictionary<int, ArchivoAdjunto> _archivosTemporales = new();
        private static List<InvoiceType> _facturasTemporales = new();

        public FacturasController(
            IFacturasNegocio facturasNegocio, 
            IEntradaMercanciaNegocio entradaMercanciaNegocio,
            ILoggerService logger)
        {
            _facturasNegocio = facturasNegocio;
            _entradaMercanciaNegocio = entradaMercanciaNegocio;
            _logger = logger;
        }

        // GET: FacturasController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CargarFactura(List<IFormFile> files)
        {
            try
            {
                _logger.LogInformation("Iniciando carga de facturas", new { FileCount = files?.Count ?? 0 });

                if (files == null || !files.Any())
                {
                    _logger.LogWarning("No se proporcionaron archivos para cargar");
                    TempData["Error"] = "Por favor, seleccione al menos un archivo.";
                    return RedirectToAction("Index");
                }

                var resultado = await _facturasNegocio.procesarFacturaAsync(files);
                
                if (resultado.Facturas.Any())
                {
                    _logger.LogInformation("Facturas procesadas exitosamente", new { 
                        FacturasCount = resultado.Facturas.Count,
                        EsArchivoZip = resultado.EsArchivoZip,
                        ArchivosAdjuntosCount = resultado.ArchivosAdjuntos.Count
                    });

                    // Guardar archivos adjuntos temporalmente
                    if (resultado.EsArchivoZip && resultado.ArchivosAdjuntos.Any())
                    {
                        for (int i = 0; i < resultado.ArchivosAdjuntos.Count; i++)
                        {
                            _archivosTemporales[i] = resultado.ArchivosAdjuntos[i];
                        }
                    }

                    // Guardar datos en Session
                    HttpContext.Session.SetInt32("FacturasCount", resultado.Facturas.Count);
                    HttpContext.Session.SetInt32("ArchivosAdjuntosCount", resultado.ArchivosAdjuntos.Count);
                    HttpContext.Session.SetString("EsArchivoZip", resultado.EsArchivoZip.ToString());
                    
                    // Guardar las facturas en memoria estática temporalmente
                    _facturasTemporales = resultado.Facturas;
                    
                    return RedirectToAction("Details");
                }
                else
                {
                    _logger.LogWarning("No se encontraron facturas válidas en los archivos");
                    TempData["Error"] = "No se encontraron facturas válidas en los archivos proporcionados.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al procesar facturas", ex);
                TempData["Error"] = $"Error al procesar las facturas: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarEntradaMercancia(int facturaIndex)
        {
            try
            {
                if (_facturasTemporales == null || !_facturasTemporales.Any() || facturaIndex >= _facturasTemporales.Count)
                {
                    TempData["Error"] = "No se encontró la factura especificada.";
                    return RedirectToAction("Details");
                }

                var factura = _facturasTemporales[facturaIndex];
                _logger.LogInformation("Iniciando procesamiento de entrada de mercancía", new { 
                    FacturaId = factura.ID?.Value,
                    FacturaIndex = facturaIndex 
                });

                var resultadoEntrada = await _entradaMercanciaNegocio.ProcesarEntradaMercanciaAsync(factura);

                if (resultadoEntrada.Exitoso)
                {
                    _logger.LogInformation("Entrada de mercancía procesada exitosamente", new { 
                        EntradaId = resultadoEntrada.EntradaId,
                        FacturaId = factura.ID?.Value 
                    });

                    TempData["Success"] = $"Entrada de mercancía creada exitosamente. ID: {resultadoEntrada.EntradaId}";
                }
                else
                {
                    _logger.LogError("Error al procesar entrada de mercancía", null, new { 
                        FacturaId = factura.ID?.Value,
                        Error = resultadoEntrada.Mensaje 
                    });

                    TempData["Error"] = $"Error al procesar entrada de mercancía: {resultadoEntrada.Mensaje}";
                }

                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al procesar entrada de mercancía", ex);
                TempData["Error"] = $"Error al procesar entrada de mercancía: {ex.Message}";
                return RedirectToAction("Details");
            }
        }

        // GET: FacturasController/Details/5
        public ActionResult Details()
        {
            var facturasCount = HttpContext.Session.GetInt32("FacturasCount");
            if (!facturasCount.HasValue || facturasCount.Value == 0)
            {
                return RedirectToAction("Index");
            }
            
            var resultado = new ResultadoProcesamiento
            {
                Facturas = _facturasTemporales,
                EsArchivoZip = bool.Parse(HttpContext.Session.GetString("EsArchivoZip") ?? "false")
            };
            
            return View(resultado);
        }

        // GET: Visualizar archivo adjunto
        [HttpGet]
        public IActionResult VisualizarAdjunto(int id)
        {
            try
            {
                if (_archivosTemporales.TryGetValue(id, out var archivo))
                {
                    _logger.LogInformation("Visualizando archivo adjunto", new { 
                        ArchivoId = id, 
                        Nombre = archivo.Nombre,
                        Extension = archivo.Extension 
                    });

                    return File(archivo.Contenido, ObtenerMimeType(archivo.Extension), archivo.Nombre);
                }

                _logger.LogWarning("Archivo adjunto no encontrado", new { ArchivoId = id });
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al visualizar archivo adjunto", ex, new { ArchivoId = id });
                return StatusCode(500, "Error al visualizar el archivo");
            }
        }

        // GET: Visualizar PDF inline específicamente
        [HttpGet]
        public IActionResult VisualizarPDF(int id)
        {
            try
            {
                if (_archivosTemporales.TryGetValue(id, out var archivo))
                {
                    _logger.LogInformation("Visualizando PDF", new { 
                        ArchivoId = id, 
                        Nombre = archivo.Nombre 
                    });

                    return File(archivo.Contenido, "application/pdf", archivo.Nombre);
                }

                _logger.LogWarning("PDF no encontrado", new { ArchivoId = id });
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al visualizar PDF", ex, new { ArchivoId = id });
                return StatusCode(500, "Error al visualizar el PDF");
            }
        }

        private string ObtenerMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".txt" => "text/plain",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
