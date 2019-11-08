using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Models
{
    public class Note
    {

        public int ID { get; set; }

        public string Content { get; set; }

        public Student Student  { get; set; }
    }
}
