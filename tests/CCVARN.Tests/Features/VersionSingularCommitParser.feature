@Versioning
@Parsing
Feature: Parsing version from singular commit

	Scenario: Passing in tag commit sets next version to same
		Given a tag commit with the data
		| IsTag | Ref             |
		| true  | refs/tags/1.5.2 |
		And no current version
		When the user is parsing the next commit
		And commits the returned version
		Then the version output should be 1.5.2

	Scenario: Passing in a non-tag commit sets next version to empty when not committing
		Given a commit with the data
		| IsTag | Ref | RawText      |
		| false |     | Merge Commit |
		And no current version
		When the user is parsing the next commit
		Then the version output should be 0.0.0

	Scenario: Passing in a non-tag commit sets next version to first stable when committing
		Given a commit with the data
		| IsTag | Ref | RawText      |
		| false |     | Merge Commit |
		And no current version
		When the user is parsing the next commit
		And commits the returned version
		Then the version output should be 1.0.0

	Scenario: Passing in non-tag commit with previous commit exist sets next version to same
		Given a commit with the data
		| IsTag | Ref | RawText                    |
		| false |     | feat: some kind of update |
		And current version is 3.5.1
		When the user is parsing the next commit
		And commits the returned version
		Then the version output should be 3.5.1

	Scenario: Passing in a tag commit with previous commit sets to <bump> bump
		Given a commit with the data
		| IsTag | Ref              | RawText                |
		| true  | refs/tags/v3.4.1 | docs: create changelog |
		And current version is empty
		And current version set to <bump> bump
		When the user is parsing the next commit
		And commits the returned version
		Then the version output should be <expected>
		Examples:
		| bump  | expected |
		| patch | 3.4.2    |
		| minor | 3.5.0    |
		| major | 4.0.0    |
		| none  | 3.4.1    |
