namespace CCVARN.Tests.Steps
{
	using System.Collections.Generic;
	using System.Linq;
	using CCVARN.Core.Models;
	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Assist;
	using tests.CCVARN.Tests.Models;

	[Binding]
	public class ExportAddSteps
	{
		private readonly ParsedData data;
		private readonly ScenarioContext context;

		public ExportAddSteps(ParsedData data, ScenarioContext context)
		{
			this.context = context;
			this.data = data;
		}

		[Given("the parsed version (.*)")]
		public void GivenTheParsedVersion(string version)
		{
			this.data.Version = VersionData.Parse(version);
		}

		[Given("the breaking changes")]
		public void GivenTheBreakingChanges(Table table)
		{
			this.data.ReleaseNotes.BreakingChanges.Add(table.Header.First());
			foreach (var line in table.Rows)
			{
				this.data.ReleaseNotes.BreakingChanges.Add(line[0]);
			}
		}

		[Given("the release notes")]
		public void GivenTheReleaseNotes(Table table)
		{
			var notes = table.CreateSet<NoteTestData>();

			foreach (var note in notes)
			{
				var actualNote = new NoteData(note.Type, note.Summary, note.Scope);
				if (!this.data.ReleaseNotes.Notes.ContainsKey(note.DisplayName))
					this.data.ReleaseNotes.Notes.Add(note.DisplayName, new List<NoteData> { actualNote });
				else
					this.data.ReleaseNotes.Notes[note.DisplayName].Add(actualNote);
			}
		}

		[Given("the path (.*)")]
		public void GivenThePath(string path)
		{
			this.context["TEST_PATH"] = path;
		}
	}
}
