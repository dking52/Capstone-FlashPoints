using Microsoft.EntityFrameworkCore.Migrations;

namespace FlashPoints.Data.Migrations
{
    public partial class finishing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventsAttendedIDs",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventsCreatedIDs",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PrizesRedeemedIDs",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ActualCost",
                table: "Prize",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CurrentInventory",
                table: "Prize",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Prize",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberAttended",
                table: "Event",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventsAttendedIDs",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EventsCreatedIDs",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PrizesRedeemedIDs",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ActualCost",
                table: "Prize");

            migrationBuilder.DropColumn(
                name: "CurrentInventory",
                table: "Prize");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Prize");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "NumberAttended",
                table: "Event");
        }
    }
}
