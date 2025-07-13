# CYS - Çiftlik Yönetim Sistemi Web API

Bu proje, süt analizi ve hayvan yönetimi için ASP.NET Core 8.0 ile geliştirilmiş RESTful Web API sunucusudur.

## 🎯 Proje Özeti

**CYS (Çiftlik Yönetim Sistemi)**, süt analizi verilerini merkezi bir sunucuda toplayarak çiftlik yönetimi için kapsamlı bir API sağlar.

## 📋 Özellikler

### Core Features
- ✅ ASP.NET Core 8.0 Web API
- ✅ MySQL veritabanı ile Entity Framework Core
- ✅ JWT Bearer token authentication
- ✅ Swagger UI documentation
- ✅ CORS desteği
- ✅ Logging ve error handling
- ✅ Bluetooth terazi entegrasyonu
- ✅ Background service desteği

### API Endpoints
- ✅ Authentication (JWT token)
- ✅ Süt analiz verileri yönetimi
- ✅ Hayvan bilgileri ve ağırlık ölçümleri
- ✅ Kalite kontrol algoritmaları
- ✅ Toplu veri senkronizasyonu
- ✅ İstatistiksel raporlama

## 🛠️ Kurulum

### Gereksinimler
- .NET 8.0 SDK
- MySQL Server 8.0+
- Visual Studio 2022 (önerilen)

### Veritabanı Kurulumu
```bash
# MySQL'e bağlan
mysql -u root -p

# Veritabanını oluştur
CREATE DATABASE CYS;

# Kullanıcı oluştur
CREATE USER 'cysuser'@'localhost' IDENTIFIED BY '123456';
GRANT ALL PRIVILEGES ON CYS.* TO 'cysuser'@'localhost';
FLUSH PRIVILEGES;
```

### Proje Kurulumu
```bash
# Repository'yi klonla
git clone <repository-url>
cd CYSX-master/CYS

# Bağımlılıkları yükle
dotnet restore

# Migration'ları çalıştır
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
```

## 🔧 Konfigürasyon

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CYS;User=cysuser;Password=123456;AllowPublicKeyRetrieval=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyHere",
    "Issuer": "CYS-MilkAnalysis-Server",
    "Audience": "CYS-MilkAnalysis-Client",
    "ExpiryMinutes": 1440
  }
}
```

### Production Ayarları
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-server;Database=CYS;User=production-user;Password=secure-password;SslMode=Required;"
  },
  "JwtSettings": {
    "SecretKey": "production-secret-key-32-characters-long",
    "Issuer": "production-issuer",
    "Audience": "production-audience"
  }
}
```

## 📊 API Endpoints

### Authentication
```
POST /api/auth/token
POST /api/auth/refresh
```

### Süt Analiz Verileri
```
GET    /api/milkdata/analysis-results
POST   /api/milkdata/analysis-results
GET    /api/milkdata/samples
GET    /api/milkdata/animal-statistics/{animalId}
POST   /api/milkdata/sync
POST   /api/milkdata/quality-check
```

### Hayvan Yönetimi
```
GET    /api/animals
GET    /api/animals/by-rfid/{rfidTag}
GET    /api/animals/{id}
GET    /api/animals/{id}/weight-history
POST   /api/animals/{id}/weight
GET    /api/animals/{id}/herd-info
```

### Ağırlık Ölçümleri
```
GET    /api/weightmeasurement
POST   /api/weightmeasurement
GET    /api/weightmeasurement/{id}
PUT    /api/weightmeasurement/{id}
DELETE /api/weightmeasurement/{id}
```

## 🔐 Authentication

### JWT Token Alma
```bash
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "MERLAB-SutAnalizi",
    "clientSecret": "MerlabSutAnalizi2024!"
  }'
```

### Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 86400,
  "clientId": "MERLAB-SutAnalizi"
}
```

### API Kullanımı
```bash
curl -X GET http://localhost:5000/api/milkdata/analysis-results \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 📄 Veri Modelleri

