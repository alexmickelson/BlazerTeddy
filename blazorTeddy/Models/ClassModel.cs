using System.Collections.Generic;

namespace TeddyBlazor.Models
{
    public class ClassModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Teacher Teacher { get; set; }
        public IEnumerable<Student> Students { get; set; }
        
    }
}