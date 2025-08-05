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

            var sql = @"
                INSERT INTO XEntradasDeMercancia (
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

            return await connection.ExecuteScalarAsync<int>(sql, entrada);
        }

        public async Task<bool> InsertEntradaMercanciaDetalleAsync(XEntradaDeMercanciaDetalle detalle)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

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

            var result = await connection.ExecuteAsync(sql, detalle);
            return result > 0;
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

            var result = await connection.ExecuteAsync(sql, entrada);
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