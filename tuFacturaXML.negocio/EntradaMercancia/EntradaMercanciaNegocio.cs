using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuFactura.utilitarios.Herramientas.Database;
using tuFactura.utilitarios.Herramientas.Logging;
using tuFactura.utilitarios.Modelos.DIAN;
using tuFactura.utilitarios.Modelos.Factura;

namespace tuFacturaXML.negocio.EntradaMercancia
{
    public class EntradaMercanciaNegocio : IEntradaMercanciaNegocio
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILoggerService _logger;

        public EntradaMercanciaNegocio(IDatabaseService databaseService, ILoggerService logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<ResultadoEntradaMercancia> ProcesarEntradaMercanciaAsync(InvoiceType factura)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de entrada de mercancía", new { FacturaId = factura.ID?.Value });

                // Crear la entrada de mercancía
                var entrada = new XEntradaDeMercancia
                {
                    BodegaId = 1, // Bodega por defecto
                    CentroCostoId = 1, // Centro de costo por defecto
                    FechaFactura = factura.IssueDate?.Value ?? DateTime.Now,
                    Observacion = $"Factura procesada desde XML: {factura.ID?.Value}",
                    EstadoMovimiento = 1, // Activo
                    EquipoId = 1, // Equipo por defecto
                    UsuarioId = 1, // Usuario por defecto
                    FechaDeSistema = DateTime.Now,
                    TipoCompra = 1, // Compra normal
                    EquipoModificacionId = 1,
                    UsuarioModificacionId = 1,
                    FechaModificacion = DateTime.Now,
                    ProveedorId = ObtenerProveedorId(factura),
                    Factura = factura.ID?.Value ?? "",
                    FechaVencimiento = factura.DueDate?.Value ?? DateTime.Now.AddDays(30),
                    OrdenId = null,
                    NombreVendedor = factura.AccountingCustomerParty?.Party?.PartyName?.FirstOrDefault()?.Name?.Value,
                    MoverExistencias = true,
                    TotalEntrada = 0, // Se calculará después
                    EnsaEntradaId = 0,
                    EnsaSalidaId = 0,
                    EntradaNo = await ObtenerSiguienteEntradaNoAsync(),
                    MotivoAnulacion = null,
                    RegimenId = 1 // Régimen común
                };

                // Insertar la entrada de mercancía
                var entradaId = await _databaseService.InsertEntradaMercanciaAsync(entrada);
                entrada.EntradaId = entradaId;

                _logger.LogInformation("Entrada de mercancía creada", new { EntradaId = entradaId });

                // Procesar los detalles de la factura
                var detalles = new List<XEntradaDeMercanciaDetalle>();
                var totalEntrada = 0.0;

                foreach (var linea in factura.InvoiceLine)
                {
                    var detalle = await CrearDetalleEntradaMercanciaAsync(entradaId, linea, detalles.Count + 1);
                    detalles.Add(detalle);
                    totalEntrada += detalle.Total;

                    _logger.LogInformation("Detalle de entrada procesado", new { 
                        EntradaId = entradaId, 
                        ProductoId = detalle.ProductoId, 
                        Unidades = detalle.Unidades 
                    });
                }

                // Actualizar el total de la entrada
                entrada.TotalEntrada = totalEntrada;
                await _databaseService.UpdateEntradaMercanciaAsync(entrada);

                _logger.LogInformation("Entrada de mercancía completada exitosamente", new { 
                    EntradaId = entradaId, 
                    TotalProductos = detalles.Count, 
                    TotalEntrada = totalEntrada 
                });

                return new ResultadoEntradaMercancia
                {
                    Exitoso = true,
                    Mensaje = "Entrada de mercancía procesada exitosamente",
                    EntradaId = entradaId,
                    Entrada = entrada,
                    Detalles = detalles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al procesar entrada de mercancía", ex, new { FacturaId = factura.ID?.Value });
                return new ResultadoEntradaMercancia
                {
                    Exitoso = false,
                    Mensaje = $"Error al procesar entrada de mercancía: {ex.Message}"
                };
            }
        }

