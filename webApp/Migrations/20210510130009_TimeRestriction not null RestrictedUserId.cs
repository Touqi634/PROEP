using Microsoft.EntityFrameworkCore.Migrations;

namespace webApp.Migrations
{
    public partial class TimeRestrictionnotnullRestrictedUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeRestriction_Child_RestrictedUserId",
                table: "TimeRestriction");

            migrationBuilder.AlterColumn<string>(
                name: "RestrictedUserId",
                table: "TimeRestriction",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeRestriction_Child_RestrictedUserId",
                table: "TimeRestriction",
                column: "RestrictedUserId",
                principalTable: "Child",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeRestriction_Child_RestrictedUserId",
                table: "TimeRestriction");

            migrationBuilder.AlterColumn<string>(
                name: "RestrictedUserId",
                table: "TimeRestriction",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeRestriction_Child_RestrictedUserId",
                table: "TimeRestriction",
                column: "RestrictedUserId",
                principalTable: "Child",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
