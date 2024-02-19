using FacturasAxoft.Clases;
using System.Xml;
using System;
using System.IO;
using FacturasAxoft.Validaciones;
using FacturasAxoft.Excepciones;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Globalization;
using System.Linq.Expressions;
using Newtonsoft.Json;
using FacturasAxoft.Tools;
namespace FacturasAxoft
{
    public class FacturasAxoft
    {
        private readonly string connectionString;
        private readonly FacturasAxoftDbContext _context;
        /// <summary>
        /// Instancia un FacturasAxoft que usaremos como fachada de la aplicación.
        /// </summary>
        /// <param name="conectionString">Datos necesarios para conectarse a la base de datos</param>
        /// <exception>Debe tirar una excepción con mensaje de error correspondiente en caso de no poder conectar a la base de datos</exception>
        public FacturasAxoft(string connectionString)
        {
            this.connectionString = connectionString;
            this._context = new FacturasAxoftDbContext(connectionString);
        }

        /// <summary>
        /// Lee las facturas desde el archivo XML y las graba en la base de datos.
        /// Da de alta los clientes o los artículos que lleguen en el xml y no estén en la base de datos.
        /// </summary>
        /// <param name="path">Ubicación del archivo xml que contiene las facturas</param>
        /// <exception>Si no se puede acceder al archivo, no es un xml válido, o no cumple con las reglas de negocio, 
        /// devuelve una excepción con el mensaje de error correspondiente</exception>/// 
        public void CargarFacturas(string path)
        {
            //VALIDO RUTA HASTA EL ARCHIVO
            XmlDocument documentoXml = new XmlDocument();
            if (!File.Exists(path)){
                throw new RutaIncorrectaException();
            }

            //VALIDO QUE SEA UN ARCHIVO XML CON FORMATO VALIDO
            try{
                documentoXml.Load(path);
            }catch (Exception ex) 
            {
                throw new XmlFormatException(ex);
            }

            //CARGO LA INFO DEL XML A UNA LISTA DE FACTURAS
            List<Factura> nuevasFacturas = new List<Factura>();

            //NOTA -> DESERIALIZE EL XML A MANO (SE QUE ES FEO) SUELO TRABAJAR CON JSON Y PENSE QUE SERIA MAS SENCILLO 
            nuevasFacturas.LLenarListaConXml(documentoXml);


            //Itero Las facturas y si es valida la agrego
            foreach(Factura validando in nuevasFacturas)
            {
                ValidadorFacturasAxoft validador = new ValidadorFacturasAxoft(_context.Clientes.ToList(), _context.Articulos.ToList(), _context.Facturas.ToList());
                validador.ValidarNuevaFactura(validando);
                _context.Facturas.Add(validando);
                _context.SaveChanges();
            }

        }

        /// <summary>
        /// Obtiene los 3 artículos mas vendidos
        /// </summary>
        /// <returns>JSON con los 3 artículos mas vendidos</returns>
        /// <exception>Nunca devuelve excepción, en caso de no existir 3 artículos vendidos devolver los que existan, en caso de
        /// tener artículos con la misma cantidad de ventas devolver cualquiera</exception>
        public string Get3ArticulosMasVendidos()
        {
            List<Articulo> topArticulosMasVendidos = _context.Articulos
                .OrderByDescending(a => a.Renglones.Sum(r => r.Cantidad))
                .Take(3)
                .ToList();

            return JsonConvert.SerializeObject(topArticulosMasVendidos, Newtonsoft.Json.Formatting.Indented); 
        }

        /// <summary>
        /// Obtiene los 3 clientes que mas compraron
        /// </summary>
        /// <returns>JSON con los 3 clientes que mas compraron</returns>
        /// <exception>Mismo criterio que para artículos</exception>
        public string Get3Compradores()
        {
            List<Cliente> topCompradores = _context.Clientes
                .OrderByDescending(c => c.Facturas.SelectMany(f => f.Renglones).Sum(p => p.Cantidad))
                .Take(3)
                .ToList();

            return JsonConvert.SerializeObject(topCompradores, Newtonsoft.Json.Formatting.Indented); 
        }

