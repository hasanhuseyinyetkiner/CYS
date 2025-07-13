-- Normal Ağırlık Ölçümleri Tablosu
CREATE TABLE IF NOT EXISTS WeightMeasurements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnimalId INT,
    Weight DECIMAL(10, 2) NOT NULL,
    MeasurementDate DATETIME NOT NULL DEFAULT GETDATE(),
    RFID NVARCHAR(100),
    Notes NVARCHAR(MAX),
    UserId INT,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_WeightMeasurements_Animals FOREIGN KEY (AnimalId) REFERENCES Animals(Id)
);

-- Sütten Kesim Ağırlık Ölçümleri Tablosu
CREATE TABLE IF NOT EXISTS WeaningWeightMeasurements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnimalId INT,
    Weight DECIMAL(10, 2) NOT NULL,
    WeaningDate DATETIME,
    WeaningAge INT,
    MeasurementDate DATETIME NOT NULL DEFAULT GETDATE(),
    RFID NVARCHAR(100),
    MotherRFID NVARCHAR(100),
    Notes NVARCHAR(MAX),
    UserId INT,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_WeaningWeightMeasurements_Animals FOREIGN KEY (AnimalId) REFERENCES Animals(Id)
);

-- Doğum Ağırlık Ölçümleri Tablosu
CREATE TABLE IF NOT EXISTS BirthWeightMeasurements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnimalId INT,
    Weight DECIMAL(10, 2) NOT NULL,
    BirthDate DATETIME,
    BirthPlace NVARCHAR(255),
    MeasurementDate DATETIME NOT NULL DEFAULT GETDATE(),
    RFID NVARCHAR(100),
    MotherRFID NVARCHAR(100),
    Notes NVARCHAR(MAX),
    UserId INT,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_BirthWeightMeasurements_Animals FOREIGN KEY (AnimalId) REFERENCES Animals(Id)
);

-- Index oluşturma
CREATE INDEX IX_WeightMeasurements_AnimalId ON WeightMeasurements(AnimalId);
CREATE INDEX IX_WeightMeasurements_RFID ON WeightMeasurements(RFID);
CREATE INDEX IX_WeightMeasurements_MeasurementDate ON WeightMeasurements(MeasurementDate);

CREATE INDEX IX_WeaningWeightMeasurements_AnimalId ON WeaningWeightMeasurements(AnimalId);
CREATE INDEX IX_WeaningWeightMeasurements_RFID ON WeaningWeightMeasurements(RFID);
CREATE INDEX IX_WeaningWeightMeasurements_MeasurementDate ON WeaningWeightMeasurements(MeasurementDate);
CREATE INDEX IX_WeaningWeightMeasurements_WeaningDate ON WeaningWeightMeasurements(WeaningDate);

CREATE INDEX IX_BirthWeightMeasurements_AnimalId ON BirthWeightMeasurements(AnimalId);
CREATE INDEX IX_BirthWeightMeasurements_RFID ON BirthWeightMeasurements(RFID);
CREATE INDEX IX_BirthWeightMeasurements_MeasurementDate ON BirthWeightMeasurements(MeasurementDate);
CREATE INDEX IX_BirthWeightMeasurements_BirthDate ON BirthWeightMeasurements(BirthDate); 