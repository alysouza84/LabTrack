// A API grava as datas em UTC. Quando a data volta do banco, ela chega sem o
// "Z" no final, e o navegador a interpretaria como horário local (3h de erro).
// Por isso, se faltar a indicação de fuso, acrescentamos o "Z" antes de converter.
export function formatarData(data: string | null | undefined): string {
  if (!data) {
    return "—";
  }
  const temFuso = /(Z|[+-]\d{2}:\d{2})$/.test(data);
  const dataUtc = new Date(temFuso ? data : data + "Z");
  return dataUtc.toLocaleString("pt-BR");
}

// Lê a mensagem de erro enviada pela API no formato { "mensagem": "..." }.
// Se a resposta não tiver esse formato, devolve uma mensagem genérica com o status.
export async function lerMensagemErro(resposta: Response): Promise<string> {
  try {
    const corpo = await resposta.json();
    if (corpo && typeof corpo.mensagem === "string") {
      return corpo.mensagem;
    }
  } catch {
    // corpo vazio ou que não é JSON
  }
  return `Erro ${resposta.status} ao comunicar com a API.`;
}

export const ERRO_CONEXAO =
  "Não foi possível conectar à API. Verifique se ela está rodando (dotnet run).";
