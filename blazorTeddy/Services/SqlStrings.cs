namespace TeddyBlazor.Services
{
    public static class SqlStrings
    {
        public static string GetStudents {get;} = @"
            SELECT s.Id as StudentId,
                s.Name as StudentName,
                c.Id as CourseId,
                c.Name as CourseName,
                c.TeacherId as TeacherId
            FROM Student as s
            LEFT JOIN StudentCourse as sc ON s.Id = sc.StudentId
            LEFT JOIN Course as c ON c.Id = sc.CourseId;";
    }
}