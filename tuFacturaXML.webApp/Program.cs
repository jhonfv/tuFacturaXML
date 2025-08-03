using tuFactura.utilitarios.Herramientas.Database;
using tuFactura.utilitarios.Herramientas.Logging;
using tuFactura.utilitarios.Herramientas.Facturas;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;
using tuFacturaXML.negocio.EntradaMercancia;
using tuFacturaXML.negocio.Facturas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

// Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Facturas}/{action=Index}/{id?}");

app.Run();
