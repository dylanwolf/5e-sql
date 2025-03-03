<Query Kind="Program" />

void Main()
{
	var csvData = ParseCsvFile(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "../Source Files/monsters.csv"));
	var rows = ConvertToSqlStructure(csvData);
	
	SaveMonsters(rows);
}

public void SaveMonsters(IEnumerable<MonsterRow> rows)
{
	foreach (var row in rows)
	{
		var monster = new Monsters();
		monster.MonsterName = row.Name;
		monster.Size = row.Size;
		monster.IsLegendary = row.Legendary;
		monster.IsUnique = row.Unique;
		monster.HasLair = row.Lair;
		monster.CR = row.CR;
		monster.ReferenceUrl = row.Url;
		
		this.Monsters.InsertOnSubmit(monster);
		
		if (row.Types != null)
		{
			foreach (var t in row.Types)
			{
				var monsterType = new MonsterTypes();
				monsterType.MonsterType = t;
				monster.MonsterTypes.Add(monsterType);
			}
		}

		if (row.Environments != null)
		{
			foreach (var t in row.Environments)
			{
				var monsterEnv = new MonsterEnvironments();
				monsterEnv.MonsterEnvironment = t;
				monster.MonsterEnvironments.Add(monsterEnv);
			}
		}

		if (row.Tags != null)
		{
			foreach (var t in row.Tags)
			{
				var monsterTag = new MonsterTags();
				monsterTag.MonsterTag = t;
				monster.MonsterTags.Add(monsterTag);
			}
		}

		if (row.Sources != null)
		{
			foreach (var t in row.Sources)
			{
				var monsterSrc = new MonsterSources();
				monsterSrc.SourceName = t.Name;
				monsterSrc.SourcePageReference = t.PageReference;
				monster.MonsterSources.Add(monsterSrc);
			}
		}
	}

	this.SubmitChanges();
}

public string GetCsvString(IDictionary<string, string> row, string key)
{
	if (!row.ContainsKey(key)) return null;
	if (string.IsNullOrWhiteSpace(row[key])) return null;
	return row[key];
}

public bool GetCsvFlag(IDictionary<string, string> row, string key)
{
	if (!row.ContainsKey(key)) return false;
	if (string.IsNullOrWhiteSpace(row[key])) return false;
	return true;
}

public decimal? ConvertCsvDecimal(string value)
{
	if (string.IsNullOrWhiteSpace(value)) return null;
	decimal result;
	if (decimal.TryParse(value, out result)) return result;
	return null;
}

public IEnumerable<MonsterRow> ConvertToSqlStructure(IEnumerable<IDictionary<string, string>> csvData)
{
	// ignored HP, AC, Initiative, Alignment
	// boolean = legendary, lair, unique
	
	var results = new List<MonsterRow>();
	
	foreach (var row in csvData)
	{
		var rowData = FixupRow(row);
		
		results.Add(new MonsterRow() {
			Name = GetCsvString(rowData, "Name"),
			Size = FixupSize(GetCsvString(rowData, "Size")),
			Types = FixupType(GetCsvString(rowData, "Type")),
			Environments = FixupEnvironments(GetCsvString(rowData, "Environment")),
			Legendary = GetCsvFlag(rowData, "Legendary"),
			Lair = GetCsvFlag(rowData, "Lair"),
			Unique = GetCsvFlag(rowData, "Unique"),
			CR = ConvertCsvDecimal(FixupCR(GetCsvString(rowData, "CR"))),
			Tags = FixupTags(GetCsvString(rowData, "Tags")),
			Sources = FixupSource(GetCsvString(rowData, "Source")),
			Url = GetCsvString(rowData, "Url")
		});
	}
	
	return results;
}

public IDictionary<string, string> CloneDictionaryWithEdits(IDictionary<string, string> original, IDictionary<string, string> edits)
{
	var result = new Dictionary<string, string>();
	
	foreach (var key in original.Keys)
	{
		result[key] = original[key];
	}
	
	foreach (var key in edits.Keys)
	{
		if (edits[key] == null)
		{
			result.Remove(key);
		}
		else
		{
			result[key] = edits[key];
		}
	}
	
	return result;
}

public IDictionary<string, string> FixupRow(IDictionary<string, string> data)
{
	if (data["Name"] == "Necromancer" && data["Source"].Contains("Creature Codex"))
	{
		return CloneDictionaryWithEdits(
			data,
			new Dictionary<string, string> {
				{ "Legenday", null },
				{ "Lair", null },
				{ "Alignment", "any neutral" }
			}
		);
	}
	
	return data;
}

