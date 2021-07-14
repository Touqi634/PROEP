using Microsoft.EntityFrameworkCore.Migrations;

namespace webApp.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage");

            migrationBuilder.AddForeignKey(
                name: "FK_FlaggedMessage_User_SenderID",
                table: "FlaggedMessage",
                column: "SenderID",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlaggedMessage_User_SenderID",
                table: "FlaggedMessage");

            migrationBuilder.AddForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage",
                column: "SenderID",
                principalTable: "Child",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
