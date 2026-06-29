using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_genda_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "936d1473-fbe5-4d0e-b3bd-eec2c90261d7",
                column: "ConcurrencyStamp",
                value: "6303a117-de04-41f9-b31f-d554e84808b9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2153bc2-8cba-45ad-a733-3f24eab7b299",
                column: "ConcurrencyStamp",
                value: "9ea95a74-301a-472f-a408-68121faa3a98");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPid8sQx+nxXop9jnkwJV0IEbXkl4RRXdIvGpCyqjoX9IohG4hx1SLYzoqtvNF0UXw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "936d1473-fbe5-4d0e-b3bd-eec2c90261d7",
                column: "ConcurrencyStamp",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2153bc2-8cba-45ad-a733-3f24eab7b299",
                column: "ConcurrencyStamp",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOW4qUXUJTPHFkTAiPBWaqlphfM7AYfQhjEJjrzIta4wvGPbgjBTagjg+kFzZUWAww==");
        }
    }
}
