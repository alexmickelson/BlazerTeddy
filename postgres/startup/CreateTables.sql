-- SQLite

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
    ClassRoomName text
);
--second tier
CREATE TABLE Class(
    ClassId SERIAL PRIMARY KEY,
    TeacherId integer REFERENCES Teacher(TeacherId),
    ClassRoomId integer REFERENCES ClassRoom(ClassRoomId)
);
CREATE TABLE Note(
    NoteId SERIAL PRIMARY KEY,
    Content text,
    StudentId integer REFERENCES Student(StudentId)
);
CREATE TABLE Course (
    CourseId SERIAL PRIMARY KEY,
    Name text,
    TeacherId integer REFERENCES Teacher(TeacherId)
);
CREATE TABLE StudentRestriction (
    Student1 integer REFERENCES Student(StudentId),
    Student2 integer REFERENCES Student(StudentId)
);

--third tier
CREATE TABLE SeatingAssignment(
    ClassId integer REFERENCES Class(ClassId),
    StudentId integer REFERENCES Student(StudentId),
    xSeatCoordinate integer,
    ySeatCoordinate integer
);
CREATE TABLE StudentCourse(
    StudentId integer REFERENCES Student(StudentId),
    CourseId integer REFERENCES Course(CourseId)
);
CREATE TABLE Assignment(
    AssignmentId SERIAL PRIMARY KEY,
    CourseId integer REFERENCES Course(CourseId),
    Description text
);

