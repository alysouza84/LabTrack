using LabTrack.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Le a connection string "LabTrack"do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("LabTrack")
    ?? throw new InvalidOperationException("Connection string 'LabTrack' não configurada.");

// Registra o LabTrackContext no container do injeção de dependencia, usando SQlite
builder.Services.AddDbContext<LabTrackContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// GET: / -> verifica se a API esta no ar;
app.MapGet("/", () => Results.Ok (new { mensagem = "LabTrack API working!"}));

app.Run();