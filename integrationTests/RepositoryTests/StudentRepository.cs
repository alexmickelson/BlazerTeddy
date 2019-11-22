namespace IntegrationTests.RepositoryTests
{
    public class StudentRepositoryTests
    {
        private DbContextOptions<OurDbContext> dbOptions;
        private StudentRepository studentRepository;
        private OurDbContext context;
        private OurDbContext context2;
        private SqliteConnection sqlite;
        private Mock<IConfiguration> configMoq;
        private Func<IDbConnection> getDbConnection;

        [SetUp]
        public void Setup()
        {

            var dbOptions = new DbContextOptionsBuilder<OurDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentRepository")
                .EnableSensitiveDataLogging()
                .Options;
            context = new OurDbContext(dbOptions);
            context2 = new OurDbContext(dbOptions);

            getDbConnection = DbConnectionFactory.GetMemoryConnection();


            studentRepository = new StudentRepository(getDbConnection);
        }

        [TearDown]
        public void TearDown()
        {
        }

        // [Test]
        // public async Task EditingReferenceOfTeddyBlazorListEditsRepostoryList()
        // {
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo()
        //     {
        //         Name = "Sam"
        //     };

        //     students.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();
        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     var sam = TeddyBlazorRepostiory.Get(3);
        //     students.Should().BeSameAs(TeddyBlazorRepostiory.GetList());
        //     sam.Name.Should().Be(TeddyBlazor.Name);
        // }

        // [Test]
        // public void AddingNotesOnstudentsPersistsToRepostitory()
        // {
        //     var students = studentRepository.GetList();
        //     int studentId = 3;
        //     var student = new Student()
        //     {
        //         StudentId = studentId,
        //         Name = "Sam"
        //     };

        //     students.Add(student);
        //     student.Notes = new List<Note>();
        //     student.Notes.Add(new Note() { Content = "this is a note" });

        //     var sam = studentRepository.Get(studentId);
        //     sam.Notes.First().Content.Should().Be("this is a note");
        // }

        // [Test]
        // public async Task ChangesPersistAfterReloadingDatabase()
        // {
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };
        //     students.Add(TeddyBlazor);

        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     var sam = TeddyBlazorRepostiory.Get(TeddyBlazorId);
        //     students.Should().BeSameAs(TeddyBlazorRepostiory.GetList());
        //     sam.Name.Should().Be(TeddyBlazor.Name);
        // }

        // [Test]
        // public async Task ChangesTostudentsPersistAcrossStudentRepositoryInstances()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context2, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);

        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();
        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();

        //     TeddyBlazorRepostiory2.Get(TeddyBlazorId).Name.Should().Be("Sam");
        // }

        // [Test]
        // public async Task UpdatingSingleTeddyBlazorPersistAcrossStudentRepositoryInstances()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };

        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     TeddyBlazor.Name = "richard";
        //     TeddyBlazorRepostiory2.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory2.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     TeddyBlazorRepostiory.Get(TeddyBlazorId).Name.Should().Be("richard");
        // }

        // [Test]
        // public async Task UpdatingTeddyBlazorRestrictionsPersists()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = 3,
        //         Name = "Sam"
        //     };
        //     var TeddyBlazor2 = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = 4,
        //         Name = "bill"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     TeddyBlazorRepostiory.Add(TeddyBlazor2);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     TeddyBlazor.Restrictions = new List<TeddyBlazorInfo>(){ TeddyBlazor2 };
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();
        //     TeddyBlazorRepostiory2.Get(3).Restrictions.Should().Contain(TeddyBlazor2);
        // }

        // [Test]
        // public async Task UpdatingTeddyBlazorNotesPersists()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo()
        //     {
        //         TeddyBlazorInfoId = 3,
        //         Name = "Sam"
        //     };
        //     var note = new Note()
        //     {
        //         NoteId = 5,
        //         Content = "sam's note"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory.AddNoteAsync(TeddyBlazor, note);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();
        //     TeddyBlazorRepostiory2.Get(3).Notes.Should().Contain(note);
        // }
    }
}