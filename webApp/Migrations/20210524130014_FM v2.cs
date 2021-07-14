using Microsoft.EntityFrameworkCore.Migrations;

namespace webApp.Migrations
{
    public partial class FMv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlaggedMessage",
                columns: table => new
                {
                    FlaggedMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    SenderID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlaggedMessage", x => x.FlaggedMessageId);
                    table.ForeignKey(
                        name: "FK_FlaggedMessage_Child_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Child",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlaggedMessage_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlaggedMessage_MessageId",
                table: "FlaggedMessage",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggedMessage_SenderID",
                table: "FlaggedMessage",
                column: "SenderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlaggedMessage");
        }
    }
}
