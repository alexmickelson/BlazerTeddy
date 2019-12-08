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
        private Func<IDbConnection> GetDbConnection { get; }

        public StudentRepository(Func<IDbConnection> getDbConnection,
                                 ILogger<StudentRepository> logger)
        {
            GetDbConnection = getDbConnection;
            this.logger = logger;
        }

        public async Task AddStudentAsync(Student student)
        {
            using(var dbConnection = GetDbConnection())
            {
                student.StudentId = await dbConnection.QueryFirstAsync<int>(
                    @"INSERT INTO Student (StudentName) 
                      Values ( @studentName )
                      RETURNING StudentId;",
                    new { studentName = student.StudentName });
            }
        }

        public async Task<IEnumerable<Student>> GetListAsync()
        {
            using(var dbConnection = GetDbConnection())
            {
                return await dbConnection.QueryAsync<Student>(
                    @"select * from Student"
                );
            }
        }

        public async Task<Student> GetStudentAsync(int studentId)
        {
            using(var dbConnection = GetDbConnection())
            {
                var student = await dbConnection.QueryFirstAsync<Student>(
                    @"select * from Student
                      where StudentId = @studentId",
                    new { studentId }
                );
                student.Notes = await getStudentNotes(student.StudentId, dbConnection);
                student.Restrictions = await getStudentRestrictions(student.StudentId, dbConnection);
                return student;
            }
        }

        private async Task<IEnumerable<int>> getStudentRestrictions(int studentId, IDbConnection dbConnection)
        {
            var firstList =  await dbConnection.QueryAsync<int>(
                @"select Student2 from StudentRestriction
                  where Student1 = @studentId",
                new { studentId }
            );
            var secondList =  await dbConnection.QueryAsync<int>(
                @"select Student1 from StudentRestriction
                  where Student2 = @studentId",
                new { studentId }
            );
            return firstList.Concat(secondList);
        }

        private async Task<IEnumerable<Note>> getStudentNotes(int studentId, IDbConnection dbConnection)
        {
            return await dbConnection.QueryAsync<Note>(
                @"select * from Note
                  where StudentId = @studentId",
                new { studentId }
            );
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
            using (var connection = GetDbConnection())
            {
                logger.LogInformation($"adding restriction to database");
                await connection.ExecuteAsync(
                    @"Insert into StudentRestriction values (@studentId1, @studentId2);",
                    new { studentId1 = StudentId1, studentId2 = StudentId2 }
                );
            }
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