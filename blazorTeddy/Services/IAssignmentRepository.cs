using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IAssignmentRepository
    {
        Task AddAssignmentAsync(Assignment assignment);
        Task<Assignment> GetAssignmentAsync(int assignmentId);
        Task<IEnumerable<Assignment>> GetAssignmentsAsync(int courseId);
        Task UpdateAssignmentAsync(Assignment assignment);
    }
}