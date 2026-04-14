using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiometricAttendance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[] { "019d8a96-d2af-7fd4-be73-922f85b81569", "019d8a97-499e-78da-8340-b2551ae86a19", true, false, "Pending", "PENDING" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019d8a96-d2af-7fd4-be73-922f85b81569");
        }
    }
}
