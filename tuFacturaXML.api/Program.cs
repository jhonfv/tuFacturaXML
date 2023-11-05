using tuFactura.utilitarios.Herramientas.Facturas;
using tuFactura.utilitarios.Herramientas.Facturas.Iterfaces;
using tuFacturaXML.negocio.Facturas;

var builder = WebApplication.CreateBuilder(args);

// Dependencias
builder.Services.AddScoped<IConversiones,Conversiones>();
builder.Services.AddScoped<IProcesarZip,ProcesarZIP>();
builder.Services.AddScoped<IFacturasNegocio, FacturasNegocio>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
