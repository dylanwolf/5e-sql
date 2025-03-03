<Query Kind="Program" />

void Main()
{
	// Run this after importing monsters.csv
	var csvData = ParseCsvFile(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "../Source Files/flee-mortals.csv"), '\t');	
	SaveMonsters(csvData);
}

public void SaveMonsters(IEnumerable<IDictionary<string, string>> rows)
{
	foreach (var row in rows)
	{
		var monster = new Monsters();
		monster.MonsterName = FixupName(row["Creature Name"]);
		monster.CR = FixupCR(row["Challenge Rating"]);
		monster.Size = FixupSize(row["Size"]);
		
		var roleTag = new MonsterTags();
		roleTag.MonsterTag = row["Role"].Trim();
		monster.MonsterTags.Add(roleTag);
		
		var type = new MonsterTypes();
		type.MonsterType = row["Type"].Trim();
		monster.MonsterTypes.Add(type);
		
		var src = new MonsterSources();
		src.SourceName = "Flee, Mortals!";
		src.SourcePageReference = row["Page"].Trim();
		monster.MonsterSources.Add(src);
		
		var entryResult = MapEntry(row["Entry"]);
		if (!entryResult.HasValue)
		{
			// Do nothing
		}
		else if (entryResult.Value.Mapping == EntryParseMapping.Environment)
		{
			var env = new MonsterEnvironments();
			env.MonsterEnvironment = entryResult.Value.Value;
			monster.MonsterEnvironments.Add(env);
		}
		else if (entryResult.Value.Mapping == EntryParseMapping.Type)
		{
			var entType = new MonsterTypes();
			entType.MonsterType = entryResult.Value.Value;
			monster.MonsterTypes.Add(entType);
		}
		else if (entryResult.Value.Mapping == EntryParseMapping.Tag)
		{
			var entTag = new MonsterTags();
			entTag.MonsterTag = entryResult.Value.Value;
			monster.MonsterTags.Add(entTag);
		}

		this.Monsters.InsertOnSubmit(monster);
	}
	
	this.SubmitChanges();
}

public enum EntryParseMapping
{
	Tag,
	Type,
	Environment
}

public struct EntryParseResult
{
	public string Value;
	public EntryParseMapping Mapping;
}

public EntryParseResult? MapEntry(string value)
{
	if (new string[] { "Animals", "Dragons", "Elementals", "Giants", "Humans", "Undead" }.Contains(value))
	{
		// Covered by Type
		return null;
	}
	
	if (new string[] { "Cave", "Enchanted Forest", "Graveyards and Tombs", "Road", "Ruined Keep", "Sewers", "Swamp" }.Contains(value))
	{
		if (value == "Cave") value = "Underground";
		
		return new EntryParseResult() { Value = value, Mapping = EntryParseMapping.Environment };
	}
	
	return new EntryParseResult() { Value = value, Mapping = EntryParseMapping.Tag };
}

public string FixupSize(string value)
{
	if (string.IsNullOrWhiteSpace(value) || value == "N/A") return null;
	return value.Replace(" or ", " / ");
}

public decimal? FixupCR(string value)
{
	if (string.IsNullOrWhiteSpace(value) || value == "N/A") return null;
	
	if (value.Contains("/"))
	{
		var parts = value.Split(new char[] { '/' });
		return decimal.Parse(parts[0]) / decimal.Parse(parts[1]);
	}
	
	return decimal.Parse(value);
}

public string FixupName(string value)
{
	value = value.Trim();
	if (this.Monsters.Any(m => m.MonsterName == value)) value = $"{value} (MCDM)";
	return value;
}

IEnumerable<IDictionary<string, string>> ParseCsvFile(string filename, char separator = ',')
{
	var csv = File.ReadAllText(filename);

	var parseResult = ParseNextCsvRow(csv, separator);
	var columns = parseResult.RowData.ToArray();
	csv = parseResult.RemainingText;

	var results = new List<Dictionary<string, string>>();

	while (!parseResult.IsComplete)
	{
		parseResult = ParseNextCsvRow(parseResult.RemainingText, separator);

		if (parseResult.RowData.Any())
		{
			var row = new Dictionary<string, string>();
			for (var i = 0; i < columns.Length; i++)
			{
				var value = parseResult.RowData.Skip(i).FirstOrDefault();
				if (value != null)
				{
					row[columns[i]] = value;
				}
			}
			results.Add(row);
		}
	}

	return results;
}

struct ParseCsvRowResult
{
	public IEnumerable<string> RowData;
	public string RemainingText;
	public bool IsComplete
	{
		get { return string.IsNullOrWhiteSpace(RemainingText); }
	}
}

ParseCsvRowResult ParseNextCsvRow(string txt, char separator = ',')
{
	var results = new List<string>();

	var isComplete = false;
	var isEscaped = false;
	var isInQuote = false;
	var idx = 0;
	var buffer = new List<char>();

	while (!isComplete && idx < txt.Length)
	{
		var c = txt[idx];

		if (c == '\r')
		{
			// Ignore
		}
		else if (c == '\n')
		{
			if (isInQuote)
			{
				buffer.Add('\n');
			}
			else
			{
				if (isEscaped) buffer.Add('\\');
				results.Add(string.Join("", buffer));
				buffer.Clear();
				isComplete = true;
				isEscaped = false;
			}
		}
		else if (isEscaped)
		{
			if (isInQuote)
			{
				buffer.Add('\\');
				buffer.Add(c);
			}
			else
			{
				buffer.Add(c);
			}
			isEscaped = false;
		}
		else if (isInQuote)
		{
			if (c == '"')
			{
				isInQuote = false;
			}
			else
			{
				buffer.Add(c);
			}
		}
		else if (c == '"')
		{
			isInQuote = true;
		}
		else if (c == separator)
		{
			results.Add(string.Join("", buffer));
			buffer.Clear();
		}
		else
		{
			buffer.Add(c);
		}
		idx++;
	}

	if (buffer.Any())
	{
		results.Add(string.Join("", buffer));
	}

	return new ParseCsvRowResult()
	{
		RemainingText = txt.Substring(idx),
		RowData = results
	};
}