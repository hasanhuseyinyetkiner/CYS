-- Fix for missing AgirlikOlcum table
CREATE TABLE IF NOT EXISTS `AgirlikOlcum` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `hayvanId` int(11) NOT NULL DEFAULT 0,
  `agirlik` double NOT NULL DEFAULT 0,
  `olcumTarihi` datetime NOT NULL,
  `bluetoothOlcum` tinyint(1) NOT NULL DEFAULT 0,
  `userId` int(11) NOT NULL,
  `olcumNotu` varchar(255) DEFAULT NULL,
  `requestId` varchar(255) DEFAULT NULL,
  `agirlikOlcumu` varchar(255) DEFAULT NULL,
  `aktif` tinyint(1) DEFAULT 1,
  `tarih` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `hayvanId_idx` (`hayvanId`),
  KEY `userId_idx` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- Copy data from old table if it exists (optional)
-- Uncomment if you want to migrate existing data
/*
INSERT IGNORE INTO `AgirlikOlcum` (
  `id`, `hayvanId`, `agirlik`, `olcumTarihi`, `bluetoothOlcum`, 
  `userId`, `olcumNotu`, `requestId`, `agirlikOlcumu`, `aktif`, `tarih`
)
SELECT 
  `id`, 0 as `hayvanId`, 0 as `agirlik`, NOW() as `olcumTarihi`, 0 as `bluetoothOlcum`, 
  `userId`, NULL as `olcumNotu`, `requestId`, `agirlikOlcumu`, `aktif`, `tarih`
FROM `agirlikolcum`;
*/ 