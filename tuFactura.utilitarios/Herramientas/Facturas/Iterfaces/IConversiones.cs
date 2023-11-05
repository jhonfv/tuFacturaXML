using Microsoft.AspNetCore.Http;

namespace tuFactura.utilitarios.Herramientas.Facturas.Iterfaces
{
    public interface IConversiones
    {
        public byte[] ConvertirIFormFileABytes(IFormFile file);
    }
}
