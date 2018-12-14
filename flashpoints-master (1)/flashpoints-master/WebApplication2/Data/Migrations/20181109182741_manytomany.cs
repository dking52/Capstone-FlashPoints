using Microsoft.EntityFrameworkCore.Migrations;

namespace FlashPoints.Data.Migrations
{
    public partial class manytomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "EventAttended",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttended", x => new { x.UserID, x.EventID });
                    table.ForeignKey(
                        name: "FK_EventAttended_Event_EventID",
                        column: x => x.EventID,
                        principalTable: "Event",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttended_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrizeRedeemed",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    PrizeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizeRedeemed", x => new { x.UserID, x.PrizeID });
                    table.ForeignKey(
                        name: "FK_PrizeRedeemed_Prize_PrizeID",
                        column: x => x.PrizeID,
                        principalTable: "Prize",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrizeRedeemed_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttended_EventID",
                table: "EventAttended",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_PrizeRedeemed_PrizeID",
                table: "PrizeRedeemed",
                column: "PrizeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttended");

            migrationBuilder.DropTable(
                name: "PrizeRedeemed");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
