using System.Text.Json.Serialization;

namespace LabTrack.Api.Models;

public class Emprestimo{
    //id do emprestimo
    public Guid Id{get; set;} = Guid.NewGuid();

    //id do aluno
    public Guid AlunoId{get; set;}
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    //id do equipamento emprestado
    public Guid EquipamentoId{get; set;}
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    //data do emprestimo
    public DateTime DataEmprestimo{get; set;} = DateTime.UtcNow;

    //Null da devolução ´´e um equipamento nao devolvido
    public DateTime? DataDevolucao{get; set;}
}