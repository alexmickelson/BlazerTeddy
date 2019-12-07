INSERT INTO Student (StudentName) VALUES ('Sam');
INSERT INTO Student (StudentName) VALUES ('Tim');
INSERT INTO Student (StudentName) VALUES ('Spencer');

INSERT INTO Teacher (TeacherName) VALUES ('Bob');
INSERT INTO Teacher (TeacherName) VALUES ('Jim');

INSERT INTO ClassRoom VALUES (1, 'Science Room', 5, 6);

INSERT INTO ClassModel (ClassName, TeacherId, ClassRoomId)
VALUES ('Science Room 1:00', 1, 1);
INSERT INTO ClassModel (ClassName, TeacherId, ClassRoomId)
VALUES ('Science Room 3:00', 1, 1);

INSERT INTO Note (Content, StudentId, NoteType, DateCreated)
VALUES ('A note about sam', 1, 1, NOW());

INSERT INTO Course (Name, TeacherId) VALUES ('Math', 1);

INSERT INTO StudentRestriction VALUES(1,2);

--enrole sam and tim in science room at 1 for math
INSERT INTO StudentCourse VALUES(1, 1, 1);
INSERT INTO StudentCourse VALUES(2, 1, 1);

--enrole sam and spencer in science room at 3 for math
INSERT INTO StudentCourse VALUES(1, 2, 1);
INSERT INTO StudentCourse VALUES(3, 2, 1);

INSERT INTO Assignment VALUES(1, 1, 'Math Assignment');

--Science room at 1 seating chart
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (1, 1, 0, 3);
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (1, 2, 1, 1);

--Science room at 3 seating chart
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (2, 1, 0, 3);
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (2, 2, 1, 1);

-- SELECT * FROM Student;
-- SELECT * FROM Teacher;
-- SELECT * FROM Course;
-- SELECT * FROM StudentRestriction;
-- SELECT * FROM Note;
-- SELECT * FROM StudentCourse;
-- SELECT * FRom Assignment;