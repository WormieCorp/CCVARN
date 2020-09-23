@Versioning
Feature: Version Data
	When parsing a reference or a raw version
	I want to know what version will be detected.

	Scenario: Parsing a tag reference without a prefix
		Given a tag reference of 1.4.4
		When the user parses the reference
		Then the version output should be 1.4.4

	Scenario: Parsing a version without a prefix
		Given a version of 5.2.1
		When the user parses the version
		Then the version output should be 5.2.1

	Scenario: Parsing a tag reference with a v prefix
		Given a tag reference of v3.2.1
		When the user parses the version
		Then the version output should be 3.2.1

	Scenario: Parsing a version with with a v prefix
		Given a version of v3.1.2
		When the user parses the version
		Then the version output should be 3.1.2

	Scenario: Parsing a tag reference with a pre-release tag
		Given a tag reference of 10.4.4-alpha
		When the user parses the version
		Then the version output should be 10.4.4-alpha

	Scenario: Parsing a version with a pre-release tag
		Given a version of 1.0.0-rc.1
		When the user parses the version
		Then the version output should be 1.0.0-rc.1

	Scenario: Parsing a tag reference with meta information
		Given a version of 3.0.4+2-329ab2937
		When the user parses the version
		Then the version output should be 3.0.4

	Scenario: Parsing a version with meta information
		Given a version of 2.0.0+6-23abe323fd22
		When the user parses the version
		Then the version output should be 2.0.0

	Scenario: Parsing a tag reference with pre-release and meta information
		Given a tag reference of 1.2.5-beta.54+2-32bac654
		When the user parses the version
		Then the version output should be 1.2.5-beta.54

	Scenario: Invalid tag reference should result in no default version output
		Given a tag  of refs/tags/invalid-tag
		When the user parses the version
		Then the version output should be 0.0.0
