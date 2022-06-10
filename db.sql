-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 10-06-2022 a las 08:42:02
-- Versión del servidor: 10.1.37-MariaDB
-- Versión de PHP: 7.3.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `almacenventas`
--

DELIMITER $$
--
-- Procedimientos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_NuevaUnidadMedida` (IN `p_codigo` VARCHAR(3), IN `p_descripcion` VARCHAR(30))  BEGIN
INSERT INTO tblunidadmedida (codigo,descripcion) VALUES (p_codigo,p_descripcion);
SELECT * FROM tblunidadmedida;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_totalFacturas` (IN `v_year` INT(4))  BEGIN
SELECT FA.numero as 'Número factura', sum(PF.cantidad * PF.valor) as 'Total Factura',
year(FA.fechafact) as 'Año'
FROM tblfactura as FA INNER JOIN tblproductofactura as PF
ON FA.numero = PF.numfactura
GROUP BY FA.numero, year(FA.fechafact) = v_year;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblcliente`
--

CREATE TABLE `tblcliente` (
  `docid` varchar(15) COLLATE utf8mb4_spanish_ci NOT NULL,
  `nombres` varchar(40) COLLATE utf8mb4_spanish_ci NOT NULL,
  `apellidos` varchar(30) COLLATE utf8mb4_spanish_ci NOT NULL,
  `telefono` varchar(15) COLLATE utf8mb4_spanish_ci DEFAULT NULL,
  `direccion` varchar(50) COLLATE utf8mb4_spanish_ci DEFAULT NULL,
  `email` varchar(40) COLLATE utf8mb4_spanish_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblcliente`
--

