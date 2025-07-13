0# CYS Veritabanı Tabloları Açıklamaları

Bu belge, Çiftlik Yönetim Sistemi (CYS) veritabanındaki mevcut ve eksik tabloların detaylı açıklamalarını içermektedir.

## Mevcut Tablolar

### Hayvan Yönetimi

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `hayvan` | Hayvan kaydı bilgilerini saklar | id, userId, rfidKodu, kupeIsmi, cinsiyet, agirlik, aktif, tarih, kategoriId, requestId, sonGuncelleme |
| `kategori` | Hayvan kategorilerini tanımlar | id, kategoriAdi, isActive, ustKategoriId |
| `hayvankriterunsur` | Hayvanların kriter unsurlarıyla ilişkilerini saklar | id, hayvanId, kriterUnsurId, isActive |
| `kupeatama` | Küpe atama işlemlerini saklar | id, userId, kupeRfid, aktif, tarih, requestId |
| `kupehayvan` | Küpe-hayvan ilişkilerini saklar | id, hayvanId, kupeId, isActive |
| `soyagaci` | Hayvanların soyağacı bilgilerini saklar | id, ustHayvanId, hayvanId, isActive |

### Ölçüm ve Ağırlık Yönetimi

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `agirlikolcum` | Hayvanların ağırlık ölçümlerini saklar | id, hayvanId, agirlik, olcumTarihi, bluetoothOlcum, userId, olcumNotu, requestId, agirlikOlcumu, aktif, tarih |
| `AgirlikOlcum` | Ağırlık ölçümlerini saklar (büyük harf duyarlı) | id, hayvanId, agirlik, olcumTarihi, bluetoothOlcum, userId, olcumNotu, requestId, agirlikOlcumu, aktif, tarih |
| `agirlikhayvan` | Hayvan-ağırlık ilişkisini saklar | id, hayvanId, agirlikId, tarih, requestId |
| `agirliksu` | Ağırlık ve su ölçümlerini saklar | id, olcumId, suMiktari, isActive |
| `dogumagirlikolcum` | Doğum sırasındaki ağırlık ölçümlerini saklar | id, hayvanId, agirlik, dogumTarihi, olcumTarihi, bluetoothOlcum, userId, olcumNotu, anneRfid, dogumYeri, requestId, agirlikOlcumu, aktif, tarih |
| `suttenkesimagirlikolcum` | Sütten kesim sırasındaki ağırlık ölçümlerini saklar | id, hayvanId, agirlik, kesimTarihi, olcumTarihi, bluetoothOlcum, userId, olcumNotu, anneRfid, kesimYasi, requestId, agirlikOlcumu, aktif, tarih |
| `olcum` | Genel ölçüm kayıtlarını saklar | id, userId, hayvanId, olcumTarihi, isActive |
| `olcumSession` | Ölçüm oturumlarını saklar | id, userId, sessionBaslangic, sessionBitis |
| `mobilolcum` | Mobil cihazlardan yapılan ölçümleri saklar | Id, Rfid, Weight, CihazId, AmacId, Amac, HayvanId, Tarih, OlcumTipi |
| `kantarayari` | Kantar cihazlarının kalibrasyon ayarlarını saklar | id, kullaniciId, cihazId, ayarDegeri, olcumTarihi, aktif |

### Kriter ve Özellikler

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `kriter` | Değerlendirme kriterlerini tanımlar | id, kriterAdi, isActive, badge |
| `kriterunsur` | Kriterlere ait unsurları tanımlar | id, kriterId, unsurAdi, isActive |
| `konum` | Konum bilgilerini saklar | id, konumAdi, isActive |

### İşlem ve Süreç Yönetimi

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `process` | İşlem/süreç kaydını tutar | id, userId, processTypeId, tarih, isActive |
| `processsetting` | İşlem/süreç ayarlarını saklar | id, settingKey, settingValue, isActive |
| `processtype` | İşlem/süreç tiplerini tanımlar | id, typeName, isActive |
| `surec` | Süreç bilgilerini saklar | id, surecAdi, surecTarihi, surecDurumu, surecTipi |
| `sureclog` | Süreç loglarını saklar | id, surecId, logTarihi, logAciklamasi, isActive |

