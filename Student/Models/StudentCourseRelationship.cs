namespace Student.Models
{
    public class StudentCourseRelationship
    {
        public int StudentInfoId { get; set; }
        public StudentInfo student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}