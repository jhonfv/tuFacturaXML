# Guía de Desarrollo - tuFacturaXML

## 🎯 Objetivos del Proyecto

**tuFacturaXML** es una aplicación web para procesar facturas electrónicas DIAN (Colombia) con las siguientes características principales:

- Procesamiento de archivos XML y ZIP
- Validación de productos contra base de datos
- Interfaz web moderna y responsiva
- Entrada de mercancía automatizada
- Logging estructurado para auditoría

## 🏗️ Arquitectura del Proyecto

### Estructura de Capas

```
┌─────────────────────────────────────┐
│           tuFacturaXML.webApp       │ ← Capa de Presentación (MVC)
├─────────────────────────────────────┤
│           tuFacturaXML.api          │ ← API REST
├─────────────────────────────────────┤
│         tuFacturaXML.negocio        │ ← Capa de Negocio
├─────────────────────────────────────┤
│         tuFactura.utilitarios       │ ← Utilidades y Herramientas
├─────────────────────────────────────┤
│           tuFactura.data            │ ← Capa de Datos
└─────────────────────────────────────┘
```

### Principios de Diseño

1. **Separación de Responsabilidades**: Cada capa tiene una responsabilidad específica
2. **Inyección de Dependencias**: Uso de DI para desacoplar componentes
3. **Interfaces**: Definición de contratos claros entre capas
4. **Async/Await**: Uso consistente de programación asíncrona

## 📁 Estructura de Directorios

### tuFacturaXML.webApp
```
tuFacturaXML.webApp/
├── Controllers/           # Controladores MVC
│   ├── HomeController.cs
│   └── Facturas/
│       └── FacturasController.cs
├── Views/                 # Vistas Razor
│   ├── Home/
│   │   └── Index.cshtml
│   └── Facturas/
│       └── Details.cshtml
├── wwwroot/              # Archivos estáticos
│   ├── css/
│   ├── js/
│   └── lib/
└── Program.cs            # Configuración de la aplicación
```

### tuFactura.utilitarios
```
tuFactura.utilitarios/
├── Herramientas/
│   ├── Database/         # Acceso a datos
│   │   ├── IDatabaseService.cs
│   │   └── SqlServerDatabaseService.cs
│   ├── Facturas/         # Procesamiento de facturas
│   │   ├── Conversiones.cs
│   │   ├── ProcesarZIP.cs
│   │   └── Interfaces/
│   │       ├── IConversiones.cs
│   │       └── IProcesarZip.cs
│   └── Logging/          # Sistema de logging
│       ├── ILoggerService.cs
│       └── FileLoggerService.cs
└── Modelos/
    ├── DIAN/             # Modelos DIAN
    │   ├── Invoice.cs
    │   └── Attach.cs
    └── Factura/          # Modelos de negocio
        ├── XEntradaDeMercancia.cs
        ├── XEntradaDeMercanciaDetalle.cs
        └── ValidacionProducto.cs
```

## 🛠️ Tecnologías y Herramientas

### Backend
- **ASP.NET Core 8.0**: Framework web
- **C# 12**: Lenguaje de programación
- **Dapper 2.1.28**: Micro-ORM para acceso a datos
- **SQL Server**: Base de datos relacional
- **xUnit**: Framework de testing
- **Moq**: Framework de mocking

### Frontend
- **Bootstrap 5**: Framework CSS
- **JavaScript ES6+**: Interactividad del cliente
- **Font Awesome**: Iconografía
- **AJAX/Fetch API**: Comunicación asíncrona

## 📋 Convenciones de Código

### Nomenclatura

#### Clases e Interfaces
```csharp
// ✅ Correcto
public class SqlServerDatabaseService : IDatabaseService
public interface ILoggerService
public class XEntradaDeMercancia

// ❌ Incorrecto
public class sqlServerDatabaseService
public interface iloggerService
```

