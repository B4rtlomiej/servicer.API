using Microsoft.EntityFrameworkCore.Migrations;

namespace servicer.API.Migrations
{
    public partial class AddItemIdToNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Notes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ItemId",
                table: "Notes",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Items_ItemId",
                table: "Notes",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Items_ItemId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_ItemId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Notes");
        }
    }
}
