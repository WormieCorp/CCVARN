Feature: Unstable Version Parsing
I would like to know what the version I should
expect to be outputted will be when I am
expecting some unstable release.

	Scenario: Parsing without any commits
		Given no commits
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1
		And with the commit count 0

	Scenario: Parsing with a single feature commit
		Given the following commit feat: some kind of awesome feature
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing with a single feature commit with previous tag
		Given the following commit feat: some kind of awesome feature
		And the previous tag 1.2.3 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 1.3.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing with multiple feature commits
		Given the following commits
			| rawText                              |
			| feat: some kind of awesome feature 2 |
			| feat: some kind of awesome feature 1 |
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1.2
		And with the commit count 2

	Scenario: Parsing with multiple feature commits with previous tag
		Given the following commits
			| rawText                             |
			| feat: awesomesauce                  |
			| feat: best new feature ever created |
		And the previous tag 2.4.4 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 2.5.0-alpha.1.2
		And with the commit count 2

	Scenario: Parsing with single fix commit
		Given the following commit fix: correct unintentional bug
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing with single fix commit with previous tag
		Given the following commit fix: correct code
		And the previous tag 3.1.1 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 3.1.2-alpha.1.1
		And with the commit count 1

	Scenario: When tag-suffix is configured with <tag-suffix>
		Given the following commit fix: correct code
		And configured tag suffix is <tag-suffix>
		When the user parses the commits
		Then the resulting version should be 1.0.0-<tag-suffix>.1.1
		And with the commit count 1
		Examples:
			| tag-suffix  |
			| alpha       |
			| beta        |
			| ceta        |
			| rc          |
			| unstable    |
			| pre-release |

	Scenario: When next-version is configured with <next-version>
		Given the following commit feat: awesome feature
		And the next version is set to <next-version>
		When the user parses the commits
		Then the resulting version should be <next-version>-alpha.1.1
		And with the commit count 1
		Examples:
			| next-version |
			| 0.1.0        |
			| 0.5.0        |
			| 1.0.0        |
			| 5.0.0        |

	Scenario: Parsing higher version than configured next-version
		Given the following commit feat: some test commit
		And the previous tag 2.4.5 with the message docs: update changelog
		And the next version is set to 2.0.0
		When the user parses the commits
		Then the resulting version should be 2.5.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing with single fix commit with previous <tag-suffix> tag
		Given the following commit fix: something
		And the previous tag v1.3.1-<tag-suffix>.2 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 1.3.1-<tag-suffix>.3.1
		And with the commit count 1
		Examples:
			| tag-suffix |
			| alpha      |
			| beta       |
			| rc         |

	Scenario: Parsing single breaking feature commit
		Given the following commit feat!: this is a major breaking change
		And the previous tag v1.2.0 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 2.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing single breaking feature commit on pre-1.0 with next version 0.1.0
		Given the following commit feat!: some major break
		And the next version is set to 0.1.0
		And the previous tag 0.2.0 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 0.3.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing feature commit configured with stable next version and previous non-stable
		Given the following commit feat: some kind of new feature
		And the previous tag 0.2.0 with the message docs: update changelog
		And the next version is set to 1.0.0
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing commit with breaking change in body
		Given the following commit feat: some new feature\n\nSome body\n\nBREAKING CHANGE: Important breaking change
		And the previous tag 1.0.0 with the message docs: update changelog
		When the user parses the commits
		Then the resulting version should be 2.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing non-bumping commits should not bump version
		Given the following commits
			| isTag | ref             | rawText                     |
			| false |                 | chore: some kind of update  |
			| false |                 | build: build changes        |
			| true  | refs/tags/1.3.3 | feat: important new feature |
		When the user parses the commits
		Then the resulting version should be 1.3.3
		And with the commit count 2
