import { Aluno } from "./Aluno";
import { Equipamento } from "./Equipamento";

// Formato devolvido por GET /api/emprestimos (com os dados do aluno e do equipamento)
export interface Emprestimo {
  id: string;
  dataEmprestimo: string;
  dataDevolucao: string | null;
  aluno: Aluno;
  equipamento: Equipamento;
}
