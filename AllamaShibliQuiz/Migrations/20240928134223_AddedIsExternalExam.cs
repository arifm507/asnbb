using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    public partial class AddedIsExternalExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalExamCentre",
                table: "Schools",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsExternalExamCentre",
                table: "Schools");
        }
    }
}
