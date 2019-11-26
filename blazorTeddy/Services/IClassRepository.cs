using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IClassRepository
    {
        Task AddClassAsync(ClassModel classModel);
        Task<ClassModel> GetClassAsync(int classId);
        Task AddClassRoomAsync(ClassRoom classRoom);
        Task<ClassRoom> GetClassRoomAsync(int classRoomId);
        Task AddTeacherAsync(Teacher teacher);
        Task<Teacher> GetTeacherAsync(int teacherId);
    }
}