import { useState } from "react";
import "./App.css";
import Equipamentos from "./components/Equipamentos";
import Alunos from "./components/Alunos";
import Emprestimos from "./components/Emprestimos";

// As quatro abas da aplicação
type Aba = "equipamentos" | "alunos" | "emprestimos" | "disponiveis";

const ABAS: { id: Aba; titulo: string }[] = [
  { id: "equipamentos", titulo: "Equipamentos" },
  { id: "alunos", titulo: "Alunos" },
  { id: "emprestimos", titulo: "Empréstimos" },
  { id: "disponiveis", titulo: "Disponíveis" },
];

function App() {
  // Guarda qual aba está aberta; trocar o valor faz o React mostrar outra tela
  const [abaAtual, setAbaAtual] = useState<Aba>("equipamentos");

  return (
    <div className="app">
      <header className="cabecalho">
        <div className="cabecalho-conteudo">
          <h1 className="logo">LabTrack</h1>
          <p className="subtitulo">Controle de empréstimos de equipamentos acadêmicos</p>
        </div>
        <nav className="abas">
          {ABAS.map((aba) => (
            <button
              key={aba.id}
              className={aba.id === abaAtual ? "aba aba-ativa" : "aba"}
              onClick={() => setAbaAtual(aba.id)}
            >
              {aba.titulo}
            </button>
          ))}
        </nav>
      </header>

      <main className="conteudo">
        {abaAtual === "equipamentos" && <Equipamentos />}
        {abaAtual === "alunos" && <Alunos />}
        {abaAtual === "emprestimos" && <Emprestimos />}

        {abaAtual === "disponiveis" && (
          <div className="card em-construcao">
            Tela em desenvolvimento.
          </div>
        )}
      </main>
    </div>
  );
}

export default App;
