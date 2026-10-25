using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class RestringirExclusaoEquipamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_Equipamentos_EquipamentoId",
                table: "Emprestimos");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_Equipamentos_EquipamentoId",
                table: "Emprestimos",
                column: "EquipamentoId",
                principalTable: "Equipamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_Equipamentos_EquipamentoId",
                table: "Emprestimos");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_Equipamentos_EquipamentoId",
                table: "Emprestimos",
                column: "EquipamentoId",
                principalTable: "Equipamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
