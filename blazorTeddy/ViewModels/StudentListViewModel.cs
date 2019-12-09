
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using TeddyBlazor.Data;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class StudentListViewModel : IViewModel
    {
        public readonly IStudentRepository StudentRepository;
        private readonly IClassRepository classRepository;
        private readonly IRefreshService refreshService;
        private readonly ILogger<StudentListViewModel> logger;
        public string NameFilter = "";
        public int ClassIdFilter = -1;
        public Action Refresh { get; set; }

        public StudentListViewModel(IStudentRepository StudentRepository,
                                    IClassRepository classRepository,
                                    IRefreshService refreshService,
                                    ILogger<StudentListViewModel> logger)
        {
            this.StudentRepository = StudentRepository;
            this.classRepository = classRepository;
            this.refreshService = refreshService;
            this.logger = logger;
        }
        ~StudentListViewModel()
        {
            refreshService.RemoveRefresh(nameof(StudentListViewModel));
        }

        public IEnumerable<ClassModel> GetClassOptions()
        {
            var task = Task.Run(async () => 
            {
                return await classRepository.GetAllClassesAsync();
            });
            return task.Result;
        }

        public IEnumerable<Student> GetFilteredStudents()
        {
            var t = Task.Run(async () => 
            {
                return await StudentRepository.GetListAsync();
            });
            IEnumerable<Student> students = t.Result;

            if(ClassIdFilter > 0)
            {
                students = filterByName(students);
                return filterByClass(students);
            }
            else
            {
                return filterByName(students);
            }
        }

        private IEnumerable<Student> filterByClass(IEnumerable<Student> students)
        {
            var t = Task.Run(async () => 
            {
                return await StudentRepository.GetStudentsByClassAsync(ClassIdFilter);
            });
            var newStudents = t.Result;
            return students.Join(
                newStudents,
                s => s.StudentId,
                ns => ns.StudentId,
                (s, ns) => s);
        }

        public IEnumerable<Student> filterByName(IEnumerable<Student> students)
        {
            return students.Where(s => s.StudentName.ToLower().StartsWith(NameFilter.ToLower()));
        }

        public IEnumerable<Student> filterByName(IEnumerable<Student> students, string namefilter)
        {
            return students.Where(s => s.StudentName.ToLower().StartsWith(namefilter.ToLower()));
        }

        public void OnInitialized()
        {
            logger.LogInformation("Adding self to refresh service");
            refreshService.AddOrUpdateRefresh(nameof(StudentListViewModel), Refresh);
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