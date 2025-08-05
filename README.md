# tuFacturaXML

## Descripción del Proyecto

**tuFacturaXML** es una aplicación web desarrollada en ASP.NET Core para procesar facturas electrónicas DIAN (Colombia) en formato XML. La aplicación permite cargar archivos XML individuales o archivos ZIP que contengan múltiples facturas, procesar su contenido y validar productos contra una base de datos SQL Server.

## Características Principales

### ✅ Funcionalidades Implementadas

- **Carga de Archivos**: Soporte para archivos XML individuales y archivos ZIP con múltiples facturas
- **Procesamiento DIAN**: Deserialización y procesamiento de facturas electrónicas según estándares DIAN
- **Validación de Productos**: Validación de SKUs contra base de datos con soporte para productos principales y alternos
- **Interfaz Web**: Interfaz moderna y responsiva con Bootstrap 5
- **Edición en Línea**: Campos editables para SKU y descuentos con validación en tiempo real
- **Visualización de Impuestos**: Mostrar impuestos adicionales (INC, bolsa, impoconsumo, licores, cigarrillos, etc.)
- **Entrada de Mercancía**: Procesamiento de facturas como entrada de mercancía en el sistema
- **Logging Estructurado**: Sistema de logging para manejo de errores y auditoría
- **Tests Unitarios**: Pruebas unitarias para la capa de negocio

### 🏗️ Arquitectura del Proyecto

```
tuFacturaXML/
├── tuFacturaXML.webApp/          # Aplicación web principal (MVC)
├── tuFacturaXML.api/             # API REST para integraciones
├── tuFacturaXML.negocio/         # Lógica de negocio
├── tuFactura.utilitarios/        # Utilidades y herramientas
├── tuFactura.data/               # Capa de datos
└── tuFacturaXML.negocio.Tests/   # Tests unitarios
```

## Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework web
- **C# 12** - Lenguaje de programación
- **Dapper** - Micro-ORM para acceso a datos
- **SQL Server** - Base de datos
- **xUnit** - Framework de testing
- **Moq** - Framework de mocking

### Frontend
- **Bootstrap 5** - Framework CSS
- **JavaScript ES6+** - Interactividad del cliente
- **Font Awesome** - Iconografía
- **AJAX/Fetch API** - Comunicación asíncrona

### Herramientas de Desarrollo
- **Visual Studio 2022** / **VS Code**
- **Git** - Control de versiones
- **NuGet** - Gestión de paquetes

## Estructura de la Base de Datos

### Tablas Principales

#### Productos
- `Productos` - Productos principales del sistema
- `ProductosAlternos` - Códigos alternos de productos

#### Entrada de Mercancía
- `XEntradasDeMercancia` - Cabecera de entrada de mercancía
- `XEntradasDeMercanciaDetalle` - Detalles de entrada de mercancía

## Funcionalidades Detalladas

### 1. Carga y Procesamiento de Facturas

#### Formatos Soportados
- **XML Individual**: Facturas DIAN en formato XML
- **ZIP**: Archivos comprimidos con múltiples facturas XML

#### Procesamiento
- Deserialización automática de XML DIAN
- Extracción de información de proveedor y cliente
- Procesamiento de productos y líneas de factura
- Cálculo de impuestos (IVA, otros impuestos)

### 2. Validación de Productos

#### Características
- Validación en tiempo real de SKUs
- Búsqueda en productos principales y alternos
- Visualización de estado de validación
- Interfaz de edición inline

#### Estados de Validación
- ✅ **Encontrado**: Producto existe en base de datos
- ❌ **No encontrado**: Producto no existe
- ⚠️ **Modificado**: SKU editado, requiere re-validación

### 3. Interfaz de Usuario

#### Características de la Tabla
- **Diseño Excel-like**: Sin cuadrícula, sombras sutiles
- **Headers sticky**: Encabezados fijos al hacer scroll
- **Filas alternadas**: Colores alternados para mejor legibilidad
- **Responsive**: Adaptable a diferentes tamaños de pantalla

#### Columnas Disponibles
- **#**: Número de línea
- **SKU**: Código del producto (editable)
- **Descripción**: Descripción del producto
- **Cantidad**: Cantidad con unidad de medida
- **Unidad**: Código de unidad de medida
- **Precio Unit.**: Precio unitario
- **Descuento**: Descuento aplicado (editable)
- **% IVA**: Porcentaje de IVA
- **Valor con IVA**: Valor incluyendo IVA
- **Otros Impuestos**: Impuestos adicionales con iconos
- **Total**: Valor total de la línea

### 4. Impuestos Adicionales

#### Tipos de Impuestos Soportados
- **INC**: Impuesto Nacional al Consumo
- **Bolsa**: Impuesto a bolsas plásticas
- **Impoconsumo**: Impuesto al consumo
- **Licores**: Impuesto a licores
- **Cigarrillos**: Impuesto a cigarrillos
- **Retención**: Retenciones aplicables
- **Recargo**: Recargos adicionales

#### Visualización
- Icono de diamante azul (◆) para cada impuesto
- Porcentaje o valor según tipo de impuesto
- Soporte para impuestos dinámicos del XML

### 5. Entrada de Mercancía

#### Procesamiento
- Mapeo de datos XML a entidades de base de datos
- Validación de productos antes del procesamiento
- Generación de entrada de mercancía completa
- Logging de operaciones

## Configuración del Proyecto

### Requisitos Previos
- **.NET 8.0 SDK**
- **SQL Server** (2019 o superior)
- **Visual Studio 2022** o **VS Code**

### Instalación

