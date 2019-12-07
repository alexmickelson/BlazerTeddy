using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{

    public class CourseRepository : ICourseRepository
    {
        private readonly Func<IDbConnection> getDbConnection;
        private readonly ILogger<CourseRepository> logger;

        public CourseRepository(Func<IDbConnection> getDbConnection,
                                ILogger<CourseRepository> logger)
        {
            this.getDbConnection = getDbConnection;
            this.logger = logger;
        }

        public async Task AddCourseAsync(Course course)
        {
            using (var dbConnection = getDbConnection())
            {
                await validateTacherId(course.TeacherId, dbConnection);

                course.CourseId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into Course (CourseName, TeacherId) 
                      values (@courseName, @teacherId)
                      RETURNING CourseId;",
                    new { courseName = course.CourseName, teacherId = course.TeacherId }
                );
            }
        }

        private static async Task validateTacherId(int teacherId, IDbConnection dbConnection)
        {
            var teacherExists = await dbConnection.QueryFirstAsync<bool>(
                @"select exists(
                  select 1 from Teacher 
                  where TeacherId = @teacherId
                  );",
                new { teacherId }
            );
            if (!teacherExists)
            {
                throw new ArgumentException($"cannot save course with invalid teacherId: {teacherId}");
            }
        }

        public async Task<Course> GetCourseAsync(int courseId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<Course>(
                    @"select * from Course
                      where CourseId = @courseId",
                    new { courseId }
                );
            }
        }

        public async Task<Course> GetCourseAsync(int studentId, int classId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<Course>(
                    @"select * from Course c
                      inner join StudentCourse sc on c.CourseId = sc.CourseId
                      where sc.StudentId = @studentId and sc.ClassId = @classId",
                    new { studentId, classId }
                );
            }
        }
    }
}