using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
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
                var course = await dbConnection.QueryFirstAsync<Course>(
                    @"select *
                      from Course
                      where CourseId = @courseId",
                    new { courseId }
                );
                course.StudentIds = await getStudentIds(courseId, dbConnection);

                return course;
            }
        }

        private async Task<IEnumerable<int>> getStudentIds(int courseId, IDbConnection dbConnection)
        {
             return await dbConnection.QueryAsync<int>(
                @"select StudentId from StudentCourse
                      where CourseId = @courseId",
                new { courseId }
            );
        }

        public async Task<Course> GetCourseAsync(int studentId, int classId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<Course>(
                    @"select * from Course c
                      inner join StudentCourse sc on c.CourseId = sc.CourseId
                      where sc.StudentId = @studentId and sc.ClassId = @classId;",
                    new { studentId, classId }
                );
            }
        }

        public async Task<IEnumerable<Course>> GetCoursesByClassId(int classId)
        {
            using (var dbConnection = getDbConnection())
            {
                var courses = await dbConnection.QueryAsync<Course>(
                    @"select c.* from course c
                      inner join StudentCourse sc on c.CourseId = sc.CourseId
                      where sc.ClassId = @classId
                      group by c.courseid",
                    new { classId }
                );
                foreach(var course in courses)
                {
                    course.StudentIds = await getStudentIdsByClassAndCourse(course.CourseId, classId, dbConnection);
                }
                return courses;
            }
        }

        private async Task<IEnumerable<int>> getStudentIdsByClassAndCourse(int courseId, int classId, IDbConnection dbConnection)
        {
             return await dbConnection.QueryAsync<int>(
                @"select StudentId 
                  from StudentCourse
                  where CourseId = @courseId
                    and ClassId = @classId",
                new { courseId, classId }
            );
        }
    }
}