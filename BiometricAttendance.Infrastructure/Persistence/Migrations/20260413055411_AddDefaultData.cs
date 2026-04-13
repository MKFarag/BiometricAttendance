using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiometricAttendance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019d8540-1a1b-7ee7-899d-305ada12c7a6", "019d8540-1a1b-7ee7-899d-305ba6339a26", false, false, "Admin", "ADMIN" },
                    { "019d8540-1a1b-7ee7-899d-305c85cbce80", "019d8540-1a1b-7ee7-899d-305d89670d46", true, false, "Student", "STUDENT" },
                    { "019d8563-a991-7ed3-85ca-3ce1df3896c2", "019d8563-d6df-7c2e-9b3d-223f0d1d0465", false, false, "Instructor", "INSTRUCTOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserNameChangeAvailableAt" },
                values: new object[] { "019d8540-1a1b-7ee7-899d-305e17ebaed8", 0, "019d8540-1a1b-7ee7-899d-305fb7e89535", "Admin@BiometricAttendance.com", true, "Admin", false, "- Mohamed Khaled", false, null, "ADMIN@BIOMETRICATTENDANCE.COM", "MKFARAG", "Pass@123", null, false, "FE0CF7654EC8496CA09765228BE3FF0A", false, "MKFarag", null });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019d8540-1a1b-7ee7-899d-305ada12c7a6", "019d8540-1a1b-7ee7-899d-305e17ebaed8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019d8540-1a1b-7ee7-899d-305c85cbce80");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019d8563-a991-7ed3-85ca-3ce1df3896c2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019d8540-1a1b-7ee7-899d-305ada12c7a6", "019d8540-1a1b-7ee7-899d-305e17ebaed8" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019d8540-1a1b-7ee7-899d-305ada12c7a6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019d8540-1a1b-7ee7-899d-305e17ebaed8");
        }
    }
}
