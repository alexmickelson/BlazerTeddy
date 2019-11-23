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

        public List<Student> students { get; set; }
        public Func<IDbConnection> GetDbConnection { get; }

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
                    @"SELECT * FROM Student")).ToList();
                var notes = (await dbConnection.QueryAsync<Note>(
                    @"SELECT * FROM Note"));
                foreach (var student in students)
                {
                    student.Notes = notes.Where(n => n.StudentId == student.StudentId);
                }

            }
        }

        public async Task SaveChangesAsync()
        {
            using (var dbConnection = GetDbConnection())
            {
                foreach (var student in students)
                {
                    if (student.StoredInDatabase)
                    {
                        await UpdateStudentToDb(dbConnection, student);
                    }
                    else
                    {
                        student.StudentId = await AddStudentToDb(dbConnection, student);
                    }
                }
            }
        }

        private static async Task UpdateStudentToDb(IDbConnection dbConnection, Student student)
        {
            await dbConnection.ExecuteAsync(
                @"UPDATE Student SET StudentName='@studentName' WHERE StudentId=@id;",
                new { studentName = student.StudentName, id = student.StudentId });
        }

        private static async Task<int> AddStudentToDb(IDbConnection dbConnection, Student student)
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

        public async Task AddAsync(Student Student)
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
            var Student1 = await GetStudentAsync(StudentId1);
            var Student2 = await GetStudentAsync(StudentId2);
            if (Student1.Restrictions == null)
            {
                Student1.Restrictions = new List<Student>();
            }
            if (Student2.Restrictions == null)
            {
                Student2.Restrictions = new List<Student>();
            }
            // Student1.Restrictions.Add(Student2);
            // Student2.Restrictions.Add(Student1);
            //TODO: Update database to include student restrictions
        }

    }
}