using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using tuFactura.utilitarios.Modelos.Factura;

namespace tuFactura.utilitarios.Herramientas.Database
{
    public class SqlServerDatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public SqlServerDatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=localhost;initial catalog=APLBDSFE;user ID=sa; password=mc300900..*.";
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> InsertEntradaMercanciaAsync(XEntradaDeMercancia entrada)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO XEntradaDeMercancia (
                    BodegaId, CentroCostoId, FechaFactura, Observacion, EstadoMovimiento,
                    EquipoId, UsuarioId, FechaDeSistema, TipoCompra, EquipoModificacionId,
                    UsuarioModificacionId, FechaModificacion, ProveedorId, Factura, FechaVencimiento,
                    OrdenId, NombreVendedor, MoverExistencias, TotalEntrada, EnsaEntradaId,
                    EnsaSalidaId, EntradaNo, MotivoAnulacion, RegimenId
                ) VALUES (
                    @BodegaId, @CentroCostoId, @FechaFactura, @Observacion, @EstadoMovimiento,
                    @EquipoId, @UsuarioId, @FechaDeSistema, @TipoCompra, @EquipoModificacionId,
                    @UsuarioModificacionId, @FechaModificacion, @ProveedorId, @Factura, @FechaVencimiento,
                    @OrdenId, @NombreVendedor, @MoverExistencias, @TotalEntrada, @EnsaEntradaId,
                    @EnsaSalidaId, @EntradaNo, @MotivoAnulacion, @RegimenId
                );
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BodegaId", entrada.BodegaId);
            command.Parameters.AddWithValue("@CentroCostoId", entrada.CentroCostoId);
            command.Parameters.AddWithValue("@FechaFactura", entrada.FechaFactura);
            command.Parameters.AddWithValue("@Observacion", (object?)entrada.Observacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstadoMovimiento", entrada.EstadoMovimiento);
            command.Parameters.AddWithValue("@EquipoId", entrada.EquipoId);
            command.Parameters.AddWithValue("@UsuarioId", entrada.UsuarioId);
            command.Parameters.AddWithValue("@FechaDeSistema", entrada.FechaDeSistema);
            command.Parameters.AddWithValue("@TipoCompra", entrada.TipoCompra);
            command.Parameters.AddWithValue("@EquipoModificacionId", entrada.EquipoModificacionId);
            command.Parameters.AddWithValue("@UsuarioModificacionId", entrada.UsuarioModificacionId);
            command.Parameters.AddWithValue("@FechaModificacion", entrada.FechaModificacion);
            command.Parameters.AddWithValue("@ProveedorId", entrada.ProveedorId);
            command.Parameters.AddWithValue("@Factura", entrada.Factura);
            command.Parameters.AddWithValue("@FechaVencimiento", entrada.FechaVencimiento);
            command.Parameters.AddWithValue("@OrdenId", (object?)entrada.OrdenId ?? DBNull.Value);
            command.Parameters.AddWithValue("@NombreVendedor", (object?)entrada.NombreVendedor ?? DBNull.Value);
            command.Parameters.AddWithValue("@MoverExistencias", entrada.MoverExistencias);
            command.Parameters.AddWithValue("@TotalEntrada", entrada.TotalEntrada);
            command.Parameters.AddWithValue("@EnsaEntradaId", entrada.EnsaEntradaId);
            command.Parameters.AddWithValue("@EnsaSalidaId", entrada.EnsaSalidaId);
            command.Parameters.AddWithValue("@EntradaNo", entrada.EntradaNo);
            command.Parameters.AddWithValue("@MotivoAnulacion", (object?)entrada.MotivoAnulacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@RegimenId", entrada.RegimenId);

            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<bool> InsertEntradaMercanciaDetalleAsync(XEntradaDeMercanciaDetalle detalle)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO XEntradaDeMercanciaDetalle (
                    EntradaId, Id, ProductoId, CodigoAlterno, DescripcionLarga, Embalaje,
                    Unidades, Cajas, ImpoConsumo, IvaCompraId, PrecioCosto, PrecioCostoPromedio,
                    PrecioPublico, Total, Empaque, Flete, Descargue, Observacion, UnidadesOrden,
                    OrdenId, Dc1, Dc2, Dc3, Dc4, Dc5, AplicaDf, Df1, Df2, Df3, Df4, Df5,
                    UnidadesDif, CajasDif, PrecioCostoDif, ImpoConsumoDif, Dc1Dif, Dc2Dif,
                    Dc3Dif, Dc4Dif, Dc5Dif, Df1Dif, Df2Dif, Df3Dif, Df4Dif, Df5Dif, Bonificado,
                    TotalIva, Pendiente, PrecioCostoProducto, Interno, Ordenar, RetencionId,
                    VerificarNovedad, ObservacionNovedad, AplicaDc, AplicaIvaFlete, IvaFleteId
                ) VALUES (
                    @EntradaId, @Id, @ProductoId, @CodigoAlterno, @DescripcionLarga, @Embalaje,
                    @Unidades, @Cajas, @ImpoConsumo, @IvaCompraId, @PrecioCosto, @PrecioCostoPromedio,
                    @PrecioPublico, @Total, @Empaque, @Flete, @Descargue, @Observacion, @UnidadesOrden,
                    @OrdenId, @Dc1, @Dc2, @Dc3, @Dc4, @Dc5, @AplicaDf, @Df1, @Df2, @Df3, @Df4, @Df5,
                    @UnidadesDif, @CajasDif, @PrecioCostoDif, @ImpoConsumoDif, @Dc1Dif, @Dc2Dif,
                    @Dc3Dif, @Dc4Dif, @Dc5Dif, @Df1Dif, @Df2Dif, @Df3Dif, @Df4Dif, @Df5Dif, @Bonificado,
                    @TotalIva, @Pendiente, @PrecioCostoProducto, @Interno, @Ordenar, @RetencionId,
                    @VerificarNovedad, @ObservacionNovedad, @AplicaDc, @AplicaIvaFlete, @IvaFleteId
                )";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EntradaId", detalle.EntradaId);
            command.Parameters.AddWithValue("@Id", detalle.Id);
            command.Parameters.AddWithValue("@ProductoId", detalle.ProductoId);
            command.Parameters.AddWithValue("@CodigoAlterno", (object?)detalle.CodigoAlterno ?? DBNull.Value);
            command.Parameters.AddWithValue("@DescripcionLarga", (object?)detalle.DescripcionLarga ?? DBNull.Value);
            command.Parameters.AddWithValue("@Embalaje", detalle.Embalaje);
            command.Parameters.AddWithValue("@Unidades", detalle.Unidades);
            command.Parameters.AddWithValue("@Cajas", detalle.Cajas);
            command.Parameters.AddWithValue("@ImpoConsumo", detalle.ImpoConsumo);
            command.Parameters.AddWithValue("@IvaCompraId", detalle.IvaCompraId);
            command.Parameters.AddWithValue("@PrecioCosto", detalle.PrecioCosto);
            command.Parameters.AddWithValue("@PrecioCostoPromedio", detalle.PrecioCostoPromedio);
            command.Parameters.AddWithValue("@PrecioPublico", detalle.PrecioPublico);
            command.Parameters.AddWithValue("@Total", detalle.Total);
            command.Parameters.AddWithValue("@Empaque", detalle.Empaque);
            command.Parameters.AddWithValue("@Flete", detalle.Flete);
            command.Parameters.AddWithValue("@Descargue", detalle.Descargue);
            command.Parameters.AddWithValue("@Observacion", (object?)detalle.Observacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnidadesOrden", detalle.UnidadesOrden);
            command.Parameters.AddWithValue("@OrdenId", (object?)detalle.OrdenId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Dc1", detalle.Dc1);
            command.Parameters.AddWithValue("@Dc2", detalle.Dc2);
            command.Parameters.AddWithValue("@Dc3", detalle.Dc3);
            command.Parameters.AddWithValue("@Dc4", detalle.Dc4);
            command.Parameters.AddWithValue("@Dc5", detalle.Dc5);
            command.Parameters.AddWithValue("@AplicaDf", detalle.AplicaDf);
            command.Parameters.AddWithValue("@Df1", detalle.Df1);
            command.Parameters.AddWithValue("@Df2", detalle.Df2);
            command.Parameters.AddWithValue("@Df3", detalle.Df3);
            command.Parameters.AddWithValue("@Df4", detalle.Df4);
            command.Parameters.AddWithValue("@Df5", detalle.Df5);
            command.Parameters.AddWithValue("@UnidadesDif", detalle.UnidadesDif);
            command.Parameters.AddWithValue("@CajasDif", detalle.CajasDif);
            command.Parameters.AddWithValue("@PrecioCostoDif", detalle.PrecioCostoDif);
            command.Parameters.AddWithValue("@ImpoConsumoDif", detalle.ImpoConsumoDif);
            command.Parameters.AddWithValue("@Dc1Dif", detalle.Dc1Dif);
            command.Parameters.AddWithValue("@Dc2Dif", detalle.Dc2Dif);
            command.Parameters.AddWithValue("@Dc3Dif", detalle.Dc3Dif);
            command.Parameters.AddWithValue("@Dc4Dif", detalle.Dc4Dif);
            command.Parameters.AddWithValue("@Dc5Dif", detalle.Dc5Dif);
            command.Parameters.AddWithValue("@Df1Dif", detalle.Df1Dif);
            command.Parameters.AddWithValue("@Df2Dif", detalle.Df2Dif);
            command.Parameters.AddWithValue("@Df3Dif", detalle.Df3Dif);
            command.Parameters.AddWithValue("@Df4Dif", detalle.Df4Dif);
            command.Parameters.AddWithValue("@Df5Dif", detalle.Df5Dif);
            command.Parameters.AddWithValue("@Bonificado", detalle.Bonificado);
            command.Parameters.AddWithValue("@TotalIva", detalle.TotalIva);
            command.Parameters.AddWithValue("@Pendiente", detalle.Pendiente);
            command.Parameters.AddWithValue("@PrecioCostoProducto", detalle.PrecioCostoProducto);
            command.Parameters.AddWithValue("@Interno", detalle.Interno);
            command.Parameters.AddWithValue("@Ordenar", detalle.Ordenar);
            command.Parameters.AddWithValue("@RetencionId", (object?)detalle.RetencionId ?? DBNull.Value);
            command.Parameters.AddWithValue("@VerificarNovedad", (object?)detalle.VerificarNovedad ?? DBNull.Value);
            command.Parameters.AddWithValue("@ObservacionNovedad", (object?)detalle.ObservacionNovedad ?? DBNull.Value);
            command.Parameters.AddWithValue("@AplicaDc", (object?)detalle.AplicaDc ?? DBNull.Value);
            command.Parameters.AddWithValue("@AplicaIvaFlete", (object?)detalle.AplicaIvaFlete ?? DBNull.Value);
            command.Parameters.AddWithValue("@IvaFleteId", (object?)detalle.IvaFleteId ?? DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<List<XEntradaDeMercancia>> GetEntradasMercanciaAsync()
        {
            var entradas = new List<XEntradaDeMercancia>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradaDeMercancia ORDER BY FechaDeSistema DESC";
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entradas.Add(MapEntradaMercancia(reader));
            }

            return entradas;
        }

        public async Task<XEntradaDeMercancia?> GetEntradaMercanciaByIdAsync(int entradaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradaDeMercancia WHERE EntradaId = @EntradaId";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EntradaId", entradaId);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapEntradaMercancia(reader);
            }

            return null;
        }

        public async Task<List<XEntradaDeMercanciaDetalle>> GetEntradaMercanciaDetallesAsync(int entradaId)
        {
            var detalles = new List<XEntradaDeMercanciaDetalle>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradaDeMercanciaDetalle WHERE EntradaId = @EntradaId ORDER BY Ordenar";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EntradaId", entradaId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                detalles.Add(MapEntradaMercanciaDetalle(reader));
            }

            return detalles;
        }

