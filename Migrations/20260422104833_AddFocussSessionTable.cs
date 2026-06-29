using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_genda_API.Migrations
{
    /// <inheritdoc />
    public partial class AddFocussSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FocusSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    SpaceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    AmbientSound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreakAfter = table.Column<bool>(type: "bit", nullable: false),
                    BlockNotifications = table.Column<bool>(type: "bit", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Interruptions = table.Column<int>(type: "int", nullable: false),
                    InitialCompletedSubtasks = table.Column<int>(type: "int", nullable: false),
                    InitialTotalSubtasks = table.Column<int>(type: "int", nullable: false),
                    CompletedSubtasks = table.Column<int>(type: "int", nullable: false),
                    TotalSubtasks = table.Column<int>(type: "int", nullable: false),
                    PausedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalPausedSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FocusSessions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGMdLZy4dqKpdjVhFlSe6M/dazAAOnnfGzA9lTARJxomhjGI9fh9A4rvczjD9rAOvg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FocusSessions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG2tiIWPsicgyhlmVIMCkEUEzJz+jxUAL92ma5oG01I/ol6QbvXy4hdzz9gfQsMptg==");
        }
    }
}
