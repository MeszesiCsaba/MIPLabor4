-- LAB4 command-line exercise.
-- Open sqlite3.exe, then run these statements.

CREATE TABLE users
(
    FirstName TEXT,
    LastName  TEXT,
    CNP       TEXT
);

INSERT INTO users (FirstName, LastName, CNP)
VALUES ('Ana', 'Popescu', '0012345678901');

INSERT INTO users (FirstName, LastName, CNP)
VALUES ('Mihai', 'Ionescu', '1980202123456');

INSERT INTO users (FirstName, LastName, CNP)
VALUES ('Elena', 'Dumitru', '2990303123456');

SELECT * FROM users;
DELETE FROM users;
DROP TABLE users;

