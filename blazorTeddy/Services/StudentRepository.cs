using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TeddyBlazor.Data;
using TeddyBlazor.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.Extensions.Logging;

namespace TeddyBlazor.Services
{

    public class StudentRepository : IStudentRepository
    {
        private readonly ILogger<StudentRepository> logger;

        private List<Student> students { get; set; }
        private Func<IDbConnection> GetDbConnection { get; }

        public StudentRepository(Func<IDbConnection> getDbConnection,
                                 ILogger<StudentRepository> logger)
        {
            students = new List<Student>();
            GetDbConnection = getDbConnection;
            this.logger = logger;
        }
        public async Task UpdateStudentsAsync()
        {
            logger.LogInformation("Updating student list");
            using (var dbConnection = GetDbConnection())
            {
                students = safeGetStudentsFromDb(dbConnection);
                var notes = await dbConnection.QueryAsync<Note>(
                    @"SELECT * FROM Note;");
                var restrictions = await dbConnection.QueryAsync<(int, int)>(
                    @"SELECT * FROM StudentRestriction;");
                foreach (var student in students)
                {
                    student.Notes = notes.Where(n => n.StudentId == student.StudentId);
                    student.Restrictions = assignRestrictions(restrictions, student.StudentId);
                }
            }
        }

        private List<Student> safeGetStudentsFromDb(IDbConnection dbConnection)
        {
            var studentList = dbConnection.Query<Student>(@"SELECT * FROM Student;");
            return studentList == null
                ? new List<Student>()
                : studentList.ToList();
        }

        private IEnumerable<int> assignRestrictions(IEnumerable<(int, int)> restrictions, int studentId)
        {
            var firstRestrictions = restrictions
                .Where(r => r.Item1 == studentId)
                .Select(r => r.Item2);
            var secondRestrictions = restrictions
                .Where(r => r.Item2 == studentId)
                .Select(r => r.Item1);
            return firstRestrictions.Concat(secondRestrictions);
        }

        public async Task SaveChangesAsync()
        {
            logger.LogInformation("saving student list to database");
            using (var dbConnection = GetDbConnection())
            {
                foreach (var student in students)
                {
                    if (student.StoredInDatabase)
                    {
                        await updateStudentToDbAsync(dbConnection, student);
                    }
                    else
                    {
                        student.StudentId = addStudentToDb(dbConnection, student);
                    }
                }
            }
        }

        private async Task updateStudentToDbAsync(IDbConnection dbConnection, Student student)
        {
            await dbConnection.ExecuteAsync(
                @"UPDATE Student SET StudentName=@studentName WHERE StudentId=@id;",
                new { studentName = student.StudentName, id = student.StudentId });
        }

        private int addStudentToDb(IDbConnection dbConnection, Student student)
        {
            return dbConnection.QueryFirst<int>(
                @"INSERT INTO Student (StudentName) Values ( @studentName ) RETURNING StudentId;",
                new { studentName = student.StudentName });
        }

        public async Task<List<Student>> GetListAsync()
        {
            await UpdateStudentsAsync();
            return students;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            await UpdateStudentsAsync();
            return students.FirstOrDefault(s => s.StudentId == id);
        }

        public async Task AddUnsignedNoteAsync(Student student, Note note)
        {
            note.StudentId = student.StudentId;
            initializeEmptyNoteList(student);
            using (var dbConnection = GetDbConnection())
            {
                logger.LogInformation($"adding note to database");
                note.NoteId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into Note (Content, StudentId, NoteType, DateCreated)
                      values (@content, @studentId, @noteType, @dateCreated)
                      RETURNING NoteId;",
                    note);
            }
            student.Notes = student.Notes.Append(note);
        }

        public async Task AddSignedNoteAsync(Student student, Note note, int teacherId)
        {
            note.StudentId = student.StudentId;
            note.TeacherId = teacherId;
            using (var dbConnection = GetDbConnection())
            {
                logger.LogInformation($"adding note to database");
                note.NoteId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into Note (Content, StudentId, TeacherId, NoteType, DateCreated)
                      values (@content, @studentId, @teacherId, @noteType, @dateCreated)
                      RETURNING NoteId;",
                    note);
            }
            initializeEmptyNoteList(student);
            student.Notes = student.Notes.Append(note);
        }

        private static void initializeEmptyNoteList(Student student)
        {
            if (student.Notes == null)
            {
                student.Notes = new List<Note>();
            }
        }


        public async Task AddRestrictionAsync(int StudentId1, int StudentId2)
        {
            await UpdateStudentsAsync();
            using (var connection = GetDbConnection())
            {
                logger.LogInformation($"adding restriction to database");
                connection.Execute(
                    @"Insert into StudentRestriction values (@studentId1, @studentId2);",
                    new { studentId1 = StudentId1, studentId2 = StudentId2 }
                );
            }
        }

        public async Task AddStudentAsync(Student Student)
        {
            students.Add(Student);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByClassAsync(int classId)
        {
            using(var dbConnection = GetDbConnection())
            {
                return await dbConnection.QueryAsync<Student>(
                    @"select * 
                      from Student s
                      inner join StudentCourse sc on s.StudentId = sc.StudentId
                      where sc.ClassId = @classId",
                    new { classId }
                );
            }
        }
    }
}