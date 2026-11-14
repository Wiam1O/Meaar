using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meaar5.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCLO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLO",
                columns: table => new
                {
                    CLOId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CLOCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLO", x => x.CLOId);
                });

            migrationBuilder.CreateTable(
                name: "CourseCLO",
                columns: table => new
                {
                    CourseCLOId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CLOId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCLO", x => x.CourseCLOId);
                    table.ForeignKey(
                        name: "FK_CourseCLO_CLO_CLOId",
                        column: x => x.CLOId,
                        principalTable: "CLO",
                        principalColumn: "CLOId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCLO_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCLO_CLOId",
                table: "CourseCLO",
                column: "CLOId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCLO_CourseId",
                table: "CourseCLO",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCLO");

            migrationBuilder.DropTable(
                name: "CLO");
        }
    }
}
