-- Weight Measurement Tables Creation Script

-- Create AgirlikOlcum table if not exists
CREATE TABLE IF NOT EXISTS agirlikolcum (
    id INT PRIMARY KEY AUTO_INCREMENT,
    hayvanId INT NOT NULL,
    agirlik FLOAT NOT NULL,
    olcumTarihi DATETIME NOT NULL,
    bluetoothOlcum BOOLEAN NOT NULL DEFAULT 0,
    userId INT NOT NULL,
    olcumNotu VARCHAR(255) NULL,
    requestId VARCHAR(36) NULL,
    agirlikOlcumu VARCHAR(50) NULL,
    aktif BOOLEAN NOT NULL DEFAULT 1,
    tarih DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create SuttenKesimAgirlikOlcum table if not exists
CREATE TABLE IF NOT EXISTS suttenkesimagirlikolcum (
    id INT PRIMARY KEY AUTO_INCREMENT,
    hayvanId INT NOT NULL,
    agirlik FLOAT NOT NULL,
    kesimTarihi DATETIME NOT NULL,
    olcumTarihi DATETIME NOT NULL,
    bluetoothOlcum BOOLEAN NOT NULL DEFAULT 0,
    userId INT NOT NULL,
    olcumNotu VARCHAR(255) NULL,
    anneRfid VARCHAR(50) NULL,
    kesimYasi INT NULL,
    requestId VARCHAR(36) NULL,
    agirlikOlcumu VARCHAR(50) NULL,
    aktif BOOLEAN NOT NULL DEFAULT 1,
    tarih DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create DogumAgirlikOlcum table if not exists
CREATE TABLE IF NOT EXISTS dogumagirlikolcum (
    id INT PRIMARY KEY AUTO_INCREMENT,
    hayvanId INT NOT NULL,
    agirlik FLOAT NOT NULL,
    dogumTarihi DATETIME NOT NULL,
    olcumTarihi DATETIME NOT NULL,
    bluetoothOlcum BOOLEAN NOT NULL DEFAULT 0,
    userId INT NOT NULL,
    olcumNotu VARCHAR(255) NULL,
    anneRfid VARCHAR(50) NULL,
    dogumYeri VARCHAR(100) NULL,
    requestId VARCHAR(36) NULL,
    agirlikOlcumu VARCHAR(50) NULL,
    aktif BOOLEAN NOT NULL DEFAULT 1,
    tarih DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create SyncLog table if not exists
CREATE TABLE IF NOT EXISTS SyncLog (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    KullaniciId INT NOT NULL,
    SyncTarihi DATETIME NOT NULL,
    CihazId VARCHAR(50) NOT NULL,
    CihazAdi VARCHAR(100) NULL,
    CihazIP VARCHAR(50) NOT NULL,
    YuklenenKayitSayisi INT NOT NULL DEFAULT 0,
    IndirilenKayitSayisi INT NOT NULL DEFAULT 0,
    SyncDurumu VARCHAR(50) NOT NULL,
    HataMesaji TEXT NULL,
    FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id)
);

-- Create Dogum table if not exists
CREATE TABLE IF NOT EXISTS Dogum (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    AnneId INT NOT NULL,
    BabaId INT NULL,
    DogumTarihi DATETIME NOT NULL,
    YavruSayisi INT NOT NULL DEFAULT 0,
    Notlar VARCHAR(500) NULL,
    OluDogum BOOLEAN NOT NULL DEFAULT 0,
    DogumYeri VARCHAR(50) NULL,
    KayitTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    KaydedenKullaniciId INT NOT NULL,
    FOREIGN KEY (AnneId) REFERENCES hayvan(id),
    FOREIGN KEY (BabaId) REFERENCES hayvan(id),
    FOREIGN KEY (KaydedenKullaniciId) REFERENCES Kullanici(Id)
);

-- Create DogumYavru table if not exists
CREATE TABLE IF NOT EXISTS DogumYavru (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    DogumId INT NOT NULL,
    HayvanId INT NOT NULL,
    Cinsiyet VARCHAR(1) NOT NULL,
    DogumAgirligi FLOAT NULL,
    KupeNo VARCHAR(50) NOT NULL,
    Notlar TEXT NULL,
    Yasayan BOOLEAN NOT NULL DEFAULT 1,
    KayitTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (DogumId) REFERENCES Dogum(Id),
    FOREIGN KEY (HayvanId) REFERENCES hayvan(id)
); 