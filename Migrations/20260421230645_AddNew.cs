using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meaar5.Migrations
{
    /// <inheritdoc />
    public partial class AddNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Grades_QuestionId",
                table: "Grades",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Questions_QuestionId",
                table: "Grades",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Questions_QuestionId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_QuestionId",
                table: "Grades");
        }
    }
}