        /// <summary>
        /// Devuelve el promedio de facturas y el artículo que mas compro.
        /// </summary>
        /// <param name="cuil"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si no existe el cliente, o si no tiene compras devolver un mensaje de error con el mensaje correspondiente</exception>
        public string GetPromedioYArticuloMasCompradoDeCliente(string cuil)
        {
            if (_context.Clientes.Count(c=> c.Cuil == cuil) < 1) 
            {
                throw new ClienteInexistenteException();
            }
            
            Articulo? articuloMasComprado = _context.Clientes
                .Where(c => c.Cuil == cuil) // eligo el cliente indicado
                .SelectMany(c => c.Facturas.SelectMany(f => f.Renglones)) // eligo todas las compras (renglones) del cliente
                .GroupBy(r => r.Articulo) //junto todas las compras del mismo producto
                .OrderByDescending(g => g.Sum(p => p.Cantidad)) //en cada grupo sumo el campo cantidad de cada renglon y ordeno descendiendo
                .Select(g => g.Key) // agarro el unico campo unico, es decir el campo que use para agrupar, el producto
                .FirstOrDefault();

            int totalFacturas = _context.Clientes
                .Where(c => c.Cuil == cuil)
                .Select(c => c.Facturas.Count)
                .FirstOrDefault();

            if (totalFacturas == 0) 
            {
                throw new ClienteSinComprasException();
            }
            double totalGastoCliente = _context.Clientes
                .Where(c => c.Cuil == cuil)
                .SelectMany(c => c.Facturas)
                .Sum(F => F.TotalConImpuestos);

            var json = new
            {
                promedioDineroGastadoPorFactura = (totalGastoCliente/totalFacturas) ,
                ArticuloMasComprado = articuloMasComprado
            };

            return JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Devuelve el total y promedio facturado para una fecha.
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si el dato de fecha ingresado no es válido, o si no existen facturas para la fecha dada,
        /// mostrar el mensaje de error correspondiente</exception>
        public string GetTotalYPromedioFacturadoPorFecha(string fecha)
        {
            DateTime fechaConvertida;
            if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaConvertida))
            {
                throw new FechaMalFormateadaException();
            }

            int totalFacturasFecha = _context.Facturas.Where(F => F.Fecha == fechaConvertida).Count();

            if(totalFacturasFecha == 0)
            {
                throw new FechaSinComprasException();
            }

            double totalFacturadoEnFecha = _context.Facturas.Where(F => F.Fecha == fechaConvertida).Sum(F => F.TotalSinImpuestos);

            var json = new
            {
                totalFacturadoEnFecha = totalFacturadoEnFecha,
                promedioFacturadoEnFecha = (totalFacturadoEnFecha / totalFacturasFecha),
            };

            return JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Devuelve los 3 clientes que mas compraron el artículo
        /// </summary>
        /// <param name="codigoArticulo"></param>
        /// <returns>JSON con los datos pedidos</returns>
        /// <exception>Si el artículo no existe, o no fue comprado por al menos 3 clientes devolver un mensaje de error correspondiente</exception>
        public string GetTop3ClientesDeArticulo(string codigoArticulo)
        {
            int articulosCodigo = _context.Articulos.Where(a => a.Codigo == codigoArticulo).Count();
            if (articulosCodigo == 0)
            {
                throw new CodigoArticuloException(); 
            }


            List<Cliente> clientesTop3 = _context.Facturas
                .SelectMany(factura => factura.Renglones) // tenemos una lista de todos los renglones (articulo + cantidad)
                .Where(renglon => renglon.Articulo.Codigo == codigoArticulo) // reducimos la lista de renglones a los renglones que tengan el articulo que queremos
                .GroupBy(renglon => renglon.Factura.Cliente) // juntamos todos los renglones que vengan de una factura que tenga el mismo cliente
                .OrderByDescending(g => g.Sum(renglon => renglon.Cantidad)) // sumamos las cantidades de cada grupito renglon de cada usuario
                .Take(3) //agarramos los 3 primeros grupitos
                .Select(g => g.Key) // sacamos las llaves de esos grupitos, es decir la unica cosa unica que tienen, el cliente por el cual armamos el grupo
                .ToList();

            if (clientesTop3.Count() < 3) 
            {
                throw new ArticuloSinClientesException();
            }

            return JsonConvert.SerializeObject(clientesTop3, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Devuelve el total de IVA de las facturas generadas desde la fechaDesde hasta la fechaHasta, ambas inclusive.
        /// </summary>
        /// <returns>JSON con el dato requerido</returns>
        /// <exception>Si no existen facturas para las fechas ingresadas mostrar un mensaje de error</exception>
        public string GetTotalIva(string fechaDesde, string fechaHasta)
        {
            DateTime desde;
            DateTime hasta;
            if (!DateTime.TryParseExact(fechaDesde, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out desde) || !DateTime.TryParseExact(fechaHasta, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out hasta))
            {
                throw new FechaMalFormateadaException();
            }


            if (_context.Facturas.Where(f => f.Fecha >= desde && f.Fecha <= hasta).Count() == 0)
            {
                throw new FechaSinComprasException();
            }

            double TotalIva = _context.Facturas.Where(f => f.Fecha >= desde && f.Fecha <= hasta).Sum(f => f.ImporteIva);

            return JsonConvert.SerializeObject(TotalIva, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
