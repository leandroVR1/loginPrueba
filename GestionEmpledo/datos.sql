-- Active: 1713482749915@@bd76xrvkuikebvb3u1ta-mysql.services.clever-cloud.com@3306@bd76xrvkuikebvb3u1ta
CREATE TABLE ROL(
    IdRol INT PRIMARY KEY AUTO_INCREMENT,
    Descripcion VARCHAR (50)
    
)

INSERT INTO ROL (`Descripcion`) VALUES
('Administrador'),
('Empleado');

ALTER TABLE Empleados
ADD COLUMN IdRol INT,
ADD CONSTRAINT FK_IdRol FOREIGN KEY (IdRol) REFERENCES ROL(IdRol);



ALTER TABLE Empleados
DROP COLUMN IdRol;
