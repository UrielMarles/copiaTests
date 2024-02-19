namespace FacturasAxoft.Clases
{
    /// <summary>
    /// Clase que representa a una factura.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    /// <summary>
    /// Clase que representa a una factura.
    /// </summary>
    public class Factura
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public ICollection<RenglonFactura> Renglones { get; set; }
        public double TotalSinImpuestos { get; set; }
        public double Iva { get; set; }
        public double ImporteIva { get; set; }
        public double TotalConImpuestos { get; set; }
    }

    /// <summary>
    /// Clase que representa el renglon de una factura.
    /// </summary>
    public class RenglonFactura
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public Factura Factura { get; set; }
        public int ArticuloId { get; set; }
        public Articulo Articulo { get; set; }
        public int Cantidad { get; set; }
        public double Total { get; set; }
    }
}
