using System.Collections.Generic;

namespace TeddyBlazor.Models
{
    public class ClassModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int ClassRoomId { get; set; }
        public int TeacherId { get; set; }
        public IEnumerable<int> StudentIds { get; set; }
        public int[,] SeatingChartByStudentID { get; set; } = new int[0,0];
        
    }
}