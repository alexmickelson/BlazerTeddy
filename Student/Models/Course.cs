using System.Collections.Generic;

namespace Student.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public ICollection<StudentCourseRelationship> StudentCourses { get; set; }
    }
}