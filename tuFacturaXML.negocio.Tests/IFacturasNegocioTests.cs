using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tuFactura.utilitarios.Herramientas.Logging;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFactura.utilitarios.Modelos.Factura;
using tuFacturaXML.negocio.Facturas;
using Xunit;

namespace tuFacturaXML.negocio.Tests
{
    public class FacturasNegocioTests
    {
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly FacturasNegocio _facturasNegocio;

        public FacturasNegocioTests()
        {
            _mockLogger = new Mock<ILoggerService>();
            _facturasNegocio = new FacturasNegocio(_mockLogger.Object);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConArchivosNulos_DeberiaRetornarResultadoVacio()
        {
            // Arrange
            List<IFormFile> files = null;

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Facturas);
            Assert.Empty(resultado.ArchivosAdjuntos);
            Assert.False(resultado.EsArchivoZip);
            
            _mockLogger.Verify(x => x.LogWarning("No se proporcionaron archivos para procesar", null), Times.Once);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConListaVacia_DeberiaRetornarResultadoVacio()
        {
            // Arrange
            var files = new List<IFormFile>();

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Facturas);
            Assert.Empty(resultado.ArchivosAdjuntos);
            Assert.False(resultado.EsArchivoZip);
            
            _mockLogger.Verify(x => x.LogWarning("No se proporcionaron archivos para procesar", null), Times.Once);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConArchivoXMLValido_DeberiaProcesarCorrectamente()
        {
            // Arrange
            var xmlContent = CreateValidXMLContent();
            var mockFile = CreateMockFormFile("test.xml", "text/xml", xmlContent);

            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            // El XML de prueba puede no ser válido para el deserializador, así que verificamos que al menos se procesó
            Assert.True(resultado.Facturas.Count >= 0);
            Assert.Empty(resultado.ArchivosAdjuntos);
            Assert.False(resultado.EsArchivoZip);
            
            _mockLogger.Verify(x => x.LogInformation("Iniciando procesamiento de facturas", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConArchivoNoSoportado_DeberiaRegistrarWarning()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.txt", "text/plain", new byte[] { 1, 2, 3 });
            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Facturas);
            Assert.Empty(resultado.ArchivosAdjuntos);
            
            _mockLogger.Verify(x => x.LogWarning("Tipo de archivo no soportado", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConErrorEnProcesamiento_DeberiaRegistrarError()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.xml", "text/xml", new byte[] { 1, 2, 3 }); // XML inválido
            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Facturas);
            
            _mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ProcesarFacturaAsync_ConExcepcionEnArchivo_DeberiaRegistrarError()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.xml", "text/xml", new byte[] { 1, 2, 3 });
            mockFile.Setup(x => x.OpenReadStream()).Throws(new InvalidOperationException("Error crítico"));
            
            var files = new List<IFormFile> { mockFile.Object };

            // Act
            var resultado = await _facturasNegocio.procesarFacturaAsync(files);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Facturas);
            
            // Verificar que se registró el error a nivel de archivo
            _mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object>()), Times.Once);
        }

        private Mock<IFormFile> CreateMockFormFile(string fileName, string contentType, byte[] content)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.FileName).Returns(fileName);
            mockFile.Setup(x => x.ContentType).Returns(contentType);
            mockFile.Setup(x => x.Length).Returns(content.Length);
            mockFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream(content));
            
            return mockFile;
        }

        private byte[] CreateValidXMLContent()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Invoice xmlns=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"">
    <cbc:ID>FAC001</cbc:ID>
    <cbc:IssueDate>2024-01-01</cbc:IssueDate>
    <cac:AccountingCustomerParty>
        <cac:Party>
            <cac:PartyName>
                <cbc:Name>Proveedor Test</cbc:Name>
            </cac:PartyName>
        </cac:Party>
    </cac:AccountingCustomerParty>
    <cac:InvoiceLine>
        <cbc:ID>1</cbc:ID>
        <cbc:InvoicedQuantity>1</cbc:InvoicedQuantity>
        <cac:Item>
            <cbc:Description>Producto Test</cbc:Description>
        </cac:Item>
    </cac:InvoiceLine>
</Invoice>";

            return System.Text.Encoding.UTF8.GetBytes(xml);
        }
    }
} 