using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.ViewModels
{
    public interface INewNoteViewModel
    {
        Student Student { get; set; }
        Note Note { get; set; }
        int TeacherId { get; set; }
        bool IsAnonymousNote { get; set; }
        string errorAlert { get; set; }
        int NoteType { get; set; }

        Task AddNoteAsync();
        IEnumerable<(int, string)> GetNoteTypeOptions();
        void SetStudent(Student student);
    }

}