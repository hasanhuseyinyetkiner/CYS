-- Kullanıcı Yönetimi Tabloları
CREATE TABLE IF NOT EXISTS `Kullanici` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KullaniciAdi` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Sifre` varchar(100) NOT NULL,
  `Ad` varchar(50) DEFAULT NULL,
  `Soyad` varchar(50) DEFAULT NULL,
  `Telefon` varchar(20) DEFAULT NULL,
  `RolId` int DEFAULT NULL,
  `ProfilResmi` varchar(255) DEFAULT NULL,
  `KayitTarihi` datetime NOT NULL,
  `SonGirisTarihi` datetime DEFAULT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT '1',
  `Tema` varchar(50) DEFAULT 'light',
  `Dil` varchar(10) DEFAULT 'tr',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_KullaniciAdi` (`KullaniciAdi`),
  UNIQUE KEY `UK_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Rol` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RolAdi` varchar(50) NOT NULL,
  `Aciklama` varchar(250) DEFAULT NULL,
  `OlusturmaTarihi` datetime NOT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_RolAdi` (`RolAdi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Izin` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `IzinAdi` varchar(50) NOT NULL,
  `IzinKodu` varchar(100) DEFAULT NULL,
  `Aciklama` varchar(250) DEFAULT NULL,
  `Modul` varchar(50) DEFAULT NULL,
  `Aktif` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_IzinKodu` (`IzinKodu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `RolIzin` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RolId` int NOT NULL,
  `IzinId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_RolIzin` (`RolId`,`IzinId`),
  KEY `FK_RolIzin_Rol` (`RolId`),
  KEY `FK_RolIzin_Izin` (`IzinId`),
  CONSTRAINT `FK_RolIzin_Izin` FOREIGN KEY (`IzinId`) REFERENCES `Izin` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RolIzin_Rol` FOREIGN KEY (`RolId`) REFERENCES `Rol` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `AktiviteLog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KullaniciId` int NOT NULL,
  `Islem` varchar(50) DEFAULT NULL,
  `ModulAdi` varchar(50) DEFAULT NULL,
  `IslemDetayi` varchar(250) DEFAULT NULL,
  `IPAdresi` varchar(20) DEFAULT NULL,
  `Tarayici` varchar(100) DEFAULT NULL,
  `IslemTarihi` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AktiviteLog_Kullanici` (`KullaniciId`),
  KEY `IX_IslemTarihi` (`IslemTarihi`),
  CONSTRAINT `FK_AktiviteLog_Kullanici` FOREIGN KEY (`KullaniciId`) REFERENCES `Kullanici` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Doğum Kayıt Tabloları
CREATE TABLE IF NOT EXISTS `Dogum` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `AnneId` int NOT NULL,
  `BabaId` int DEFAULT NULL,
  `DogumTarihi` datetime NOT NULL,
  `YavruSayisi` int NOT NULL DEFAULT '0',
  `Notlar` varchar(500) DEFAULT NULL,
  `OluDogum` tinyint(1) NOT NULL DEFAULT '0',
  `DogumYeri` varchar(50) DEFAULT NULL,
  `KayitTarihi` datetime NOT NULL,
  `KaydedenKullaniciId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Dogum_Anne` (`AnneId`),
  KEY `FK_Dogum_Baba` (`BabaId`),
  KEY `FK_Dogum_Kullanici` (`KaydedenKullaniciId`),
  KEY `IX_DogumTarihi` (`DogumTarihi`),
  CONSTRAINT `FK_Dogum_Anne` FOREIGN KEY (`AnneId`) REFERENCES `Animal` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dogum_Baba` FOREIGN KEY (`BabaId`) REFERENCES `Animal` (`Id`),
  CONSTRAINT `FK_Dogum_Kullanici` FOREIGN KEY (`KaydedenKullaniciId`) REFERENCES `Kullanici` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `DogumYavru` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DogumId` int NOT NULL,
  `HayvanId` int NOT NULL,
  `Cinsiyet` varchar(10) DEFAULT NULL,
  `DogumAgirligi` double DEFAULT NULL,
  `KupeNo` varchar(100) DEFAULT NULL,
  `Notlar` varchar(500) DEFAULT NULL,
  `Yasayan` tinyint(1) NOT NULL DEFAULT '1',
  `KayitTarihi` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_DogumYavru_Dogum` (`DogumId`),
  KEY `FK_DogumYavru_Hayvan` (`HayvanId`),
  CONSTRAINT `FK_DogumYavru_Dogum` FOREIGN KEY (`DogumId`) REFERENCES `Dogum` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DogumYavru_Hayvan` FOREIGN KEY (`HayvanId`) REFERENCES `Animal` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Senkronizasyon Tabloları
CREATE TABLE IF NOT EXISTS `SyncLog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KullaniciId` int NOT NULL,
  `SyncTarihi` datetime NOT NULL,
  `CihazId` varchar(20) DEFAULT NULL,
  `CihazAdi` varchar(100) DEFAULT NULL,
  `CihazIP` varchar(20) DEFAULT NULL,
  `YuklenenKayitSayisi` int NOT NULL DEFAULT '0',
  `IndirilenKayitSayisi` int NOT NULL DEFAULT '0',
  `SyncDurumu` varchar(50) DEFAULT NULL,
  `HataMesaji` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_SyncLog_Kullanici` (`KullaniciId`),
  KEY `IX_SyncTarihi` (`SyncTarihi`),
  CONSTRAINT `FK_SyncLog_Kullanici` FOREIGN KEY (`KullaniciId`) REFERENCES `Kullanici` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Varsayılan veriler
INSERT INTO `Rol` (`RolAdi`, `Aciklama`, `OlusturmaTarihi`, `Aktif`)
VALUES ('Admin', 'Tam yetkili sistem yöneticisi', NOW(), 1),
       ('Kullanıcı', 'Standart kullanıcı yetkileri', NOW(), 1),
       ('Çiftlik Yöneticisi', 'Çiftlik yönetim yetkileri', NOW(), 1),
       ('Veteriner', 'Veteriner yetkileri', NOW(), 1);

-- Temel izinler
INSERT INTO `Izin` (`IzinAdi`, `IzinKodu`, `Aciklama`, `Modul`, `Aktif`)
VALUES ('Hayvan Listeleme', 'HAYVAN_LISTE', 'Hayvanları listeleme izni', 'Hayvan', 1),
       ('Hayvan Ekleme', 'HAYVAN_EKLE', 'Hayvan ekleme izni', 'Hayvan', 1),
       ('Hayvan Düzenleme', 'HAYVAN_DUZENLE', 'Hayvan düzenleme izni', 'Hayvan', 1),
       ('Hayvan Silme', 'HAYVAN_SIL', 'Hayvan silme izni', 'Hayvan', 1),
       ('Ölçüm Listeleme', 'OLCUM_LISTE', 'Ölçümleri listeleme izni', 'Ölçüm', 1),
       ('Ölçüm Ekleme', 'OLCUM_EKLE', 'Ölçüm ekleme izni', 'Ölçüm', 1),
       ('Ölçüm Düzenleme', 'OLCUM_DUZENLE', 'Ölçüm düzenleme izni', 'Ölçüm', 1),
       ('Ölçüm Silme', 'OLCUM_SIL', 'Ölçüm silme izni', 'Ölçüm', 1),
       ('Doğum Listeleme', 'DOGUM_LISTE', 'Doğumları listeleme izni', 'Doğum', 1),
       ('Doğum Ekleme', 'DOGUM_EKLE', 'Doğum ekleme izni', 'Doğum', 1),
       ('Doğum Düzenleme', 'DOGUM_DUZENLE', 'Doğum düzenleme izni', 'Doğum', 1),
       ('Doğum Silme', 'DOGUM_SIL', 'Doğum silme izni', 'Doğum', 1),
       ('Kullanıcı Listeleme', 'KULLANICI_LISTE', 'Kullanıcıları listeleme izni', 'Kullanıcı', 1),
       ('Kullanıcı Ekleme', 'KULLANICI_EKLE', 'Kullanıcı ekleme izni', 'Kullanıcı', 1),
       ('Kullanıcı Düzenleme', 'KULLANICI_DUZENLE', 'Kullanıcı düzenleme izni', 'Kullanıcı', 1),
       ('Kullanıcı Silme', 'KULLANICI_SIL', 'Kullanıcı silme izni', 'Kullanıcı', 1),
       ('Senkronizasyon', 'SYNC', 'Senkronizasyon izni', 'Sistem', 1),
       ('Rol Yönetimi', 'ROL_YONETIM', 'Rol yönetimi izni', 'Kullanıcı', 1),
       ('İzin Yönetimi', 'IZIN_YONETIM', 'İzin yönetimi izni', 'Kullanıcı', 1),
       ('Aktivite Log Görüntüleme', 'LOG_GORUNTULE', 'Aktivite loglarını görüntüleme izni', 'Sistem', 1);

-- Admin rolüne tüm izinleri atama
INSERT INTO `RolIzin` (`RolId`, `IzinId`)
SELECT 1, `Id` FROM `Izin`;

-- Varsayılan admin kullanıcısı oluşturma (şifre: admin123)
INSERT INTO `Kullanici` (`KullaniciAdi`, `Email`, `Sifre`, `Ad`, `Soyad`, `RolId`, `KayitTarihi`, `Aktif`)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin', 'User', 1, NOW(), 1); 