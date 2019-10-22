--2-Villain Names
SELECT v.[Name], COUNT(m.Id) AS Minions
FROM Villains v
INNER JOIN MinionsVillains mv ON v.Id = mv.VillainId
INNER JOIN Minions m ON mv.MinionId = m.Id
GROUP BY v.[Name]
HAVING COUNT(m.Id) >= 3
ORDER BY Minions DESC
--3-Minion Names
SELECT m.[Name], m.Age
FROM Minions m
INNER JOIN MinionsVillains mv ON m.Id = mv.MinionId
WHERE mv.VillainId = 12
ORDER BY m.[Name]

SELECT v.[Name]
FROM Villains v
WHERE v.Id = 3
--4-Add Minion
SELECT COUNT(*)
FROM Villains
WHERE Name = 'Cathleen'

SELECT COUNT(*)
FROM Towns
WHERE Name = 'Vidin'

INSERT INTO Towns(Name) VALUES ('Vidin')

INSERT INTO Villains(Name,EvilnessFactorId) VALUES
('Cathleen','evil')

SELECT Id
FROM Towns
WHERE Name = 'Vidin'

SELECT Id
FROM Villains
WHERE Name = 'Cathleen'

INSERT INTO Minions(Name,Age,TownId) VALUES
('Bob', 14, 12)

SELECT Id
FROM Minions
WHERE Name = 'Bob'

INSERT INTO MinionsVillains VALUES
(1,2)

--5-Change Town Names Casing
SELECT COUNT(*)
FROM Countries
WHERE Name = ''

SELECT COUNT(*)
FROM Towns t
INNER JOIN Countries c ON t.CountryCode = c.Id
WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = 'Germany')

UPDATE Towns
SET Name = UPPER(Name)
WHERE Id IN (SELECT t.Id
FROM Towns t
INNER JOIN Countries c ON t.CountryCode = c.Id
WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = 'Germany'))

SELECT t.Id
FROM Towns t
INNER JOIN Countries c ON t.CountryCode = c.Id
WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = 'Germany')

--6-Remove Villain
SELECT COUNT(*)
FROM MinionsVillains
WHERE VillainId = 1

DELETE FROM MinionsVillains
WHERE VillainId = 1

DELETE FROM Villains
WHERE Id = 222

SELECT Name FROM Villains WHERE Id = 1