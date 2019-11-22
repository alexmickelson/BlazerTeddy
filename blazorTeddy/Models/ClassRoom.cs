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
        public int[,] SeatingChartByStudentID { get; set; }
        public string Name { get; set; }
    }
}