INSERT INTO `tblcliente` (`docid`, `nombres`, `apellidos`, `telefono`, `direccion`, `email`) VALUES
('123', 'db63be45433b6a9e931565099f554e4e', 'b', '48', '51', 'yu'),
('15420350', 'Juan', 'Perez', '5616161', 'Calle 28 No.10-50', 'jp@gmail.com'),
('25136150', 'Pedro', 'Ayala', '3202020', 'Cra 8 No. 16-38', 'pa@hotmail.com'),
('39426514', 'Maria', 'García', '5313131', 'Calle 45 No. 20-10', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblfactura`
--

CREATE TABLE `tblfactura` (
  `numero` int(11) NOT NULL,
  `cliente` varchar(15) COLLATE utf8mb4_spanish_ci NOT NULL,
  `fechafact` datetime NOT NULL,
  `fechapago` date DEFAULT NULL,
  `formapago` varchar(3) COLLATE utf8mb4_spanish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblfactura`
--

INSERT INTO `tblfactura` (`numero`, `cliente`, `fechafact`, `fechapago`, `formapago`) VALUES
(1, '15420350', '2013-05-02 08:04:02', NULL, '1'),
(2, '25136150', '2013-04-29 13:05:00', '2013-05-01', '4'),
(3, '15420350', '2013-04-15 20:02:16', '2013-04-18', '2'),
(4, '25136150', '2013-04-05 11:12:25', '2013-04-20', '1');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblformapago`
--

CREATE TABLE `tblformapago` (
  `codigo` varchar(3) COLLATE utf8mb4_spanish_ci NOT NULL,
  `descripcion` varchar(30) COLLATE utf8mb4_spanish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblformapago`
--

INSERT INTO `tblformapago` (`codigo`, `descripcion`) VALUES
('1', 'Efectivo'),
('2', 'Tarjeta Crédito'),
('3', 'Tarjeta Débito'),
('4', 'Cheque');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblproducto`
--

CREATE TABLE `tblproducto` (
  `codigo` varchar(6) COLLATE utf8mb4_spanish_ci NOT NULL,
  `nombre` varchar(30) COLLATE utf8mb4_spanish_ci NOT NULL,
  `costo` int(11) NOT NULL,
  `existencia` int(11) NOT NULL,
  `unidadmedida` varchar(3) COLLATE utf8mb4_spanish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblproducto`
--

INSERT INTO `tblproducto` (`codigo`, `nombre`, `costo`, `existencia`, `unidadmedida`) VALUES
('1', 'Yuca', 1000, 10, '1'),
('2', 'Papa', 2000, 15, '1'),
('3', 'Arroz', 1500, 12, '5'),
('4', 'Avena', 600, 50, '4'),
('5', 'Café', 10000, 35, '5');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblproductofactura`
--

CREATE TABLE `tblproductofactura` (
  `numfactura` int(11) NOT NULL,
  `codproducto` varchar(6) COLLATE utf8mb4_spanish_ci NOT NULL,
  `cantidad` int(11) NOT NULL,
  `valor` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblproductofactura`
--

INSERT INTO `tblproductofactura` (`numfactura`, `codproducto`, `cantidad`, `valor`) VALUES
(1, '1', 3, 4000),
(1, '2', 2, 5000),
(1, '3', 8, 3000),
(2, '4', 3, 8000),
(2, '5', 2, 15000),
(3, '1', 5, 4000),
(3, '4', 10, 4000),
(4, '1', 5, 4000),
(4, '2', 5, 5000),
(4, '3', 12, 3000),
(4, '5', 28, 15000);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tblunidadmedida`
--

CREATE TABLE `tblunidadmedida` (
  `codigo` varchar(3) COLLATE utf8mb4_spanish_ci NOT NULL,
  `descripcion` varchar(30) COLLATE utf8mb4_spanish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish_ci;

--
-- Volcado de datos para la tabla `tblunidadmedida`
--

INSERT INTO `tblunidadmedida` (`codigo`, `descripcion`) VALUES
('1', 'Kilo'),
('10', 'Miligramo'),
('20', 'Gramo'),
('25', 'Bono'),
('4', 'Gramo'),
('5', 'Libra'),
('6', 'Metro');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `tblcliente`
--
ALTER TABLE `tblcliente`
  ADD PRIMARY KEY (`docid`);

--
-- Indices de la tabla `tblfactura`
--
ALTER TABLE `tblfactura`
  ADD PRIMARY KEY (`numero`),
  ADD KEY `fk_tblfactura_tblformapago1_idx` (`formapago`),
  ADD KEY `fk_tblfactura_tblcliente1_idx` (`cliente`);

--
-- Indices de la tabla `tblformapago`
--
ALTER TABLE `tblformapago`
  ADD PRIMARY KEY (`codigo`);

--
-- Indices de la tabla `tblproducto`
--
ALTER TABLE `tblproducto`
  ADD PRIMARY KEY (`codigo`),
  ADD KEY `fk_tblproducto_tblunidadmedida1_idx` (`unidadmedida`);

--
-- Indices de la tabla `tblproductofactura`
--
ALTER TABLE `tblproductofactura`
  ADD PRIMARY KEY (`numfactura`,`codproducto`),
  ADD KEY `fk_tblproductofactura_tblproducto1_idx` (`codproducto`),
  ADD KEY `fk_tblproductofactura_tblfactura_idx` (`numfactura`);

--
-- Indices de la tabla `tblunidadmedida`
--
ALTER TABLE `tblunidadmedida`
  ADD PRIMARY KEY (`codigo`);

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `tblfactura`
--
ALTER TABLE `tblfactura`
  ADD CONSTRAINT `fk_tblfactura_tblcliente1` FOREIGN KEY (`cliente`) REFERENCES `tblcliente` (`docid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_tblfactura_tblformapago1` FOREIGN KEY (`formapago`) REFERENCES `tblformapago` (`codigo`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Filtros para la tabla `tblproducto`
--
ALTER TABLE `tblproducto`
  ADD CONSTRAINT `fk_tblproducto_tblunidadmedida1` FOREIGN KEY (`unidadmedida`) REFERENCES `tblunidadmedida` (`codigo`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Filtros para la tabla `tblproductofactura`
--
ALTER TABLE `tblproductofactura`
  ADD CONSTRAINT `fk_tblproductofactura_tblfactura` FOREIGN KEY (`numfactura`) REFERENCES `tblfactura` (`numero`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_tblproductofactura_tblproducto1` FOREIGN KEY (`codproducto`) REFERENCES `tblproducto` (`codigo`) ON DELETE NO ACTION ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
