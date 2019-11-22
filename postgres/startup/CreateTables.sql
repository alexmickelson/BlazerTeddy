-- SQLite

--first tier
CREATE TABLE Student (
    Id SERIAL PRIMARY KEY,
    Name text
);
CREATE TABLE Teacher(
    Id SERIAL PRIMARY KEY,
    Name text
);
--second tier
CREATE TABLE Note(
    Id SERIAL PRIMARY KEY,
    Content text,
    StudentId integer REFERENCES Student(Id)
);
CREATE TABLE Course (
    Id SERIAL PRIMARY KEY,
    Name text,
    TeacherId integer REFERENCES Teacher(Id)
);
CREATE TABLE StudentRestriction (
    Student1 integer REFERENCES Student(Id),
    Student2 integer REFERENCES Student(Id)
);

--third tier
CREATE TABLE StudentCourse(
    StudentId integer REFERENCES Student(Id),
    CourseId integer REFERENCES Course(Id)
);
CREATE TABLE Assignment(
    Id SERIAL PRIMARY KEY,
    CourseId integer REFERENCES Course(Id),
    Description text
);
