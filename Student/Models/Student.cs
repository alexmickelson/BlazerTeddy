using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<Note> Notes { get; set; }


    }
}
