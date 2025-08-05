namespace tuFactura.utilitarios.Modelos.Factura
{
    public class ProductoAlterno
    {
        public string CodigoAlterno { get; set; } = "";
        public int ProductoId { get; set; }
        public bool Principal { get; set; }
        public double Cantidad { get; set; }
    }
} 