#### Métodos
```csharp
// ✅ Correcto
public async Task<List<ValidacionProducto>> ValidarProductosAsync(List<string> skus)
public async Task<int> InsertEntradaMercanciaAsync(XEntradaDeMercancia entrada)

// ❌ Incorrecto
public async Task<List<ValidacionProducto>> validarProductos(List<string> skus)
public async Task<int> insertEntradaMercancia(XEntradaDeMercancia entrada)
```

#### Variables y Propiedades
```csharp
// ✅ Correcto
private readonly string _connectionString;
public string SKU { get; set; }
public bool ExisteEnBaseDeDatos { get; set; }

// ❌ Incorrecto
private readonly string connectionString;
public string sku { get; set; }
public bool existeEnBaseDeDatos { get; set; }
```

### Estructura de Archivos

#### Controladores
```csharp
public class FacturasController : Controller
{
    private readonly IFacturasNegocio _facturasNegocio;
    private readonly ILoggerService _logger;

    public FacturasController(IFacturasNegocio facturasNegocio, ILoggerService logger)
    {
        _facturasNegocio = facturasNegocio;
        _logger = logger;
    }

    // Métodos públicos primero
    public async Task<IActionResult> Index()
    {
        // Implementación
    }

    // Métodos privados al final
    private async Task<ResultadoProcesamiento> ProcesarArchivos(List<IFormFile> files)
    {
        // Implementación
    }
}
```

#### Servicios de Base de Datos
```csharp
public class SqlServerDatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public SqlServerDatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<ValidacionProducto>> ValidarProductosAsync(List<string> skus)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Implementación con Dapper
            return await connection.QueryAsync<ValidacionProducto>(sql, parameters);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error validating products: {ex.Message}", ex);
        }
    }
}
```

## 🔧 Configuración y Dependencias

### Configuración de Dependencias

#### Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar logging
builder.Services.AddSingleton<ILoggerService, FileLoggerService>();

// Configurar servicios de negocio
builder.Services.AddScoped<IFacturasNegocio, FacturasNegocio>();
builder.Services.AddScoped<IEntradaMercanciaNegocio, EntradaMercanciaNegocio>();

// Configurar servicios de utilidades
builder.Services.AddScoped<IConversiones, Conversiones>();
builder.Services.AddScoped<IProcesarZip, ProcesarZIP>();

// Configurar servicios de base de datos
builder.Services.AddScoped<IDatabaseService, SqlServerDatabaseService>();
```

### Configuración de Base de Datos

#### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=APLBDSFE;User ID=sa;Password=tu_password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🧪 Testing

### Estructura de Tests

```
tuFacturaXML.negocio.Tests/
├── Facturas/
│   └── IFacturasNegocioTests.cs
├── EntradaMercancia/
│   └── IEntradaMercanciaNegocioTests.cs
└── tuFacturaXML.negocio.Tests.csproj
```

### Ejemplos de Tests

```csharp
[Fact]
public async Task ProcesarFacturaAsync_ConArchivoXMLValido_DeberiaRetornarResultadoExitoso()
{
    // Arrange
    var mockConversiones = new Mock<IConversiones>();
    var mockProcesarZip = new Mock<IProcesarZip>();
    var facturasNegocio = new FacturasNegocio(mockConversiones.Object, mockProcesarZip.Object);

    var files = new List<IFormFile>
    {
        CreateMockFile("factura.xml", "content")
    };

    // Act
    var resultado = await facturasNegocio.procesarFacturaAsync(files);

    // Assert
    Assert.NotNull(resultado);
    Assert.True(resultado.Facturas.Count > 0);
}
```

### Ejecutar Tests
```bash
dotnet test
dotnet test --verbosity normal
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Logging

### Configuración de Logging

