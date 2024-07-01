using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllamaShibliQuiz.Migrations
{
    public partial class UpdatedTableTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberPhoto",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Teams",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "MemberPhoto",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
