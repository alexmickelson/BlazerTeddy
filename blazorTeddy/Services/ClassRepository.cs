using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public class ClassRepository : IClassRepository
    {
        private readonly Func<IDbConnection> getDbConnection;

        public ClassRepository(Func<IDbConnection> getDbConnection)
        {
            this.getDbConnection = getDbConnection;
        }

        public async Task AddClassAsync(ClassModel classModel)
        {
            await validateStudentIds(classModel);
            await validateClassRoomSize(classModel);
            await storeClassInDb(classModel);
        }

        private async Task validateClassRoomSize(ClassModel classModel)
        {
            using(var dbConnection = getDbConnection())
            {
                var classRoom = await dbConnection.QueryFirstAsync<ClassRoom>(
                    @"select * from ClassRoom
                    where ClassRoomId = @classRoomId",
                    new { classRoomId = classModel.ClassRoomId }
                );
                if (!sameDimensions(classModel, classRoom))
                {
                    var exceptionString = String.Format(
                        "cannot save seating chart {0},{1} in classroom with dimensions {2},{3}",
                        classModel.SeatingChartByStudentID.GetLength(0),
                        classModel.SeatingChartByStudentID.GetLength(1),
                        classRoom.SeatsHorizontally,
                        classRoom.SeatsVertically);
                    throw new ArgumentException(exceptionString);
                };

            }
        }

        private static bool sameDimensions(ClassModel classModel, ClassRoom classRoom)
        {
            return classModel.SeatingChartByStudentID.GetLength(0) == classRoom.SeatsHorizontally 
                && classModel.SeatingChartByStudentID.GetLength(1) == classRoom.SeatsVertically;
        }

        private async Task storeClassInDb(ClassModel classModel)
        {
            using (var dbConnection = getDbConnection())
            {
                classModel.ClassId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into ClassModel (TeacherId, ClassRoomId, ClassName) 
                    values (@teacherId, @classRoomId, @className)
                    RETURNING ClassId;",
                    new
                    {
                        teacherId = classModel.TeacherId,
                        classRoomId = classModel.ClassRoomId,
                        className = classModel.ClassName
                    }
                );
            }
        }

        private async Task validateStudentIds(ClassModel classModel)
        {
            using (var dbConnection = getDbConnection())
            {
                var studentIds = await dbConnection.QueryAsync<int>(
                    @"select StudentId from Student;"
                );
                foreach (var studentId in classModel.SeatingChartByStudentID)
                {
                    if (!studentIdIsValid(studentIds, studentId))
                    {
                        throw new ArgumentException($"cannot assign seat, no student with id {studentId}");
                    }
                }
            }
        }

        private static bool studentIdIsValid(IEnumerable<int> studentIds, int studentId)
        {
            return studentId == default(int) || studentIds.Contains(studentId);
        }

        public async Task AddClassRoomAsync(ClassRoom classRoom)
        {
            using(var dbConnection = getDbConnection())
            {
                classRoom.ClassRoomId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into ClassRoom (ClassRoomName, SeatsHorizontally, SeatsVertically)
                    values (@classRoomName, @seatsHorizontally, @seatsVertically) 
                    returning ClassRoomId;",
                    new { 
                        classRoomName = classRoom.ClassRoomName,
                        seatsHorizontally = classRoom.SeatsHorizontally,
                        seatsVertically = classRoom.SeatsVertically
                    }
                );
            }
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            using(var dbConnection = getDbConnection())
            {
                teacher.TeacherId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into Teacher (TeacherName)
                    values (@teacherName) 
                    returning TeacherId;",
                    new { teacherName = teacher.TeacherName }
                );
            }
        }

        public async Task<ClassModel> GetClassAsync(int classId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<ClassModel>(
                    @"select * from ClassModel
                    where ClassId = @classId",
                    new { classId = classId}
                );
            }
        }

        public async Task<ClassRoom> GetClassRoomAsync(int classRoomId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<ClassRoom>(
                    @"select * from ClassRoom
                    where ClassRoomId = @classRoomId",
                    new { classRoomId = classRoomId }
                );
            }
        }

        public async Task<Teacher> GetTeacherAsync(int teacherId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<Teacher>(
                    @"select * from Teacher
                    where TeacherId = @teacherId",
                    new { teacherId = teacherId}
                );
            }
        }
    }
}