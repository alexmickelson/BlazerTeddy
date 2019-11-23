-- SQLite
--need to get all students with all assignments and all notes

SELECT *
FROM Student as s
LEFT JOIN StudentCourse as sc ON s.StudentId = sc.StudentId
    LEFT JOIN Course as c ON c.CourseId = sc.CourseId
LEFT JOIN Note as n on n.StudentId = s.StudentId;
