using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tuFactura.utilitarios.Modelos.Factura;

namespace tuFactura.utilitarios.Herramientas.Database
{
    public interface IDatabaseService
    {
        Task<int> InsertEntradaMercanciaAsync(XEntradaDeMercancia entrada);
        Task<bool> InsertEntradaMercanciaDetalleAsync(XEntradaDeMercanciaDetalle detalle);
        Task<List<XEntradaDeMercancia>> GetEntradasMercanciaAsync();
        Task<XEntradaDeMercancia?> GetEntradaMercanciaByIdAsync(int entradaId);
        Task<List<XEntradaDeMercanciaDetalle>> GetEntradaMercanciaDetallesAsync(int entradaId);
        Task<bool> UpdateEntradaMercanciaAsync(XEntradaDeMercancia entrada);
        Task<bool> DeleteEntradaMercanciaAsync(int entradaId);
        
        // Métodos para validación de productos
        Task<List<ValidacionProducto>> ValidarProductosAsync(List<string> skus);
    }
} 