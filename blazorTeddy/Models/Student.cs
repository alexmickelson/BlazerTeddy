using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace TeddyBlazor.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        [Write(false)]
        public List<Note> Notes { get; set; } = new List<Note>();

        [Write(false)]
        public List<Student> Restrictions { get; set; } = new List<Student>();

        [Write(false)]
        public IEnumerable<Course> Courses {get; set;} = new List<Course>();
        
        internal void Update(Student updated)
        {
            if(updated.StudentId != StudentId)
            {
                return;
            }
            StudentName = updated.StudentName;
            Notes = updated.Notes ?? Notes;
            Restrictions = updated.Restrictions ?? Restrictions;
        }
    }
}
