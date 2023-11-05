using tuFactura.utilitarios.Herramientas.Facturas;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;
using tuFacturaXML.negocio.Facturas;

var builder = WebApplication.CreateBuilder(args);

// Dependencias
builder.Services.AddScoped<IConversiones, Conversiones>();
builder.Services.AddScoped<IProcesarZip, ProcesarZIP>();
builder.Services.AddScoped<IFacturasNegocio, FacturasNegocio>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
