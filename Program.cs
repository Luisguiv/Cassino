using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ApiCassino.Data;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Repositories.Implementations;
using ApiCassino.Services.Interfaces;
using ApiCassino.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework com MySQL + Retry Policy
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 28)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173") // Porta padrão do Vite
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Registrar Repositories
builder.Services.AddScoped<IJogadorRepository, JogadorRepository>();
builder.Services.AddScoped<ICarteiraRepository, CarteiraRepository>();
builder.Services.AddScoped<IApostaRepository, ApostaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

// Registrar Services
builder.Services.AddScoped<IJogadorService, JogadorService>();
builder.Services.AddScoped<IApostaService, ApostaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cassino API",
        Version = "v1",
        Description = "API para sistema de apostas com .NET 6 e MySQL",
        Contact = new OpenApiContact
        {
            Name = "Desenvolvedor",
            Email = "dev@exemplo.com"
        }
    });
});

var app = builder.Build();

// Configurar o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevPolicy");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cassino API V1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();