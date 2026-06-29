using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_genda_API.Migrations
{
    /// <inheritdoc />
    public partial class updatingProfileController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAmeuZnmVQNw+PV3BycAkds2w/Yy4u8M0USBsM+jjBwwfbaflBjAkKcwwxEsbbMLqQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7b85e7-0058-40a4-b306-8974504ac779",
                columns: new[] { "ImageId", "PasswordHash" },
                values: new object[] { 0, "AQAAAAIAAYagAAAAEIlPCL/NYYGElS8Awn0vK/vcI3vH6XoTU2mtr4uRlS+32kqT4P/3xN/6vgSyuiloAw==" });
        }
    }
}
