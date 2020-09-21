Feature: Version Parsing
	When running in a git repository
	I want to know the version that
	will be asserted.

	Scenario: Parsing a git repository without any commits
		Given a repository without any commits
		And without any tags
		When the user execute version parsing
		Then the result should be 1.0.0

	Scenario: Parsing a git repository with feature commits without any tags
		Given a repository with 2 commits
			| Message                         |
			| chore: some kind of chore       |
			| feat: some new awesome feature |
		And without any tags
		When the user execute version parsing
		Then the result should be 1.0.0

	Scenario: Parsing a git repository with bug/fix commits and with previous tag
		Given a repository with the tag v1.0.0
		And a repository with 1 commit since last tag
			| Message                    |
			| fix: oh yes, got that down |
		When the user execute version parsing
		Then the result should be 1.0.1
	
	Scenario: Parsing a git repository with feature commits and with previous tag
		Given a repository with the tag v1.5.3
		And a repository with 5 commits since last tag
			| Message                       |
			| build: some build commit      |
			| fix: some kind of fix         |
			| feat: new awesome feature     |
			| feat: another awesome feature |
			| style: formatting             |
		When the user execute version parsing
		Then the result should be 1.6.0

	Scenario: Parsing a git repository with breaking change in head and with previous tag
		Given a repository with the tag v2.8.2
		And a repository with 2 commits since last tag
			| Message                           |
			| feat!: this is a breaking feature |
			| fix: some fix                     |
		When the user execute version parsing
		Then the result should be 3.0.0

	Scenario: Parsing a git repository without version bump commits and with previous tag
		Given a repository with the tag 1.2.0
		And a repository with 1 commit since last tag
			| Message                         |
			| chore: nothing important to see |
		When the user execute version parsing
		Then the result should be 1.2.0