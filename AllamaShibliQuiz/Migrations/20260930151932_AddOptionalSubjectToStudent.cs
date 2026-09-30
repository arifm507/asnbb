using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalSubjectToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OptionalSubject",
                table: "Students",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionalSubject",
                table: "Students");
        }
    }
}
