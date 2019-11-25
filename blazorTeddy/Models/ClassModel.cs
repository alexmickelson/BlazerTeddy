using System.Collections.Generic;

namespace TeddyBlazor.Models
{
    public class ClassModel
    {
        public ClassRoom ClassRoom { get; set; }
        public Teacher Teacher { get; set; }
        public IEnumerable<Student> Students { get; set; }
        
    }
}