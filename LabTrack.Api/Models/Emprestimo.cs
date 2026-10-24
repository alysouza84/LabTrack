namespace LabTrack.Api.Models;

public class Emprestimo{
    //id do emprestimo
    public Guid Id{get; set;} = Guid.NewGuid();

    //id do aluno
    public Guid AlunoId{get; set;}

    //id do equipamento emprestado
    public Guid EquipamentoId{get; set;}

    //data do emprestimo
    public DateTime DataEmprestimo{get; set;} = DateTime.UtcNow;

    //Null da devolução ´´e um equipamento nao devolvido
    public DateTime? DataDevolucao{get; set;}
}