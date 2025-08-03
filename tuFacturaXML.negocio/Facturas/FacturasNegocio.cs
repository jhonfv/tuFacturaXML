using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFactura.utilitarios.Modelos.Factura;
using static tuFactura.utilitarios.Modelos.DIAN.Attach;

namespace tuFacturaXML.negocio.Facturas
{
    public class FacturasNegocio : IFacturasNegocio
    {
        readonly IConversiones _FileTools;
        readonly IProcesarZip _ProcesarZip;

        public FacturasNegocio(IConversiones fileTools, IProcesarZip procesarZip)
        {
            _FileTools = fileTools;
            _ProcesarZip = procesarZip;
        }

        public async Task<ResultadoProcesamiento> procesarFacturaAsync(List<IFormFile> files)
        {
            var resultado = new ResultadoProcesamiento();
            
            foreach (var file in files)
            {
                switch (Path.GetExtension(file.FileName).ToLower())
                {
                    case ".zip":
                        resultado.EsArchivoZip = true;
                        var resultadoZip = ProcesarArchivoZip(file);
                        resultado.Facturas.AddRange(resultadoZip.Facturas);
                        resultado.ArchivosAdjuntos.AddRange(resultadoZip.ArchivosAdjuntos);
                        break;
                    case ".xml":
                        using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                        {
                            string xmlString = await streamReader.ReadToEndAsync();
                            resultado.Facturas.Add(ProcesarArchivoXML(xmlString));
                        }                        
                        break;
                }
            }

            return resultado;
        }

        private ResultadoProcesamiento ProcesarArchivoZip(IFormFile fileZip)
        {
            var resultado = new ResultadoProcesamiento { EsArchivoZip = true };
            var zipBytes = _FileTools.ConvertirIFormFileABytes(fileZip);

            using (var stream = new MemoryStream(zipBytes))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            using (var entryStream = entry.Open())
                            using (var ms = new MemoryStream())
                            {
                                entryStream.CopyTo(ms);
                                var contenido = ms.ToArray();

                                if (entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Procesar XML
                                    string xmlString = Encoding.UTF8.GetString(contenido);
                                    resultado.Facturas.Add(ProcesarArchivoXML(xmlString));
                                }
                                else
                                {
                                    // Agregar como archivo adjunto
                                    var archivoAdjunto = new ArchivoAdjunto
                                    {
                                        Nombre = entry.Name,
                                        Extension = Path.GetExtension(entry.Name).ToLower(),
                                        Contenido = contenido,
                                        TipoMime = ObtenerTipoMime(entry.Name),
                                        Tamaño = entry.Length
                                    };
                                    resultado.ArchivosAdjuntos.Add(archivoAdjunto);
                                }
                            }
                        }
                    }
                }
            }
            return resultado;
        }

        private string ObtenerTipoMime(string nombreArchivo)
        {
            var extension = Path.GetExtension(nombreArchivo).ToLower();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        private InvoiceType ProcesarArchivoXML(string xmlString)
        {
            try
            {
                // Primero intentar deserializar directamente como InvoiceType
                XmlSerializer invoiceSerializer = new XmlSerializer(typeof(InvoiceType));
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    var invoiceObject = invoiceSerializer.Deserialize(stringReader);
                    if (invoiceObject != null)
                    {
                        return (InvoiceType)invoiceObject;
                    }
                }
            }
            catch
            {
                // Si falla, intentar como AttachedDocumentType
                try
                {
                    var attachedDocument = new AttachedDocumentType();
                    XmlSerializer attachedSerializer = new XmlSerializer(typeof(AttachedDocumentType));
                    string invoice = string.Empty;

                    using (StringReader stringReader = new StringReader(xmlString))
                    {
                        var data = attachedSerializer.Deserialize(stringReader);
                        if (data != null)
                        {
                            attachedDocument = (AttachedDocumentType)data;

                            if (attachedDocument?.Attachment?.ExternalReference?.Description != null && 
                                attachedDocument.Attachment.ExternalReference.Description.Length > 0)
                            {
                                invoice = attachedDocument.Attachment.ExternalReference.Description[0].Value;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(invoice))
                    {
                        byte[] invoiceArray = Encoding.ASCII.GetBytes(invoice);
                        using (var invoiceStream = new MemoryStream(invoiceArray))
                        {
                            XmlSerializer invoiceSerializer = new XmlSerializer(typeof(InvoiceType));
                            var invoiceObject = invoiceSerializer.Deserialize(invoiceStream);
                            
                            if (invoiceObject != null)
                            {
                                return (InvoiceType)invoiceObject;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"No se pudo procesar el archivo XML: {ex.Message}");
                }
            }

            throw new InvalidOperationException("No se pudo deserializar el archivo XML como factura válida");
        }
    }
}
