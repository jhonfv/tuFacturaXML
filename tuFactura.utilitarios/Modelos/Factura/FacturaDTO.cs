using tuFactura.utilitarios.Modelos.DIAN;

namespace tuFactura.utilitarios.Modelos.Factura
{
    public class FacturaDTO
    {
        public string Name { get; set; } = "";
    }

    public class ArchivoAdjunto
    {
        public string Nombre { get; set; } = "";
        public string Extension { get; set; } = "";
        public byte[] Contenido { get; set; } = new byte[0];
        public string TipoMime { get; set; } = "";
        public long Tamaño { get; set; }
    }

    public class ResultadoProcesamiento
    {
        public List<InvoiceType> Facturas { get; set; } = new List<InvoiceType>();
        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; } = new List<ArchivoAdjunto>();
        public bool EsArchivoZip { get; set; }
    }
}
