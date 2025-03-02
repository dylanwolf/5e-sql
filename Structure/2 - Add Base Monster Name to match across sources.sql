DROP TABLE IF EXISTS MonsterBaseName
GO

CREATE TABLE dbo.MonsterBaseName (
	MonsterName VARCHAR(255) NOT NULL,
	BaseName VARCHAR(255) NOT NULL
	PRIMARY KEY (MonsterName)

	CONSTRAINT FK_Monster_MonsterBaseName
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO


INSERT INTO MonsterBaseName
SELECT m.MonsterName,
	IIF(
		m.MonsterName LIKE '% (%)',
		SUBSTRING(m.MonsterName, 0, CHARINDEX(' (', m.MonsterName)),
		m.MonsterName
	)
FROM Monsters m