-- SQLite
--need to get all students with all assignments and all notes

SELECT * 
FROM Student as s
LEFT JOIN StudentCourse as sc ON s.Id = sc.StudentId
LEFT JOIN Course as c ON c.Id = sc.CourseId;
