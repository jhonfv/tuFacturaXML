using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tuFacturaXML.negocio.Facturas;
using tuFactura.utilitarios.Modelos.Factura;
using System.Text.Json;

namespace tuFacturaXML.webApp.Controllers.Facturas
{
    public class FacturasController : Controller
    {
        private readonly IFacturasNegocio _facturasNegocio;
        private static readonly Dictionary<string, List<ArchivoAdjunto>> _archivosTemporales = new();

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
                return View("Details", null);
            }
            
            var resultado = await _facturasNegocio.procesarFacturaAsync(file);
            
            // Guardar archivos adjuntos en memoria temporal
            if (resultado.EsArchivoZip && resultado.ArchivosAdjuntos.Any())
            {
                var sessionId = HttpContext.Session.Id ?? Guid.NewGuid().ToString();
                _archivosTemporales[sessionId] = resultado.ArchivosAdjuntos;
                HttpContext.Session.SetString("SessionId", sessionId);
            }
            
            return View("Details", resultado);
        }

        // GET: FacturasController/Details/5
        public ActionResult Details()
        {
            return View();
        }

        // GET: Visualizar archivo adjunto
        [HttpGet]
        public IActionResult VisualizarAdjunto(int index)
        {
            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                
                if (!string.IsNullOrEmpty(sessionId) && _archivosTemporales.TryGetValue(sessionId, out var archivos))
                {
                    if (index >= 0 && index < archivos.Count)
                    {
                        var archivo = archivos[index];
                        
                        // Configurar headers específicos para PDFs
                        if (archivo.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{archivo.Nombre}\"");
                            Response.Headers.Add("X-Content-Type-Options", "nosniff");
                            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                            Response.Headers.Add("Pragma", "no-cache");
                            Response.Headers.Add("Expires", "0");
                        }
                        else
                        {
                            // Para otros archivos, permitir descarga
                            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{archivo.Nombre}\"");
                        }
                        
                        return File(archivo.Contenido, archivo.TipoMime, archivo.Nombre);
                    }
                }
                
                // Si no encuentra el archivo, devolver un error más descriptivo
                return NotFound($"Archivo no encontrado. Index: {index}, SessionId: {sessionId}, Archivos disponibles: {(_archivosTemporales.ContainsKey(sessionId ?? "") ? _archivosTemporales[sessionId ?? ""].Count : 0)}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al procesar archivo: {ex.Message}");
            }
        }

        // GET: Visualizar PDF inline específicamente
        [HttpGet]
        public IActionResult VisualizarPDF(int index)
        {
            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                Console.WriteLine($"VisualizarPDF - Index: {index}, SessionId: {sessionId}");
                
                if (!string.IsNullOrEmpty(sessionId) && _archivosTemporales.TryGetValue(sessionId, out var archivos))
                {
                    Console.WriteLine($"Archivos encontrados: {archivos.Count}");
                    
                    if (index >= 0 && index < archivos.Count)
                    {
                        var archivo = archivos[index];
                        Console.WriteLine($"Archivo: {archivo.Nombre}, Extension: {archivo.Extension}, Tamaño: {archivo.Contenido.Length}");
                        
                        if (archivo.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            // Headers específicos para visualización inline de PDFs
                            Response.Headers.Add("Content-Type", "application/pdf");
                            Response.Headers.Add("Content-Disposition", "inline");
                            Response.Headers.Add("Accept-Ranges", "bytes");
                            Response.Headers.Add("Cache-Control", "public, max-age=0");
                            Response.Headers.Add("X-Content-Type-Options", "nosniff");
                            
                            Console.WriteLine("Devolviendo PDF con headers inline");
                            return File(archivo.Contenido, "application/pdf");
                        }
                        else
                        {
                            Console.WriteLine($"El archivo no es un PDF: {archivo.Extension}");
                            return BadRequest("El archivo no es un PDF");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Index fuera de rango: {index}, archivos disponibles: {archivos.Count}");
                    }
                }
                else
                {
                    Console.WriteLine($"No se encontraron archivos para SessionId: {sessionId}");
                }
                
                return NotFound("PDF no encontrado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en VisualizarPDF: {ex.Message}");
                return BadRequest($"Error al procesar PDF: {ex.Message}");
            }
        }
    }
}
