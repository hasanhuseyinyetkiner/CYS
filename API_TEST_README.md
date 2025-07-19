# API Test con PostgreSQL

Este documento explica cómo configurar PostgreSQL y probar la API de CYS.

## Configuración de PostgreSQL

1. Instalar PostgreSQL si no lo tienes ya instalado:
   - Windows: Descarga e instala desde [PostgreSQL Downloads](https://www.postgresql.org/download/windows/)
   - Linux: `sudo apt-get install postgresql postgresql-contrib`
   - macOS: `brew install postgresql`

2. Crear una base de datos para la API de prueba:
   ```sql
   CREATE DATABASE cys_api_test;
   ```

3. Ejecutar el script SQL para crear la tabla:
   - El script se encuentra en `DbScripts/test_api_weights.sql`
   - Puedes ejecutarlo con pgAdmin o con la herramienta de línea de comandos psql:
     ```bash
     psql -U postgres -d cys_api_test -f DbScripts/test_api_weights.sql
     ```

4. Verificar la conexión en `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "PostgreSQLConnection": "Host=localhost;Port=5432;Database=cys_api_test;Username=postgres;Password=postgres;"
   }
   ```
   - Ajusta el nombre de usuario y contraseña según tu configuración de PostgreSQL

## Probar la API con Postman

1. Importar la colección de Postman:
   - Abre Postman
   - Haz clic en "Import" y selecciona el archivo `postman_collection.json`

2. Probar los endpoints:
   - **Ping API**: Verifica que la API esté funcionando
   - **Get All Weights**: Obtiene todas las mediciones de peso
   - **Add Weight**: Añade una nueva medición de peso
   - **Delete Weight**: Elimina una medición de peso por ID
   - **Clear All Weights**: Elimina todas las mediciones de peso

## Integración con las aplicaciones cliente

### Aplicación Qt (C++)
La aplicación Qt ya está configurada para conectarse a `http://localhost:5050/api/test/`. Si necesitas cambiar la URL:
```cpp
// En apiclient.cpp
ApiClient::ApiClient(QObject *parent) 
    : QObject(parent)
    , m_networkManager(new QNetworkAccessManager(this))
    , m_baseUrl("http://localhost:5050/api/test")
    , m_connected(false)
{
    qDebug() << "ApiClient initialized";
}
```

### Aplicación Flutter (Mobile)
La aplicación Flutter está configurada para conectarse a `http://10.0.2.2:5050/api/test`. Si necesitas cambiar la URL:
```dart
// En test_api_service.dart
final RxString baseUrl = 'http://10.0.2.2:5050/api/test'.obs;
```
- Nota: `10.0.2.2` es la dirección IP especial que usa el emulador de Android para acceder a localhost del host.

## Solución de problemas

1. **Error de conexión a PostgreSQL**:
   - Verifica que PostgreSQL esté en ejecución
   - Comprueba las credenciales en `appsettings.json`
   - Asegúrate de que la base de datos `cys_api_test` exista

2. **Error al ejecutar la API**:
   - Verifica que el paquete NuGet `Npgsql.EntityFrameworkCore.PostgreSQL` esté instalado
   - Comprueba los logs para ver errores específicos

3. **Error de conexión desde las aplicaciones cliente**:
   - Verifica que la API esté en ejecución en el puerto 5050
   - Comprueba la configuración del firewall si es necesario
   - Para dispositivos físicos, asegúrate de usar la IP correcta en lugar de localhost 