        public async Task<bool> UpdateEntradaMercanciaAsync(XEntradaDeMercancia entrada)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE XEntradaDeMercancia SET 
                    BodegaId = @BodegaId, CentroCostoId = @CentroCostoId, FechaFactura = @FechaFactura,
                    Observacion = @Observacion, EstadoMovimiento = @EstadoMovimiento, EquipoId = @EquipoId,
                    UsuarioId = @UsuarioId, FechaDeSistema = @FechaDeSistema, TipoCompra = @TipoCompra,
                    EquipoModificacionId = @EquipoModificacionId, UsuarioModificacionId = @UsuarioModificacionId,
                    FechaModificacion = @FechaModificacion, ProveedorId = @ProveedorId, Factura = @Factura,
                    FechaVencimiento = @FechaVencimiento, OrdenId = @OrdenId, NombreVendedor = @NombreVendedor,
                    MoverExistencias = @MoverExistencias, TotalEntrada = @TotalEntrada, EnsaEntradaId = @EnsaEntradaId,
                    EnsaSalidaId = @EnsaSalidaId, EntradaNo = @EntradaNo, MotivoAnulacion = @MotivoAnulacion,
                    RegimenId = @RegimenId
                WHERE EntradaId = @EntradaId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EntradaId", entrada.EntradaId);
            command.Parameters.AddWithValue("@BodegaId", entrada.BodegaId);
            command.Parameters.AddWithValue("@CentroCostoId", entrada.CentroCostoId);
            command.Parameters.AddWithValue("@FechaFactura", entrada.FechaFactura);
            command.Parameters.AddWithValue("@Observacion", (object?)entrada.Observacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstadoMovimiento", entrada.EstadoMovimiento);
            command.Parameters.AddWithValue("@EquipoId", entrada.EquipoId);
            command.Parameters.AddWithValue("@UsuarioId", entrada.UsuarioId);
            command.Parameters.AddWithValue("@FechaDeSistema", entrada.FechaDeSistema);
            command.Parameters.AddWithValue("@TipoCompra", entrada.TipoCompra);
            command.Parameters.AddWithValue("@EquipoModificacionId", entrada.EquipoModificacionId);
            command.Parameters.AddWithValue("@UsuarioModificacionId", entrada.UsuarioModificacionId);
            command.Parameters.AddWithValue("@FechaModificacion", entrada.FechaModificacion);
            command.Parameters.AddWithValue("@ProveedorId", entrada.ProveedorId);
            command.Parameters.AddWithValue("@Factura", entrada.Factura);
            command.Parameters.AddWithValue("@FechaVencimiento", entrada.FechaVencimiento);
            command.Parameters.AddWithValue("@OrdenId", (object?)entrada.OrdenId ?? DBNull.Value);
            command.Parameters.AddWithValue("@NombreVendedor", (object?)entrada.NombreVendedor ?? DBNull.Value);
            command.Parameters.AddWithValue("@MoverExistencias", entrada.MoverExistencias);
            command.Parameters.AddWithValue("@TotalEntrada", entrada.TotalEntrada);
            command.Parameters.AddWithValue("@EnsaEntradaId", entrada.EnsaEntradaId);
            command.Parameters.AddWithValue("@EnsaSalidaId", entrada.EnsaSalidaId);
            command.Parameters.AddWithValue("@EntradaNo", entrada.EntradaNo);
            command.Parameters.AddWithValue("@MotivoAnulacion", (object?)entrada.MotivoAnulacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@RegimenId", entrada.RegimenId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> DeleteEntradaMercanciaAsync(int entradaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM XEntradaDeMercancia WHERE EntradaId = @EntradaId";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EntradaId", entradaId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        private static XEntradaDeMercancia MapEntradaMercancia(SqlDataReader reader)
        {
            return new XEntradaDeMercancia
            {
                EntradaId = reader.GetInt32("EntradaId"),
                BodegaId = reader.GetInt16("BodegaId"),
                CentroCostoId = reader.GetInt16("CentroCostoId"),
                FechaFactura = reader.GetDateTime("FechaFactura"),
                Observacion = reader.IsDBNull("Observacion") ? null : reader.GetString("Observacion"),
                EstadoMovimiento = reader.GetByte("EstadoMovimiento"),
                EquipoId = reader.GetInt16("EquipoId"),
                UsuarioId = reader.GetInt32("UsuarioId"),
                FechaDeSistema = reader.GetDateTime("FechaDeSistema"),
                TipoCompra = reader.GetByte("TipoCompra"),
                EquipoModificacionId = reader.GetInt16("EquipoModificacionId"),
                UsuarioModificacionId = reader.GetInt32("UsuarioModificacionId"),
                FechaModificacion = reader.GetDateTime("FechaModificacion"),
                ProveedorId = reader.GetInt32("ProveedorId"),
                Factura = reader.GetString("Factura"),
                FechaVencimiento = reader.GetDateTime("FechaVencimiento"),
                OrdenId = reader.IsDBNull("OrdenId") ? null : (int?)reader.GetInt32("OrdenId"),
                NombreVendedor = reader.IsDBNull("NombreVendedor") ? null : reader.GetString("NombreVendedor"),
                MoverExistencias = reader.GetBoolean("MoverExistencias"),
                TotalEntrada = reader.GetDouble("TotalEntrada"),
                EnsaEntradaId = reader.GetInt32("EnsaEntradaId"),
                EnsaSalidaId = reader.GetInt32("EnsaSalidaId"),
                EntradaNo = reader.GetInt32("EntradaNo"),
                MotivoAnulacion = reader.IsDBNull("MotivoAnulacion") ? null : reader.GetString("MotivoAnulacion"),
                RegimenId = reader.GetByte("RegimenId")
            };
        }

        private static XEntradaDeMercanciaDetalle MapEntradaMercanciaDetalle(SqlDataReader reader)
        {
            return new XEntradaDeMercanciaDetalle
            {
                EntradaId = reader.GetInt32("EntradaId"),
                Id = reader.GetInt32("Id"),
                ProductoId = reader.GetInt32("ProductoId"),
                CodigoAlterno = reader.IsDBNull("CodigoAlterno") ? null : reader.GetString("CodigoAlterno"),
                DescripcionLarga = reader.IsDBNull("DescripcionLarga") ? null : reader.GetString("DescripcionLarga"),
                Embalaje = reader.GetDouble("Embalaje"),
                Unidades = reader.GetDouble("Unidades"),
                Cajas = reader.GetDouble("Cajas"),
                ImpoConsumo = reader.GetDouble("ImpoConsumo"),
                IvaCompraId = reader.GetByte("IvaCompraId"),
                PrecioCosto = reader.GetDouble("PrecioCosto"),
                PrecioCostoPromedio = reader.GetDouble("PrecioCostoPromedio"),
                PrecioPublico = reader.GetDouble("PrecioPublico"),
                Total = reader.GetDouble("Total"),
                Empaque = reader.GetDouble("Empaque"),
                Flete = reader.GetDouble("Flete"),
                Descargue = reader.GetDouble("Descargue"),
                Observacion = reader.IsDBNull("Observacion") ? null : reader.GetString("Observacion"),
                UnidadesOrden = reader.GetDouble("UnidadesOrden"),
                OrdenId = reader.IsDBNull("OrdenId") ? null : (int?)reader.GetInt32("OrdenId"),
                Dc1 = reader.GetDouble("Dc1"),
                Dc2 = reader.GetDouble("Dc2"),
                Dc3 = reader.GetDouble("Dc3"),
                Dc4 = reader.GetDouble("Dc4"),
                Dc5 = reader.GetDouble("Dc5"),
                AplicaDf = reader.GetBoolean("AplicaDf"),
                Df1 = reader.GetDouble("Df1"),
                Df2 = reader.GetDouble("Df2"),
                Df3 = reader.GetDouble("Df3"),
                Df4 = reader.GetDouble("Df4"),
                Df5 = reader.GetDouble("Df5"),
                UnidadesDif = reader.GetDouble("UnidadesDif"),
                CajasDif = reader.GetDouble("CajasDif"),
                PrecioCostoDif = reader.GetDouble("PrecioCostoDif"),
                ImpoConsumoDif = reader.GetDouble("ImpoConsumoDif"),
                Dc1Dif = reader.GetDouble("Dc1Dif"),
                Dc2Dif = reader.GetDouble("Dc2Dif"),
                Dc3Dif = reader.GetDouble("Dc3Dif"),
                Dc4Dif = reader.GetDouble("Dc4Dif"),
                Dc5Dif = reader.GetDouble("Dc5Dif"),
                Df1Dif = reader.GetDouble("Df1Dif"),
                Df2Dif = reader.GetDouble("Df2Dif"),
                Df3Dif = reader.GetDouble("Df3Dif"),
                Df4Dif = reader.GetDouble("Df4Dif"),
                Df5Dif = reader.GetDouble("Df5Dif"),
                Bonificado = reader.GetBoolean("Bonificado"),
                TotalIva = reader.GetDouble("TotalIva"),
                Pendiente = reader.GetBoolean("Pendiente"),
                PrecioCostoProducto = reader.GetDouble("PrecioCostoProducto"),
                Interno = reader.GetBoolean("Interno"),
                Ordenar = reader.GetInt32("Ordenar"),
                RetencionId = reader.IsDBNull("RetencionId") ? null : (byte?)reader.GetByte("RetencionId"),
                VerificarNovedad = reader.IsDBNull("VerificarNovedad") ? null : (bool?)reader.GetBoolean("VerificarNovedad"),
                ObservacionNovedad = reader.IsDBNull("ObservacionNovedad") ? null : reader.GetString("ObservacionNovedad"),
                AplicaDc = reader.IsDBNull("AplicaDc") ? null : (bool?)reader.GetBoolean("AplicaDc"),
                AplicaIvaFlete = reader.IsDBNull("AplicaIvaFlete") ? null : (bool?)reader.GetBoolean("AplicaIvaFlete"),
                IvaFleteId = reader.IsDBNull("IvaFleteId") ? null : (byte?)reader.GetByte("IvaFleteId")
            };
        }

        public async Task<List<ValidacionProducto>> ValidarProductosAsync(List<string> skus)
        {
            var resultados = new List<ValidacionProducto>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                foreach (var sku in skus)
                {
                    var validacion = new ValidacionProducto
                    {
                        SKU = sku,
                        ExisteEnBaseDeDatos = false
                    };

                    // Primero buscar en Productos por Referencia
                    var producto = await GetProductoByReferenciaAsync(sku);
                    if (producto != null)
                    {
                        validacion.ExisteEnBaseDeDatos = true;
                        validacion.ProductoId = producto.ProductoId;
                        validacion.DescripcionProducto = producto.DescripcionLarga;
                        validacion.ReferenciaProducto = producto.Referencia;
                        validacion.EsCodigoAlterno = false;
                        validacion.EsPrincipal = true;
                    }
                    else
                    {
                        // Si no se encuentra en Productos, buscar en ProductosAlternos
                        var productoAlterno = await GetProductoAlternoByCodigoAsync(sku);
                        if (productoAlterno != null)
                        {
                            validacion.ExisteEnBaseDeDatos = true;
                            validacion.ProductoId = productoAlterno.ProductoId;
                            validacion.EsCodigoAlterno = true;
                            validacion.EsPrincipal = productoAlterno.Principal;

                            // Obtener información del producto principal
                            var productoPrincipal = await GetProductoByIdAsync(productoAlterno.ProductoId);
                            if (productoPrincipal != null)
                            {
                                validacion.DescripcionProducto = productoPrincipal.DescripcionLarga;
                                validacion.ReferenciaProducto = productoPrincipal.Referencia;
                            }
                        }
                    }

                    resultados.Add(validacion);
                }

                return resultados;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error validating products: {ex.Message}", ex);
            }
        }

        public async Task<Producto?> GetProductoByReferenciaAsync(string referencia)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT ProductoId, DescripcionLarga, DescripcionCorta, Referencia, 
                           Embalaje, CasaComercialId, UnidadDeMedidaId, ProveedorId,
                           Familia1Id, Familia2Id, Familia3Id, Familia4Id, Familia5Id,
                           IvaCompraId, IvaVentaId, ImpoConsumo, Empaque, VenderXPeso,
                           VenderXFraccion, NoManejaInventario, EsConjunto, TieneLote,
                           TieneSerial, EsServicio, EsProduccion, EsConcesion, EsObsequio,
                           PerteneceAsociacion, ProductoWeb, EsBolsa, EquipoId, UsuarioId,
                           FechaDeSistema, Interno, ManoDeObra, EsAncheta, AplicaGrupoDeCosto,
                           NoAplicaRedondeo, EsInsumo, TienePreciosxSucursal, TieneCostoxSucursal,
                           CaracteristicasWeb
                    FROM Productos 
                    WHERE Referencia = @Referencia";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Referencia", referencia);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Producto
                    {
                        ProductoId = reader.GetInt32("ProductoId"),
                        DescripcionLarga = reader.GetString("DescripcionLarga"),
                        DescripcionCorta = reader.GetString("DescripcionCorta"),
                        Referencia = reader.GetString("Referencia"),
                        Embalaje = reader.GetDouble("Embalaje"),
                        CasaComercialId = reader.IsDBNull("CasaComercialId") ? null : reader.GetInt16("CasaComercialId"),
                        UnidadDeMedidaId = reader.GetByte("UnidadDeMedidaId"),
                        ProveedorId = reader.GetInt32("ProveedorId"),
                        Familia1Id = reader.GetInt16("Familia1Id"),
                        Familia2Id = reader.IsDBNull("Familia2Id") ? null : reader.GetInt16("Familia2Id"),
                        Familia3Id = reader.IsDBNull("Familia3Id") ? null : reader.GetInt16("Familia3Id"),
                        Familia4Id = reader.IsDBNull("Familia4Id") ? null : reader.GetInt16("Familia4Id"),
                        Familia5Id = reader.IsDBNull("Familia5Id") ? null : reader.GetInt16("Familia5Id"),
                        IvaCompraId = reader.GetByte("IvaCompraId"),
                        IvaVentaId = reader.GetByte("IvaVentaId"),
                        ImpoConsumo = reader.GetDouble("ImpoConsumo"),
                        Empaque = reader.GetDouble("Empaque"),
                        VenderXPeso = reader.GetBoolean("VenderXPeso"),
                        VenderXFraccion = reader.GetBoolean("VenderXFraccion"),
                        NoManejaInventario = reader.GetBoolean("NoManejaInventario"),
                        EsConjunto = reader.GetBoolean("EsConjunto"),
                        TieneLote = reader.GetBoolean("TieneLote"),
                        TieneSerial = reader.GetBoolean("TieneSerial"),
                        EsServicio = reader.GetBoolean("EsServicio"),
                        EsProduccion = reader.GetBoolean("EsProduccion"),
                        EsConcesion = reader.GetBoolean("EsConcesion"),
                        EsObsequio = reader.GetBoolean("EsObsequio"),
                        PerteneceAsociacion = reader.GetBoolean("PerteneceAsociacion"),
                        ProductoWeb = reader.GetBoolean("ProductoWeb"),
                        EsBolsa = reader.GetBoolean("EsBolsa"),
                        EquipoId = reader.GetInt16("EquipoId"),
                        UsuarioId = reader.GetInt32("UsuarioId"),
                        FechaDeSistema = reader.GetDateTime("FechaDeSistema"),
                        Interno = reader.GetBoolean("Interno"),
                        ManoDeObra = reader.GetDouble("ManoDeObra"),
                        EsAncheta = reader.GetBoolean("EsAncheta"),
                        AplicaGrupoDeCosto = reader.GetBoolean("AplicaGrupoDeCosto"),
                        NoAplicaRedondeo = reader.GetBoolean("NoAplicaRedondeo"),
                        EsInsumo = reader.GetBoolean("EsInsumo"),
                        TienePreciosxSucursal = reader.IsDBNull("TienePreciosxSucursal") ? null : reader.GetBoolean("TienePreciosxSucursal"),
                        TieneCostoxSucursal = reader.IsDBNull("TieneCostoxSucursal") ? null : reader.GetBoolean("TieneCostoxSucursal"),
                        CaracteristicasWeb = reader.IsDBNull("CaracteristicasWeb") ? null : reader.GetString("CaracteristicasWeb")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting producto by referencia: {ex.Message}", ex);
            }
        }

        public async Task<ProductoAlterno?> GetProductoAlternoByCodigoAsync(string codigoAlterno)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT CodigoAlterno, ProductoId, Principal, Cantidad
                    FROM ProductosAlternos 
                    WHERE CodigoAlterno = @CodigoAlterno";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodigoAlterno", codigoAlterno);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ProductoAlterno
                    {
                        CodigoAlterno = reader.GetString("CodigoAlterno"),
                        ProductoId = reader.GetInt32("ProductoId"),
                        Principal = reader.GetBoolean("Principal"),
                        Cantidad = reader.GetDouble("Cantidad")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting producto alterno by codigo: {ex.Message}", ex);
            }
        }

        public async Task<List<ProductoAlterno>> GetProductosAlternosByProductoIdAsync(int productoId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT CodigoAlterno, ProductoId, Principal, Cantidad
                    FROM ProductosAlternos 
                    WHERE ProductoId = @ProductoId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductoId", productoId);

                var productosAlternos = new List<ProductoAlterno>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    productosAlternos.Add(new ProductoAlterno
                    {
                        CodigoAlterno = reader.GetString("CodigoAlterno"),
                        ProductoId = reader.GetInt32("ProductoId"),
                        Principal = reader.GetBoolean("Principal"),
                        Cantidad = reader.GetDouble("Cantidad")
                    });
                }

                return productosAlternos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting productos alternos by producto id: {ex.Message}", ex);
            }
        }

        private async Task<Producto?> GetProductoByIdAsync(int productoId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT ProductoId, DescripcionLarga, DescripcionCorta, Referencia, 
                           Embalaje, CasaComercialId, UnidadDeMedidaId, ProveedorId,
                           Familia1Id, Familia2Id, Familia3Id, Familia4Id, Familia5Id,
                           IvaCompraId, IvaVentaId, ImpoConsumo, Empaque, VenderXPeso,
                           VenderXFraccion, NoManejaInventario, EsConjunto, TieneLote,
                           TieneSerial, EsServicio, EsProduccion, EsConcesion, EsObsequio,
                           PerteneceAsociacion, ProductoWeb, EsBolsa, EquipoId, UsuarioId,
                           FechaDeSistema, Interno, ManoDeObra, EsAncheta, AplicaGrupoDeCosto,
                           NoAplicaRedondeo, EsInsumo, TienePreciosxSucursal, TieneCostoxSucursal,
                           CaracteristicasWeb
                    FROM Productos 
                    WHERE ProductoId = @ProductoId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductoId", productoId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Producto
                    {
                        ProductoId = reader.GetInt32("ProductoId"),
                        DescripcionLarga = reader.GetString("DescripcionLarga"),
                        DescripcionCorta = reader.GetString("DescripcionCorta"),
                        Referencia = reader.GetString("Referencia"),
                        Embalaje = reader.GetDouble("Embalaje"),
                        CasaComercialId = reader.IsDBNull("CasaComercialId") ? null : reader.GetInt16("CasaComercialId"),
                        UnidadDeMedidaId = reader.GetByte("UnidadDeMedidaId"),
                        ProveedorId = reader.GetInt32("ProveedorId"),
                        Familia1Id = reader.GetInt16("Familia1Id"),
                        Familia2Id = reader.IsDBNull("Familia2Id") ? null : reader.GetInt16("Familia2Id"),
                        Familia3Id = reader.IsDBNull("Familia3Id") ? null : reader.GetInt16("Familia3Id"),
                        Familia4Id = reader.IsDBNull("Familia4Id") ? null : reader.GetInt16("Familia4Id"),
                        Familia5Id = reader.IsDBNull("Familia5Id") ? null : reader.GetInt16("Familia5Id"),
                        IvaCompraId = reader.GetByte("IvaCompraId"),
                        IvaVentaId = reader.GetByte("IvaVentaId"),
                        ImpoConsumo = reader.GetDouble("ImpoConsumo"),
                        Empaque = reader.GetDouble("Empaque"),
                        VenderXPeso = reader.GetBoolean("VenderXPeso"),
                        VenderXFraccion = reader.GetBoolean("VenderXFraccion"),
                        NoManejaInventario = reader.GetBoolean("NoManejaInventario"),
                        EsConjunto = reader.GetBoolean("EsConjunto"),
                        TieneLote = reader.GetBoolean("TieneLote"),
                        TieneSerial = reader.GetBoolean("TieneSerial"),
                        EsServicio = reader.GetBoolean("EsServicio"),
                        EsProduccion = reader.GetBoolean("EsProduccion"),
                        EsConcesion = reader.GetBoolean("EsConcesion"),
                        EsObsequio = reader.GetBoolean("EsObsequio"),
                        PerteneceAsociacion = reader.GetBoolean("PerteneceAsociacion"),
                        ProductoWeb = reader.GetBoolean("ProductoWeb"),
                        EsBolsa = reader.GetBoolean("EsBolsa"),
                        EquipoId = reader.GetInt16("EquipoId"),
                        UsuarioId = reader.GetInt32("UsuarioId"),
                        FechaDeSistema = reader.GetDateTime("FechaDeSistema"),
                        Interno = reader.GetBoolean("Interno"),
                        ManoDeObra = reader.GetDouble("ManoDeObra"),
                        EsAncheta = reader.GetBoolean("EsAncheta"),
                        AplicaGrupoDeCosto = reader.GetBoolean("AplicaGrupoDeCosto"),
                        NoAplicaRedondeo = reader.GetBoolean("NoAplicaRedondeo"),
                        EsInsumo = reader.GetBoolean("EsInsumo"),
                        TienePreciosxSucursal = reader.IsDBNull("TienePreciosxSucursal") ? null : reader.GetBoolean("TienePreciosxSucursal"),
                        TieneCostoxSucursal = reader.IsDBNull("TieneCostoxSucursal") ? null : reader.GetBoolean("TieneCostoxSucursal"),
                        CaracteristicasWeb = reader.IsDBNull("CaracteristicasWeb") ? null : reader.GetString("CaracteristicasWeb")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting producto by id: {ex.Message}", ex);
            }
        }
    }
} 