import { useEffect, useState } from "react";
import { API_URL } from "../config";
import { Equipamento } from "../models/Equipamento";
import { ERRO_CONEXAO } from "../utils";
import Mensagem, { MensagemTela } from "./Mensagem";

function Disponiveis() {
  const [disponiveis, setDisponiveis] = useState<Equipamento[]>([]);
  const [mensagem, setMensagem] = useState<MensagemTela>(null);
  const [atualizacao, setAtualizacao] = useState(0);

  // GET: equipamentos sem empréstimo ativo (calculado pela API)
  useEffect(() => {
    fetch(`${API_URL}/equipamentos/disponiveis`)
      .then((resposta) => resposta.json())
      .then((dados: Equipamento[]) => {
        setDisponiveis(dados);
        setMensagem(null);
      })
      .catch(() => setMensagem({ tipo: "erro", texto: ERRO_CONEXAO }));
  }, [atualizacao]);

  return (
    <section>
      <Mensagem mensagem={mensagem} />

      <div className="card">
        <div className="card-cabecalho">
          <h2>Equipamentos disponíveis para empréstimo</h2>
          <button
            className="botao botao-pequeno botao-secundario"
            onClick={() => setAtualizacao((valor) => valor + 1)}
          >
            Atualizar
          </button>
        </div>
        <table className="tabela">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Patrimônio</th>
            </tr>
          </thead>
          <tbody>
            {disponiveis.length === 0 && (
              <tr>
                <td colSpan={2} className="tabela-vazia">
                  Nenhum equipamento disponível no momento.
                </td>
              </tr>
            )}
            {disponiveis.map((equipamento) => (
              <tr key={equipamento.id}>
                <td>{equipamento.nome}</td>
                <td><span className="codigo">{equipamento.patrimonio}</span></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default Disponiveis;
