using System;

namespace tuFactura.utilitarios.Modelos.Factura
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string DescripcionLarga { get; set; } = "";
        public string DescripcionCorta { get; set; } = "";
        public string Referencia { get; set; } = "";
        public double Embalaje { get; set; }
        public short? CasaComercialId { get; set; }
        public byte UnidadDeMedidaId { get; set; }
        public int ProveedorId { get; set; }
        public short Familia1Id { get; set; }
        public short? Familia2Id { get; set; }
        public short? Familia3Id { get; set; }
        public short? Familia4Id { get; set; }
        public short? Familia5Id { get; set; }
        public byte IvaCompraId { get; set; }
        public byte IvaVentaId { get; set; }
        public double ImpoConsumo { get; set; }
        public double Empaque { get; set; }
        public bool VenderXPeso { get; set; }
        public bool VenderXFraccion { get; set; }
        public bool NoManejaInventario { get; set; }
        public bool EsConjunto { get; set; }
        public bool TieneLote { get; set; }
        public bool TieneSerial { get; set; }
        public bool EsServicio { get; set; }
        public bool EsProduccion { get; set; }
        public bool EsConcesion { get; set; }
        public bool EsObsequio { get; set; }
        public bool PerteneceAsociacion { get; set; }
        public bool ProductoWeb { get; set; }
        public bool EsBolsa { get; set; }
        public short EquipoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaDeSistema { get; set; }
        public bool Interno { get; set; }
        public double ManoDeObra { get; set; }
        public bool EsAncheta { get; set; }
        public bool AplicaGrupoDeCosto { get; set; }
        public bool NoAplicaRedondeo { get; set; }
        public bool EsInsumo { get; set; }
        public bool? TienePreciosxSucursal { get; set; }
        public bool? TieneCostoxSucursal { get; set; }
        public string? CaracteristicasWeb { get; set; }
    }
} 