DROP TABLE IF EXISTS dbo.MonsterHuntingQuarry
DROP TABLE IF EXISTS dbo.MonsterBaseName
DROP TABLE IF EXISTS dbo.MonsterSources
DROP TABLE IF EXISTS dbo.MonsterTags
DROP TABLE IF EXISTS dbo.MonsterEnvironments
DROP TABLE IF EXISTS dbo.MonsterTypes
DROP TABLE IF EXISTS dbo.Monsters
GO

CREATE TABLE dbo.Monsters (
	MonsterName VARCHAR(255) PRIMARY KEY,
	Size VARCHAR(255) NULL,
	IsLegendary BIT NULL,
	HasLair BIT NULL,
	IsUnique BIT NULL,
	CR DECIMAL(8, 3) NULL,
	ReferenceUrl VARCHAR(MAX) NULL
)
GO

CREATE TABLE dbo.MonsterTypes (
	MonsterName VARCHAR(255) NOT NULL,
	MonsterType VARCHAR(255) NOT NULL
	PRIMARY KEY (MonsterName, MonsterType)

	CONSTRAINT FK_Monster_MonsterType
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

CREATE TABLE dbo.MonsterEnvironments (
	MonsterName VARCHAR(255) NOT NULL,
	MonsterEnvironment VARCHAR(255) NOT NULL
	PRIMARY KEY (MonsterName, MonsterEnvironment)

	CONSTRAINT FK_Monster_MonsterEnvironment
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

CREATE TABLE dbo.MonsterTags (
	MonsterName VARCHAR(255) NOT NULL,
	MonsterTag VARCHAR(255) NOT NULL
	PRIMARY KEY (MonsterName, MonsterTag)

	CONSTRAINT FK_Monster_MonsterTag
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

CREATE TABLE dbo.MonsterSources (
	MonsterName VARCHAR(255),
	SourceName VARCHAR(255) NOT NULL,
	SourcePageReference VARCHAR(255) NULL
	PRIMARY KEY (MonsterName, SourceName)

	CONSTRAINT FK_Monster_MonsterSource
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO