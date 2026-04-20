using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiometricAttendance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentCourses");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Courses",
                newName: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Courses",
                newName: "Level");

            migrationBuilder.CreateTable(
                name: "DepartmentCourses",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentCourses", x => new { x.DepartmentId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_DepartmentCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentCourses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCourses_CourseId",
                table: "DepartmentCourses",
                column: "CourseId");
        }
    }
}
