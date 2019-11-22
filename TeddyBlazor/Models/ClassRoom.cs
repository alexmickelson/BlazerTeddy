using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeddyBlazor.Models
{
    public class ClassRoom
    {

        public int Id{ get; set; }

        [ForeignKey("TeddyBlazorForeignKey")]
        public List<Student> students { get; set; }

        [ForeignKey("TeacherForeignKey")]
        public List<TeacherInfo> Teachers { get; set; }

        public int[,] SeatingChartByTeddyBlazorID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
