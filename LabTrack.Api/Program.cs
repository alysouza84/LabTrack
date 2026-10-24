using LabTrack.Api.Data;
using LabTrack.Api.Models;
using LabTrack.Api.Requests;
using Microsoft.AspNetCore.Mvc;
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

// GET: /api/equipamentos -> lista todos os equipamentos, em ordem alfabetica
app.MapGet("/api/equipamentos", async ([FromServices] LabTrackContext context) =>
{
    var equipamentos = await context.Equipamentos
        .AsNoTracking()
        .OrderBy(e => e.Nome)
        .ToListAsync();
    
    return Results.Ok(equipamentos);
});

// GET: /api/equipamentos/{id.guid} -> busca um equipamento pelo id
app.MapGet("/api/equipamentos/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] LabTrackContext context) =>
{
    var equipamento = await context.Equipamentos
        .AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == id);

    if (equipamento is null)
    {
        return Results.NotFound(new { mensagem = "Equipamento não encontrado." });
    }

    return Results.Ok(equipamento);
});

// POST: /api/equipamentos -> cadast5ra um equipamento
app.MapPost("/api/equipamentos", async (
    [FromBody] EquipamentoRequest requisicao,
    [FromServices] LabTrackContext context) =>
{
    // Normaliza dos dados recebdo
    var nome = requisicao.Nome?.Trim();
    var patrimonio = requisicao.Patrimonio?.Trim().ToUpperInvariant();

    // Validacao 1: campos orbigatorios
    if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(patrimonio))
    {
        return Results.BadRequest(new { mensagem = "Nome e patrimonio são campos obrigatórios." });
    }

    // Validacao 2: tamhos maximmos dos campos
    if (nome.Length > 100 || patrimonio.Length > 50)
    {
        return Results.BadRequest(new { mensagem = "Nome deve ter no máximo 100 caracteres e patrimonio no máximo 50 caracteres." });
    }

    // Validacao 3: patrimonio unico
    var patrimonioExiste = await context.Equipamentos
        .AnyAsync(e => e.Patrimonio == patrimonio);
    
    if (patrimonioExiste)
    {
        return Results.Conflict(new { mensagem = "Já existe um equipamento com o mesmo patrimonio." });
    }

    var equipamento = new Equipamento
    {
        Nome = nome,
        Patrimonio = patrimonio
    };

    context.Equipamentos.Add(equipamento);
    await context.SaveChangesAsync();

    return Results.Created($"/api/equipamentos/{equipamento.Id}", equipamento);
});

app.Run();