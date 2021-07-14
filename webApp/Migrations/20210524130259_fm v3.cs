using Microsoft.EntityFrameworkCore.Migrations;

namespace webApp.Migrations
{
    public partial class fmv3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage");

            migrationBuilder.AlterColumn<string>(
                name: "SenderID",
                table: "FlaggedMessage",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "FlaggedMessage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage",
                column: "SenderID",
                principalTable: "Child",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage");

            migrationBuilder.AlterColumn<string>(
                name: "SenderID",
                table: "FlaggedMessage",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "FlaggedMessage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_FlaggedMessage_Child_SenderID",
                table: "FlaggedMessage",
                column: "SenderID",
                principalTable: "Child",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
