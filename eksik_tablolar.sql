-- Eksik Tablolar için Oluşturma Betikleri
-- Bu betik Ubuntu sunucudaki CYS veritabanında eksik olan tabloları oluşturacaktır

-- Değişken karakter kodlaması ve motor ayarları
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- 1. Modules tablosu
CREATE TABLE IF NOT EXISTS `modules` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `modulename` varchar(100) NOT NULL,
  `ispublic` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- 2. Device tablosu
CREATE TABLE IF NOT EXISTS `devices` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `devicename` varchar(100) NOT NULL,
  `deviceguid` varchar(50) NOT NULL,
  `devicetype` int(11) NOT NULL DEFAULT 0,
  `active` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UK_deviceguid` (`deviceguid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- 3. UserDevice tablosu
CREATE TABLE IF NOT EXISTS `userdevice` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `deviceid` int(11) NOT NULL,
  `userid` int(11) NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `FK_userdevice_device` (`deviceid`),
  KEY `FK_userdevice_user` (`userid`),
  CONSTRAINT `FK_userdevice_device` FOREIGN KEY (`deviceid`) REFERENCES `devices` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_userdevice_user` FOREIGN KEY (`userid`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- 4. UserModule tablosu
CREATE TABLE IF NOT EXISTS `usermodule` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` int(11) NOT NULL,
  `moduleid` int(11) NOT NULL,
  `aktif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `FK_usermodule_module` (`moduleid`),
  KEY `FK_usermodule_user` (`userid`),
  CONSTRAINT `FK_usermodule_module` FOREIGN KEY (`moduleid`) REFERENCES `modules` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_usermodule_user` FOREIGN KEY (`userid`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- 5. UserSession tablosu
CREATE TABLE IF NOT EXISTS `usersession` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` int(11) NOT NULL,
  `devicename` varchar(100) DEFAULT NULL,
  `deviceguid` varchar(50) DEFAULT NULL,
  `devicekey` varchar(100) DEFAULT NULL,
  `sessiontimeout` datetime NOT NULL,
  `ipaddress` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_usersession_user` (`userid`),
  CONSTRAINT `FK_usersession_user` FOREIGN KEY (`userid`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- 6. Kullanici tablosu (api_extensions.sql'den)
CREATE TABLE IF NOT EXISTS `Kullanici` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `KullaniciAdi` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Sifre` varchar(100) NOT NULL,
  `Ad` varchar(50) DEFAULT NULL,
  `Soyad` varchar(50) DEFAULT NULL,
  `Telefon` varchar(20) DEFAULT NULL,
  `RolId` int(11) DEFAULT NULL,
  `ProfilResmi` varchar(255) DEFAULT NULL,
  `KayitTarihi` datetime NOT NULL,
  `SonGirisTarihi` datetime DEFAULT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT 1,
  `Tema` varchar(50) DEFAULT 'light',
  `Dil` varchar(10) DEFAULT 'tr',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_KullaniciAdi` (`KullaniciAdi`),
  UNIQUE KEY `UK_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. Rol tablosu
CREATE TABLE IF NOT EXISTS `Rol` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RolAdi` varchar(50) NOT NULL,
  `Aciklama` varchar(250) DEFAULT NULL,
  `OlusturmaTarihi` datetime NOT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_RolAdi` (`RolAdi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Kullanici-Rol ilişkisi için kısıtlama ekleme
ALTER TABLE `Kullanici` 
ADD CONSTRAINT `FK_Kullanici_Rol` FOREIGN KEY (`RolId`) REFERENCES `Rol` (`Id`);

-- 8. Izin tablosu
CREATE TABLE IF NOT EXISTS `Izin` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IzinAdi` varchar(50) NOT NULL,
  `IzinKodu` varchar(100) DEFAULT NULL,
  `Aciklama` varchar(250) DEFAULT NULL,
  `Modul` varchar(50) DEFAULT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_IzinKodu` (`IzinKodu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9. RolIzin tablosu
CREATE TABLE IF NOT EXISTS `RolIzin` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RolId` int(11) NOT NULL,
  `IzinId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_RolIzin` (`RolId`,`IzinId`),
  KEY `FK_RolIzin_Rol` (`RolId`),
  KEY `FK_RolIzin_Izin` (`IzinId`),
  CONSTRAINT `FK_RolIzin_Izin` FOREIGN KEY (`IzinId`) REFERENCES `Izin` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RolIzin_Rol` FOREIGN KEY (`RolId`) REFERENCES `Rol` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 10. DogumYavru tablosu
CREATE TABLE IF NOT EXISTS `DogumYavru` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DogumId` int(11) NOT NULL,
  `HayvanId` int(11) NOT NULL,
  `Cinsiyet` varchar(10) DEFAULT NULL,
  `DogumAgirligi` double DEFAULT NULL,
  `KupeNo` varchar(100) DEFAULT NULL,
  `Notlar` varchar(500) DEFAULT NULL,
  `Yasayan` tinyint(1) NOT NULL DEFAULT 1,
  `KayitTarihi` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_DogumYavru_Dogum` (`DogumId`),
  KEY `FK_DogumYavru_Hayvan` (`HayvanId`),
  CONSTRAINT `FK_DogumYavru_Hayvan` FOREIGN KEY (`HayvanId`) REFERENCES `hayvan` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 11. ustkategori tablosu
CREATE TABLE IF NOT EXISTS `ustkategori` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL,
  `isActive` int(11) DEFAULT 1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- Bu tabloyu kategori tablosuna bağla
ALTER TABLE `kategori` 
ADD CONSTRAINT `FK_kategori_ustkategori` FOREIGN KEY (`ustKategoriId`) REFERENCES `ustkategori` (`id`);

-- 12. Varsayılan veri girişleri - Rol
INSERT INTO `Rol` (`RolAdi`, `Aciklama`, `OlusturmaTarihi`, `Aktif`)
VALUES ('Admin', 'Tam yetkili sistem yöneticisi', NOW(), 1),
       ('Kullanıcı', 'Standart kullanıcı yetkileri', NOW(), 1),
       ('Çiftlik Yöneticisi', 'Çiftlik yönetim yetkileri', NOW(), 1),
       ('Veteriner', 'Veteriner yetkileri', NOW(), 1);

-- 13. Varsayılan veri girişleri - Izin
INSERT INTO `Izin` (`IzinAdi`, `IzinKodu`, `Aciklama`, `Modul`, `Aktif`)
VALUES ('Hayvan Listeleme', 'HAYVAN_LISTE', 'Hayvanları listeleme izni', 'Hayvan', 1),
       ('Hayvan Ekleme', 'HAYVAN_EKLE', 'Hayvan ekleme izni', 'Hayvan', 1),
       ('Hayvan Düzenleme', 'HAYVAN_DUZENLE', 'Hayvan düzenleme izni', 'Hayvan', 1),
       ('Hayvan Silme', 'HAYVAN_SIL', 'Hayvan silme izni', 'Hayvan', 1),
       ('Ölçüm Listeleme', 'OLCUM_LISTE', 'Ölçümleri listeleme izni', 'Ölçüm', 1),
       ('Ölçüm Ekleme', 'OLCUM_EKLE', 'Ölçüm ekleme izni', 'Ölçüm', 1),
       ('Ölçüm Düzenleme', 'OLCUM_DUZENLE', 'Ölçüm düzenleme izni', 'Ölçüm', 1),
       ('Ölçüm Silme', 'OLCUM_SIL', 'Ölçüm silme izni', 'Ölçüm', 1);

-- 14. Varsayılan veri girişleri - RolIzin (Admin rolüne tüm izinleri atama)
INSERT INTO `RolIzin` (`RolId`, `IzinId`)
SELECT 1, `Id` FROM `Izin`;

-- 15. Varsayılan veri girişleri - Kullanici (admin kullanıcısı)
INSERT INTO `Kullanici` (`KullaniciAdi`, `Email`, `Sifre`, `Ad`, `Soyad`, `RolId`, `KayitTarihi`, `Aktif`)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin', 'User', 1, NOW(), 1);

-- 16. Varsayılan veri girişleri - ustkategori
INSERT INTO `ustkategori` (`name`, `isActive`)
VALUES ('Büyükbaş', 1),
       ('Küçükbaş', 1),
       ('Kanatlı Hayvan', 1);

-- 17. Varsayılan veri girişleri - modules
INSERT INTO `modules` (`modulename`, `ispublic`)
VALUES ('Hayvan', 1),
       ('Ölçüm', 1),
       ('Raporlama', 0),
       ('Ayarlar', 0);

-- Kısıtlamaları tekrar aktif hale getir
SET FOREIGN_KEY_CHECKS = 1; 