public MonsterRowSource[] FixupSource(string value)
{
	if (string.IsNullOrWhiteSpace(value)) return new MonsterRowSource[0];
	
	if (value.Contains("300Gamemaster")) value = value.Replace("300Gamemaster", "300, Gamemaster");
	
	var sources = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	var results = new List<MonsterRowSource>();	
	
	foreach (var src in sources)
	{
		var keyValuePair = src.Split(new string[] { ":"}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		results.Add(new MonsterRowSource() {
			Name = keyValuePair[0],
			PageReference = keyValuePair.Length > 1 ? keyValuePair[1] : null
		});		
	}
	
	return results.ToArray();
}

public string FixupCR(string value)
{
	if (value == null) return value;
	
	var match = Regex.Match(value, @"^(.+) \(.*");
	if (match.Success) value = match.Groups[1].Value;
	
	match = Regex.Match(value, @"^[^0-9]+\s+([0-9]+)");
	if (match.Success) value = match.Groups[1].Value;
	
	switch (value)
	{
		case "8/14": return "8";
		default: return value;
	}
}

public string[] FixupTags(string value)
{
	if (string.IsNullOrWhiteSpace(value)) return new string[] { };

	return value
		.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Select(x => x[0].ToString().ToUpper() + x.Substring(1))
		.Select(x => FixupEnvironmentValue(x))
		.Where(x => !string.IsNullOrWhiteSpace(x))
		.ToArray();
}

public string[] FixupEnvironments(string value)
{
	if (string.IsNullOrWhiteSpace(value)) return new string[] { };
	
	return value
		.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Select(x => x[0].ToString().ToUpper() + x.Substring(1))
		.Select(x => FixupEnvironmentValue(x))
		.Where(x => !string.IsNullOrWhiteSpace(x))
		.Distinct()
		.ToArray();
}

public string FixupEnvironmentValue(string value)
{
	switch (value.ToLower())
	{
		case "caverns":
		case "caves":
		case "cave":
			return "Underground";
			
		case "deserts":
			return "Desert";
			
		case "coast":
			return "Coastal";
			
		case "forests":
			return "forest";
		
		case "mountains":
			return "Mountain";
			
		case "ruins":
			return "Ruin";
	
		default:
			return value;
	}
}

public string[] FixupType(string value)
{
	if (string.IsNullOrWhiteSpace(value)) return new string[] { };

	var valueStr = value.Trim();
	
	var match = Regex.Match(valueStr, @"^(.+) \(.*");
	if (match.Success) valueStr = match.Groups[1].Value;

	switch (valueStr.ToLower())
	{
		case "(undead)":
			return new string[] { "Undead" };
			
		case "beasts":
			return new string[] {"Beast"};

		case "constructs":
			return new string[] { "Constructs" };

		case "fiends":
			return new string[] { "Fiend" };

		case "mosntrosity":
			return new string[] { "Monstrosity" };

		default:
			return valueStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);
	}
}

public string FixupSize(string value)
{
	if (value == null) return null;
	
	switch (value.Trim().ToLower())
	{
		case "":
			return null;
			
		case "m":
		case "medium":
		case "meidum":
		case "mwsium":
			return "Medium";
			
		case "g":
		case "gargatuan":
			return "Gargantuan";
			
		case "s":
		case "sall":
			return "Small";
			
		case "l":
			return "Large";
			
		case "h":
		case "huge":
			return "Huge";
			
		case "t":
			return "Tiny";
			
		case "huge swarm":
			return "Huge Swarm";

		case "large swarm":
			return "Large Swarm";

		case "medium swarm":
			return "Medium Swarm";

		default:
			return value.Trim();
	}
}

public class MonsterRow
{
	public string Name;
	public string Size;
	public IEnumerable<string> Types;
	public IEnumerable<string> Environments;
	public bool Legendary;
	public bool Lair;
	public bool Unique;
	public decimal? CR;
	public IEnumerable<string> Tags;
	public IEnumerable<MonsterRowSource> Sources;
	public string Url;
}

public class MonsterRowSource
{
	public string Name;
	public string PageReference;
}

IEnumerable<IDictionary<string, string>> ParseCsvFile(string filename)
{
	var csv = File.ReadAllText(filename);
	
	var parseResult = ParseNextCsvRow(csv);
	var columns = parseResult.RowData.ToArray();
	csv = parseResult.RemainingText;
	
	var results = new List<Dictionary<string, string>>();
	
	while (!parseResult.IsComplete)
	{
		parseResult = ParseNextCsvRow(parseResult.RemainingText);
		
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
	
	return new ParseCsvRowResult() {
		RemainingText = txt.Substring(idx),
		RowData = results
	};
}