-- SQLite
DROP TABLE StudentCourse;
DROP TABLE Assignment;

DROP TABLE Course;
Drop TABLE Note;
DROP TABLE StudentRestriction;

DROP TABLE Teacher;
DROP TABLE Student;

--first tier
CREATE TABLE Student (
    Id integer PRIMARY KEY,
    Name text
);
CREATE TABLE Teacher(
    Id integer PRIMARY KEY,
    Name text
);
--second tier
CREATE TABLE Note(
    Id integer PRIMARY KEY,
    Content text,
    StudentId,
    FOREIGN KEY (StudentId) REFERENCES Student(Id)
);
CREATE TABLE Course (
    Id integer PRIMARY KEY,
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
    Id integer PRIMARY KEY,
    CourseId integer,
    Description text,
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
);

INSERT INTO Student VALUES (1, "Sam");
INSERT INTO Student VALUES (2, "Tim");
INSERT INTO Teacher VALUES (1, "Bob");

INSERT INTO Note VALUES (1, "A note about sam", 1);
INSERT INTO Course VALUES (1, "Math", 1);
INSERT INTO StudentRestriction VALUES(1,2);
INSERT INTO StudentRestriction VALUES(2,1);

INSERT INTO StudentCourse VALUES(1, 1);
INSERT INTO StudentCourse VALUES(2, 1);
INSERT INTO Assignment VALUES(1, 1, "Math Assignment");

SELECT * FROM Student;
SELECT * FROM Teacher;
SELECT * FROM Course;
SELECT * FROM StudentRestriction;
SELECT * FROM Note;
SELECT * FROM StudentCourse;
SELECT * FRom Assignment;
