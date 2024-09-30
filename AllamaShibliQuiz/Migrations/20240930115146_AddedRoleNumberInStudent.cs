using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    public partial class AddedRoleNumberInStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExamCentre",
                table: "Students",
                newName: "ExamCentreId");

            migrationBuilder.AddColumn<string>(
                name: "RollNumber",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CentreCode",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RollNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CentreCode",
                table: "Schools");

            migrationBuilder.RenameColumn(
                name: "ExamCentreId",
                table: "Students",
                newName: "ExamCentre");
        }
    }
}
