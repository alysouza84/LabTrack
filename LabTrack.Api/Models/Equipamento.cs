namespace LabTrack.Api.Models;

public class Equipamento
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Nome { get; set; } = string.Empty;
	public string Patrimonio { get; set; } = string.Empty;
}
