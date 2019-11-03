using Microsoft.EntityFrameworkCore.Migrations;

namespace servicer.API.Migrations
{
    public partial class UpdateItemDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductSpecifications",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductSpecifications");

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Items",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Items",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
