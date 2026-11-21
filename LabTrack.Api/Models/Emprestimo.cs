using System.Text.Json.Serialization;

namespace LabTrack.Api.Models;

public class Emprestimo{
    //id do emprestimo
    public Guid Id{get; set;} = Guid.NewGuid();

    //id do aluno
    public Guid AlunoId{get; set;}
    
    //navegação: aluno que pegou o equipamento
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Aluno? Aluno { get; set; }

    //id do equipamento emprestado
    public Guid EquipamentoId{get; set;}
    
    //navegação: equipamento emprestado
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Equipamento? Equipamento { get; set; }

    //data do emprestimo
    public DateTime DataEmprestimo{get; set;} = DateTime.UtcNow;

    //Null da devolução ´´e um equipamento nao devolvido
    public DateTime? DataDevolucao{get; set;}
}