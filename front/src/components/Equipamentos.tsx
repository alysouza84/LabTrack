import { FormEvent, useEffect, useState } from "react";
import { API_URL } from "../config";
import { Equipamento } from "../models/Equipamento";
import { ERRO_CONEXAO, lerMensagemErro } from "../utils";
import Mensagem, { MensagemTela } from "./Mensagem";

function Equipamentos() {
  // Estados da tela
  const [equipamentos, setEquipamentos] = useState<Equipamento[]>([]);
  const [nome, setNome] = useState("");
  const [patrimonio, setPatrimonio] = useState("");
  const [idEmEdicao, setIdEmEdicao] = useState<string | null>(null);
  const [mensagem, setMensagem] = useState<MensagemTela>(null);
  const [atualizacao, setAtualizacao] = useState(0);

  // Carrega a lista ao abrir a tela e sempre que "atualizacao" mudar
  useEffect(() => {
    fetch(`${API_URL}/equipamentos`)
      .then((resposta) => resposta.json())
      .then((dados: Equipamento[]) => setEquipamentos(dados))
      .catch(() => setMensagem({ tipo: "erro", texto: ERRO_CONEXAO }));
  }, [atualizacao]);

  function limparFormulario() {
    setNome("");
    setPatrimonio("");
    setIdEmEdicao(null);
  }

  // POST (cadastrar) ou PUT (alterar), dependendo se há um equipamento em edição
  async function salvar(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setMensagem(null);

    const editando = idEmEdicao !== null;
    const url = editando
      ? `${API_URL}/equipamentos/${idEmEdicao}`
      : `${API_URL}/equipamentos`;

    try {
      const resposta = await fetch(url, {
        method: editando ? "PUT" : "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ nome, patrimonio }),
      });

      if (!resposta.ok) {
        setMensagem({ tipo: "erro", texto: await lerMensagemErro(resposta) });
        return;
      }

      setMensagem({
        tipo: "sucesso",
        texto: editando
          ? "Equipamento alterado com sucesso."
          : "Equipamento cadastrado com sucesso.",
      });
      limparFormulario();
      setAtualizacao((valor) => valor + 1);
    } catch {
      setMensagem({ tipo: "erro", texto: ERRO_CONEXAO });
    }
  }

  // Preenche o formulário com os dados do equipamento escolhido
  function editar(equipamento: Equipamento) {
    setNome(equipamento.nome);
    setPatrimonio(equipamento.patrimonio);
    setIdEmEdicao(equipamento.id);
    setMensagem(null);
  }

  // DELETE: a API responde 409 se o equipamento tiver empréstimos registrados
  async function excluir(equipamento: Equipamento) {
    const confirmado = window.confirm(
      `Excluir o equipamento "${equipamento.nome}" (${equipamento.patrimonio})?`,
    );
    if (!confirmado) {
      return;
    }
    setMensagem(null);

    try {
      const resposta = await fetch(`${API_URL}/equipamentos/${equipamento.id}`, {
        method: "DELETE",
      });

      if (!resposta.ok) {
        setMensagem({ tipo: "erro", texto: await lerMensagemErro(resposta) });
        return;
      }

      setMensagem({ tipo: "sucesso", texto: "Equipamento excluído com sucesso." });
      if (idEmEdicao === equipamento.id) {
        limparFormulario();
      }
      setAtualizacao((valor) => valor + 1);
    } catch {
      setMensagem({ tipo: "erro", texto: ERRO_CONEXAO });
    }
  }

  return (
    <section>
      <Mensagem mensagem={mensagem} />

      <div className="card">
        <h2>{idEmEdicao ? "Editar equipamento" : "Cadastrar equipamento"}</h2>
        <form className="formulario" onSubmit={salvar}>
          <div className="campo">
            <label htmlFor="equip-nome">Nome</label>
            <input
              id="equip-nome"
              type="text"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              maxLength={100}
              placeholder="Ex.: Notebook Dell"
              required
            />
          </div>
          <div className="campo">
            <label htmlFor="equip-patrimonio">Patrimônio</label>
            <input
              id="equip-patrimonio"
              type="text"
              value={patrimonio}
              onChange={(e) => setPatrimonio(e.target.value)}
              maxLength={50}
              placeholder="Ex.: LAB001"
              required
            />
          </div>
          <div className="formulario-acoes">
            <button type="submit" className="botao botao-primario">
              {idEmEdicao ? "Salvar alteração" : "Cadastrar"}
            </button>
            {idEmEdicao && (
              <button type="button" className="botao botao-secundario" onClick={limparFormulario}>
                Cancelar
              </button>
            )}
          </div>
        </form>
      </div>

      <div className="card">
        <h2>Equipamentos cadastrados</h2>
        <table className="tabela">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Patrimônio</th>
              <th className="coluna-acoes">Ações</th>
            </tr>
          </thead>
          <tbody>
            {equipamentos.length === 0 && (
              <tr>
                <td colSpan={3} className="tabela-vazia">Nenhum equipamento cadastrado.</td>
              </tr>
            )}
            {equipamentos.map((equipamento) => (
              <tr key={equipamento.id}>
                <td>{equipamento.nome}</td>
                <td><span className="codigo">{equipamento.patrimonio}</span></td>
                <td className="coluna-acoes">
                  <button className="botao botao-pequeno botao-secundario" onClick={() => editar(equipamento)}>
                    Editar
                  </button>
                  <button className="botao botao-pequeno botao-perigo" onClick={() => excluir(equipamento)}>
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default Equipamentos;
