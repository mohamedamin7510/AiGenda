using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_genda_API.Migrations
{
    /// <inheritdoc />
    public partial class AddFocussSessionTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SpaceId",
                table: "FocusSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELoC3Ftr7L/ETjjZyLUxdqudicv4FkYoy7kjq8JONDzz1URghftfhl4WkiDDSJje9Q==");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_SpaceId",
                table: "FocusSessions",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FocusSessions_Spaces_SpaceId",
                table: "FocusSessions",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FocusSessions_Spaces_SpaceId",
                table: "FocusSessions");

            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_SpaceId",
                table: "FocusSessions");

            migrationBuilder.AlterColumn<string>(
                name: "SpaceId",
                table: "FocusSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGMdLZy4dqKpdjVhFlSe6M/dazAAOnnfGzA9lTARJxomhjGI9fh9A4rvczjD9rAOvg==");
        }
    }
}
