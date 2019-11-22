-- SQLite
INSERT INTO TeddyBlazor (Name) VALUES ("Sam");
INSERT INTO TeddyBlazor (Name) VALUES ("Tim");
INSERT INTO TeddyBlazor (Name) VALUES ("Spencer");

INSERT INTO Teacher (Name) VALUES ("Bob");
INSERT INTO Teacher (Name) VALUES ("Jim");

INSERT INTO Note VALUES (1, "A note about sam", 1);

INSERT INTO Course (Name, TeacherId) VALUES ("Math", 1);

INSERT INTO TeddyBlazorRestriction VALUES(1,2);
INSERT INTO TeddyBlazorRestriction VALUES(2,1);

INSERT INTO TeddyBlazorCourse VALUES(1, 1);
INSERT INTO TeddyBlazorCourse VALUES(2, 1);

INSERT INTO Assignment VALUES(1, 1, "Math Assignment");

SELECT * FROM TeddyBlazor;
SELECT * FROM Teacher;
SELECT * FROM Course;
SELECT * FROM TeddyBlazorRestriction;
SELECT * FROM Note;
SELECT * FROM TeddyBlazorCourse;
SELECT * FRom Assignment;