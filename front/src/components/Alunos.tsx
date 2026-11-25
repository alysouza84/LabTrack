import { FormEvent, useEffect, useState } from "react";
import { API_URL } from "../config";
import { Aluno } from "../models/Aluno";
import { ERRO_CONEXAO, lerMensagemErro } from "../utils";
import Mensagem, { MensagemTela } from "./Mensagem";

function Alunos() {
  // Estados da tela
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [nome, setNome] = useState("");
  const [matricula, setMatricula] = useState("");
  const [mensagem, setMensagem] = useState<MensagemTela>(null);
  const [atualizacao, setAtualizacao] = useState(0);

  // Carrega a lista ao abrir a tela e sempre que "atualizacao" mudar
  useEffect(() => {
    fetch(`${API_URL}/alunos`)
      .then((resposta) => resposta.json())
      .then((dados: Aluno[]) => setAlunos(dados))
      .catch(() => setMensagem({ tipo: "erro", texto: ERRO_CONEXAO }));
  }, [atualizacao]);

  // POST: cadastra o aluno (a API responde 409 se a matrícula já existir)
  async function cadastrar(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setMensagem(null);

    try {
      const resposta = await fetch(`${API_URL}/alunos`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ nome, matricula }),
      });

      if (!resposta.ok) {
        setMensagem({ tipo: "erro", texto: await lerMensagemErro(resposta) });
        return;
      }

      setMensagem({ tipo: "sucesso", texto: "Aluno cadastrado com sucesso." });
      setNome("");
      setMatricula("");
      setAtualizacao((valor) => valor + 1);
    } catch {
      setMensagem({ tipo: "erro", texto: ERRO_CONEXAO });
    }
  }

  return (
    <section>
      <Mensagem mensagem={mensagem} />

      <div className="card">
        <h2>Cadastrar aluno</h2>
        <form className="formulario" onSubmit={cadastrar}>
          <div className="campo">
            <label htmlFor="aluno-nome">Nome</label>
            <input
              id="aluno-nome"
              type="text"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              maxLength={100}
              placeholder="Ex.: João Silva"
              required
            />
          </div>
          <div className="campo">
            <label htmlFor="aluno-matricula">Matrícula</label>
            <input
              id="aluno-matricula"
              type="text"
              value={matricula}
              onChange={(e) => setMatricula(e.target.value)}
              maxLength={20}
              placeholder="Ex.: 2026001"
              required
            />
          </div>
          <div className="formulario-acoes">
            <button type="submit" className="botao botao-primario">
              Cadastrar
            </button>
          </div>
        </form>
      </div>

      <div className="card">
        <h2>Alunos cadastrados</h2>
        <table className="tabela">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Matrícula</th>
            </tr>
          </thead>
          <tbody>
            {alunos.length === 0 && (
              <tr>
                <td colSpan={2} className="tabela-vazia">Nenhum aluno cadastrado.</td>
              </tr>
            )}
            {alunos.map((aluno) => (
              <tr key={aluno.id}>
                <td>{aluno.nome}</td>
                <td><span className="codigo">{aluno.matricula}</span></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default Alunos;