1. **Clonar el repositorio**
   ```bash
   git clone [URL_DEL_REPOSITORIO]
   cd tuFacturaXML
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Configurar base de datos**
   - Crear base de datos SQL Server
   - Ejecutar scripts de creación de tablas
   - Configurar connection string en `appsettings.json`

4. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project tuFacturaXML.webApp
   ```

### Configuración de Base de Datos

#### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=APLBDSFE;User ID=sa;Password=tu_password;"
  }
}
```

## Estructura de Código

### Capa de Utilidades (`tuFactura.utilitarios`)

#### Herramientas de Base de Datos
- `IDatabaseService` - Interfaz para operaciones de BD
- `SqlServerDatabaseService` - Implementación con Dapper

#### Herramientas de Facturas
- `IConversiones` - Conversión de archivos
- `IProcesarZip` - Procesamiento de archivos ZIP
- `Conversiones` - Implementación de conversiones
- `ProcesarZIP` - Implementación de procesamiento ZIP

#### Modelos DIAN
- `InvoiceType` - Modelo de factura DIAN
- `AttachedDocumentType` - Modelo de documento adjunto

#### Modelos de Factura
- `XEntradaDeMercancia` - Entidad de entrada de mercancía
- `XEntradaDeMercanciaDetalle` - Detalle de entrada de mercancía
- `ValidacionProducto` - Resultado de validación de producto

### Capa de Negocio (`tuFacturaXML.negocio`)

#### Facturas
- `IFacturasNegocio` - Interfaz de lógica de facturas
- `FacturasNegocio` - Implementación de procesamiento de facturas

#### Entrada de Mercancía
- `IEntradaMercanciaNegocio` - Interfaz de entrada de mercancía
- `EntradaMercanciaNegocio` - Implementación de entrada de mercancía

### Capa Web (`tuFacturaXML.webApp`)

#### Controladores
- `HomeController` - Página principal
- `FacturasController` - Gestión de facturas

#### Vistas
- `Index.cshtml` - Página de carga de archivos
- `Details.cshtml` - Vista detallada de facturas procesadas

## Optimizaciones Recientes

### ✅ Limpieza de Código (Última Actualización)

#### Funciones Eliminadas
- `IProcesarXML` y `ProcesarXML` - No utilizados
- `TestConnectionAsync()` - Método de prueba no utilizado
- `GetProductoByReferenciaAsync()` - Reemplazado por Dapper
- `GetProductoAlternoByCodigoAsync()` - Reemplazado por Dapper
- `GetProductosAlternosByProductoIdAsync()` - No utilizado
- `GetProductoByIdAsync()` - Método privado no utilizado
- Métodos de mapeo manual (`MapEntradaMercancia`, `MapEntradaMercanciaDetalle`)

#### Migración a Dapper
- **Agregado Dapper 2.1.28** al proyecto
- **Optimización de queries** con queries paralelas
- **Eliminación de mapeo manual** propenso a errores
- **Reducción de ~400 líneas** de código no utilizado

#### Métodos Optimizados con Dapper
- `ValidarProductosAsync()` - Queries optimizadas con diccionarios
- `InsertEntradaMercanciaAsync()` - `ExecuteScalarAsync<int>()`
- `InsertEntradaMercanciaDetalleAsync()` - `ExecuteAsync()`
- `GetEntradasMercanciaAsync()` - `QueryAsync<T>()`
- `GetEntradaMercanciaByIdAsync()` - `QueryFirstOrDefaultAsync<T>()`
- `GetEntradaMercanciaDetallesAsync()` - `QueryAsync<T>()`
- `UpdateEntradaMercanciaAsync()` - `ExecuteAsync()`
- `DeleteEntradaMercanciaAsync()` - `ExecuteAsync()`

### Beneficios Obtenidos
- **Código más limpio** y mantenible
- **Mejor rendimiento** en consultas de base de datos
- **Menos errores** por eliminación de mapeo manual
- **Mejor legibilidad** del código
- **Funcionalidad preservada** al 100%

## Testing

### Tests Unitarios
- **Framework**: xUnit
- **Mocking**: Moq
- **Cobertura**: Capa de negocio
- **Ubicación**: `tuFacturaXML.negocio.Tests/`

### Ejecutar Tests
```bash
dotnet test
```

## Logging

### Sistema de Logging
- **Implementación**: `FileLoggerService`
- **Formato**: Estructurado
- **Ubicación**: Archivos de texto
- **Niveles**: Information, Warning, Error

### Configuración
```csharp
builder.Services.AddSingleton<ILoggerService, FileLoggerService>();
```

## Despliegue

### Requisitos de Producción
- **Windows Server** o **Linux**
- **IIS** o **Kestrel**
- **SQL Server** configurado
- **.NET 8.0 Runtime**

### Pasos de Despliegue
1. Publicar la aplicación
2. Configurar connection string
3. Configurar logging
4. Verificar permisos de base de datos

## Contribución

### Guías de Desarrollo
- Seguir convenciones de nomenclatura C#
- Mantener cobertura de tests
- Documentar cambios importantes
- Usar Dapper para acceso a datos

### Estructura de Commits
```
feat: nueva funcionalidad
fix: corrección de bug
refactor: refactorización de código
docs: actualización de documentación
test: agregar o modificar tests
```

## Licencia

Este proyecto es propiedad de la empresa y está destinado para uso interno.

## Contacto

Para soporte técnico o consultas sobre el proyecto, contactar al equipo de desarrollo.

---

**Versión**: 1.0.0  
**Última actualización**: Diciembre 2024  
**Estado**: En desarrollo activo