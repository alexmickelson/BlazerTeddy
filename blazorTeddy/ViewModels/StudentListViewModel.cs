
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeddyBlazor.Data;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class StudentListViewModel : IBaseViewModel
    {
        public readonly IStudentRepository StudentRepository;
        private readonly IClassRepository classRepository;
        public string NameFilter = "";
        public int ClassIdFilter = -1;

        public StudentListViewModel(IStudentRepository StudentRepository,
                                    IClassRepository classRepository)
        {
            this.StudentRepository = StudentRepository;
            this.classRepository = classRepository;
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
            throw new NotImplementedException();
        }

        public void OnInitializedAsync()
        {
            throw new NotImplementedException();
        }

        public void OnParametersSet()
        {
            throw new NotImplementedException();
        }

        public void OnParametersSetAsync()
        {
            throw new NotImplementedException();
        }

        public void OnAfterRender()
        {
            throw new NotImplementedException();
        }

        public void OnAfterRenderAsync()
        {
            throw new NotImplementedException();
        }
    }
}