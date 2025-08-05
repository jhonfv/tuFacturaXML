namespace tuFactura.utilitarios.Modelos.Factura
{
    public class ValidacionProducto
    {
        public string SKU { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public bool ExisteEnBaseDeDatos { get; set; }
        public int? ProductoId { get; set; }
        public string? DescripcionProducto { get; set; }
        public string? ReferenciaProducto { get; set; }
        public bool EsCodigoAlterno { get; set; }
        public bool EsPrincipal { get; set; }
    }
} 