import { FormEvent, useEffect, useState } from "react";
import { API_URL } from "../config";
import { Aluno } from "../models/Aluno";
import { Emprestimo } from "../models/Emprestimo";
import { Equipamento } from "../models/Equipamento";
import { ERRO_CONEXAO, formatarData, lerMensagemErro } from "../utils";
import Mensagem, { MensagemTela } from "./Mensagem";

function Emprestimos() {
  // Listas carregadas da API
  const [emprestimos, setEmprestimos] = useState<Emprestimo[]>([]);
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [disponiveis, setDisponiveis] = useState<Equipamento[]>([]);

  // Valores escolhidos no formulário
  const [alunoId, setAlunoId] = useState("");
  const [equipamentoId, setEquipamentoId] = useState("");

  const [mensagem, setMensagem] = useState<MensagemTela>(null);
  const [atualizacao, setAtualizacao] = useState(0);

  // Carrega empréstimos, alunos e equipamentos disponíveis
  useEffect(() => {
    Promise.all([
      fetch(`${API_URL}/emprestimos`).then((resposta) => resposta.json()),
      fetch(`${API_URL}/alunos`).then((resposta) => resposta.json()),
      fetch(`${API_URL}/equipamentos/disponiveis`).then((resposta) => resposta.json()),
    ])
      .then(([dadosEmprestimos, dadosAlunos, dadosDisponiveis]) => {
        setEmprestimos(dadosEmprestimos);
        setAlunos(dadosAlunos);
        setDisponiveis(dadosDisponiveis);
      })
      .catch(() => setMensagem({ tipo: "erro", texto: ERRO_CONEXAO }));
  }, [atualizacao]);

  // POST: registra o empréstimo (a API responde 409 se o equipamento já estiver emprestado)
  async function registrar(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setMensagem(null);

    try {
      const resposta = await fetch(`${API_URL}/emprestimos`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ alunoId, equipamentoId }),
      });

      if (!resposta.ok) {
        setMensagem({ tipo: "erro", texto: await lerMensagemErro(resposta) });
        return;
      }

      setMensagem({ tipo: "sucesso", texto: "Empréstimo registrado com sucesso." });
      setAlunoId("");
      setEquipamentoId("");
      setAtualizacao((valor) => valor + 1);
    } catch {
      setMensagem({ tipo: "erro", texto: ERRO_CONEXAO });
    }
  }

  // PATCH: registra a devolução (a API responde 409 se já tiver sido devolvido)
  async function devolver(emprestimo: Emprestimo) {
    setMensagem(null);

    try {
      const resposta = await fetch(`${API_URL}/emprestimos/${emprestimo.id}/devolucao`, {
        method: "PATCH",
      });

      if (!resposta.ok) {
        setMensagem({ tipo: "erro", texto: await lerMensagemErro(resposta) });
        return;
      }

      setMensagem({
        tipo: "sucesso",
        texto: `Devolução registrada: ${emprestimo.equipamento.nome} (${emprestimo.equipamento.patrimonio}).`,
      });
      setAtualizacao((valor) => valor + 1);
    } catch {
      setMensagem({ tipo: "erro", texto: ERRO_CONEXAO });
    }
  }

  return (
    <section>
      <Mensagem mensagem={mensagem} />

      <div className="card">
        <h2>Registrar empréstimo</h2>
        <form className="formulario" onSubmit={registrar}>
          <div className="campo">
            <label htmlFor="emp-aluno">Aluno</label>
            <select
              id="emp-aluno"
              value={alunoId}
              onChange={(e) => setAlunoId(e.target.value)}
              required
            >
              <option value="">Selecione o aluno...</option>
              {alunos.map((aluno) => (
                <option key={aluno.id} value={aluno.id}>
                  {aluno.nome} ({aluno.matricula})
                </option>
              ))}
            </select>
          </div>
          <div className="campo">
            <label htmlFor="emp-equipamento">Equipamento disponível</label>
            <select
              id="emp-equipamento"
              value={equipamentoId}
              onChange={(e) => setEquipamentoId(e.target.value)}
              required
            >
              <option value="">Selecione o equipamento...</option>
              {disponiveis.map((equipamento) => (
                <option key={equipamento.id} value={equipamento.id}>
                  {equipamento.nome} ({equipamento.patrimonio})
                </option>
              ))}
            </select>
          </div>
          <div className="formulario-acoes">
            <button type="submit" className="botao botao-primario">
              Registrar empréstimo
            </button>
          </div>
        </form>
      </div>

      <div className="card">
        <h2>Empréstimos</h2>
        <table className="tabela">
          <thead>
            <tr>
              <th>Aluno</th>
              <th>Equipamento</th>
              <th>Empréstimo</th>
              <th>Devolução</th>
              <th>Situação</th>
              <th className="coluna-acoes">Ações</th>
            </tr>
          </thead>
          <tbody>
            {emprestimos.length === 0 && (
              <tr>
                <td colSpan={6} className="tabela-vazia">Nenhum empréstimo registrado.</td>
              </tr>
            )}
            {emprestimos.map((emprestimo) => {
              const ativo = emprestimo.dataDevolucao === null;
              return (
                <tr key={emprestimo.id}>
                  <td>
                    {emprestimo.aluno.nome}
                    <br />
                    <span className="texto-suave">{emprestimo.aluno.matricula}</span>
                  </td>
                  <td>
                    {emprestimo.equipamento.nome}
                    <br />
                    <span className="codigo">{emprestimo.equipamento.patrimonio}</span>
                  </td>
                  <td>{formatarData(emprestimo.dataEmprestimo)}</td>
                  <td>{formatarData(emprestimo.dataDevolucao)}</td>
                  <td>
                    <span className={ativo ? "selo selo-ativo" : "selo selo-devolvido"}>
                      {ativo ? "Ativo" : "Devolvido"}
                    </span>
                  </td>
                  <td className="coluna-acoes">
                    {ativo && (
                      <button
                        className="botao botao-pequeno botao-sucesso"
                        onClick={() => devolver(emprestimo)}
                      >
                        Devolver
                      </button>
                    )}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default Emprestimos;
