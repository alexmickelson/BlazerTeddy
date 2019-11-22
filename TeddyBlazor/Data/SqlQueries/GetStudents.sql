-- SQLite
--need to get all students with all assignments and all notes

SELECT * 
FROM TeddyBlazor as s
LEFT JOIN TeddyBlazorCourse as sc ON s.Id = sc.TeddyBlazorId
LEFT JOIN Course as c ON c.Id = sc.CourseId;