### Profil ve Kullanıcı

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `profile` | Kullanıcı profil bilgilerini saklar | id, userId, ad, soyad, isletmeAdi, adres, telefon, email, logo, isActive |
| `user` | Temel kullanıcı bilgilerini saklar | id, userName, password, isActive |

## Eksik Tablolar

### Mobil Uygulama Entegrasyonu

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `devices` | Mobil cihazları tanımlar | id, devicename, deviceguid, devicetype, active |
| `userdevice` | Kullanıcı cihaz ilişkilerini saklar | id, deviceid, userid, active |
| `modules` | Sistem modüllerini tanımlar | id, modulename, ispublic |
| `usermodule` | Kullanıcı modül ilişkilerini saklar | id, userid, moduleid, aktif |
| `usersession` | Kullanıcı oturumlarını saklar | id, userid, devicename, deviceguid, devicekey, sessiontimeout, ipaddress |

### Kullanıcı Yönetimi

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `Kullanici` | Gelişmiş kullanıcı bilgilerini saklar | Id, KullaniciAdi, Email, Sifre, Ad, Soyad, Telefon, RolId, ProfilResmi, KayitTarihi, SonGirisTarihi, Aktif, Tema, Dil |
| `Rol` | Kullanıcı rollerini tanımlar | Id, RolAdi, Aciklama, OlusturmaTarihi, Aktif |
| `Izin` | Sistem izinlerini tanımlar | Id, IzinAdi, IzinKodu, Aciklama, Modul, Aktif |
| `RolIzin` | Rol-izin ilişkilerini saklar | Id, RolId, IzinId |
| `users` | Alternatif kullanıcı bilgilerini saklar | id, name, email, created_at, updated_at, password_digest, login_name, vb. |

### Doğum ve Hayvan Yönetimi

| Tablo Adı | Açıklama | Alanlar |
|-----------|----------|---------|
| `DogumYavru` | Doğum kayıtlarına ait yavru bilgilerini saklar | Id, DogumId, HayvanId, Cinsiyet, DogumAgirligi, KupeNo, Notlar, Yasayan, KayitTarihi |
| `ustkategori` | Hayvan kategorilerinin üst kategorilerini saklar | id, name, isActive |

## İlişkiler ve Bağlantılar

- `hayvan` tablosu `kategori` tablosuna kategoriId ile bağlanır
- `kategori` tablosu `ustkategori` tablosuna ustKategoriId ile bağlanır
- `hayvankriterunsur` tablosu `hayvan` ve `kriterunsur` tablolarına bağlanır
- `kriterunsur` tablosu `kriter` tablosuna kriterId ile bağlanır
- `Kullanici` tablosu `Rol` tablosuna RolId ile bağlanır
- `RolIzin` tablosu `Rol` ve `Izin` tablolarını birbirine bağlar
- `agirlikolcum`, `dogumagirlikolcum` ve `suttenkesimagirlikolcum` tabloları `hayvan` tablosuna hayvanId ile bağlanır
- `userdevice` tablosu `user` ve `devices` tablolarını birbirine bağlar
- `usermodule` tablosu `user` ve `modules` tablolarını birbirine bağlar

## Veritabanı İşlemleri

Eksik tablolar oluşturulduğunda, CYS sistemi aşağıdaki özellikleri kazanacaktır:

1. **Mobil Uygulama Entegrasyonu:** Mobil cihazlar ve kullanıcı oturumları ile ilgili işlemler
2. **Genişletilmiş Kullanıcı Yönetimi:** Roller ve izin sistemi ile daha güvenli ve yönetilebilir kullanıcı yetkilendirmesi
3. **Doğum Yönetimi:** Doğum kayıtları ve yavru takibi
4. **Kategori Yönetimi:** Üst kategorilerle daha kapsamlı hayvan sınıflandırılması 