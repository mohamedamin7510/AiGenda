using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_genda_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedtablesonmosterASP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGHSHxdvbU56b6B699SWf2r7D22u6e74j1n9b5zB0S7e+lf7+mPIM01v6rgWJIUhWQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPBi8kdyLukPjxsx/hfWm1di66wvdM+wxS0fEMqgJ/kEBTXJYw//edmv7DQ2D697bw==");
        }
    }
}
