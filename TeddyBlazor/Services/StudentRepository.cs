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
        public async Task InitializeStudentsAsync()
        {
            using (var dbConnection = GetDbConnection())
            {
                var multipleResults =  await dbConnection.QueryMultipleAsync(SqlStrings.GetStudents);
                students = multipleResults.Read<Student>().ToList();
            }
        }
        public async Task UpdateDatabaseAsync()
        {
            foreach (var student in students)
            {
            }
        }

        public List<Student> GetList()
        {
            var t = InitializeStudentsAsync();
            return students;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            await InitializeStudentsAsync();
            return students.FirstOrDefault(s => s.StudentId == id);
        }

        public Student Get(int StudentId)
        {
            var s = students.FirstOrDefault(s => s.StudentId == StudentId);
            return s;
        }

        public void Add(Student Student)
        {
            students.Add(Student);
        }


        public async Task AddNoteAsync(Student Student, Note note)
        {
            if (Student.Notes == null)
            {
                Student.Notes = new List<Note>();
            }
            //TODO: add note to database
            Student.Notes.Add(note);
            //TODO: update student in database
            
        }

        public async Task AddRestrictionAsync(int StudentId1, int StudentId2)
        {
            var Student1 = Get(StudentId1);
            var Student2 = Get(StudentId2);
            if (Student1.Restrictions == null)
            {
                Student1.Restrictions = new List<Student>();
            }
            if (Student2.Restrictions == null)
            {
                Student2.Restrictions = new List<Student>();
            }
            Student1.Restrictions.Add(Student2);
            Student2.Restrictions.Add(Student1);
            //TODO: Update database to include student restrictions
        }

    }
}