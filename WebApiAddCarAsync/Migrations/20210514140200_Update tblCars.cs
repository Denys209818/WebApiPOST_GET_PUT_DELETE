using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiAddCarAsync.Migrations
{
    public partial class UpdatetblCars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "tblCars",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "tblCars");
        }
    }
}
