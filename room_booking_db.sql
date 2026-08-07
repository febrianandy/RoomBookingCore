/*
SQLyog Community v13.3.1 (64 bit)
MySQL - 8.0.46 : Database - room_booking_db
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`room_booking_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `room_booking_db`;

/*Table structure for table `Bookings` */

DROP TABLE IF EXISTS `Bookings`;

CREATE TABLE `Bookings` (
  `BookingId` int NOT NULL AUTO_INCREMENT,
  `StartTime` datetime(6) NOT NULL,
  `EndTime` datetime(6) NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoomId` int NOT NULL,
  `UserId` int NOT NULL,
  `DepartmentId` int NOT NULL,
  `Title` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `RejectionReason` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`BookingId`),
  KEY `IX_Bookings_DepartmentId` (`DepartmentId`),
  KEY `IX_Bookings_RoomId` (`RoomId`),
  KEY `IX_Bookings_UserId` (`UserId`),
  CONSTRAINT `FK_Bookings_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments` (`DepartmentId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bookings_Rooms_RoomId` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`RoomId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bookings_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `Bookings` */

insert  into `Bookings`(`BookingId`,`StartTime`,`EndTime`,`Status`,`RoomId`,`UserId`,`DepartmentId`,`Title`,`RejectionReason`) values 
(3,'2026-08-06 20:20:00.000000','2026-08-06 20:17:00.000000','Approved',1,1,1,'',NULL),
(4,'2026-08-19 20:43:00.000000','2026-08-21 20:46:00.000000','Rejected',3,1,1,'',NULL),
(5,'2026-08-06 22:41:00.000000','2026-08-06 23:41:00.000000','Approved',3,2,2,'',NULL),
(6,'2026-08-06 21:18:00.000000','2026-08-06 22:18:00.000000','Approved',2,1,1,'',NULL),
(7,'2026-08-06 22:34:00.000000','2026-08-06 23:02:00.000000','Rejected',3,1,1,'Rapat Kordinasi',NULL),
(8,'2026-08-06 12:38:00.000000','2026-08-06 15:38:00.000000','Rejected',3,1,1,'Rapat Kordinasi',NULL),
(9,'2026-08-06 22:39:00.000000','2026-08-06 23:39:00.000000','Rejected',2,1,1,'Rapat Kordinasi HR',NULL),
(10,'2026-08-06 22:42:00.000000','2026-08-06 23:42:00.000000','Rejected',2,1,1,'Rapat Kordinasi HR','Pokoknya tidak'),
(11,'2026-08-06 22:56:00.000000','2026-08-06 23:59:00.000000','Approved',3,1,1,'Rapat Kordinasi HR',NULL);

/*Table structure for table `Departments` */

DROP TABLE IF EXISTS `Departments`;

CREATE TABLE `Departments` (
  `DepartmentId` int NOT NULL AUTO_INCREMENT,
  `DepartmentName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`DepartmentId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `Departments` */

insert  into `Departments`(`DepartmentId`,`DepartmentName`) values 
(1,'IT & Development'),
(2,'Human Resources (Human Capital)'),
(3,'Operational Admin');

/*Table structure for table `Rooms` */

DROP TABLE IF EXISTS `Rooms`;

CREATE TABLE `Rooms` (
  `RoomId` int NOT NULL AUTO_INCREMENT,
  `RoomName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Capacity` int NOT NULL,
  `Facilities` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`RoomId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `Rooms` */

insert  into `Rooms`(`RoomId`,`RoomName`,`Capacity`,`Facilities`) values 
(1,'RUANG01',12,'AC'),
(2,'RUANG-02',1,'AC,SOUND'),
(3,'RUANG-03',10,'Projector');

/*Table structure for table `Users` */

DROP TABLE IF EXISTS `Users`;

CREATE TABLE `Users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DepartmentId` int NOT NULL,
  PRIMARY KEY (`UserId`),
  KEY `IX_Users_DepartmentId` (`DepartmentId`),
  CONSTRAINT `FK_Users_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments` (`DepartmentId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `Users` */

insert  into `Users`(`UserId`,`Email`,`Role`,`DepartmentId`) values 
(1,'superuser@roombooking.com','SuperUser',1),
(2,'admin@roombooking.com','Admin',1),
(3,'pegawai@roombooking.com','Pegawai',1);

/*Table structure for table `__EFMigrationsHistory` */

DROP TABLE IF EXISTS `__EFMigrationsHistory`;

CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `__EFMigrationsHistory` */

insert  into `__EFMigrationsHistory`(`MigrationId`,`ProductVersion`) values 
('20260806104655_InitialCreate','9.0.0'),
('20260806152828_AddBookingTitle','9.0.0'),
('20260806154447_AddRejectionReason','9.0.0');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