        public async Task<XEntradaDeMercancia?> ObtenerEntradaMercanciaAsync(int entradaId)
        {
            try
            {
                return await _databaseService.GetEntradaMercanciaByIdAsync(entradaId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener entrada de mercancía", ex, new { EntradaId = entradaId });
                return null;
            }
        }

        public async Task<bool> EliminarEntradaMercanciaAsync(int entradaId)
        {
            try
            {
                var resultado = await _databaseService.DeleteEntradaMercanciaAsync(entradaId);
                if (resultado)
                {
                    _logger.LogInformation("Entrada de mercancía eliminada", new { EntradaId = entradaId });
                }
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar entrada de mercancía", ex, new { EntradaId = entradaId });
                return false;
            }
        }

        public async Task<bool> ActualizarEntradaMercanciaAsync(XEntradaDeMercancia entrada)
        {
            try
            {
                var resultado = await _databaseService.UpdateEntradaMercanciaAsync(entrada);
                if (resultado)
                {
                    _logger.LogInformation("Entrada de mercancía actualizada", new { EntradaId = entrada.EntradaId });
                }
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar entrada de mercancía", ex, new { EntradaId = entrada.EntradaId });
                return false;
            }
        }

        private int ObtenerProveedorId(InvoiceType factura)
        {
            // Lógica para obtener el ID del proveedor basado en el NIT/RUT
            var nitRut = factura.AccountingCustomerParty?.Party?.PartyIdentification?.FirstOrDefault()?.ID?.Value 
                         ?? factura.AccountingCustomerParty?.Party?.PartyLegalEntity?.FirstOrDefault()?.CompanyID?.Value;

            if (!string.IsNullOrEmpty(nitRut))
            {
                // Aquí podrías implementar lógica para buscar el proveedor en la base de datos
                // Por ahora, usamos un ID por defecto
                return 1;
            }

            return 1; // Proveedor por defecto
        }

        private async Task<int> ObtenerSiguienteEntradaNoAsync()
        {
            try
            {
                var entradas = await _databaseService.GetEntradasMercanciaAsync();
                return entradas.Count > 0 ? entradas.Max(e => e.EntradaNo) + 1 : 1;
            }
            catch
            {
                return 1;
            }
        }

        private async Task<XEntradaDeMercanciaDetalle> CrearDetalleEntradaMercanciaAsync(
            int entradaId, 
            InvoiceLineType linea, 
            int orden)
        {
            var precioUnitario = Convert.ToDouble(linea.Price?.PriceAmount?.Value ?? 0);
            var cantidad = Convert.ToDouble(linea.InvoicedQuantity?.Value ?? 0);
            var total = precioUnitario * cantidad;

            // Calcular IVA
            var totalIva = 0.0;
            if (linea.TaxTotal != null && linea.TaxTotal.Length > 0)
            {
                foreach (var tax in linea.TaxTotal)
                {
                    if (tax.TaxAmount?.Value != null)
                    {
                        totalIva += Convert.ToDouble(tax.TaxAmount.Value);
                    }
                }
            }

            // Obtener descuentos
            var descuento = 0.0;
            if (linea.AllowanceCharge != null && linea.AllowanceCharge.Length > 0)
            {
                var descuentoCharge = linea.AllowanceCharge.FirstOrDefault(ac => ac.ChargeIndicator?.Value == false);
                if (descuentoCharge?.Amount?.Value != null)
                {
                    descuento = Convert.ToDouble(descuentoCharge.Amount.Value);
                }
            }

            var detalle = new XEntradaDeMercanciaDetalle
            {
                EntradaId = entradaId,
                Id = orden,
                ProductoId = await ObtenerProductoIdAsync(linea),
                CodigoAlterno = linea.Item?.SellersItemIdentification?.ID?.Value 
                               ?? linea.Item?.StandardItemIdentification?.ID?.Value,
                DescripcionLarga = linea.Item?.Description?.FirstOrDefault()?.Value,
                Embalaje = 1, // Embalaje por defecto
                Unidades = cantidad,
                Cajas = cantidad, // Asumiendo 1 unidad = 1 caja
                ImpoConsumo = 0, // Por defecto
                IvaCompraId = 1, // IVA por defecto
                PrecioCosto = precioUnitario,
                PrecioCostoPromedio = precioUnitario,
                PrecioPublico = precioUnitario,
                Total = total,
                Empaque = 0,
                Flete = 0,
                Descargue = 0,
                Observacion = null,
                UnidadesOrden = cantidad,
                OrdenId = null,
                Dc1 = descuento,
                Dc2 = 0,
                Dc3 = 0,
                Dc4 = 0,
                Dc5 = 0,
                AplicaDf = false,
                Df1 = 0,
                Df2 = 0,
                Df3 = 0,
                Df4 = 0,
                Df5 = 0,
                UnidadesDif = cantidad,
                CajasDif = cantidad,
                PrecioCostoDif = precioUnitario,
                ImpoConsumoDif = 0,
                Dc1Dif = descuento,
                Dc2Dif = 0,
                Dc3Dif = 0,
                Dc4Dif = 0,
                Dc5Dif = 0,
                Df1Dif = 0,
                Df2Dif = 0,
                Df3Dif = 0,
                Df4Dif = 0,
                Df5Dif = 0,
                Bonificado = false,
                TotalIva = totalIva,
                Pendiente = false,
                PrecioCostoProducto = precioUnitario,
                Interno = false,
                Ordenar = orden,
                RetencionId = null,
                VerificarNovedad = false,
                ObservacionNovedad = null,
                AplicaDc = true,
                AplicaIvaFlete = false,
                IvaFleteId = null
            };

            // Insertar el detalle en la base de datos
            await _databaseService.InsertEntradaMercanciaDetalleAsync(detalle);

            return detalle;
        }

        private async Task<int> ObtenerProductoIdAsync(InvoiceLineType linea)
        {
            // Lógica para obtener el ID del producto basado en el código
            var codigoProducto = linea.Item?.SellersItemIdentification?.ID?.Value 
                                ?? linea.Item?.StandardItemIdentification?.ID?.Value;

            if (!string.IsNullOrEmpty(codigoProducto))
            {
                // Aquí podrías implementar lógica para buscar el producto en la base de datos
                // Por ahora, usamos un ID por defecto
                return 1;
            }

            return 1; // Producto por defecto
        }
    }
} 