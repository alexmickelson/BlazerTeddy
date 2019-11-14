using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Models
{
    public class ClassRoomModel
    {

        public int Id{ get; set; }
        public List<StudentInfo> Students { get; set; }

        //public List<TeacherInfo> Teachers { get; set; }

        //public SeatingChart seatingChart { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
