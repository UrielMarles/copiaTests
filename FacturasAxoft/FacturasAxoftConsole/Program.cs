// See https://aka.ms/new-console-template for more information
Console.WriteLine("Inicio: Facturas Axoft");

string connectionString = args[0];
Console.WriteLine($"connectionString: {connectionString}");
FacturasAxoft.FacturasAxoft facturasAxoft = new(connectionString);

string metodo = args[1];
Console.WriteLine($"metodo: {metodo}");
string result = "OK";
switch (metodo)
{
    case "CargarFacturas":
        string path = args[2];
        Console.WriteLine($"path: {path}");
        facturasAxoft.CargarFacturas(path);
        break;
    case "Get3ArticulosMasVendidos":
        result = facturasAxoft.Get3ArticulosMasVendidos();
        break;
    case "Get3Compradores":
        result = facturasAxoft.Get3Compradores();
        break;
    case "GetPromedioYArticuloMasCompradoDeCliente":
        string cuil = args[2];
        Console.WriteLine($"cuil: {cuil}");
        result = facturasAxoft.GetPromedioYArticuloMasCompradoDeCliente(cuil);
        break;
    case "GetTotalYPromedioFacturadoPorFecha":
        string fecha = args[2];
        Console.WriteLine($"fecha: {fecha}");
        result = facturasAxoft.GetTotalYPromedioFacturadoPorFecha(fecha);
        break;
    case "GetTop3ClientesDeArticulo":
        string codigoArticulo = args[2];
        Console.WriteLine($"codigoArticulo: {codigoArticulo}");
        result = facturasAxoft.GetTop3ClientesDeArticulo(codigoArticulo);
        break;
    case "GetTotalIva":
        string fechaDesde = args[2];
        string fechaHasta = args[3];
        Console.WriteLine($"fechaDesde: {fechaDesde}");
        Console.WriteLine($"fechaHasta: {fechaHasta}");
        result = facturasAxoft.GetTotalIva(fechaDesde, fechaHasta);
        break;
    default:
        break;
}
Console.WriteLine($"result: {result}");
Console.WriteLine("Fin: Facturas Axoft");