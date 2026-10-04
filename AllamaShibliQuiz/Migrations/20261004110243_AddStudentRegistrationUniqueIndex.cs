using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentRegistrationUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Students_Name_Class_MobileNumber_AadharNumber",
                table: "Students",
                columns: new[] { "Name", "Class", "MobileNumber", "AadharNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_Name_Class_MobileNumber_AadharNumber",
                table: "Students");
        }
    }
}
