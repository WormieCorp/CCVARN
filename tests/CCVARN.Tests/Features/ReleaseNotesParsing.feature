Feature: Release Notes parsing
	I want to know which release notes details I
	can expect when Parsing commits for stable
	releases.

	Scenario: Parsing a singular feature tag commit
		Given the tag with the version 1.1.0 with the message feat: some kind of new feature
		When the user parses the commits
		Then should contain 1 note
			| type | summary                  |
			| feat | some kind of new feature |
		And with no breaking changes

	Scenario: Parsing a singular fix tag commit
		Given the tag with the version 1.0.1 with the message fix: some kind of bugfix
		When the user parses the commits
		Then should contain 1 note
			| type | summary             |
			| fix  | some kind of bugfix |
		And with no breaking changes

	Scenario: Parsing a non-changelog commit
		Given the tag with the version 1.0.0 with the message chore: some update
		When the user parses the commits
		Then the results should not have any notes
		And with no breaking changes

	Scenario: Parsing multiple types
		Given the tag with the version 2.0.0 with the message chore: some update
		And the following commits
			| rawText                                       |
			| feat: some kind of new feature                |
			| fix(scoping): some kind of bug fix            |
			| refactor!: renamed a method in the public API |
		When the user parses the commits
		Then the results should have 3 notes
			| type     | summary                            | scope   |
			| feat     | some kind of new feature           |         |
			| fix      | some kind of bug fix               | scoping |
			| refactor | renamed a method in the public API |         |
		And contain note with the name Feature
		And contain note with the name Bug fix
		And contain note with the name BREAKING CHANGE

	Scenario: Parsing breaking change with body
		Given the tag with the version 2.0.0 with the message chore: some update
		And the following commits
			| rawText                                                                                     |
			| refactor!:renamed a method in the public API\n\nAweSomeName was renamed to AwesomeNameAsync |
		When the user parses the commits
		Then the results should have 1 note
			| type     | summary                            |
			| refactor | renamed a method in the public API |
		And contain note with the name Code Refactoring
		And the breaking changes
			| AweSomeName was renamed to AwesomeNameAsync |

	Scenario: Parsing breaking change in body
		Given the tag with the version 2.0.0 with the message feat: some update
		And the following commits
			| rawText                                                                                                                    |
			| feat(scopy): some new breaking feature\n\nThis is a normal body\n\nBREAKING-CHANGE: This is the breaking message\nSo be it |
		When the user parses the commits
		Then the results should have 2 notes
			| type | summary                   | scope |
			| feat | some new breaking feature | scopy |
			| feat | some update               |       |
		And contain note with the name Features
		And the breaking changes
			| This is the breaking message |
			| So be it                     |

	Scenario: Parsing commits with issue references
		Given the tag with the version 1.0.0 with the message docs: update changelog
		And the following commits
			| rawText                                                  |
			| feat: some feature\n\n<close-type> #54\n<close-type> #43 |
			| chore: some chore\n\n<close-type> #22                    |
		When the user parses the commits
		Then the release notes should reference issue 54
		And the release notes should reference issue 43
		And the release notes should not reference issue 22
		Examples:
			| close-type |
			| close      |
			| closes     |
			| closed     |
			| fix        |
			| fixes      |
			| fixed      |
			| resolve    |
			| resolves   |
			| resolved   |
	Scenario: Parsing commits with merge
		Given the tag with the version 1.0.0 with the message docs: update changelog
		And the following commits
			| rawText                   |
			| Merge develop into master |
			| feat: some feature        |
			| (build) not considered    |
		When the user parses the commits
		Then the result should have 1 note
			| type | summary      |
			| feat | some feature |
		And contain note with the name Feature
