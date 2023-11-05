using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public async Task<List<InvoiceType>> procesarFacturaAsync(List<IFormFile> files)
        {
            var facturasXML = new List<InvoiceType>();
            foreach (var file in files)
            {
                switch (Path.GetExtension(file.FileName).ToLower())
                {
                    case ".zip":
                        var facturas = ProcesarArchivoZip(file);
                        foreach(var factura in facturas)
                        {
                            facturasXML.Add(ProcesarArchivoXML(factura));
                        }

                        break;
                    case ".xml":
                        using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                        {
                            string xmlString = await streamReader.ReadToEndAsync();
                            facturasXML.Add(ProcesarArchivoXML(xmlString));
                        }                        
                        break;
                }
            }

            return facturasXML;
        }

        private List<string> ProcesarArchivoZip(IFormFile fileZip)
        {
            var zipBytes = _FileTools.ConvertirIFormFileABytes(fileZip);
            var result = new List<string>();

            using (var stream = new MemoryStream(zipBytes))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        // Verifica si el archivo tiene la extensión .xml
                        if (entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var entryStream = entry.Open())
                            using (var reader = new StreamReader(entryStream))
                            {
                                string contenidoXML = reader.ReadToEnd();
                                result.Add(contenidoXML);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private InvoiceType ProcesarArchivoXML(string xmlString)
        {
            var attachedDocument = new AttachedDocumentType();
            XmlSerializer serializer = new XmlSerializer(typeof(AttachedDocumentType));
            InvoiceType factura = new InvoiceType();
            string invoice = string.Empty;
            byte[] invoiceArray = null;
            Stream invoiceStream = null;

            using (StringReader stringReader = new StringReader(xmlString))
            {
                var data = serializer.Deserialize(stringReader);
                if(data != null)
                    attachedDocument = (AttachedDocumentType)data;

                if(attachedDocument != null)
                    invoice = attachedDocument.Attachment.ExternalReference.Description[0].Value;
            }

            invoiceArray = Encoding.ASCII.GetBytes(invoice);
            invoiceStream = new MemoryStream(invoiceArray);

            serializer = new XmlSerializer(typeof(InvoiceType));
            var invoiceObject = serializer.Deserialize(invoiceStream);
            
            if (invoiceObject != null)
                factura=(InvoiceType)invoiceObject;

            return factura;
        }

    }
}
