using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    public partial class AddedSubjectToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Students",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Students");
        }
    }
}
