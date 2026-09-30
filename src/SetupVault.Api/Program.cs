using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SetupVault.Api.Data;
using SetupVault.Api.Middlewares;
using SetupVault.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Controllers + JSON ----------
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Enums trafegam como texto ("Mousepad") em vez de número
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ---------- Banco de dados (EF Core + SQLite) ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Injeção de dependência ----------
builder.Services.AddScoped<IFabricanteService, FabricanteService>();
builder.Services.AddScoped<IPerifericoService, PerifericoService>();

// ---------- Tratamento global de erros ----------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------- Versionamento (/api/v1/...) ----------
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true; // header api-supported-versions
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";      // grupo "v1" no Swagger
        options.SubstituteApiVersionInUrl = true; // mostra /api/v1/... em vez de /api/v{version}/...
    });

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SetupVault API",
        Version = "v1",
        Description = "API RESTful para catálogo e controle de estoque de periféricos (CP5 - C# Software Development)."
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Aplica as migrations pendentes ao iniciar (cria o arquivo setupvault.db se não existir)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SetupVault API v1");
        options.DocumentTitle = "SetupVault API";
    });
}

app.MapControllers();

app.Run();
