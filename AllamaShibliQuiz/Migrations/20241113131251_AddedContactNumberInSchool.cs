using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    public partial class AddedContactNumberInSchool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Schools",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Schools");
        }
    }
}