### MilkAnalysisResult
```csharp
public class MilkAnalysisResult
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public DateTime AnalysisDate { get; set; }
    public string? AnalysisMethod { get; set; }
    public double? PhValue { get; set; }
    public double? FatPercentage { get; set; }
    public double? ProteinPercentage { get; set; }
    public double? LactosePercentage { get; set; }
    // ... diğer özellikler
}
```

### Animal
```csharp
public class Animal
{
    public int Id { get; set; }
    public string RfidTag { get; set; }
    public string? Name { get; set; }
    public string? Breed { get; set; }
    public DateTime? BirthDate { get; set; }
    public double? CurrentWeight { get; set; }
    // ... diğer özellikler
}
```

## 🧪 Test

### Swagger UI
Sunucu çalışırken: http://localhost:5000/swagger

### Unit Tests
```bash
# Test projesini çalıştır
dotnet test

# Coverage raporu
dotnet test --collect:"XPlat Code Coverage"
```

### Postman Collection
API test collection dosyası proje içinde mevcuttur.

## 📊 Kalite Kontrol

### TS 1018 Standardı
```csharp
var qualityStandards = new Dictionary<string, (decimal min, decimal max)>
{
    {"fat_percentage", (3.0m, 6.0m)},
    {"protein_percentage", (2.8m, 4.5m)},
    {"ph_value", (6.6m, 6.8m)},
    {"density", (1.028m, 1.034m)},
    {"bacteria_count", (0m, 100000m)},
    {"somatic_cell_count", (0m, 400000m)}
};
```

### Kalite Skoru Hesaplama
```csharp
public decimal CalculateQualityScore(double? fatPercentage, double? proteinPercentage, double? bacteriaCount)
{
    decimal score = 100;
    
    // Yağ oranı değerlendirmesi
    if (fatPercentage.HasValue)
    {
        if (fatPercentage < 3.0 || fatPercentage > 5.0)
            score -= 20;
    }
    
    // Protein değerlendirmesi
    if (proteinPercentage.HasValue)
    {
        if (proteinPercentage < 2.8)
            score -= 25;
    }
    
    // Bakteri sayısı değerlendirmesi
    if (bacteriaCount.HasValue)
    {
        if (bacteriaCount > 100000)
            score -= 30;
    }
    
    return Math.Max(score, 0);
}
```

## 🚀 Deployment

### IIS Deployment
```bash
# Publish
dotnet publish -c Release -o ./publish

# IIS'e kopyala
xcopy /s /y publish C:\inetpub\wwwroot\CYS\
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CYS.csproj", "."]
RUN dotnet restore "./CYS.csproj"
COPY . .
RUN dotnet build "CYS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CYS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CYS.dll"]
```

### Windows Service
```bash
# Service olarak yükle
sc create CYSService binpath="C:\path\to\CYS.exe"
sc start CYSService
```

## 🔄 Background Services

### CYSBackgroundService
```csharp
public class CYSBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Periyodik görevler
            await ProcessPendingData();
            await CheckSystemHealth();
            await CleanupOldData();
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

## 📈 Performans Optimizasyonu

### Database Indexleri
```sql
-- RFID için unique index
CREATE UNIQUE INDEX IX_Animals_RfidTag ON Animals(RfidTag);

-- Analiz tarihleri için index
CREATE INDEX IX_MilkAnalysisResults_AnalysisDate ON MilkAnalysisResults(AnalysisDate);

-- Hayvan ID'leri için index
CREATE INDEX IX_MilkAnalysisResults_AnimalId ON MilkAnalysisResults(AnimalId);
```

### Connection Pooling
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));
```

## 🚨 Sorun Giderme

### Yaygın Sorunlar
1. **Database Connection**: Connection string kontrolü
2. **JWT Token**: Secret key ve audience kontrolü
3. **CORS**: Client domain'i allowed origins'e ekleme
4. **Memory Leaks**: Dispose pattern kontrolü

### Logging
```csharp
// Startup.cs
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    builder.AddFile("Logs/cys-{Date}.log");
});
```

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır.

## 📞 İletişim

- **Email**: hasanhuseyinyetkiner@gmail.com
- **GitHub**: [@hasanhuseyinyetkiner](https://github.com/hasanhuseyinyetkiner)
