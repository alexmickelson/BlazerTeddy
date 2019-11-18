using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Models
{
    public class StudentInfo
    {
        public int StudentInfoId { get; set; }
        public string Name { get; set; }

        [ForeignKey("NoteForeignKey")]
        public List<Note> Notes { get; set; }

        [ForeignKey("StudentInfoForeignKey")]
        public List<StudentInfo> Restrictions { get; set; }

        public ICollection<StudentCourseRelationship> StudentCourses { get; set; }
        
        internal void Update(StudentInfo updated)
        {
            if(updated.StudentInfoId != StudentInfoId)
            {
                return;
            }
            Name = updated.Name;
            Notes = updated.Notes ?? Notes;
            Restrictions = updated.Restrictions ?? Restrictions;
        }
    }
}
