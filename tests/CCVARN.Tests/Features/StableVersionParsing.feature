Feature: Stable Version parsing
I would like to know what the version I should expect to
be outputted will be when I am expecting a stable release.
Stable releases typically only happen on tag builds and
uses a specific tag as its version.

	Scenario: Parsing tag with past feature commits without previous tag
		Given the tag with version 1.0.0 with the message docs: update changelog
		And have the previous raw commits
			| rawText                                  |
			| feat: some kind of awesome new feature   |
			| chore: just some non-source maintainance |
		When the user parses the commits
		Then the resulting version should be 1.0.0
		And with the commit count 3

	Scenario: Parsing tag without previous commits
		Given the tag with the version 1.5.2 with the message feat: some feature
		When the user parses the commits
		Then the resulting version should be 1.5.2
		And with the commit count 1

	Scenario: Parsing tag with with previous tag
		Given the tag with the version 1.6.0 with the message fix: something
		And the previous tag with version 1.5.0 with the message docs: something
		When the user parses the commits
		Then the resulting version should be 1.6.0
		And with the commit count 1

	Scenario: Parsing a tag with a lower version than previous tag
		Given the tag with the version 2.1.0 with the message fix: major breaking change
		And the previous tag with the version 3.0.0 with the message feat!: previous breaking
		When the user parses the commits
		Then the resulting version should be 2.1.0
		And with the commit count 1

	Scenario: Parsing a commit when config tag suffix is set to empty
		Given the following commits
			| rawText                    |
			| feat: some kind of feature |
		And configured tag suffix is empty
		And the previous tag with the version 1.5.2 with the message fix: something
		When the user parses the commits
		Then the resulting version should be 1.6.0
		And with the commit count 1

	Scenario: Parsing a commit with empty tag suffix and no previous tag
		Given the following commit feat: some kind of feature
		And configured tag suffix is empty
		When the user parses the commits
		Then the resulting version should be 1.0.0
		And with the commit count 1

	Scenario: Parsing non-bumping commits since last tag
		Given the following commits
			| rawText                   |
			| chore: something          |
			| build: build related only |
		And the previous tag with the version 1.7.0 with the message feat: awesome
		When the user parses the commits
		Then the resulting version should be 1.7.0
		And with the commit count 2

	Scenario: Parsing a tag with a v prefix
		Given the tag with the version v.5.2.2 with the message fix: something
		When the user parses the commits
		Then the resulting version should be 5.2.2
		And with the commit count 1

	Scenario: Parsing a merge commit with tag commit
		Given the following commit Merge master into develop
		And the following tag 1.3.5 with the message feat: awesome
		When the user parses the commits
		Then the resulting version should be 1.3.5
		And sha is set to tag commit
		And with the commit count 1
