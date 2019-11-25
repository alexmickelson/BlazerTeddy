using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IClassRepository
    {
        Task AddClassAsync(ClassModel classModel);
        Task<ClassModel> GetClassAsync(int classId);
    }
}