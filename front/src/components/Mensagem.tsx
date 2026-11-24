// Tipo usado pelas telas para guardar a mensagem atual (ou null quando não há)
export type MensagemTela = {
  tipo: "sucesso" | "erro";
  texto: string;
} | null;

interface MensagemProps {
  mensagem: MensagemTela;
}

// Mostra uma faixa verde (sucesso) ou vermelha (erro); não mostra nada se for null
function Mensagem({ mensagem }: MensagemProps) {
  if (mensagem === null) {
    return null;
  }
  return <div className={`mensagem mensagem-${mensagem.tipo}`}>{mensagem.texto}</div>;
}

export default Mensagem;
