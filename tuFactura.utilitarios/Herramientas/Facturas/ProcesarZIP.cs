using System.IO.Compression;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;

namespace tuFactura.utilitarios.Herramientas.Facturas
{
    public class ProcesarZIP : IProcesarZip
    {

        public List<byte[]> ExtraerBytesDeArchivosZip(byte[] contenidoArchivoZip)
        {
            var bytesArchivosExtraidos = new List<byte[]>();

            using (var memoryStream = new MemoryStream(contenidoArchivoZip))
            using (var zipArchive = new ZipArchive(memoryStream))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        using (var entryStream = entry.Open())
                        using (var ms = new MemoryStream())
                        {
                            entryStream.CopyTo(ms);
                            bytesArchivosExtraidos.Add(ms.ToArray());
                        }
                    }
                }
            }

            return bytesArchivosExtraidos;
        }

    }
}
