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

namespace TeddyBlazor.Services {

    public class StudentRepository : IStudentRepository
    {

        private List<Student> students { get; set; }
        private Func<IDbConnection> GetDbConnection { get; }

        public StudentRepository(Func<IDbConnection> getDbConnection)
        {
            students = new List<Student>();
            GetDbConnection = getDbConnection;
            
        }
        public async Task UpdateStudentsAsync()
        {
            using (var dbConnection = GetDbConnection())
            {
                students = (await dbConnection.QueryAsync<Student>(
                    @"SELECT * FROM Student;")).ToList();
                var notes = (await dbConnection.QueryAsync<Note>(
                    @"SELECT * FROM Note;"));
                var restrictions = (await dbConnection.QueryAsync<(int, int)>(
                    @"SELECT * FROM StudentRestriction;"));
                foreach (var student in students)
                {
                    student.Notes = notes.Where(n => n.StudentId == student.StudentId);
                    student.Restrictions = assignRestrictions(restrictions, student.StudentId);
                }

            }
        }

        private static IEnumerable<int> assignRestrictions(IEnumerable<(int, int)> restrictions, int studentId)
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
            using (var dbConnection = GetDbConnection())
            {
                foreach (var student in students)
                {
                    if (student.StoredInDatabase)
                    {
                        await updateStudentToDb(dbConnection, student);
                    }
                    else
                    {
                        student.StudentId = await addStudentToDb(dbConnection, student);
                    }
                }
            }
        }

        private static async Task updateStudentToDb(IDbConnection dbConnection, Student student)
        {
            await dbConnection.ExecuteAsync(
                @"UPDATE Student SET StudentName='@studentName' WHERE StudentId=@id;",
                new { studentName = student.StudentName, id = student.StudentId });
        }

        private static async Task<int> addStudentToDb(IDbConnection dbConnection, Student student)
        {
            return await dbConnection.QueryFirstAsync<int>(
                @"INSERT INTO Student (StudentName) Values (@studentName) RETURNING StudentId;",
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

        public async Task AddStudentAsync(Student Student)
        {
            students.Add(Student);
            await SaveChangesAsync();
            
        }

        public async Task AddNoteAsync(Student student, Note note)
        {
            if (student.Notes == null)
            {
                student.Notes = new List<Note>();
            }
            using(var dbConnection = GetDbConnection())
            {
                note.NoteId = await dbConnection.QueryFirstAsync<int>(
                    "insert into Note (Content, StudentId) values (@content, @studentId) RETURNING NoteId;",
                    new { content = note.Content, studentId = student.StudentId });
            }
        }

        public async Task AddRestrictionAsync(int StudentId1, int StudentId2)
        {
            await UpdateStudentsAsync();
            using (var connection = GetDbConnection())
            {
                await connection.ExecuteAsync(
                    @"Insert into StudentRestriction values (@studentId1, @studentId2);",
                    new { studentId1 = StudentId1, studentId2 = StudentId2 }
                );
            }
        }

    }
}