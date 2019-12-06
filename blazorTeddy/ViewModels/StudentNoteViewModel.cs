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

    public class StudentNoteViewModel : INewNoteViewModel, IViewModel
    {
        public Student Student { get; set; }
        public Note Note { get; set; }
        public int TeacherId { get; set; }
        public bool IsAnonymousNote { get; set; }
        public int NoteType { get; set; }
        public string errorAlert { get; set; }
        private readonly IStudentRepository studentRepository;
        private readonly ILogger<StudentNoteViewModel> logger;

        public StudentNoteViewModel(IStudentRepository studentRepository, ILogger<StudentNoteViewModel> logger)
        {
            this.studentRepository = studentRepository;
            this.logger = logger;
            Note = new Note();
        }
        
        public void AddNote()
        {
            logger.LogInformation($"Adding new note");
            if (String.IsNullOrEmpty(Note.Content))
            {
                errorAlert = "Note cannot be Empty";
                return;
            }

            Note.DateCreated = DateTime.Now;
            Note.NoteType = (NoteTypes)NoteType;
            var t = Task.Run(async () =>
            {
                if (IsAnonymousNote)
                {
                    await studentRepository.AddUnsignedNoteAsync(Student, Note);
                }
                else
                {
                    await studentRepository.AddSignedNoteAsync(Student, Note, TeacherId);
                }
            });
            t.Wait();
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

        public void OnInitialized()
        {

        }

        public Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public void OnParametersSet()
        {

        }

        public Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        }
    }

}