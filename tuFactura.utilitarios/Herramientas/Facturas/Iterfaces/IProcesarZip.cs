namespace tuFactura.utilitarios.Herramientas.Facturas.Iterfaces
{
    public interface IProcesarZip
    {
        public List<byte[]> ExtraerBytesDeArchivosZip(byte[] contenidoArchivoZip);
    }
}
