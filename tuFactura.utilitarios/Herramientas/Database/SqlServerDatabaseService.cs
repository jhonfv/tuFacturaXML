using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using tuFactura.utilitarios.Modelos.Factura;
using Dapper;

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



        public async Task<int> InsertEntradaMercanciaAsync(XEntradaDeMercancia entrada)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Obtener el siguiente EntradaId
                var getNextIdSql = "SELECT ISNULL(MAX(EntradaId), 0) + 1 FROM XEntradasDeMercancia";
                var nextId = await connection.ExecuteScalarAsync<int>(getNextIdSql, null, transaction);
                
                // Asignar el ID al objeto entrada
                entrada.EntradaId = nextId;

                var sql = @"
                    INSERT INTO XEntradasDeMercancia (
                        EntradaId, BodegaId, CentroCostoId, FechaFactura, Observacion, EstadoMovimiento,
                        EquipoId, UsuarioId, FechaDeSistema, TipoCompra, EquipoModificacionId,
                        UsuarioModificacionId, FechaModificacion, ProveedorId, Factura, FechaVencimiento,
                        OrdenId, NombreVendedor, MoverExistencias, TotalEntrada, EnsaEntradaId,
                        EnsaSalidaId, EntradaNo, MotivoAnulacion, RegimenId
                    ) VALUES (
                        @EntradaId, @BodegaId, @CentroCostoId, @FechaFactura, @Observacion, @EstadoMovimiento,
                        @EquipoId, @UsuarioId, @FechaDeSistema, @TipoCompra, @EquipoModificacionId,
                        @UsuarioModificacionId, @FechaModificacion, @ProveedorId, @Factura, @FechaVencimiento,
                        @OrdenId, @NombreVendedor, @MoverExistencias, @TotalEntrada, @EnsaEntradaId,
                        @EnsaSalidaId, @EntradaNo, @MotivoAnulacion, @RegimenId
                    )";

                var parameters = new
                {
                    entrada.EntradaId,
                    entrada.BodegaId,
                    entrada.CentroCostoId,
                    entrada.FechaFactura,
                    Observacion = entrada.Observacion ?? (object)DBNull.Value,
                    entrada.EstadoMovimiento,
                    entrada.EquipoId,
                    entrada.UsuarioId,
                    entrada.FechaDeSistema,
                    entrada.TipoCompra,
                    entrada.EquipoModificacionId,
                    entrada.UsuarioModificacionId,
                    entrada.FechaModificacion,
                    entrada.ProveedorId,
                    entrada.Factura,
                    entrada.FechaVencimiento,
                    OrdenId = entrada.OrdenId ?? (object)DBNull.Value,
                    NombreVendedor = entrada.NombreVendedor ?? (object)DBNull.Value,
                    entrada.MoverExistencias,
                    entrada.TotalEntrada,
                    entrada.EnsaEntradaId,
                    entrada.EnsaSalidaId,
                    entrada.EntradaNo,
                    MotivoAnulacion = entrada.MotivoAnulacion ?? (object)DBNull.Value,
                    entrada.RegimenId
                };

                await connection.ExecuteAsync(sql, parameters, transaction);
                
                transaction.Commit();
                return entrada.EntradaId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> InsertEntradaMercanciaDetalleAsync(XEntradaDeMercanciaDetalle detalle)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Obtener el siguiente Id para el detalle
                var getNextIdSql = "SELECT ISNULL(MAX(Id), 0) + 1 FROM XEntradasDeMercanciaDetalle WHERE EntradaId = @EntradaId";
                var nextId = await connection.ExecuteScalarAsync<int>(getNextIdSql, new { detalle.EntradaId }, transaction);
                
                // Asignar el ID al objeto detalle
                detalle.Id = nextId;

                var sql = @"
                    INSERT INTO XEntradasDeMercanciaDetalle (
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

                var parameters = new
                {
                    detalle.EntradaId,
                    detalle.Id,
                    detalle.ProductoId,
                    CodigoAlterno = detalle.CodigoAlterno ?? (object)DBNull.Value,
                    DescripcionLarga = detalle.DescripcionLarga ?? (object)DBNull.Value,
                    detalle.Embalaje,
                    detalle.Unidades,
                    detalle.Cajas,
                    detalle.ImpoConsumo,
                    detalle.IvaCompraId,
                    detalle.PrecioCosto,
                    detalle.PrecioCostoPromedio,
                    detalle.PrecioPublico,
                    detalle.Total,
                    detalle.Empaque,
                    detalle.Flete,
                    detalle.Descargue,
                    Observacion = detalle.Observacion ?? (object)DBNull.Value,
                    detalle.UnidadesOrden,
                    OrdenId = detalle.OrdenId ?? (object)DBNull.Value,
                    detalle.Dc1,
                    detalle.Dc2,
                    detalle.Dc3,
                    detalle.Dc4,
                    detalle.Dc5,
                    detalle.AplicaDf,
                    detalle.Df1,
                    detalle.Df2,
                    detalle.Df3,
                    detalle.Df4,
                    detalle.Df5,
                    detalle.UnidadesDif,
                    detalle.CajasDif,
                    detalle.PrecioCostoDif,
                    detalle.ImpoConsumoDif,
                    detalle.Dc1Dif,
                    detalle.Dc2Dif,
                    detalle.Dc3Dif,
                    detalle.Dc4Dif,
                    detalle.Dc5Dif,
                    detalle.Df1Dif,
                    detalle.Df2Dif,
                    detalle.Df3Dif,
                    detalle.Df4Dif,
                    detalle.Df5Dif,
                    detalle.Bonificado,
                    detalle.TotalIva,
                    detalle.Pendiente,
                    detalle.PrecioCostoProducto,
                    detalle.Interno,
                    detalle.Ordenar,
                    RetencionId = detalle.RetencionId ?? (object)DBNull.Value,
                    VerificarNovedad = detalle.VerificarNovedad ?? (object)DBNull.Value,
                    ObservacionNovedad = detalle.ObservacionNovedad ?? (object)DBNull.Value,
                    AplicaDc = detalle.AplicaDc ?? (object)DBNull.Value,
                    AplicaIvaFlete = detalle.AplicaIvaFlete ?? (object)DBNull.Value,
                    IvaFleteId = detalle.IvaFleteId ?? (object)DBNull.Value
                };

                var result = await connection.ExecuteAsync(sql, parameters, transaction);
                
                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<XEntradaDeMercancia>> GetEntradasMercanciaAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradasDeMercancia ORDER BY FechaDeSistema DESC";
            return (await connection.QueryAsync<XEntradaDeMercancia>(sql)).ToList();
        }

        public async Task<XEntradaDeMercancia?> GetEntradaMercanciaByIdAsync(int entradaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradasDeMercancia WHERE EntradaId = @EntradaId";
            return await connection.QueryFirstOrDefaultAsync<XEntradaDeMercancia>(sql, new { EntradaId = entradaId });
        }

        public async Task<List<XEntradaDeMercanciaDetalle>> GetEntradaMercanciaDetallesAsync(int entradaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM XEntradasDeMercanciaDetalle WHERE EntradaId = @EntradaId ORDER BY Ordenar";
            return (await connection.QueryAsync<XEntradaDeMercanciaDetalle>(sql, new { EntradaId = entradaId })).ToList();
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

            var parameters = new
            {
                entrada.EntradaId,
                entrada.BodegaId,
                entrada.CentroCostoId,
                entrada.FechaFactura,
                Observacion = entrada.Observacion ?? (object)DBNull.Value,
                entrada.EstadoMovimiento,
                entrada.EquipoId,
                entrada.UsuarioId,
                entrada.FechaDeSistema,
                entrada.TipoCompra,
                entrada.EquipoModificacionId,
                entrada.UsuarioModificacionId,
                entrada.FechaModificacion,
                entrada.ProveedorId,
                entrada.Factura,
                entrada.FechaVencimiento,
                OrdenId = entrada.OrdenId ?? (object)DBNull.Value,
                NombreVendedor = entrada.NombreVendedor ?? (object)DBNull.Value,
                entrada.MoverExistencias,
                entrada.TotalEntrada,
                entrada.EnsaEntradaId,
                entrada.EnsaSalidaId,
                entrada.EntradaNo,
                MotivoAnulacion = entrada.MotivoAnulacion ?? (object)DBNull.Value,
                entrada.RegimenId
            };

            var result = await connection.ExecuteAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> DeleteEntradaMercanciaAsync(int entradaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM XEntradasDeMercancia WHERE EntradaId = @EntradaId";
            var result = await connection.ExecuteAsync(sql, new { EntradaId = entradaId });
            return result > 0;
        }



        public async Task<List<ValidacionProducto>> ValidarProductosAsync(List<string> skus)
        {
            var resultados = new List<ValidacionProducto>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Query para buscar productos principales
                var queryProductos = @"
                    SELECT ProductoId, DescripcionLarga, Referencia
                    FROM Productos 
                    WHERE Referencia IN @Referencias";

                // Query para buscar productos alternos con información del producto principal
                var queryProductosAlternos = @"
                    SELECT pa.ProductoId, pa.CodigoAlterno, pa.Principal,
                           p.DescripcionLarga, p.Referencia
                    FROM ProductosAlternos pa
                    INNER JOIN Productos p ON pa.ProductoId = p.ProductoId
                    WHERE pa.CodigoAlterno IN @CodigosAlternos";

                var referencias = skus.ToArray();
                var codigosAlternos = skus.ToArray();

                var productos = await connection.QueryAsync<dynamic>(queryProductos, new { Referencias = referencias });
                var productosAlternos = await connection.QueryAsync<dynamic>(queryProductosAlternos, new { CodigosAlternos = codigosAlternos });

                // Crear diccionarios para búsqueda rápida
                var productosDict = productos.ToDictionary(p => (string)p.Referencia, p => p);
                var productosAlternosDict = productosAlternos.ToDictionary(p => (string)p.CodigoAlterno, p => p);

                foreach (var sku in skus)
                {
                    var validacion = new ValidacionProducto
                    {
                        SKU = sku,
                        ExisteEnBaseDeDatos = false
                    };

                    // Buscar primero en productos principales
                    if (productosDict.TryGetValue(sku, out var producto))
                    {
                        validacion.ExisteEnBaseDeDatos = true;
                        validacion.ProductoId = producto.ProductoId;
                        validacion.DescripcionProducto = producto.DescripcionLarga;
                        validacion.ReferenciaProducto = producto.Referencia;
                        validacion.EsCodigoAlterno = false;
                        validacion.EsPrincipal = true;
                    }
                    // Si no se encuentra, buscar en productos alternos
                    else if (productosAlternosDict.TryGetValue(sku, out var productoAlterno))
                    {
                        validacion.ExisteEnBaseDeDatos = true;
                        validacion.ProductoId = productoAlterno.ProductoId;
                        validacion.DescripcionProducto = productoAlterno.DescripcionLarga;
                        validacion.ReferenciaProducto = productoAlterno.Referencia;
                        validacion.EsCodigoAlterno = true;
                        validacion.EsPrincipal = productoAlterno.Principal;
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


    }
} 