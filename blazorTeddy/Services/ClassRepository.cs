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
            using (var dbConnection = getDbConnection())
            {
                await validateStudentIds(classModel, dbConnection);
                await validateClassRoomDimensions(classModel, dbConnection);
                await storeClassInDb(classModel, dbConnection);
                await storeSeatingChartInDb(classModel, dbConnection);
            }
        }

        public async Task UpdateClassAsync(ClassModel classModel)
        {
            using (var dbConnection = getDbConnection())
            {
                await validateStudentIds(classModel, dbConnection);
                await validateClassRoomDimensions(classModel, dbConnection);
                await UpdateClassInDb(classModel, dbConnection);
                await storeSeatingChartInDb(classModel, dbConnection);
            }
        }

        private async Task validateStudentIds(ClassModel classModel, IDbConnection dbConnection)
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

        private async Task validateClassRoomDimensions(ClassModel classModel, IDbConnection dbConnection)
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

        private async Task UpdateClassInDb(ClassModel classModel, IDbConnection dbConnection)
        {
            await dbConnection.ExecuteAsync(
                @"update ClassModel set 
                    TeacherId   = @teacherId,
                    ClassRoomId = @classRoomId,
                    ClassName   = @className
                where ClassId = @ClassId",
                classModel
            );
        }

        private async Task storeSeatingChartInDb(ClassModel classModel, IDbConnection dbConnection)
        {
            var seatingAssigments = exctractSeatingAssigments(classModel);
            await deleteOldAssignmentsFromDb(classModel, dbConnection);
            await insertNewAssigmentsFromDb(dbConnection, seatingAssigments);
        }

        private List<SeatingAssigment> exctractSeatingAssigments(ClassModel classModel)
        {
            var seatingAssigments = new List<SeatingAssigment>();
            for (int i = 0; i < classModel.SeatingChartByStudentID.GetLength(0); i++)
            {
                for (int j = 0; j < classModel.SeatingChartByStudentID.GetLength(0); j++)
                {
                    makeAssignmentIfNotEmpty(classModel, seatingAssigments, i, j);
                }
            }
            return seatingAssigments;
        }
        private async Task deleteOldAssignmentsFromDb(ClassModel classModel, IDbConnection dbConnection)
        {
            await dbConnection.ExecuteAsync(
                @"delete from SeatingAssignment
                where ClassId = @ClassId",
                classModel
            );
        }

        private async Task insertNewAssigmentsFromDb(IDbConnection dbConnection, List<SeatingAssigment> seatingAssigments)
        {
            await dbConnection.ExecuteAsync(
                @"insert into SeatingAssignment values 
                (@ClassId, @StudentId, @HorizontalCoordinate, @VerticalCoordinate);",
                seatingAssigments
            );
        }

        private void makeAssignmentIfNotEmpty(ClassModel classModel, List<SeatingAssigment> seatingAssigments, int i, int j)
        {
            if (classModel.SeatingChartByStudentID[i, j] != default(int))
            {
                seatingAssigments.Add(new SeatingAssigment()
                {
                    ClassId = classModel.ClassId,
                    StudentId = classModel.SeatingChartByStudentID[i, j],
                    HorizontalCoordinate = i,
                    VerticalCoordinate = j
                });
            }
        }

        private bool sameDimensions(ClassModel classModel, ClassRoom classRoom)
        {
            return classModel.SeatingChartByStudentID.GetLength(0) == classRoom.SeatsHorizontally 
                && classModel.SeatingChartByStudentID.GetLength(1) == classRoom.SeatsVertically;
        }

        private async Task storeClassInDb(ClassModel classModel, IDbConnection dbConnection)
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

        private bool studentIdIsValid(IEnumerable<int> studentIds, int studentId)
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
                    classRoom
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
                    teacher
                );
            }
        }

        public async Task<ClassModel> GetClassAsync(int classId)
        {
            using (var dbConnection = getDbConnection())
            {
                var classModel = await getClassFromDbAsync(classId, dbConnection);
                classModel.SeatingChartByStudentID = await getSeatingChartFromDbAsync(classModel.ClassId, dbConnection);
                return classModel;
            }
        }

        private async Task<int[,]> getSeatingChartFromDbAsync(int classId, IDbConnection dbConnection)
        {
            var seatingAssigments = await dbConnection.QueryAsync<SeatingAssigment>(
                @"select * from SeatingAssignment
                where ClassId = @classid",
                new {classId}
            );
            if(seatingAssigments.Count() == 0)
            {
                return new int[0,0];
            }
            var horizontalSize = seatingAssigments.Max(sa => sa.HorizontalCoordinate) + 1;
            var verticalSize = seatingAssigments.Max(sa => sa.VerticalCoordinate) + 1;
            var seatingChart = new int[horizontalSize, verticalSize];

            foreach (var assignment in seatingAssigments)
            {
                seatingChart[assignment.HorizontalCoordinate, assignment.VerticalCoordinate] = assignment.StudentId;
            }
            
            return seatingChart;
        }

        private async Task<ClassModel> getClassFromDbAsync(int classId, IDbConnection dbConnection)
        {
            return await dbConnection.QueryFirstAsync<ClassModel>(
                @"select * from ClassModel
                where ClassId = @classId",
                new { classId }
            );
        }

        public async Task<ClassRoom> GetClassRoomAsync(int classRoomId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<ClassRoom>(
                    @"select * from ClassRoom
                    where ClassRoomId = @classRoomId",
                    new { classRoomId }
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
                    new { teacherId }
                );
            }
        }

        public async Task UpdateClassRoomAsync(ClassRoom classRoom)
        {
            using(var dbConnection = getDbConnection())
            {
                await dbConnection.ExecuteAsync(
                    @"update Classroom set 
                        ClassRoomName     = @ClassRoomName,
                        SeatsHorizontally = @SeatsHorizontally,
                        SeatsVertically   = @SeatsVertically
                    where ClassRoomId = @ClassRoomId",
                    classRoom
                );
            }
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            using(var dbConnection = getDbConnection())
            {
                await dbConnection.ExecuteAsync(
                    @"update Teacher set TeacherName = @TeacherName
                    where TeacherId = @TeacherId;",
                    teacher
                );
            }
        }
    }
}