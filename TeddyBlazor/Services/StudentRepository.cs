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
        private readonly OurDbContext dbContext;
        private readonly string connectionString;
        private string getStudentsSql;

        public List<Student> students { get; set; }
        public Func<IDbConnection> GetDbConnection { get; }

        public StudentRepository(OurDbContext dbContext, Func<IDbConnection> getDbConnection)
        {

            getStudentsSql = File.ReadAllText(Directory.GetCurrentDirectory() + "/../../../../TeddyBlazor/Data/SqlQueries/Getstudents.sql");
            students = new List<Student>();
            this.dbContext = dbContext;
            GetDbConnection = getDbConnection;

            var t = InitializeTeddyBlazorAsync();
        }
        public async Task InitializeTeddyBlazorAsync()
        {
            using (var dbConnection = GetDbConnection())
            {
                var multipleResults =  await dbConnection.QueryMultipleAsync(getStudentsSql);
                students = multipleResults.Read<Student>().ToList();
            }
        }
        public async Task UpdateDatabaseAsync()
        {
            foreach (var student in students)
            {
                if (dbContext.Students.Contains(student))
                {
                    var s = dbContext.Students.First(s => s.StudentId == student.StudentId);
                    s.Update(student);
                    dbContext.Students.Update(s);
                }
                else
                {
                    await dbContext.Students.AddAsync(student);
                }
            }
        }

        public List<Student> GetList()
        {
            var t = InitializeTeddyBlazorAsync();
            return students;
        }

        public async Task<Student> GetTeddyBlazorAsync(int id)
        {
            await InitializeTeddyBlazorAsync();
            return students.FirstOrDefault(s => s.StudentId == id);
        }

        public Student Get(int TeddyBlazorId)
        {
            var s = students.FirstOrDefault(s => s.StudentId == TeddyBlazorId);
            return s;
        }

        public void Add(Student TeddyBlazor)
        {
            students.Add(TeddyBlazor);
        }


        public async Task AddNoteAsync(Student TeddyBlazor, Note note)
        {
            if (TeddyBlazor.Notes == null)
            {
                TeddyBlazor.Notes = new List<Note>();
            }
            await dbContext.Notes.AddAsync(note);
            TeddyBlazor.Notes.Add(note);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddRestrictionAsync(int TeddyBlazorId1, int TeddyBlazorId2)
        {
            var TeddyBlazor1 = Get(TeddyBlazorId1);
            var TeddyBlazor2 = Get(TeddyBlazorId2);
            if (TeddyBlazor1.Restrictions == null)
            {
                TeddyBlazor1.Restrictions = new List<Student>();
            }
            if (TeddyBlazor2.Restrictions == null)
            {
                TeddyBlazor2.Restrictions = new List<Student>();
            }
            TeddyBlazor1.Restrictions.Add(TeddyBlazor2);
            TeddyBlazor2.Restrictions.Add(TeddyBlazor1);
            await dbContext.SaveChangesAsync();
        }

    }
}