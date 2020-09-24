@Versioning
Feature: Version Data Bump
	When a version is set and I try
	to bump the version. I want to know
	what version I should expect it to bump
	to.

	Scenario: Bumping <type> should also update version
		Given a version of 5.2.1
		When the user parses the version
		And the user bumps the <type> part
		Then the version output should be <version>
		Examples:
		| type  | version |
		| major | 6.0.0   |
		| minor | 5.3.0   |
		| patch | 5.2.2   |

	Scenario: Calling version bump with 'none' should not bump the version
		Given a version of 1.0.0
		When the user parses the version
		And the user tries to bump the version when none is used
		Then the version output should be 1.0.0

	Scenario: Calling version bump on a pre-release bumps the weight
		Given a version of <version>
		When the user parses the version
		And the user bumps the weight part
		Then the version output should be <expectedVersion>
		Examples:
		| version      | expectedVersion |
		| 1.0.0-alpha  | 1.0.0-alpha.1    |
		| 23.5-beta.55 | 23.5.0-beta.56   |
		| 0.5.2-rc.1   | 0.5.2-rc.2       |
