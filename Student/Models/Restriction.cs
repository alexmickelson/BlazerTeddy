namespace Student.Models
{
    public class Restriction
    {

        public int ID { get; set; }

        public StudentInfo Student { get; set; }

        public StudentInfo RestrictedStudent { get; set; }
    }
}