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

// o PUT: /api/equipamentos/{id} atualiza um equipamento existente
app.MapPut("/api/equipamentos/{id:guid}", async(
    [FromRoute] Guid id,
    [FromBody] EquipamentoRequest requisicao,
    [FromServices] LabTrackContext context) =>{
        //Busca por ID
        var equipamento = await context.Equipamentos.FirstOrDefaultAsync(e => e.Id == id);

        if(equipamento is null){
            return Results.NotFound(new{ mensagem = "Equipamento não encontrado."});
        }

        //normalização dos dados
        var nome = requisicao.Nome?.Trim();
        var patrimonio = requisicao.Patrimonio?.Trim().ToUpperInvariant();

        //validação dos campos
        if(string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(patrimonio)){
            return Results.BadRequest(new{ mensagem = "Nome e patrimonio são campos obrigatórios."});
        }

        //validação do tamanho
        if(nome.Length > 100 || patrimonio.Length > 50){
            return Results.BadRequest(new{  mensagem = "Nome tem limite máximo de 100 caracteres e patrimonio máximo de 50 caracteres."});
        }

        //verificca duplicidade de patrimonio
        var patrimonioExiste = await context.Equipamentos.AnyAsync(e => e.Patrimonio == patrimonio && e.Id != id);
        if (patrimonioExiste){return Results.Conflict(new{ mensagem = "Já existe equipamento com esse patrimonio"});}

        //atualiza os dados do equipamento
        equipamento.Nome = nome;
        equipamento.Patrimonio = patrimonio;

        //save
        await context.SaveChangesAsync();

        return Results.Ok(equipamento);
    }
);

//DELETE: /api/equipamentos/{id} exclui o equipamento
app.MapDelete("/api/equipamentos/{id:guid}", async(
    [FromRoute] Guid id, 
    [FromServices] LabTrackContext context) => {
        //busca pelo id
        var equipamento = await context.Equipamentos.FirstOrDefaultAsync(e => e.Id == id);

        //verifica existencia
        if(equipamento is null){return Results.NotFound(new{mensagem = "Equipamento não encontrado."});}

        //marca para exclusao
        context.Equipamentos.Remove(equipamento);

        //confirma a exclusão no banco
        await context.SaveChangesAsync();

        //sucesso
        return Results.NoContent();
    }
);

app.Run();