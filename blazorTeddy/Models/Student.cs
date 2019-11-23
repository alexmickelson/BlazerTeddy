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
        public IEnumerable<Note> Notes { get; set; } = new Note[0];

        [Write(false)]
        public IEnumerable<Student> Restrictions { get; set; } = new Student[0];

        [Write(false)]
        public bool StoredInDatabase { get => StudentId != default(int);}
        
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
