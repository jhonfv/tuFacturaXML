using System;

namespace tuFactura.utilitarios.Modelos.Factura
{
    public class XEntradaDeMercanciaDetalle
    {
        public int EntradaId { get; set; }
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? CodigoAlterno { get; set; }
        public string? DescripcionLarga { get; set; }
        public double Embalaje { get; set; }
        public double Unidades { get; set; }
        public double Cajas { get; set; }
        public double ImpoConsumo { get; set; }
        public byte IvaCompraId { get; set; }
        public double PrecioCosto { get; set; }
        public double PrecioCostoPromedio { get; set; }
        public double PrecioPublico { get; set; }
        public double Total { get; set; }
        public double Empaque { get; set; }
        public double Flete { get; set; }
        public double Descargue { get; set; }
        public string? Observacion { get; set; }
        public double UnidadesOrden { get; set; }
        public int? OrdenId { get; set; }
        public double Dc1 { get; set; }
        public double Dc2 { get; set; }
        public double Dc3 { get; set; }
        public double Dc4 { get; set; }
        public double Dc5 { get; set; }
        public bool AplicaDf { get; set; }
        public double Df1 { get; set; }
        public double Df2 { get; set; }
        public double Df3 { get; set; }
        public double Df4 { get; set; }
        public double Df5 { get; set; }
        public double UnidadesDif { get; set; }
        public double CajasDif { get; set; }
        public double PrecioCostoDif { get; set; }
        public double ImpoConsumoDif { get; set; }
        public double Dc1Dif { get; set; }
        public double Dc2Dif { get; set; }
        public double Dc3Dif { get; set; }
        public double Dc4Dif { get; set; }
        public double Dc5Dif { get; set; }
        public double Df1Dif { get; set; }
        public double Df2Dif { get; set; }
        public double Df3Dif { get; set; }
        public double Df4Dif { get; set; }
        public double Df5Dif { get; set; }
        public bool Bonificado { get; set; }
        public double TotalIva { get; set; }
        public bool Pendiente { get; set; }
        public double PrecioCostoProducto { get; set; }
        public bool Interno { get; set; }
        public int Ordenar { get; set; }
        public byte? RetencionId { get; set; }
        public bool? VerificarNovedad { get; set; }
        public string? ObservacionNovedad { get; set; }
        public bool? AplicaDc { get; set; }
        public bool? AplicaIvaFlete { get; set; }
        public byte? IvaFleteId { get; set; }
    }
} 