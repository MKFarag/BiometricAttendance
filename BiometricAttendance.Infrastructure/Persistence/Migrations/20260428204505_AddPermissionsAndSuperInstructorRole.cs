using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiometricAttendance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsAndSuperInstructorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "attendance:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 2, "permissions", "attendance:mark", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 3, "permissions", "course:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 4, "permissions", "course:add", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 5, "permissions", "course:modify", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 6, "permissions", "course:remove", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 7, "permissions", "department:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 8, "permissions", "department:add", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 9, "permissions", "department:modify", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 10, "permissions", "department:remove", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 11, "permissions", "fingerprint:action", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 12, "permissions", "fingerprint:register", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 13, "permissions", "instructor:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 14, "permissions", "instructor:get-pass", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 15, "permissions", "role:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 16, "permissions", "role:add", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 17, "permissions", "role:modify", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 18, "permissions", "role:toggle-status", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 19, "permissions", "student:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 20, "permissions", "student:change-department", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 21, "permissions", "student:change-level", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 22, "permissions", "student:promote", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 23, "permissions", "student:force-remove", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 24, "permissions", "user:read", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 25, "permissions", "user:add", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 26, "permissions", "user:modify", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 27, "permissions", "user:toggle-status", "019d8540-1a1b-7ee7-899d-305ada12c7a6" },
                    { 28, "permissions", "course:read", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 29, "permissions", "department:read", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 30, "permissions", "student:read", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 31, "permissions", "attendance:read", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 32, "permissions", "attendance:mark", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 33, "permissions", "fingerprint:action", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 34, "permissions", "fingerprint:register", "019d8563-a991-7ed3-85ca-3ce1df3896c2" },
                    { 48, "permissions", "course:read", "019d8540-1a1b-7ee7-899d-305c85cbce80" },
                    { 49, "permissions", "department:read", "019d8540-1a1b-7ee7-899d-305c85cbce80" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[] { "019dd5a8-a53b-7bab-9115-df41914dc7b6", "019dd5aa-01b4-70a7-b2b8-a6929b0cfec5", false, false, "SuperInstructor", "SUPERINSTRUCTOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 35, "permissions", "course:read", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 36, "permissions", "course:add", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 37, "permissions", "course:modify", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 38, "permissions", "department:read", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 39, "permissions", "student:read", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 40, "permissions", "attendance:read", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 41, "permissions", "attendance:mark", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 42, "permissions", "student:change-department", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 43, "permissions", "student:change-level", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 44, "permissions", "student:promote", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 45, "permissions", "fingerprint:action", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 46, "permissions", "fingerprint:register", "019dd5a8-a53b-7bab-9115-df41914dc7b6" },
                    { 47, "permissions", "instructor:read", "019dd5a8-a53b-7bab-9115-df41914dc7b6" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019dd5a8-a53b-7bab-9115-df41914dc7b6");
        }
    }
}
