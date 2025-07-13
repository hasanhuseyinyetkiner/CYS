# CYS Veritabanı Güncelleme Uygulama Planı

Bu belge, eksik tabloların Ubuntu sunucudaki CYS veritabanına uygulanması için adım adım bir plan sunmaktadır.

## 1. Hazırlık Aşaması

### 1.1. Veritabanı Yedeği Alınması
```sql
-- Mevcut veritabanının yedeğini alın
mysqldump -u [kullanıcı_adı] -p CYS > cys_backup_$(date +%Y%m%d).sql
```

### 1.2. Yedek Doğrulama
```bash
# Yedeğin başarıyla alındığını kontrol edin
ls -lh cys_backup_*.sql
```

## 2. Eksik Tabloların Oluşturulması

### 2.1. eksik_tablolar.sql Betiğinin Yüklenmesi
```bash
# Dosyayı sunucuya kopyalayın
scp eksik_tablolar.sql [kullanıcı_adı]@[sunucu_adresi]:/tmp/

# MySQL'e bağlanıp betikleri çalıştırın
mysql -u [kullanıcı_adı] -p CYS < /tmp/eksik_tablolar.sql
```

### 2.2. Tablo Oluşturma Kontrolü
```sql
-- MySQL'de aşağıdaki sorguyu çalıştırarak yeni oluşturulan tabloları kontrol edin
USE CYS;
SHOW TABLES;
```

## 3. Dikkat Edilmesi Gereken Hususlar

### 3.1. Tablo İsimleri ve Büyük/Küçük Harf Duyarlılığı
- MySQL'in Linux'ta büyük/küçük harf duyarlı olduğunu unutmayın
- `AgirlikOlcum` ve `agirlikolcum` farklı tablolar olarak değerlendirilir
- Sorgularda ve kod içerisinde tablo isimlerinin doğru yazıldığından emin olun

### 3.2. Foreign Key Kısıtlamaları
- Eksik tabloların oluştururken foreign key kısıtlamalarına dikkat edin
- İlişkili tablolar ve alanlar doğru bir şekilde bağlanmalıdır
- Özellikle `Kullanici` ve `SyncLog` tabloları arasındaki ilişki kritiktir

### 3.3. Karakter Seti ve Collation Ayarları
- Türkçe karakter desteği için UTF-8 karakter setini kullandığınızdan emin olun
- `utf8_turkish_ci` collation ayarını kullanın

## 4. Mobil Uygulama Entegrasyonu için Ek Adımlar

### 4.1. Mobil API Anahtarları Güncelleme
```sql
-- Mobil uygulama için API anahtarları ekleyin
INSERT INTO `processsetting` (`settingKey`, `settingValue`, `isActive`)
VALUES ('MOBILE_API_KEY', '[rastgele_güvenli_anahtar]', 1),
       ('MOBILE_API_TIMEOUT', '3600', 1);
```

### 4.2. Örnek Cihaz Tanımlama
```sql
-- Test için örnek cihaz tanımlayın
INSERT INTO `devices` (`devicename`, `deviceguid`, `devicetype`, `active`)
VALUES ('Test Android', 'test-device-guid-001', 1, 1);
```

## 5. Kullanıcı Yönetimi Kurulumu

### 5.1. Kullanıcı Entegrasyonu
```sql
-- Mevcut kullanıcıları Kullanici tablosuna aktarın
INSERT INTO `Kullanici` (`KullaniciAdi`, `Email`, `Sifre`, `Ad`, `Soyad`, `RolId`, `KayitTarihi`, `Aktif`)
SELECT u.userName, 'user@example.com', u.password, 'Kullanıcı', 'Adı', 1, NOW(), u.isActive
FROM `user` u
WHERE u.userName NOT IN (SELECT KullaniciAdi FROM `Kullanici`);
```

## 6. Doğrulama ve Test

### 6.1. Tablo Sayısı Doğrulama
```sql
-- Tüm tabloları listeleyin ve sayın
SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'CYS';
```

### 6.2. İlişki Doğrulama
```sql
-- Foreign key ilişkilerini doğrulayın
SELECT * 
FROM information_schema.key_column_usage 
WHERE referenced_table_schema = 'CYS';
```

### 6.3. Örnek Sorgular
```sql
-- Kullanıcı rolleri ve izinleri sorgusu
SELECT k.KullaniciAdi, r.RolAdi, i.IzinAdi
FROM Kullanici k
JOIN Rol r ON k.RolId = r.Id
JOIN RolIzin ri ON r.Id = ri.RolId
JOIN Izin i ON ri.IzinId = i.Id
ORDER BY k.KullaniciAdi, r.RolAdi;

-- Mobil cihaz kullanıcı ilişkisi
SELECT u.userName, d.devicename, d.deviceguid
FROM user u
JOIN userdevice ud ON u.id = ud.userid
JOIN devices d ON ud.deviceid = d.id
WHERE d.active = 1;
```

## 7. Uygulama Kodu Güncellemesi

Şu model sınıflarının, API kontrolcülerinin ve repository sınıflarının güncellenmesi veya oluşturulması gerekecektir:

1. Controller ve API Sınıfları:
   - DeviceController
   - UserSessionController
   - AuthorizationController (Rol ve İzin yönetimi için)
   - MobilSyncController (güncelleme)

2. Repository Sınıfları:
   - DeviceRepository
   - UserModuleRepository
   - UserDeviceRepository
   - RolRepository
   - IzinRepository

## 8. Geri Alma Planı

Eğer herhangi bir sorun oluşursa, veritabanını önceki durumuna geri almak için:

```sql
-- Önce mevcut veritabanını silin (dikkatli olun!)
DROP DATABASE CYS;

-- Veritabanını tekrar oluşturun
CREATE DATABASE CYS;

-- Yedeği geri yükleyin
mysql -u [kullanıcı_adı] -p CYS < cys_backup_[tarih].sql
``` 