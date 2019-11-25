using Microsoft.EntityFrameworkCore.Migrations;

namespace servicer.API.Migrations
{
    public partial class AddNotesForTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Notes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_TicketId",
                table: "Notes",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Tickets_TicketId",
                table: "Notes",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Tickets_TicketId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_TicketId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Notes");
        }
    }
}
