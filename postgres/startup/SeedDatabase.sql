-- SQLite
INSERT INTO Student (Name) VALUES ('Sam');
INSERT INTO Student (Name) VALUES ('Tim');
INSERT INTO Student (Name) VALUES ('Spencer');

INSERT INTO Teacher (Name) VALUES ('Bob');
INSERT INTO Teacher (Name) VALUES ('Jim');

INSERT INTO Note VALUES (1, 'A note about sam', 1);

INSERT INTO Course (Name, TeacherId) VALUES ('Math', 1);

INSERT INTO StudentRestriction VALUES(1,2);
INSERT INTO StudentRestriction VALUES(2,1);

INSERT INTO StudentCourse VALUES(1, 1);
INSERT INTO StudentCourse VALUES(2, 1);

INSERT INTO Assignment VALUES(1, 1, 'Math Assignment');

SELECT * FROM Student;
SELECT * FROM Teacher;
SELECT * FROM Course;
SELECT * FROM StudentRestriction;
SELECT * FROM Note;
SELECT * FROM StudentCourse;
SELECT * FRom Assignment;