```csharp
public class FileLoggerService : ILoggerService
{
    private readonly string _logFilePath;

    public FileLoggerService()
    {
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
    }

    public void LogInformation(string message, object? data = null)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            Level = "Information",
            Message = message,
            Data = data
        };

        var jsonLog = JsonSerializer.Serialize(logEntry);
        File.AppendAllText(_logFilePath, jsonLog + Environment.NewLine);
    }
}
```

### Uso del Logging

```csharp
public async Task<IActionResult> CargarFactura(List<IFormFile> files)
{
    try
    {
        _logger.LogInformation("Iniciando carga de facturas", new { FileCount = files?.Count ?? 0 });
        
        // Procesamiento
        
        _logger.LogInformation("Carga de facturas completada exitosamente");
        return RedirectToAction("Details", new { facturaIndex = 0 });
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error al cargar facturas: {ex.Message}", ex);
        return View("Error");
    }
}
```

## 🚀 Despliegue

### Requisitos de Producción

- **Sistema Operativo**: Windows Server 2019+ o Linux
- **Runtime**: .NET 8.0 Runtime
- **Base de Datos**: SQL Server 2019+
- **Web Server**: IIS o Kestrel
- **Memoria**: Mínimo 4GB RAM
- **Almacenamiento**: Mínimo 10GB espacio libre

### Pasos de Despliegue

1. **Publicar la aplicación**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configurar IIS (Windows)**
   - Crear sitio web en IIS
   - Configurar pool de aplicaciones
   - Establecer permisos de carpeta

3. **Configurar variables de entorno**
   ```bash
   set ASPNETCORE_ENVIRONMENT=Production
   set ConnectionStrings__DefaultConnection="Data Source=server;Initial Catalog=database;User ID=user;Password=pass;"
   ```

4. **Verificar permisos**
   - Permisos de lectura/escritura en carpeta de logs
   - Permisos de base de datos para el usuario de aplicación

## 🔒 Seguridad

### Mejores Prácticas

1. **Validación de Entrada**
   ```csharp
   if (files == null || !files.Any())
   {
       return BadRequest("No se proporcionaron archivos");
   }
   ```

2. **Sanitización de Datos**
   ```csharp
   var sanitizedSku = sku?.Trim().ToUpper();
   ```

3. **Manejo de Excepciones**
   ```csharp
   try
   {
       // Operación crítica
   }
   catch (SqlException ex)
   {
       _logger.LogError($"Error de base de datos: {ex.Message}", ex);
       throw new InvalidOperationException("Error al acceder a la base de datos", ex);
   }
   ```

4. **Logging Seguro**
   - No loggear información sensible
   - Usar niveles de log apropiados
   - Rotar archivos de log

## 📊 Monitoreo

### Métricas Importantes

- **Tiempo de respuesta**: < 2 segundos para operaciones normales
- **Uso de memoria**: < 80% de RAM disponible
- **Errores**: < 1% de requests con error
- **Disponibilidad**: > 99.9%

### Logs de Monitoreo

```csharp
public void LogPerformance(string operation, TimeSpan duration)
{
    _logger.LogInformation($"Performance: {operation} completed in {duration.TotalMilliseconds}ms");
}
```

## 🔄 Mantenimiento

### Tareas Regulares

1. **Backup de Base de Datos**: Diario
2. **Rotación de Logs**: Semanal
3. **Actualización de Dependencias**: Mensual
4. **Revisión de Seguridad**: Trimestral

### Comandos Útiles

```bash
# Limpiar build
dotnet clean

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Publicar
dotnet publish -c Release
```

## 📚 Recursos Adicionales

### Documentación
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Dapper Documentation](https://dapper-tutorial.net/)
- [Bootstrap Documentation](https://getbootstrap.com/docs/)

### Herramientas de Desarrollo
- **Visual Studio 2022**: IDE principal
- **SQL Server Management Studio**: Gestión de base de datos
- **Postman**: Testing de API
- **Git**: Control de versiones

---

**Última actualización**: Diciembre 2024  
**Versión**: 1.0.0 