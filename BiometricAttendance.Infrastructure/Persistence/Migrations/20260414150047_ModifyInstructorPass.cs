using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiometricAttendance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyInstructorPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "InstructorPasses");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "InstructorPasses",
                newName: "IsDisabled");

            migrationBuilder.AddColumn<int>(
                name: "MaxUsedCount",
                table: "InstructorPasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UsedBy",
                table: "InstructorPasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUsedCount",
                table: "InstructorPasses");

            migrationBuilder.DropColumn(
                name: "UsedBy",
                table: "InstructorPasses");

            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                table: "InstructorPasses",
                newName: "IsUsed");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "InstructorPasses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
