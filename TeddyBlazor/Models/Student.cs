using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeddyBlazor.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("NoteForeignKey")]
        public List<Note> Notes { get; set; }

        [ForeignKey("TeddyBlazorInfoForeignKey")]
        public List<Student> Restrictions { get; set; }

        public IEnumerable<Course> Courses {get; set;}
        
        internal void Update(Student updated)
        {
            if(updated.Id != Id)
            {
                return;
            }
            Name = updated.Name;
            Notes = updated.Notes ?? Notes;
            Restrictions = updated.Restrictions ?? Restrictions;
        }
    }
}
