using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

namespace FacturasAxoft.Validaciones
{
    /// <summary>
    /// En esta clase implementarán todas las validaciones.
    /// Se da una validación ya implementada a modo de ejemplo.
    /// </summary>
    public class ValidadorFacturasAxoft
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;

        /// <summary>
        /// Instancia un Validador facturas
        /// </summary>
        /// <param name="clientes">Clientes preexistentes, ya grabados en la base de datos</param>
        /// <param name="articulos">Artículos preexistentes, ya grabados en la base de datos</param>
        /// <param name="facturas">Facturas preexistentes, ya grabadas en la base de datos</param></param>
        public ValidadorFacturasAxoft(List<Cliente> clientes, List<Articulo> articulos, List<Factura> facturas)
        {
            this.clientes = clientes;
            this.articulos = articulos;
            this.facturas = facturas;
        }


        /// <summary>
        /// Valida una nueva factura según las reglas de negocios requeridas. en caso de no cumplir una regla levanta la exception adecuada
        /// </summary>
        /// <param name="factura">Factura a validar</param>
        public void ValidarNuevaFactura(Factura factura)
        {
            ValidarNumeroInicial(factura);
            ValidarExistenciaFacturaAnterior(factura);
            ValidarFecha(factura);
            ValidarCuil(factura);
            ValidarClienteExistente(factura);
            ValidarArticuloExistente(factura);
            ValidarTotalesRenglones(factura);
            ValidarTotalSinImpuestos(factura);
            ValidarPorcentajeIva(factura);
            ValidarImporteIva(factura);
            ValidarTotalConImpuestos(factura);
        }

        /// <summary>
        /// Valida si la fecha de la factura es mayor que cualquier fecha existente en las facturas.
        /// </summary>
        /// <param name="factura">Factura a validar la fecha</param>
        /// <exception cref="FacturaConFechaInvalidaException">Se lanza si la fecha de la factura no cumple con las reglas de negocio.</exception>
        private void ValidarFecha(Factura factura)
        {
            if (facturas.Any(f => f.Fecha > factura.Fecha))
            {
                throw new FacturaConFechaInvalidaException();
            }
        }

        /// <summary>
        /// Valida que en caso de ser la primer factura a ser ingresada en la tabla el numero sea uno
        /// </summary>
        /// <param name="factura">Factura a validar el número</param>
        /// <exception cref="FaltaPrimerFacturaException">Se lanza si el número de factura no cumple con las reglas de negocio.</exception>
        private void ValidarNumeroInicial(Factura factura)
        {
            if (facturas.All(f => f.Numero != 1) && factura.Numero != 1)
            {
                throw new FaltaPrimerFacturaException();
            }
        }

        /// <summary>
        /// Valida la existencia de la factura anterior, a no ser que sea la factura numero uno.
        /// </summary>
        /// <param name="factura">Factura a validar la existencia de la factura anterior</param>
        /// <exception cref="FaltanFacturasPreviasException">Se lanza si no se encuentra la factura anterior.</exception>
        private void ValidarExistenciaFacturaAnterior(Factura factura)
        {
            int numeroAnterior = factura.Numero - 1;
            if (facturas.All(f => f.Numero != numeroAnterior) && numeroAnterior != 0)
            {
                throw new FaltanFacturasPreviasException();
            }
        }

        /// <summary>
        /// Valida el formato del CUIL del cliente en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el CUIL del cliente</param>
        /// <exception cref="CUIlInvalidoException">Se lanza si el CUIL del cliente no cumple con el formato requerido.</exception>
        private void ValidarCuil(Factura factura)
        {
            // En caso de ser un CUIL con guiones, la regex sería @"^\d{2}-\d{8}-\d{1}$"
            if (!Regex.IsMatch(factura.Cliente.Cuil, @"^\d{11}$"))
            {
                throw new CUIlInvalidoException();
            }
        }

