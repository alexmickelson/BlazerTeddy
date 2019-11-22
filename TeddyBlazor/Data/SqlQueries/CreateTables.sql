-- SQLite

--first tier
CREATE TABLE TeddyBlazor (
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
    TeddyBlazorId,
    FOREIGN KEY (TeddyBlazorId) REFERENCES TeddyBlazor(Id)
);
CREATE TABLE Course (
    Id integer PRIMARY KEY autoincrement,
    Name text,
    TeacherId integer,
    FOREIGN KEY (TeacherId) REFERENCES Teacher(Id)
);
CREATE TABLE TeddyBlazorRestriction (
    TeddyBlazor1 integer,
    TeddyBlazor2 integer,
    FOREIGN KEY (TeddyBlazor1) REFERENCES TeddyBlazor(Id),
    FOREIGN KEY (TeddyBlazor2) REFERENCES TeddyBlazor(Id)
);

--third tier
CREATE TABLE TeddyBlazorCourse(
    TeddyBlazorId integer,
    CourseId integer,
    FOREIGN KEY (TeddyBlazorId) REFERENCES TeddyBlazor(Id),
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
);
CREATE TABLE Assignment(
    Id integer PRIMARY KEY autoincrement,
    CourseId integer,
    Description text,
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
);
