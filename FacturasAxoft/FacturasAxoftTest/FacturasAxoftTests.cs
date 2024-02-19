using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Validaciones;
using Xunit;
using Xunit.Sdk;

namespace FacturasAxoftTest
{
    [TestClass]
    public class FacturasAxoftTests
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;
        private readonly ValidadorFacturasAxoft validador;

        public FacturasAxoftTests()
        {
            clientes = new List<Cliente>();
            articulos = new List<Articulo>();
            facturas = new List<Factura>();

            validador = new ValidadorFacturasAxoft(clientes, articulos, facturas);
        }
        /// <summary>
        /// La primer factura a ingresar, con número 1 es válida.
        /// </summary>
        [TestMethod]
        public void PimerFacturaEsValida()
        {
            // No tengo facturas preexistentes

            // La primer factura que voy a agregar tiene el número 1
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };

            // La factura es válida, no tiene que tirar la excepción.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        /// <summary>
        /// La primer factura a ingresar, con número 2 es válida.
        /// </summary>
        [TestMethod]
        public void SegundaFacturaEsValida()
        {
            // Tengo preexistente una factura número 1 con fecha uno de enero
            facturas.Add(new()
                {
                    Numero = 1,
                    Fecha = new DateTime(2020, 1, 1),
                    Cliente = new Cliente
                    {
                        Cuil = "20123456781",
                        Direccion = "Calle falsa 123",
                        Nombre = "Juan"
                    },
                    Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                Codigo = "ART01",
                                Descripcion = "artículo cero uno",
                                Precio = 25
                            },
                            Cantidad = 2,
                            Total = 50
                        }
                    },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            }
            );

            // Tengo una nueva factura nro dos con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };

            // La factura es válida, no tiene que tirar la excepción.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        /// <summary>
        /// Este test verifica si tengo una factura con número 1 y fecha 2 de enero no pueda ingregar la factura nro 2 con fecha
        /// 1 de enero porque no estaría respetando el órden cronológico.
        /// </summary>
        [TestMethod]
        public void FacturaConFechaInvalida()
        {
            // Tengo una factura número 1 con fecha dos de enero
            facturas.Add(new()
                {
                    Numero = 1,
                    Fecha = new DateTime(2020, 1, 2),
                    Cliente = new Cliente
                    {
                        Cuil = "20123456781",
                        Direccion = "Calle falsa 123",
                        Nombre = "Juan"
                    },
                    Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                Codigo = "ART01",
                                Descripcion = "artículo cero uno",
                                Precio = 25
                            },
                            Cantidad = 2,
                            Total = 50
                        }
                    },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            }
            );

            // Voy a querer ageegar la factura número 2 con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };

            // Al validar la nueva factura salta una excepción tipada, y con el mensaje de error correspondiente.
            Assert.ThrowsException<FacturaConFechaInvalidaException>( () => validador.ValidarNuevaFactura(factura),
                "La fecha de la factura es inválida. Existen facturas grabadas con número inferior y fecha posterior a la ingresada.");
        }

        /// <summary>
        /// Este test verifica que no se pueda agregar como primer factura una factura sin el numero 1
        /// </summary>
        [TestMethod]
        public void FacturasNoComienzanEnUno()
        {
            //creo una factura con numero 23
            Factura factura = new()
            {
                Numero = 23,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            // tiene que tirar error porque la lista esta vacia y la primer factura ingresada no tiene el numero uno
            Assert.ThrowsException<FaltaPrimerFacturaException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// Este test verifica que al intentar cargar una factura con un numero sin que se encuentre su numero anterior ya cargado, surge un error
        /// </summary>
        [TestMethod]
        public void NoExisteFacturaAnterior()
        {
            // Tengo preexistente una factura número 1 
            facturas.Add(new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                    {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                    },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            }
            );
            // Tengo una nueva factura que va a tirar error porque falta la factura 2
            Factura factura = new()
            {
                Numero = 3,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<FaltanFacturasPreviasException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// Este test verifica que al intentar cargar una factura con un CUIL invalido  surge un error,
        /// el cuil es invalido si no es una cadena de 11 digitos sin espacios ni simbolos
        /// </summary>
        [TestMethod]
        public void CuilInvalido()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "cuil malo",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<CUIlInvalidoException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// Este test verifica que si un mismo cliente aparece en dos facturas pero con distinta informacion, ya sea cuil, direccion
        /// IVA o nombre de la factura, surge una exception
        /// </summary>
        [TestMethod]
        public void ConflictoDatosCliente()
        {
            // Añado el cliente a la base de datos
            clientes.Add(new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                }
            );
            // En esta factura el cliente es el mismo pero el nombre pasa de ser JUAN a JUANCHO
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "JuanCho"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<ConflictoDatosClienteException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// Este test verifica que si un mismo articulo aparece en dos facturas pero con distinta informacion, ya sea precio o descripcion
        /// se tira una exception
        /// </summary>
        [TestMethod]
        public void ConflictoDatosArticulo()
        {
            // Añado el articulo a la base de datos
            articulos.Add(new Articulo
            {
                Codigo = "ART01",
                Descripcion = "artículo cero uno",
                Precio = 10
            }
            );
            // En esta factura el segundo articulo es el mismo pero el precio aumenta quince pesos
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "SAPE"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<ConflictoDatosArticuloException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// En esta factura el total del renglon no coincide con el precio del producto y su cantidad, por lo tanto se espera un error
        /// 25 * 2 = 50
        /// 50 == 51 -> False
        /// </summary>
        [TestMethod]
        public void ErrorEnTotalDelRenglon()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 51
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            //Se espera error
            Assert.ThrowsException<TotalRenglonException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// En esta factura el totalSinImpuestos no coincide con la suma de los totales de los renglones, se espera error
        /// 50 + 60 = 110
        /// 110 == 111 -> False
        /// </summary>
        [TestMethod]
        public void ErrorEnTotalSinImpuestos()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    },new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART02",
                            Descripcion = "artículo cero dos",
                            Precio = 30
                        },
                        Cantidad = 2,
                        Total = 60
                    }
                },
                TotalConImpuestos = 133.1,
                TotalSinImpuestos = 111,
                Iva = 21.0,
                ImporteIva = 23.1,
            };
            //Se espera error
            Assert.ThrowsException<TotalSinImpuestoExcpetion>(() => validador.ValidarNuevaFactura(factura));
        }


        /// <summary>
        /// En esta factura el porcentaje no es 0,10.5,21 ni 27. Por lo tanto tiene que tirar error
        /// </summary>
        [TestMethod]
        public void PorcentajeIvaNoValido()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.1,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<PorcentajeIvaInvalidoException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// En este test el Importe del iva esta mal calculado
        /// El importe correcto es 10.5 (50 * 0.21) por lo tanto tiene que tirar error
        /// </summary>
        [TestMethod]
        public void ImporteIvaNoValido()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 60.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 11.5,
            };
            Assert.ThrowsException<ImporteIvaException>(() => validador.ValidarNuevaFactura(factura));
        }

        /// <summary>
        /// En este test el total con impuestos esta mal calculado
        /// El importe correcto es 60.5 (50 + 10.5) por lo tanto tiene que tirar error
        /// </summary>
        [TestMethod]
        public void TotalConImpuestosNoValido()
        {
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "artículo cero uno",
                            Precio = 25
                        },
                        Cantidad = 2,
                        Total = 50
                    }
                },
                TotalConImpuestos = 61.5,
                TotalSinImpuestos = 50,
                Iva = 21.0,
                ImporteIva = 10.5,
            };
            Assert.ThrowsException<TotalConImpuestosException>(() => validador.ValidarNuevaFactura(factura));
        }
    }
}