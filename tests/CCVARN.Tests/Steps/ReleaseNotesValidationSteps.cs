namespace CCVARN.Tests.Steps
{
	using System.Linq;
	using System.Text;
	using CCVARN.Core.Models;
	using CCVARN.Tests.Models;
	using Shouldly;
	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Assist;

	[Binding]
	public class ReleaseNotesValidationSteps
	{
		private readonly ParsingModel data;

		public ReleaseNotesValidationSteps(ParsingModel data)
		{
			this.data = data;
		}

		[Then("the results should have a single note")]
		public void ThenResultsShouldHaveASingleNote()
		{
			this.data.Results.ReleaseNotes.Notes.ShouldHaveSingleItem();
		}

		[Then("with no breaking changes")]
		public void ThenWithNoBreakingChanges()
		{
			this.data.Results.ReleaseNotes.BreakingChanges.ShouldBeEmpty();
		}

		[Then("contain note with the name (.*)")]
		public void ThenContainANoteWithTheName(string name)
		{
			this.data.Results.ReleaseNotes.Notes.ShouldContainKey(name);
		}

		[Then("not contain note with the name (.*)")]
		public void ThenNotContainNoteWithTheName(string name)
		{
			this.data.Results.ReleaseNotes.Notes.ShouldNotContainKey(name);
		}

		[Then("the results should not have any notes")]
		public void ThenTheResultsShouldNotHaveAnyNotes()
		{
			this.data.Results.ReleaseNotes.Notes.ShouldBeEmpty();
		}

		[Then(@"the results should have (\d+) notes?")]
		[Then("should contain (\\d+) notes?")]
		public void ThenShouldContainNotes(int count, Table table)
		{
			var notes = table.CreateSet<NoteData>();

			var allNotes = this.data.Results.ReleaseNotes.Notes.SelectMany(n => n.Value).ToList();

			allNotes.Count().ShouldBe(count);

			foreach (var note in notes)
			{
				allNotes.ShouldContain(note);
			}
		}

		[Then("the breaking changes?")]
		public void TheTheBreakingChanges(Table table)
		{
			var expected = new StringBuilder();

			expected.AppendLine(table.Header.First());

			foreach (var row in table.Rows)
			{
				expected.AppendLine(row[0]);
			}

			var actual = new StringBuilder();

			foreach (var line in this.data.Results.ReleaseNotes.BreakingChanges)
			{
				actual.AppendLine(line);
			}

			actual.ToString().ShouldBe(expected.ToString());
		}

		[Then(@"the release notes should reference issue (\d+)")]
		public void ThenTheReleaseNotesShouldReferenceIssue(int num)
		{
			var issues = this.data.Results.ReleaseNotes.Notes.SelectMany(n => n.Value.SelectMany(v => v.Issues));

			issues.ShouldContain(num);
		}

		[Then(@"the release notes should not reference issue (\d+)")]
		public void ThenTheReleaseNotesShouldNotReferenceIssue(int num)
		{
			var issues = this.data.Results.ReleaseNotes.Notes.SelectMany(n => n.Value.SelectMany(v => v.Issues));

			issues.ShouldNotContain(num);
		}
	}
}
