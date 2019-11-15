using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Models
{
    public class ClassRoomModel
    {

        public int Id{ get; set; }

        [ForeignKey("StudentForeignKey")]
        public List<StudentInfo> Students { get; set; }

        [ForeignKey("TeacherForeignKey")]
        public List<TeacherInfo> Teachers { get; set; }

        public int[,] SeatingChartByStudentID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
