-- Defines some flags for creating a "hunting quarries" reference / randomization table
-- Some of these are Monster Manual 2024 and won't match without an updated list
-- Kruthik missing from Xio's Guide to Monsters?

DROP TABLE IF EXISTS MonsterHuntingQuarry

CREATE TABLE dbo.MonsterHuntingQuarry (
	MonsterName VARCHAR(255) PRIMARY KEY,
	HuntingNotes VARCHAR(MAX) NULL

	CONSTRAINT FK_Monster_MonsterHuntingQuarry
		FOREIGN KEY (MonsterName)
		REFERENCES Monsters (MonsterName)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

DECLARE @MonsterHuntingNotes TABLE (NamePrefix VARCHAR(255), Notes VARCHAR(MAX))

INSERT INTO @MonsterHuntingNotes (NamePrefix, Notes) VALUES
('Brown Bear', NULL),
('Deinonychus', NULL),
('Dire Wolf', NULL),
('Female Steeder', NULL),
('Wildcat', NULL),
('Giant Eagle', NULL),
('Giant Hyena', NULL),
('Giant Octopus', NULL),
('Giant Spider', NULL),
('Giant Strider', NULL),
('Giant Toad', NULL),
('Hippogriff', NULL),
('Lion', NULL),
('Myconid Spore Servant', NULL),
('Salamander Fire Snake', NULL),
('Adult Kruthik', NULL),
('Allosaurus', NULL),
('Ankheg', NULL),
('Aurochs', NULL),
('Awakened Tree', NULL),
('Bear', NULL),
('Bulette Pup', NULL),
('Cave Bear', NULL),
('Giant Boar', NULL),
('Giant Constrictor Snake', NULL),
('Giant Elk', NULL),
('Griffon', NULL),
('Myconid Sovereign', 'retheme as mushroom beast'),
('Plesiosaurus', NULL),
('Polar Bear', NULL),
('Quetzalcoatlus', NULL),
('Rhinoceros', NULL),
('Saber-Toothed Tiger', NULL),
('Shadow Mastiff', NULL),
('Ankylosaurus', NULL),
('Basilisk', NULL),
('Cave Fisher', NULL),
('Displacer Beast', NULL),
('Flail Snail', NULL),
('Giant Scorpion', NULL),
('Killer Whale', 'retheme as large fish'),
('Manticore', NULL),
('Owlbear', NULL),
('Shadow Mastiff Alpha', NULL),
('Trapper', 'manta-like creature that hides in subterranean environments'),
('Water Weird', NULL),
('Winter Wolf', NULL),
('Yeti', NULL),
('Archelon', 'giant turtle'),
('Chuul', NULL),
('Hippopotamus', NULL),
('Stegosaurus', NULL),
('Brontosaurus', NULL),
('Bulette', NULL),
('Catoblepas', NULL),
('Giant Axe Beak', NULL),
('Giant Crocodile', NULL),
('Giant Shark', NULL),
('Kruthik Hive Lord', 'rules a kruthik hive'),
('Roper', NULL),
('Salamander', NULL),
('Shambling Mound', 'retheme as awakened shrub'),
('Triceratops', NULL),
('Umber Hulk', NULL),
('Young Remorhaz', NULL),
('Chimera', NULL),
('Giant Squid', NULL),
('Mammoth', NULL),
('Primeval Owlbear', NULL),
('Tree Blight', NULL),
('Violet Fungus Necrohulk', NULL),
('Cloaker', NULL),
('Cockatrice Regent', NULL),
('Hydra', NULL),
('Tyrannosaurus Rex', NULL),
('Abominable Yeti', NULL),
('Treant', NULL),
('Froghemoth', NULL),
('Balhannoth', NULL),
('Behir', NULL),
('Remorhaz', NULL),
('Roc', NULL),
('Gray Render', NULL),
('Neothelid', NULL),
('Purple Worm', NULL),
('Salamander Inferno', NULL),
('Kraken', NULL),
('Tarrasque', NULL),
('Myconid Adult', 'retheme as mushroom beast'),
('Myconid Sprout', 'retheme as mushroom beast'),
('Cockatrice', NULL)

INSERT INTO dbo.MonsterHuntingQuarry
SELECT m.MonsterName, n.Notes
FROM @MonsterHuntingNotes n
INNER JOIN Monsters m ON m.MonsterName = n.NamePrefix OR m.MonsterName LIKE n.NamePrefix + ' (%'