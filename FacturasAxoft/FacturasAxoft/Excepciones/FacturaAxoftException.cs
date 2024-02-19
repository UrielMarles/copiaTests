namespace FacturasAxoft.Excepciones
{
    public class FacturaAxoftException : Exception
    {
        public FacturaAxoftException(string message) : this(message,null)
        {
        }

        public FacturaAxoftException(string message, Exception ex) : base(message,ex)
        {
        }
    }

    public class FacturaConFechaInvalidaException : FacturaAxoftException
    {
        public FacturaConFechaInvalidaException() :
            base("La fecha de la factura es inválida. Existen facturas grabadas con fecha posterior a la ingresada.")
        {
        }
    }

    public class RutaIncorrectaException : FacturaAxoftException
    {
        public RutaIncorrectaException() : base("La ruta ingresada no es valida")
        {
        }
    }

    public class XmlFormatException : FacturaAxoftException
    {
        public XmlFormatException(Exception inner) : base("La estructura del XML o los tipos de datos son incorrectos",inner)
        {
        }
    }

    public class FaltaPrimerFacturaException : FacturaAxoftException
    {
        public FaltaPrimerFacturaException() :
            base("Aun no se ingreso una factura con Numero Uno")
        {
        }
    }

    public class FaltanFacturasPreviasException : FacturaAxoftException
    {
        public FaltanFacturasPreviasException() :
            base("Una factura ingresada tiene un numero mayor al de la ultima factura ingresada")
        {
        }
    }
    public class CUIlInvalidoException : FacturaAxoftException
    {
        public CUIlInvalidoException() :
            base("El CUIL del cliente es invalido, el formato correcto es XX-XXXXXXXX-X.")
        {
        }
    }

    public class ConflictoDatosClienteException : FacturaAxoftException
    {
        public ConflictoDatosClienteException() :
            base("El cliente tiene informacion distinta al cliente ya existente ")
        {
        }
    }

    public class ConflictoDatosArticuloException : FacturaAxoftException
    {
        public ConflictoDatosArticuloException() :
            base("El articulo contiene informacion distinta al articulo ya ingresado con el mismo codigo")
        {
        }
    }

    public class TotalRenglonException : FacturaAxoftException
    {
        public TotalRenglonException() :
            base("El total calculado en el renglon es incorrecto")
        {
        }
    }
    public class TotalSinImpuestoExcpetion : FacturaAxoftException
    {
        public TotalSinImpuestoExcpetion() :
            base("El total sin impuesto de la factura es incorrecto")
        {
        }
    }
    public class PorcentajeIvaInvalidoException : FacturaAxoftException
    {
        public PorcentajeIvaInvalidoException() :
            base("El porcentaje de IVA puede ser 0, 10.5, 21 y 27")
        {
        }
    }

    public class ImporteIvaException : FacturaAxoftException
    {
        public ImporteIvaException() :
            base("El importe de IVA calculado no coincide con el porcentaje")
        {
        }
    }

    public class TotalConImpuestosException : FacturaAxoftException
    {
        public TotalConImpuestosException() :
            base("El total Con Impuestos no coincide con la suma del total y el importe del iva")
        {
        }
    }

    public class ClienteInexistenteException : FacturaAxoftException
    {
        public ClienteInexistenteException() :
            base("No existe un cliente con el CUIL indicado")
        {
        }
    }

    public class ClienteSinComprasException : FacturaAxoftException
    {
        public ClienteSinComprasException() :
            base("No se puede calcular el promedio de gasto por compra porque aun no tiene compras el cliente")
        {
        }
    }

    public class FechaMalFormateadaException : FacturaAxoftException
    {
        public FechaMalFormateadaException() :
            base("Las fechas tienen que tener el formato dd/MM/yyyy")
        {
        }
    }
    public class FechaSinComprasException : FacturaAxoftException
    {
        public FechaSinComprasException() :
            base("La fecha elegida no tienen ninguna compra, no se puede mostrar su informacion")
        {
        }
    }

    public class ArticuloSinClientesException : FacturaAxoftException
    {
        public ArticuloSinClientesException() :
            base("El articulo aun no fue comprado por tres clientes")
        {
        }
    }

    public class CodigoArticuloException : FacturaAxoftException
    {
        public CodigoArticuloException() :
            base("No existe un articulo con el codigo ingresado")
        {
        }
    }
}
