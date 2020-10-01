Feature: Unstable Tagged Version Parsing
I want to know which versions to expect when
I am using tagged unstable releases.
Not all cases will have the version being
exactly as the tag, except for it will also
include the commits.

	Scenario: Parsing with a single feature tag commit
		Given the following tag 1.3.0-alpha.1 with the message feat: some awesome feature
		When the user parses the commits
		Then the resulting version should be 1.3.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing with multiple feature commits with current tag
		Given the following tag 1.4.4-alpha.1 with the message feat: awesome tag commit
		And the following commit feat: this is just a commit
		When the user parses the commits
		Then the resulting version should be 1.4.4-alpha.1.2
		And with the commit count 2

	Scenario: Parsing tags without a version
		Given the following tag invalid with the message feat: test commit
		When the user parses the commits
		Then the resulting version should be 1.0.0-alpha.1.1
		And with the commit count 1

	Scenario: Parsing tag with non-alpha-tag
		Given the following tag 1.3.3-beta.5 with the message fix: yup this happenend
		When the user parses the commits
		Then the resulting version should be 1.3.3-beta.5.1
		And with the commit count 1
