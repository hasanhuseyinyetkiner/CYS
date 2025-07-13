#!/bin/bash

# Dizin oluşturma
mkdir -p /var/www/cysx

# Uygulama dosyalarını kopyalama
echo "Uygulama dosyaları kopyalanıyor..."
unzip -o cys-publish.zip -d /var/www/cysx

# Veritabanı oluşturma
echo "Veritabanı oluşturuluyor..."
mysql -u root -pMerlab.702642@CYS -e "CREATE DATABASE IF NOT EXISTS CYS;"
mysql -u root -pMerlab.702642@CYS -e "CREATE USER IF NOT EXISTS 'cysuser'@'localhost' IDENTIFIED BY '123456';"
mysql -u root -pMerlab.702642@CYS -e "GRANT ALL PRIVILEGES ON CYS.* TO 'cysuser'@'localhost';"
mysql -u root -pMerlab.702642@CYS -e "FLUSH PRIVILEGES;"

# Veritabanı tablolarını oluşturma
echo "Veritabanı tabloları oluşturuluyor..."
mysql -u root -pMerlab.702642@CYS CYS < create_tables.sql
mysql -u root -pMerlab.702642@CYS CYS < fix_database.sql

# Yapılandırma dosyasını güncelleme
echo "Yapılandırma dosyası güncelleniyor..."
cat > /var/www/cysx/appsettings.json << EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CYS;User=cysuser;Password=123456;AllowPublicKeyRetrieval=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "BluetoothSettings": {
    "MinValidWeight": 0.1,
    "MaxValidWeight": 2000.0,
    "DuplicateTimeWindowMinutes": 5
  }
}
EOF

# .NET servis oluşturma
echo ".NET servis yapılandırılıyor..."
cat > /etc/systemd/system/cysx.service << EOF
[Unit]
Description=CYS Animal Tracking Application
After=network.target

[Service]
WorkingDirectory=/var/www/cysx
ExecStart=/usr/bin/dotnet /var/www/cysx/CYS.dll
Restart=always
RestartSec=10
SyslogIdentifier=cysx
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF

# Servis başlatma
echo "Servis başlatılıyor..."
systemctl daemon-reload
systemctl enable cysx
systemctl start cysx

echo "Kurulum tamamlandı!" 