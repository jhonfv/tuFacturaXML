using System;

namespace tuFactura.utilitarios.Modelos.Factura
{
    public class XEntradaDeMercancia
    {
        public int EntradaId { get; set; }
        public short BodegaId { get; set; }
        public short CentroCostoId { get; set; }
        public DateTime FechaFactura { get; set; }
        public string? Observacion { get; set; }
        public byte EstadoMovimiento { get; set; }
        public short EquipoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaDeSistema { get; set; }
        public byte TipoCompra { get; set; }
        public short EquipoModificacionId { get; set; }
        public int UsuarioModificacionId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int ProveedorId { get; set; }
        public string Factura { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int? OrdenId { get; set; }
        public string? NombreVendedor { get; set; }
        public bool MoverExistencias { get; set; }
        public double TotalEntrada { get; set; }
        public int EnsaEntradaId { get; set; }
        public int EnsaSalidaId { get; set; }
        public int EntradaNo { get; set; }
        public string? MotivoAnulacion { get; set; }
        public byte RegimenId { get; set; }
    }
} 