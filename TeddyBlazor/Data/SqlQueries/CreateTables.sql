-- SQLite

--first tier
CREATE TABLE Student (
    Id integer PRIMARY KEY autoincrement,
    Name text
);
CREATE TABLE Teacher(
    Id integer PRIMARY KEY autoincrement,
    Name text
);
--second tier
CREATE TABLE Note(
    Id integer PRIMARY KEY autoincrement,
    Content text,
    StudentId,
    FOREIGN KEY (StudentId) REFERENCES Student(Id)
);
CREATE TABLE Course (
    Id integer PRIMARY KEY autoincrement,
    Name text,
    TeacherId integer,
    FOREIGN KEY (TeacherId) REFERENCES Teacher(Id)
);
CREATE TABLE StudentRestriction (
    Student1 integer,
    Student2 integer,
    FOREIGN KEY (Student1) REFERENCES Student(Id),
    FOREIGN KEY (Student2) REFERENCES Student(Id)
);

--third tier
CREATE TABLE StudentCourse(
    StudentId integer,
    CourseId integer,
    FOREIGN KEY (StudentId) REFERENCES Student(Id),
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
);
CREATE TABLE Assignment(
    Id integer PRIMARY KEY autoincrement,
    CourseId integer,
    Description text,
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
);
