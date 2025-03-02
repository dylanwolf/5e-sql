DECLARE @Environment VARCHAR(255)
DECLARE @MarkdownOutput TABLE (Markdown VARCHAR(MAX), ID INT PRIMARY KEY IDENTITY(0, 1))
DECLARE @MonsterList TABLE (EnvironmentName VARCHAR(255), MonsterName VARCHAR(255), CRValue DECIMAL(8,2), CR VARCHAR(255), HuntingNotes VARCHAR(MAX), ReferenceUrl VARCHAR(MAX))

;WITH MonstersByEnvironment AS (
	SELECT DISTINCT
		b.BaseName AS MonsterBaseName
		, CASE e.MonsterEnvironment 
				WHEN 'Plane of Air' THEN 'Planar'
				WHEN 'Plane of Earth' THEN 'Planar'
				WHEN 'Plane of Fire' THEN 'Planar'
				WHEN 'Plane of Water' THEN 'Planar'
				WHEN 'Underground' THEN 'Cave'
				WHEN 'Water' THEN 'Aquatic'
				WHEN 'Hills' THEN 'Grassland'
				WHEN 'Jungle' THEN 'Forest'
				WHEN 'Tundra' THEN 'Arctic'
				ELSE e.MonsterEnvironment
			END AS EnvironmentName
		, q.HuntingNotes
		FROM Monsters m
		INNER JOIN MonsterHuntingQuarry q ON m.MonsterName=q.MonsterName
		INNER JOIN MonsterBaseName b ON b.MonsterName=m.MonsterName
		INNER JOIN MonsterEnvironments e ON e.MonsterName=m.MonsterName
			AND e.MonsterEnvironment NOT IN ('Feywild','Planar','Sewer','Shadowfell','Urban','Settlement','Laboratory','Road')
		INNER JOIN MonsterSources s ON m.MonsterName=s.MonsterName AND
			s.SourceName IN ('5e SRD','Volo''s Guide to Monsters','Monster-a-Day','Tome of Beasts 3','Monstrous Menagerie','Flee, Mortals!','Basic Rules v1','Monster Manual','Player''s Handbook')
)
INSERT INTO @MonsterList
SELECT
	x.EnvironmentName
	, m.MonsterName
	, m.CR
	, ISNULL(CASE m.CR
			WHEN 0.125 THEN '1/8'
			WHEN 0.25 THEN '1/4'
			WHEN 0.5 THEN '1/2'
			ELSE CAST(CAST(m.CR AS INT) AS VARCHAR)
		END, '')
	, ISNULL(x.HuntingNotes, '')
	, m.ReferenceUrl
FROM MonstersByEnvironment x
INNER JOIN MonsterBaseName b ON x.MonsterBaseName=b.BaseName
INNER JOIN Monsters m ON b.MonsterName=m.MonsterName

DECLARE EnvCursor CURSOR FOR SELECT DISTINCT EnvironmentName FROM @MonsterList ORDER BY 1
OPEN EnvCursor
FETCH NEXT FROM EnvCursor INTO @Environment
WHILE @@FETCH_STATUS = 0
BEGIN

	INSERT INTO @MarkdownOutput (Markdown)
	SELECT '# ' + @Environment AS Markdown
	UNION ALL
	SELECT ''
	UNION ALL
	SELECT '| Name | Type | CR | Tag | Source | Notes |'
	UNION ALL 
	SELECT '|------|------|----|-----|--------|-------|'

	INSERT INTO @MarkdownOutput (Markdown)
	SELECT '| ' + m.MonsterName + ' | ' +
		ISNULL((SELECT STRING_AGG(t.MonsterType, ' / ') FROM MonsterTypes t WHERE t.MonsterName = m.MonsterName), '') + ' | ' +
		m.CR + ' | ' +
		ISNULL((SELECT STRING_AGG(g.MonsterTag, ' / ') FROM MonsterTags g WHERE g.MonsterName = m.MonsterName), '') + ' | ' +
		ISNULL((SELECT STRING_AGG(
			IIF(s.SourcePageReference IS NULL, s.SourceName, s.SourceName + ': ' + s.SourcePageReference)
				, ' / ') FROM MonsterSources s WHERE s.MonsterName = m.MonsterName),'')  +
			IIF(m.ReferenceUrl IS NOT NULL, ' / ' + m.ReferenceUrl, '') +
			' | ' +
		ISNULL(m.HuntingNotes,'') + ' |'
	FROM @MonsterList m
	WHERE m.EnvironmentName=@Environment
	ORDER BY m.CR, m.MonsterName

	INSERT INTO @MarkdownOutput (Markdown)
	SELECT ''

	FETCH NEXT FROM EnvCursor INTO @Environment
END
CLOSE EnvCursor
DEALLOCATE EnvCursor

SELECT Markdown FROM @MarkdownOutput ORDER BY ID