using Microsoft.EntityFrameworkCore.Migrations;

namespace Student.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentInfoId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    StudentInfoForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentInfoId);
                    table.ForeignKey(
                        name: "FK_Students_Students_StudentInfoForeignKey",
                        column: x => x.StudentInfoForeignKey,
                        principalTable: "Students",
                        principalColumn: "StudentInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    CourseForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Students_CourseForeignKey",
                        column: x => x.CourseForeignKey,
                        principalTable: "Students",
                        principalColumn: "StudentInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(nullable: true),
                    NoteForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NoteId);
                    table.ForeignKey(
                        name: "FK_Notes_Students_NoteForeignKey",
                        column: x => x.NoteForeignKey,
                        principalTable: "Students",
                        principalColumn: "StudentInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseForeignKey",
                table: "Courses",
                column: "CourseForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoteForeignKey",
                table: "Notes",
                column: "NoteForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentInfoForeignKey",
                table: "Students",
                column: "StudentInfoForeignKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