        /// <summary>
        /// Valida la existencia y consistencia de datos para un cliente en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el cliente existente</param>
        /// <exception cref="ConflictoDatosClienteException">Se lanza si se detectan conflictos en los datos del cliente.</exception>
        private void ValidarClienteExistente(Factura factura)
        {
            Cliente cliente = factura.Cliente;
            Cliente? mismoCliente = clientes.FirstOrDefault(clienteAntiguo => clienteAntiguo.Cuil == cliente.Cuil);

            if (mismoCliente != null)
            {
                Factura? facturaVieja = facturas.FirstOrDefault(f => f.Cliente.Cuil == cliente.Cuil);
                if (mismoCliente.Nombre != cliente.Nombre || mismoCliente.Direccion != cliente.Direccion || (facturaVieja != null && facturaVieja.Iva != factura.Iva))
                {
                    throw new ConflictoDatosClienteException();
                }
            }
        }

        /// <summary>
        /// Valida la existencia y consistencia de datos para los artículos en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar los artículos existentes</param>
        /// <exception cref="ConflictoDatosArticuloException">Se lanza si se detectan conflictos en los datos de los artículos.</exception>
        private void ValidarArticuloExistente(Factura factura)
        {
            List<RenglonFactura> renglones = new List<RenglonFactura>(factura.Renglones);

            foreach (RenglonFactura rf in renglones)
            {
                Articulo articuloActual = rf.Articulo;
                Articulo? mismoArticulo = articulos.FirstOrDefault(a => a.Codigo == articuloActual.Codigo);

                if (mismoArticulo != null)
                {
                    if (mismoArticulo.Descripcion != articuloActual.Descripcion || mismoArticulo.Precio != articuloActual.Precio)
                    {
                        throw new ConflictoDatosArticuloException();
                    }
                }
            }
        }


        /// <summary>
        /// Valida los totales de los renglones en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar los totales de los renglones</param>
        /// <exception cref="TotalRenglonException">Se lanza si el total de un renglón no coincide con el precio por cantidad.</exception>
        private void ValidarTotalesRenglones(Factura factura)
        {
            List<RenglonFactura> renglones = new List<RenglonFactura>(factura.Renglones);

            foreach (RenglonFactura rf in renglones)
            {
                Articulo articuloActual = rf.Articulo;

                if (articuloActual.Precio * rf.Cantidad != rf.Total)
                {
                    throw new TotalRenglonException();
                }
            }
        }

        /// <summary>
        /// Valida el total sin impuestos en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el total sin impuestos</param>
        /// <exception cref="TotalSinImpuestoExcpetion">Se lanza si el total calculado de los renglones no coincide con el total sin impuestos declarado.</exception>
        private void ValidarTotalSinImpuestos(Factura factura)
        {
            List<RenglonFactura> renglones = new List<RenglonFactura>(factura.Renglones);
            double totalCalculado = renglones.Sum(rf => rf.Total);
            if (totalCalculado != factura.TotalSinImpuestos)
            {
                throw new TotalSinImpuestoExcpetion();
            }
        }

        /// <summary>
        /// Valida el porcentaje de IVA en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el porcentaje de IVA</param>
        /// <exception cref="PorcentajeIvaInvalidoException">Se lanza si el porcentaje de IVA no es uno de los valores permitidos.</exception>
        private void ValidarPorcentajeIva(Factura factura)
        {
            double Iva = factura.Iva;
            List<double> porcentajesPermitidos = new List<double> { 0.00, 10.50, 21.00, 27.00 };

            if (porcentajesPermitidos.All(d => d != Iva))
            {
        
                throw new PorcentajeIvaInvalidoException();
            }
        }

        /// <summary>
        /// Valida el importe de IVA en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el importe de IVA</param>
        /// <exception cref="ImporteIvaException">Se lanza si el importe de IVA calculado no coincide con el declarado en la factura.</exception>
        private void ValidarImporteIva(Factura factura)
        {
            double importeCalculado = (factura.Iva / 100) * factura.TotalSinImpuestos;

            if (importeCalculado != factura.ImporteIva)
            {
                throw new ImporteIvaException();
            }
        }

        /// <summary>
        /// Valida el total con impuestos en la factura.
        /// </summary>
        /// <param name="factura">Factura a validar el total con impuestos</param>
        /// <exception cref="TotalConImpuestosException">Se lanza si el total con impuestos declarado no coincide con la suma del importe adicional del Iva y el total sin impuestos.</exception>
        private void ValidarTotalConImpuestos(Factura factura)
        {
            double totalCalculado = factura.ImporteIva + factura.TotalSinImpuestos;

            if (factura.TotalConImpuestos != totalCalculado)
            {
                throw new TotalConImpuestosException();
            }
        }

    }
}
