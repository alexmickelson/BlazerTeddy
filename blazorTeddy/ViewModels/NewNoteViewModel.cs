using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using static TeddyBlazor.Models.Note;

namespace TeddyBlazor.ViewModels
{

    public class NewNoteViewModel : INewNoteViewModel
    {
        public Student Student { get; set; }
        public Note Note { get; set; }
        public int TeacherId { get; set; }
        public bool IsAnonymousNote { get; set; }
        public int NoteType { get; set; }
        public string errorAlert { get; set; }
        private readonly IStudentRepository studentRepository;
        private readonly ILogger<NewNoteViewModel> logger;

        public NewNoteViewModel(IStudentRepository studentRepository, ILogger<NewNoteViewModel> logger)
        {
            this.studentRepository = studentRepository;
            this.logger = logger;
            Note = new Note();
        }
        public async Task AddNoteAsync()
        {
            logger.LogInformation($"Adding new note");
            if (String.IsNullOrEmpty(Note.Content))
            {
                errorAlert = "Note cannot be Empty";
                return;
            }

            Note.DateCreated = DateTime.Now;
            Note.NoteType = (NoteTypes)NoteType;
            if (IsAnonymousNote)
            {
                await studentRepository.AddUnsignedNoteAsync(Student, Note);
            }
            else
            {
                await studentRepository.AddSignedNoteAsync(Student, Note, TeacherId);
            }
            errorAlert = "";
            Note = new Note()
            {

            };
        }

        public void SetStudent(Student student)
        {
            Student = student;
        }

        public IEnumerable<(int, string)> GetNoteTypeOptions()
        {
            IEnumerable<(int, string)> options = new (int, string)[] { };
            foreach (var type in (NoteTypes[])Enum.GetValues(typeof(NoteTypes)))
            {
                var option = ((int)type, Note.TypeToString(type));
                options = options.Append(option);
            }
            return options;
        }
    }

}