using Microsoft.AspNetCore.Http;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;

namespace tuFactura.utilitarios.Herramientas.Facturas
{
    public class Conversiones: IConversiones
    {
        public byte[] ConvertirIFormFileABytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
