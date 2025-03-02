SELECT
	m.MonsterName, m.Size, m.HasLair, m.IsLegendary, m.IsUnique, m.HasLair, m.ReferenceUrl
	, (SELECT STRING_AGG(t.MonsterType, ' / ') FROM MonsterTypes t WHERE t.MonsterName = m.MonsterName) AS MonsterTypes
	, (SELECT STRING_AGG(e.MonsterEnvironment, ' / ') FROM MonsterEnvironments e WHERE e.MonsterName = m.MonsterName) AS MonsterEnvironments
	, (SELECT STRING_AGG(g.MonsterTag, ' / ') FROM MonsterTags g WHERE g.MonsterName = m.MonsterName) AS MonsterTags
	, (SELECT STRING_AGG(
			IIF(s.SourcePageReference IS NULL, s.SourceName, s.SourceName + ': ' + s.SourcePageReference)
		, ' / ') FROM MonsterSources s WHERE s.MonsterName = m.MonsterName) AS MonsterSources
FROM Monsters m
