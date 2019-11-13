using Microsoft.EntityFrameworkCore.Migrations;

namespace servicer.API.Migrations
{
    public partial class AddIsActiveToProductSpecification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductSpecifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductSpecifications");
        }
    }
}
