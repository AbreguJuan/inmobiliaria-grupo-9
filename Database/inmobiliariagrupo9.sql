-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: inmobiliariagrupo9
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `imageninmueble`
--

DROP TABLE IF EXISTS `imageninmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `imageninmueble` (
  `ID_Imagen` int NOT NULL AUTO_INCREMENT,
  `ID_Inmueble` int NOT NULL,
  `Url` varchar(255) NOT NULL,
  PRIMARY KEY (`ID_Imagen`),
  KEY `ID_Inmueble_idx` (`ID_Inmueble`),
  CONSTRAINT `FK_ImagenInmueble_Inmueble` FOREIGN KEY (`ID_Inmueble`) REFERENCES `inmueble` (`ID_Inmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `imageninmueble`
--

LOCK TABLES `imageninmueble` WRITE;
/*!40000 ALTER TABLE `imageninmueble` DISABLE KEYS */;
INSERT INTO `imageninmueble` VALUES (3,8,'/images/inmuebles/06b22843-fd9f-4f9a-88bf-389a417ba649.webp'),(4,8,'/images/inmuebles/fca27c6e-21e6-4945-b61d-c90992ba1092.jpg'),(5,8,'/images/inmuebles/a87528dd-cc12-45aa-a663-f898a613538a.jpg');
/*!40000 ALTER TABLE `imageninmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmueble`
--

DROP TABLE IF EXISTS `inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmueble` (
  `ID_Inmueble` int NOT NULL AUTO_INCREMENT,
  `ID_TipoInmueble` int NOT NULL,
  `Provincia` varchar(45) NOT NULL,
  `Localidad` varchar(45) NOT NULL,
  `Direccion` varchar(45) NOT NULL,
  `PrecioXDia` double NOT NULL,
  `PorcentajeReserva` decimal(5,2) NOT NULL DEFAULT '30.00',
  `Metros_Cuadrados` int NOT NULL,
  `Nro_Ambientes` int NOT NULL,
  `Nro_Banios` int NOT NULL,
  `ID_Propietario` int NOT NULL,
  `Habilitado` tinyint NOT NULL,
  `FotoPortada` varchar(255) DEFAULT NULL,
  `Cupo` int NOT NULL DEFAULT '1',
  `Latitud` decimal(10,7) DEFAULT NULL,
  `Longitud` decimal(10,7) DEFAULT NULL,
  PRIMARY KEY (`ID_Inmueble`),
  KEY `ID_Propietario_idx` (`ID_Propietario`),
  KEY `ID_TipoInmueble_idx` (`ID_TipoInmueble`),
  CONSTRAINT `ID_Propietario` FOREIGN KEY (`ID_Propietario`) REFERENCES `propietario` (`ID_Propietario`),
  CONSTRAINT `ID_TipoInmueble` FOREIGN KEY (`ID_TipoInmueble`) REFERENCES `tipo_inmueble` (`ID_TipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmueble`
--

LOCK TABLES `inmueble` WRITE;
/*!40000 ALTER TABLE `inmueble` DISABLE KEYS */;
INSERT INTO `inmueble` VALUES (1,2,'Buenos Aires','Palermo','Av. Santa Fe 1234',5000,30.00,45,2,1,1,1,NULL,-1,-40.5000000,-50.5100000),(2,3,'San Luis','Merlo','Av. Siempre Saa',10000,30.00,30,2,1,6,1,NULL,2,80.0000000,-40.0000000),(3,1,'Santiago del Estero','Capital','Av. Juan Domingo Peron',6000,30.00,26,2,1,6,0,NULL,2,40.0000000,-40.0000000),(4,1,'Tucuman','Tucuman','Cordoba 256',7000,30.00,30,3,1,3,1,NULL,6,85.0000000,70.0000000),(8,1,'La Pampa','Santa Rosa','Francia 123',7000,30.00,45,2,1,6,0,'/images/inmuebles/eb43844d-75f1-44a9-8fbf-ca0d3ef174cc.jpg',4,-30.0000000,-50.0000000);
/*!40000 ALTER TABLE `inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilino`
--

DROP TABLE IF EXISTS `inquilino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilino` (
  `ID_Inquilino` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(45) NOT NULL,
  `Apellido` varchar(45) NOT NULL,
  `DNI` varchar(45) NOT NULL,
  `Telefono` varchar(45) NOT NULL,
  `Email` varchar(45) NOT NULL,
  PRIMARY KEY (`ID_Inquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilino`
--

LOCK TABLES `inquilino` WRITE;
/*!40000 ALTER TABLE `inquilino` DISABLE KEYS */;
INSERT INTO `inquilino` VALUES (1,'Pedro','Montenegro','25784235','3254698745','montenegropedro@mail.com'),(2,'Maria','Antonieta','25486325','3241794648','antonietamaria@mail.com'),(3,'Milagros','Alfaro','45765345','2664037410','milagrosalfaro225@gmail.com'),(4,'das','dsa','dsa','dsa','dsa@dsa.com'),(5,'Maria','Del Carmen','58463254','54785239954','delcarmenmaria@mail.com');
/*!40000 ALTER TABLE `inquilino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pago`
--

DROP TABLE IF EXISTS `pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pago` (
  `IdPago` int NOT NULL AUTO_INCREMENT,
  `IdReserva` int NOT NULL,
  `Concepto` varchar(100) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `Importe` decimal(10,2) NOT NULL,
  `Anulado` tinyint(1) NOT NULL DEFAULT '0',
  `CreadoPor` int DEFAULT NULL,
  `AnuladoPor` int DEFAULT NULL,
  PRIMARY KEY (`IdPago`),
  KEY `FK_Pago_Reserva` (`IdReserva`),
  KEY `FK_Pago_CreadoPor` (`CreadoPor`),
  KEY `FK_Pago_AnuladoPor` (`AnuladoPor`),
  CONSTRAINT `FK_Pago_AnuladoPor` FOREIGN KEY (`AnuladoPor`) REFERENCES `usuario` (`ID_Usuario`),
  CONSTRAINT `FK_Pago_CreadoPor` FOREIGN KEY (`CreadoPor`) REFERENCES `usuario` (`ID_Usuario`),
  CONSTRAINT `FK_Pago_Reserva` FOREIGN KEY (`IdReserva`) REFERENCES `reserva` (`ID_Reserva`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pago`
--

LOCK TABLES `pago` WRITE;
/*!40000 ALTER TABLE `pago` DISABLE KEYS */;
INSERT INTO `pago` VALUES (1,1,'Seña de reserva','2026-09-14 21:52:54',1000.00,0,NULL,NULL),(2,8,'prueba','2026-09-17 15:52:59',100000.00,1,NULL,1),(3,6,'Multa por finalización anticipada','2026-09-17 16:51:28',6000.00,0,NULL,NULL),(4,9,'Seña de reserva (30%)','2026-09-17 21:01:21',4200.00,0,1,NULL),(5,9,'Resto del pago','2026-09-17 21:02:12',9800.00,0,1,NULL),(6,10,'Seña de reserva (30%)','2026-09-17 21:03:14',8400.00,0,1,NULL),(7,10,'no resta el resto','2026-09-17 21:12:46',19600.00,1,1,1),(8,10,'Multa por finalización anticipada','2026-09-17 21:17:29',10500.00,0,1,NULL);
/*!40000 ALTER TABLE `pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietario`
--

DROP TABLE IF EXISTS `propietario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietario` (
  `ID_Propietario` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(45) NOT NULL,
  `Apellido` varchar(45) NOT NULL,
  `DNI` varchar(45) NOT NULL,
  `Telefono` varchar(45) NOT NULL,
  `Email` varchar(45) NOT NULL,
  `Clave` varchar(45) NOT NULL,
  PRIMARY KEY (`ID_Propietario`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietario`
--

LOCK TABLES `propietario` WRITE;
/*!40000 ALTER TABLE `propietario` DISABLE KEYS */;
INSERT INTO `propietario` VALUES (1,'Juan','Perez','30111222','1122334455','juan.perez@mail.com','1234'),(2,'Maria','Gomez','28222333','1133445566','maria.gomez@mail.com','1234'),(3,'Carlos','Lopez','25333444','1144556677','carlos.lopez@mail.com','1234'),(6,'Lucas','Rodrigaño','45862135','4567135764','lucarodrigano@mail.com','1234'),(12,'Milagros Modificado','gomez','4566666','2665888888','prueba5@gmail.com','12345'),(13,'asdfsafd','asdfasdf','asdfasdf','asdfasdf','asdasd@mail.com','asd'),(14,'Juan','Abregu','58462356','564258753','abregujuan@mail.com','1234');
/*!40000 ALTER TABLE `propietario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reserva`
--

DROP TABLE IF EXISTS `reserva`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reserva` (
  `ID_Reserva` int NOT NULL AUTO_INCREMENT,
  `ID_Inquilino` int NOT NULL,
  `ID_Inmueble` int NOT NULL,
  `Desde` date NOT NULL,
  `Hasta` date NOT NULL,
  `FechaFinalizacion` datetime DEFAULT NULL,
  `Finalizada` tinyint(1) NOT NULL DEFAULT '0',
  `MontoDiario` decimal(10,2) NOT NULL DEFAULT '0.00',
  `CreadoPor` int DEFAULT NULL,
  `TerminadoPor` int DEFAULT NULL,
  PRIMARY KEY (`ID_Reserva`),
  KEY `ID_Inquilino_idx` (`ID_Inquilino`),
  KEY `ID_Inmueble_idx` (`ID_Inmueble`),
  KEY `FK_Reserva_CreadoPor` (`CreadoPor`),
  KEY `FK_Reserva_TerminadoPor` (`TerminadoPor`),
  CONSTRAINT `FK_Reserva_CreadoPor` FOREIGN KEY (`CreadoPor`) REFERENCES `usuario` (`ID_Usuario`),
  CONSTRAINT `FK_Reserva_TerminadoPor` FOREIGN KEY (`TerminadoPor`) REFERENCES `usuario` (`ID_Usuario`),
  CONSTRAINT `ID_Inmueble` FOREIGN KEY (`ID_Inmueble`) REFERENCES `inmueble` (`ID_Inmueble`),
  CONSTRAINT `ID_Inquilino` FOREIGN KEY (`ID_Inquilino`) REFERENCES `inquilino` (`ID_Inquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reserva`
--

LOCK TABLES `reserva` WRITE;
/*!40000 ALTER TABLE `reserva` DISABLE KEYS */;
INSERT INTO `reserva` VALUES (1,3,1,'2026-09-09','2026-09-20','2026-09-15 09:54:52',1,0.00,NULL,NULL),(2,2,1,'2026-09-21','2026-09-30',NULL,0,5000.00,NULL,NULL),(3,3,1,'2026-10-01','2026-10-15',NULL,0,5000.00,NULL,NULL),(4,1,1,'2026-10-16','2026-10-20',NULL,0,5000.00,NULL,NULL),(5,4,2,'2026-09-18','2026-09-21',NULL,0,10000.00,NULL,NULL),(6,2,3,'2026-09-18','2026-09-21','2026-09-19 00:00:00',1,6000.00,NULL,1),(7,2,4,'2026-09-16','2026-09-30',NULL,0,7000.00,NULL,NULL),(8,5,2,'2026-09-17','2026-09-18',NULL,0,10000.00,1,NULL),(9,2,8,'2026-09-18','2026-09-20',NULL,0,7000.00,1,NULL),(10,2,8,'2026-09-20','2026-09-24','2026-09-21 00:00:00',1,7000.00,1,1);
/*!40000 ALTER TABLE `reserva` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipo_inmueble`
--

DROP TABLE IF EXISTS `tipo_inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipo_inmueble` (
  `ID_TipoInmueble` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Habilitado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`ID_TipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipo_inmueble`
--

LOCK TABLES `tipo_inmueble` WRITE;
/*!40000 ALTER TABLE `tipo_inmueble` DISABLE KEYS */;
INSERT INTO `tipo_inmueble` VALUES (1,'Casa',1),(2,'Departamento',1),(3,'Habitacion de Hotel',1),(4,'Airbnb',0);
/*!40000 ALTER TABLE `tipo_inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `ID_Usuario` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `Rol` int NOT NULL,
  PRIMARY KEY (`ID_Usuario`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Administrador','Sistema','admin@mail.com','LO+JuZcDqQAU36y9RGGfGMTXSgGZeUb2ZXbf+9p+otE=','/Uploads/Avatares/avatar_1.jpg',1),(2,'Elliot','Alderson','Elliotalderson@mail.com','LO+JuZcDqQAU36y9RGGfGMTXSgGZeUb2ZXbf+9p+otE=','/Uploads/Avatares/avatar_2.jpg',2);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-22 15:33:57
