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

// GET: /api/alunos -> lista todos os alunos, em ordem alfabetica
app.MapGet("/api/alunos", async ([FromServices] LabTrackContext context) =>
{
   var alunos = await context.Alunos
        .AsNoTracking()
        .OrderBy(a => a.Nome)
        .ToListAsync();
    
    return Results.Ok(alunos); 
});

// GET: /api/alunos/{id} -> busca um aluno pelo id
app.MapGet("/api/alunos/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] LabTrackContext context) =>
{
    var aluno = await context.Alunos
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id);

    if (aluno is null)
    {
        return Results.NotFound(new { mensagem = "Aluno não encontrado." });
    }

    return Results.Ok(aluno);
});

// POST: /api/alunos -> cadastra um aluno
app.MapPost("/api/alunos", async (
    [FromBody] AlunoRequest requisicao,
    [FromServices] LabTrackContext context) =>
{
    // Normaliza os dados recebidos
    var nome = requisicao.Nome?.Trim();
    var matricula = requisicao.Matricula?.Trim();

    // Validação 1: campos obrigatórios
    if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(matricula))
    {
        return Results.BadRequest(new { mensagem = "Nome e matrícula são campos obrigatórios." });
    }

    // Validação 2: tamanhos máximos (o SQLite não verifica sozinho)
    if (nome.Length > 100 || matricula.Length > 20)
    {
        return Results.BadRequest(new { mensagem = "Nome deve ter no máximo 100 caracteres e matrícula no máximo 20." });
    }

    // Validação 3: matrícula única
    var matriculaExiste = await context.Alunos
        .AnyAsync(a => a.Matricula == matricula);

    if (matriculaExiste)
    {
        return Results.Conflict(new { mensagem = "Já existe um aluno com essa matrícula." });
    }

    var aluno = new Aluno
    {
        Nome = nome,
        Matricula = matricula
    };

    context.Alunos.Add(aluno);
    await context.SaveChangesAsync();

    return Results.Created($"/api/alunos/{aluno.Id}", aluno);
});


 // POST: /api/emprestimos registra um novo empréstimo
app.MapPost("/api/emprestimos", async (
    [FromBody] EmprestimoRequest requisicao,
    [FromServices] LabTrackContext context) =>
{
    // verifica se os identificadores foram informados
    if (requisicao.AlunoId == Guid.Empty ||
        requisicao.EquipamentoId == Guid.Empty)
    {
        return Results.BadRequest(new{mensagem = "AlunoId e EquipamentoId são obrigatórios."});
    }

    // verifica se o aluno existe
    var alunoExiste = await context.Alunos
        .AnyAsync(a => a.Id == requisicao.AlunoId);

    if (!alunoExiste){return Results.NotFound(new{mensagem = "Aluno não encontrado."});}

    // verifica se o equipamento existe
    var equipamentoExiste = await context.Equipamentos.AnyAsync(e => e.Id == requisicao.EquipamentoId);

    if (!equipamentoExiste){return Results.NotFound(new{ mensagem = "Equipamento não encontrado."});}

    // verifica se o equipamento já está emprestado
    var equipamentoEmprestado = await context.Emprestimos.AnyAsync(e =>
        e.EquipamentoId == requisicao.EquipamentoId &&
        e.DataDevolucao == null);

    if (equipamentoEmprestado){return Results.Conflict(new{mensagem = "Este equipamento já está emprestado."});}

    // Cria o empréstimo
    var emprestimo = new Emprestimo
    {
        AlunoId = requisicao.AlunoId,
        EquipamentoId = requisicao.EquipamentoId,
        DataEmprestimo = DateTime.UtcNow,
        DataDevolucao = null
    };

    context.Emprestimos.Add(emprestimo);

    try{await context.SaveChangesAsync();
    }catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx && sqliteEx.SqliteExtendedErrorCode == 2067)
    {
        // Trata violação índice único no SQLite
        return Results.Conflict(new{mensagem = "Conflito de dados: o equipamento pode já estar emprestado."});
    }

    return Results.Created($"/api/emprestimos/{emprestimo.Id}",emprestimo);
});


app.Run();