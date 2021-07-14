using Microsoft.EntityFrameworkCore.Migrations;

namespace webApp.Migrations
{
    public partial class fmupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlaggedMessage_Message_MessageId",
                table: "FlaggedMessage");

            migrationBuilder.DropIndex(
                name: "IX_FlaggedMessage_MessageId",
                table: "FlaggedMessage");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "FlaggedMessage");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "FlaggedMessage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "FlaggedMessage");

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "FlaggedMessage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FlaggedMessage_MessageId",
                table: "FlaggedMessage",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlaggedMessage_Message_MessageId",
                table: "FlaggedMessage",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
