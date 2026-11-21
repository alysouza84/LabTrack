using System.Text.Json.Serialization;

namespace LabTrack.Api.Models;

public class Aluno
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Nome { get; set; } = string.Empty;
	public string Matricula { get; set; } = string.Empty;

	[JsonIgnore]
	public ICollection<Emprestimo> Emprestimos {get; set;} = new List<Emprestimo>(); 
}

