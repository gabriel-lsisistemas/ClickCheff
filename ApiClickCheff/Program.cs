using Microsoft.Extensions.Hosting;
using ApiClickCheff.Repositorio;
using ApiClickCheff.Dao;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Host.UseWindowsService();

// Configuração dos serviços e controladores
builder.Services.AddControllers();

// Registro de repositórios
builder.Services.AddScoped<RepositorioLogin>();
builder.Services.AddScoped<RepositorioMesas>();
builder.Services.AddScoped<RepositorioPagamento>();
builder.Services.AddScoped<RepositorioProduto>();
builder.Services.AddScoped<RepositorioComandas>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
