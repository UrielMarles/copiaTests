using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FacturasAxoft.Tools
{
    public static class LlenarLista
    {
        /// <summary>
        /// Llena una lista de facturas con datos provenientes de un documento XML.
        /// </summary>
        /// <param name="facturas">Lista de facturas a llenar.</param>
        /// <param name="documentoXml">Documento XML que contiene la información de las facturas.</param>
        public static void LLenarListaConXml(this List<Factura> facturas, XmlDocument documentoXml)
        {
            //ITERO FACTURAS
            try
            {
                XmlNodeList facturasXml = documentoXml.SelectNodes("//factura");

                foreach (XmlNode facturaXml in facturasXml)
                {
                    // INSTANCIO NUEVA FACTURA
                    Factura nuevaFactura = new Factura();

                    // INFO CLIENTE
                    XmlNode clienteXml = facturaXml.SelectSingleNode("cliente");
                    string cuilCliente = clienteXml.GetS("CUIL");
                    string Direccion = clienteXml.GetS("direccion");
                    string Nombre = clienteXml.GetS("nombre");
                    Cliente clienteDeLaFactura = new Cliente
                    {
                        Cuil = cuilCliente,
                        Direccion = Direccion,
                        Nombre = Nombre
                    };
                    // ITERO RENGLONES
                    XmlNode nodoRenglones = facturaXml.SelectSingleNode("renglones");
                    List<RenglonFactura> renglones = new List<RenglonFactura>();
                    if (nodoRenglones != null)
                    {
                        XmlNodeList nodosRenglon = nodoRenglones.SelectNodes("renglon");
                        foreach (XmlNode nodoRenglon in nodosRenglon)
                        {
                            RenglonFactura nuevoRenglon = new RenglonFactura();
                            // INFO ARTICULO
                            string codigoArticulo = nodoRenglon.GetS("codigoArticulo");
                            string Descripcion = nodoRenglon.GetS("descripcion");
                            double Precio = nodoRenglon.GetD("precioUnitario");
                            Articulo articuloDelRenglon = new Articulo
                            {
                                Codigo = codigoArticulo,
                                Descripcion = Descripcion,
                                Precio = Precio
                            };
                            //INFO RENGLON
                            nuevoRenglon.Articulo = articuloDelRenglon;
                            nuevoRenglon.Factura = nuevaFactura;
                            nuevoRenglon.Cantidad = nodoRenglon.GetI("cantidad");
                            nuevoRenglon.Total = nodoRenglon.GetD("total");

                            // GUARDO RENGLON EN LA LISTA
                            renglones.Add(nuevoRenglon);
                        }
                    }
                    //INFO FACTURA
                    nuevaFactura.Numero = facturaXml.GetI("numero");
                    nuevaFactura.Fecha = Convert.ToDateTime(facturaXml.GetS("fecha"));
                    nuevaFactura.Iva = facturaXml.GetD("iva");
                    nuevaFactura.TotalSinImpuestos = facturaXml.GetD("totalSinImpuestos");
                    nuevaFactura.TotalConImpuestos = facturaXml.GetD("totalConImpuestos");
                    nuevaFactura.ImporteIva = facturaXml.GetD("importeIva");
                    nuevaFactura.Renglones = renglones;
                    nuevaFactura.Cliente = clienteDeLaFactura;
                    facturas.Add(nuevaFactura);
                }
            }
            catch (Exception ex)
            {
                throw new XmlFormatException(ex);
            }

        }

        /// <summary>
        /// Obtiene el valor de un nodo XML como cadena.
        /// </summary>
        /// <param name="nodo">Nodo XML del cual se extraerá la cadena.</param>
        /// <param name="llave">Clave que identifica el valor dentro del nodo XML.</param>
        /// <returns>Valor del nodo como cadena.</returns>
        public static string GetS(this XmlNode nodo, string llave)
        {
            return nodo.SelectSingleNode(llave).InnerText;
        }
        /// <summary>
        /// Obtiene el valor de un nodo XML como entero.
        /// </summary>
        /// <param name="nodo">Nodo XML del cual se extraerá el entero.</param>
        /// <param name="llave">Clave que identifica el valor dentro del nodo XML.</param>
        /// <returns>Valor del nodo como entero.</returns>
        public static int GetI(this XmlNode nodo, string llave)
        {
            return Convert.ToInt32(nodo.GetS(llave));
        }

        /// <summary>
        /// Obtiene el valor de un nodo XML como número de punto flotante (double).
        /// </summary>
        /// <param name="nodo">Nodo XML del cual se extraerá el número de punto flotante.</param>
        /// <param name="llave">Clave que identifica el valor dentro del nodo XML.</param>
        /// <returns>Valor del nodo como double.</returns>
        public static double GetD(this XmlNode nodo, string llave)
        {
            return double.Parse(nodo.GetS(llave),CultureInfo.InvariantCulture);
        }
    }

}
