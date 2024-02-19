DROP TABLE RenglonesFactura
DROP TABLE Facturas
DROP TABLE Clientes
DROP TABLE Articulos

CREATE TABLE Clientes (
    Id INT PRIMARY KEY IDENTITY,
    Nombre VARCHAR(255),
	Cuil VARCHAR(255),
	Direccion VARCHAR(255)
);

CREATE TABLE Articulos (
    Id INT PRIMARY KEY IDENTITY,
	Precio FLOAT,
    Codigo VARCHAR(255),
    Descripcion VARCHAR(255)
    
);

CREATE TABLE Facturas (
    Id INT PRIMARY KEY IDENTITY,
    Numero INT,
    Fecha DATETIME,
    ClienteId INT,
    TotalSinImpuestos FLOAT,
    Iva FLOAT,
    ImporteIva FLOAT,
    TotalConImpuestos FLOAT,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE RenglonesFactura (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT,
    ArticuloId INT,
    Cantidad INT,
    Total FLOAT,
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    FOREIGN KEY (ArticuloId) REFERENCES Articulos(Id)
);
