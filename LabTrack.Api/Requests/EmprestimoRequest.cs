namespace LabTrack.Api.Requests;

//DTO de entrada para cadastro de emprestimo
public record EmprestimoRequest(
    Guid AlunoId,
    Guid EquipamentoId
);
