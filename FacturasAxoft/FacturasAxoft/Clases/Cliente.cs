namespace FacturasAxoft.Clases
{
    /// <summary>
    /// Clase que representa a un cliente.
    /// </summary>
    public class Cliente
    {
        public int Id { get; set; }
        public string Cuil { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public ICollection<Factura> Facturas { get; set; }
    }
}
