-- Inserción de datos en la tabla Clientes
INSERT INTO Clientes (Nombre, Cuil, Direccion)
VALUES 
    ('Cliente1', '12345678901', 'Direccion1'),
    ('Cliente2', '23456789012', 'Direccion2'),
    ('Cliente3', '34567890123', 'Direccion3'),
    ('Cliente4', '45678901234', 'Direccion4'),
    ('Cliente5', '56789012345', 'Direccion5');

-- Inserción de datos en la tabla Articulos
INSERT INTO Articulos (Precio, Codigo, Descripcion)
VALUES 
    (20.00, 'A001', 'Articulo1'),
    (30.00, 'A002', 'Articulo2'),
    (40.00, 'A003', 'Articulo3'),
    (50.00, 'A004', 'Articulo4'),
    (60.00, 'A005', 'Articulo5');

-- Inserción de datos en la tabla Facturas
INSERT INTO Facturas (Numero, Fecha, ClienteId, TotalSinImpuestos, Iva, ImporteIva, TotalConImpuestos)
VALUES 
    (1, '2024-01-01', 1, 130.00, 0.0, 0.0, 130.00),
    (2, '2024-01-02', 2, 30.00, 10.5, 3.15, 33.15),
    (3, '2024-01-03', 3, 190.00, 21, 39.90, 229.90),
    (4, '2024-01-04', 4, 100.00, 27, 27, 127),
    (5, '2024-01-05', 5, 180.00, 0, 0, 180);

-- Inserción de datos en la tabla RenglonesFactura
INSERT INTO RenglonesFactura (FacturaId, ArticuloId, Cantidad, Total)
VALUES 
    (1, 1, 2, 40.00),
    (1, 2, 3, 90.00),
    (2, 2, 1, 30.00),
    (3, 3, 4, 160.00),
	(3, 2, 1, 30.00),
    (4, 4, 2, 100.00),
    (5, 5, 3, 180.00);