INSERT INTO Student (StudentName) VALUES ('Sam');
INSERT INTO Student (StudentName) VALUES ('Tim');
INSERT INTO Student (StudentName) VALUES ('Spencer');

INSERT INTO Teacher (TeacherName) VALUES ('Bob');
INSERT INTO Teacher (TeacherName) VALUES ('Jim');

INSERT INTO ClassRoom VALUES (1, 'Science Room', 5, 6);

INSERT INTO ClassModel VALUES (1, 'A Class', 1, 1);

INSERT INTO Note (Content, StudentId, NoteType, DateCreated)
VALUES ('A note about sam', 1, 1, NOW());

INSERT INTO Course (Name, TeacherId) VALUES ('Math', 1);

INSERT INTO StudentRestriction VALUES(1,2);

INSERT INTO StudentCourse VALUES(1, 1, 1);
INSERT INTO StudentCourse VALUES(2, 1, 1);

INSERT INTO Assignment VALUES(1, 1, 'Math Assignment');

-- SELECT * FROM Student;
-- SELECT * FROM Teacher;
-- SELECT * FROM Course;
-- SELECT * FROM StudentRestriction;
-- SELECT * FROM Note;
-- SELECT * FROM StudentCourse;
-- SELECT * FRom Assignment;