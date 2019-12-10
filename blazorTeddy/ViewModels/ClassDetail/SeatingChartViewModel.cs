using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels.ClassDetail
{
    public class SeatingChartViewModel : IViewModel
    {
        private readonly IStudentRepository studentRepository;
        private readonly IClassRepository classRepository;
        private readonly ICourseRepository courseRepository;
        public IEnumerable<Student> StudentModels { get; set; }

        public ClassModel SelectedClass { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Student[,] Students { get; set; }
        public double HorizontalFraction => ((1.0 / this.ClassRoom.SeatsHorizontally) * 100) - 1;
        public int SelectedStudentId { get; set; }
        public SeatingChartViewModel(IStudentRepository studentRepository,
                                     IClassRepository classRepository,
                                     ICourseRepository courseRepository)
        {
            this.studentRepository = studentRepository;
            this.classRepository = classRepository;
            this.courseRepository = courseRepository;
            SelectedClass = new ClassModel();
            ClassRoom = new ClassRoom();
            Students = new Student[ClassRoom.SeatsHorizontally, ClassRoom.SeatsVertically];
            StudentModels = new Student[] {};
        }

        public void AssignSeat()
        {
            return;
        }

        public string GetCourse(int i, int j)
        {
            if (!seatInBounds(i, j))
            {
                return string.Empty;
            }
            var studentId = SelectedClass.SeatingChartByStudentID[i, j];
            if (studentId == default(int))
            {
                return string.Empty;
            }
            else
            {
                var t = Task.Run(async () =>
                {
                    var course = await courseRepository.GetCourseAsync(studentId, SelectedClass.ClassId);
                    return course.CourseName;
                });
                return t.Result;
            }
        }

        public string GetStudentLink(int i, int j)
        {
            var noLink = $"/classDetail/{SelectedClass.ClassId}";
            if (!seatInBounds(i, j))
            {
                return noLink;
            }
            var studentId = SelectedClass.SeatingChartByStudentID[i, j];
            return studentId == default(int)
                ? noLink
                : $"/StudentDetail/{studentId}";
        }

        private bool seatInBounds(int i, int j)
        {
            var chartHorizontal = SelectedClass.SeatingChartByStudentID.GetLength(0);
            var chartVertical = SelectedClass.SeatingChartByStudentID.GetLength(1);

            return i < chartHorizontal && j < chartVertical;
        }

        public async Task LoadSeatingChart()
        {
            var roomHorizontal = ClassRoom.SeatsHorizontally;
            var roomVertical = ClassRoom.SeatsVertically;
            var chartHorizontal = SelectedClass.SeatingChartByStudentID.GetLength(0);
            var chartVertical = SelectedClass.SeatingChartByStudentID.GetLength(1);
            Students = newDefaultStudents(roomHorizontal, roomVertical);

            for (int i = 0; i < chartHorizontal; i++)
            {
                for (int j = 0; j < chartVertical; j++)
                {
                    var studentId = SelectedClass.SeatingChartByStudentID[i, j];
                    if (studentId != default(int))
                    {
                        Students[i, j] = await studentRepository.GetStudentAsync(studentId);
                    }
                }
            }
        }

        private Student[,] newDefaultStudents(int horizontal, int vertical)
        {
            var students = new Student[horizontal, vertical];
            for (int i = 0; i < horizontal; i++)
            {
                for (int j = 0; j < vertical; j++)
                {
                    students[i, j] = new Student();
                }
            }
            return students;
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
        public async Task OnParametersSetAsync()
        {
            ClassRoom = SelectedClass.ClassRoomId == 0
                ? ClassRoom
                : await classRepository.GetClassRoomAsync(SelectedClass.ClassRoomId);
            await LoadSeatingChart();
            StudentModels = await studentRepository.GetStudentsByClassAsync(SelectedClass.ClassId);
        }
    }
}