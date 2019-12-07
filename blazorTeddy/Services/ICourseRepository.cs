using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface ICourseRepository
    {
        Task AddCourseAsync(Course course);
        Task<Course> GetCourseAsync(int courseId);
        Task<Course> GetCourseAsync(int studentId, int classId);
        Task<IEnumerable<Course>> GetCoursesByClassId(int classId);
    }
}