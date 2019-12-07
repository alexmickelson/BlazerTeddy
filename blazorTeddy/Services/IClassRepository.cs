using System.Collections.Generic;
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
        Task UpdateClassRoomAsync(ClassRoom classRoom);
        Task UpdateTeacherAsync(Teacher teacher);
        Task UpdateClassAsync(ClassModel classModel);
        Task<IEnumerable<ClassModel>> GetClassesByTeacherId(int teacherId);
        Task<IEnumerable<ClassModel>> GetAllClassesAsync();
        Task EnrollStudentAsync(int studentId, int classId, int courseId);
    }
}