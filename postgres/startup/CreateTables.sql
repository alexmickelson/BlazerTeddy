--first tier
CREATE TABLE Student (
    StudentId SERIAL PRIMARY KEY,
    StudentName text
);
CREATE TABLE Teacher(
    TeacherId SERIAL PRIMARY KEY,
    TeacherName text
);
CREATE TABLE ClassRoom(
    ClassRoomId SERIAL PRIMARY KEY,
    ClassRoomName text,
    SeatsHorizontally integer,
    SeatsVertically integer
);
--second tier
CREATE TABLE ClassModel(
    ClassId SERIAL PRIMARY KEY,
    ClassName text,
    TeacherId integer REFERENCES Teacher(TeacherId),
    ClassRoomId integer REFERENCES ClassRoom(ClassRoomId)
);
CREATE TABLE Note(
    NoteId SERIAL PRIMARY KEY,
    Content text,
    NoteType integer,
    StudentId integer REFERENCES Student(StudentId),
    TeacherId integer REFERENCES Teacher(TeacherId),
    DateCreated timestamp
);
CREATE TABLE Course (
    CourseId SERIAL PRIMARY KEY,
    CourseName text,
    TeacherId integer REFERENCES Teacher(TeacherId)
);
CREATE TABLE StudentRestriction (
    Student1 integer REFERENCES Student(StudentId),
    Student2 integer REFERENCES Student(StudentId)
);

--third tier
CREATE TABLE SeatingAssignment(
    ClassId integer REFERENCES ClassModel(ClassId),
    StudentId integer REFERENCES Student(StudentId),
    HorizontalCoordinate integer,
    VerticalCoordinate integer
);
CREATE TABLE StudentCourse(
    StudentId integer REFERENCES Student(StudentId),
    ClassId integer REFERENCES ClassModel(ClassId),
    CourseId integer REFERENCES Course(CourseId)
);
CREATE TABLE Assignment(
    AssignmentId SERIAL PRIMARY KEY,
    CourseId integer REFERENCES Course(CourseId),
    AssignmentName text,
    AssignmentDescription text
);

