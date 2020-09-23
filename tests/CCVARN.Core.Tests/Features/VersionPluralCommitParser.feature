@Versioning
@Parser
Feature: Parsing version from multiple commits
	When having multiple commits, I wish to know
	what version I will end up with.

	Scenario: Passing in multiple commits with 1 feature commit
		Given the following commits
		| rawText                                     |
		| chore: just some chores being done          |
		| build(cake): update the build script        |
		| feat(core): the moste awesome feature ever  |
		| ci(appveyor): small update fix for appveyor |
		And with the following tag v.1.5.2 with message chore: created release 1.5.2
		When the user parses the commits
		Then the version output should be 1.6.0-alpha.1.4

	Scenario: Passing in multiple commits with a single bug fix commit
		Given the following commits
		| rawText                               |
		| fix: the most reliable bug fix        |
		| chore: update something               |
		| style: fix formatting\ntabs vs spaces |
		| Merge branch develop into master      |
	And with the following tag 1.0.0 with message chore: create first stable release
	When the user parses the commits
	Then the version output should be 1.0.1-alpha.1.4

	Scenario: Passing in multiple commits with no commits incrementing version
		Given the following commits
	| rawText              |
	| chore: some updates  |
	| style: style changes |
		And with the following tag v6.1.2 with message feat: awesome feature in previous tag
		When the user parses the commits
		Then the version output should be 6.1.2-alpha.1.2

	Scenario: Passing in multiple commits with previous tag as pre-release
		Given the following commits
	| rawText               |
	| chore: something      |
	| style: something else |
		And with the following tag v1.3.3-alpha with message docs: create changelog
		When the user parses the commits
		Then the version output should be 1.3.3-alpha.1.2

	Scenario: Passing in multiple commits configuration set to beta tag
		Given the configuration tag suffix beta
		And the following commits
	| rawText                        |
	| feat: some awesome feature     |
	| fix: ops I did it again        |
	| refactor: rename some function |
		And with the following tag 1.3.3 with message fix: important bug fix
		When the user parses the commits
		Then the version output should be 1.4.0-beta.1.3

	Scenario: Passing in multiple commtis configuration set to beta tag and previous is also beta
		Given the configuration tag suffix beta
		And the following commits
	| rawText                        |
	| feat: some awesome feature     |
	| fix: ops I did it again        |
	| refactor: rename some function |
		And with the following tag 1.3.3-beta.1 with message fix: important bug fix
		When the user parses the commits
		Then the version output should be 1.3.3-beta.2.3

	Scenario: Passing in multiple commits configuration set to alpha and previous tag is beta
		Given the configuration tag suffix alpha
		And the following commits
	| rawText                        |
	| feat: some awesome feature     |
	| fix: ops I did it again        |
	| refactor: rename some function |
		And with the following tag 1.3.3-beta.1 with message fix: important bug fix
		When the user parses the commits
		Then the version output should be 1.3.3-beta.2.3

	Scenario: Passing in multiple commits with latest being a tag
		Given the configuration tag suffix rc
		And the current tag is 1.4.0 with message feat: some important feature
		And the following commits
		| rawText                     |
		| fix: ops                    |
		| build: this should work now |
		And with the previous tag 1.3.5 with message chore: update changelog
		When the user parses the commits
		Then the version output should be 1.4.0

	Scenario: Passing in multiple commits without a previous tag
		Given the following commits
		| rawText                    |
		| feat: some awesome feature |
		| fix: this need to be fixed |
		| build: failed again :cry:  |
		When the user parses the commits
		Then the version output should be 1.0.0-alpha.1.3

	Scenario: Passing in commits with breaking changes
		Given the following commits
		| rawText                             |
		| feat!: some awesome breaking change |
		| fix: some bug fix                   |
		| chore: maintainance                 |
		And with the previous tag 3.5.2 with message fix: some previous bug fix
		When the user parses the commits
		Then the version output should be 4.0.0-alpha.1.3

	Scenario: Passing in commits with breaking changes in body
		Given the following commits
		| rawText                                                                                |
		| feat: some awesome breaking change\n\nBREAKING CHANGE: this is a major breaking change |
		| fix: some bugfix                                                                       |
		| ci(appveyor): appveyor broke again                                                     |
		And with the previous tag 1.2.0 with message docs: update changelog
		When the user parses the commits
		Then the version output should be 2.0.0-alpha.1.3
