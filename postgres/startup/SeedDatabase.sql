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

INSERT INTO Course (CourseName, TeacherId) VALUES ('Math 1010', 1);
INSERT INTO Course (CourseName, TeacherId) VALUES ('Computer Science 1400', 1);

INSERT INTO StudentRestriction VALUES(1,2);

--enrole sam and tim in science room at 1 for math1010
INSERT INTO StudentCourse VALUES(1, 1, 1);
INSERT INTO StudentCourse VALUES(2, 1, 1);
--enrole spencer in the science room at 1 for computer science
INSERT INTO StudentCourse VALUES(3, 1, 2);

--enrole sam and spencer in science room at 3 for computer Science
INSERT INTO StudentCourse VALUES(1, 2, 2);
INSERT INTO StudentCourse VALUES(3, 2, 2);

--Science room at 1 seating chart
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (1, 1, 0, 3);
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (1, 2, 1, 1);

--Science room at 3 seating chart
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (2, 1, 0, 3);
INSERT INTO SeatingAssignment (ClassId, StudentId, HorizontalCoordinate, VerticalCoordinate)
Values (2, 3, 1, 1);

--add two assignments to math1010

INSERT INTO Assignment (CourseId, AssignmentName, AssignmentDescription)
VALUES (1, 'read chapter one', 'read all of chapter one');
INSERT INTO Assignment (CourseId, AssignmentName, AssignmentDescription)
VALUES (1, 'bookwork', 'do some bookwork');

-- SELECT * FROM Student;
-- SELECT * FROM Teacher;
-- SELECT * FROM Course;
-- SELECT * FROM StudentRestriction;
-- SELECT * FROM Note;
-- SELECT * FROM StudentCourse;
-- SELECT * FRom Assignment;