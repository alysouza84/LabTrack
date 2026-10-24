namespace LabTrack.Api.Requests;
//DTO de entrada para criar ou atualizar um equipamento. DTO (Data Transfer Object) é um objeto que transporta dados entre processos, nesse caso, entre a API e o cliente. Ele é usado para encapsular os dados que serão enviados ou recebidos pela API, garantindo que apenas as informações necessárias sejam expostas e manipuladas.
public record EquipamentoRequest(string? Nome, string? Patrimonio);

