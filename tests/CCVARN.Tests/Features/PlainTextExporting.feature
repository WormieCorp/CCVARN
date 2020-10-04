@plain-text
Feature: Exporting plain text release notes

	I want to know the format being used when
	I am exporting release notes as plain text.

	Scenario: Exporting single feature note
		Given the parsed version 1.0.0
		And the release notes
			| displayName | type | summary              |
			| Feature     | feat | some kind of feature |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 1.0.0 (<TODAY>)        |
			| ==================     |
			|                        |
			| Feature                |
			| -------                |
			| - some kind of feature |

	Scenario: Exporting single bug fix note
		Given the parsed version 1.0.1
		And the release notes
			| displayName | type | summary              |
			| Bug fix     | fix  | some kind of bug fix |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 1.0.1 (<TODAY>)        |
			| ==================     |
			|                        |
			| Bug fix                |
			| -------                |
			| - some kind of bug fix |

	Scenario: Exporting multiple feature notes
		Given the parsed version 1.1.0
		And the release notes
			| displayName | type | summary                  |
			| Features    | feat | some awesome new feature |
			| Features    | feat | another awesome feature  |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 1.1.0 (<TODAY>)            |
			| ==================         |
			|                            |
			| Features                   |
			| --------                   |
			| - some awesome new feature |
			| - another awesome feature  |

	Scenario: Exporting refactor with breaking change
		Given the parsed version 1.0.0
		And the release notes
			| displayName     | type     | summary                    |
			| BREAKING CHANGE | refactor | renamed MethodA to MethodB |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 1.0.0 (<TODAY>)              |
			| ==================           |
			|                              |
			| BREAKING CHANGE              |
			| ---------------              |
			| - renamed MethodA to MethodB |

	Scenario: Exporting refactor with breaking change and body
		Given the parsed version 2.0.0
		And the release notes
			| displayName      | type     | summary                    |
			| Code refactoring | refactor | renamed MethodA to MethodB |
		And the breaking changes
			| In the class Something the MethodA was renamed to MethodB |
			| to allow something something                              |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 2.0.0 (<TODAY>)                                           |
			| ==================                                        |
			|                                                           |
			| BREAKING CHANGES                                          |
			| ----------------                                          |
			| In the class Something the MethodA was renamed to MethodB |
			| to allow something something                              |
			|                                                           |
			| Code refactoring                                          |
			| ----------------                                          |
			| - renamed MethodA to MethodB                              |

	Scenario: Exporting feature with a scope
		Given the parsed version 2.0.0
		And the release notes
			| displayName | type | summary          | scope   |
			| Feature     | feat | some new feature | awesome |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 2.0.0 (<TODAY>)               |
			| ==================            |
			|                               |
			| Feature                       |
			| -------                       |
			| - (awesome): some new feature |

	Scenario: Exporting mixed breaking changes
		Given the parsed version 3.0.0
		And the breaking changes
			| This is a very big breaking change |
		And the release notes
			| displayName      | type | summary              |
			| Feature          | feat | some feature         |
			| BREAKING CHANGE  | feat | some breaking change |
			| BREAKING CHANGES | fix  | some breaking fix    |
		When the user is exporting plain text
		Then the exported plain release notes should be
			| 3.0.0 (<TODAY>)                    |
			| ==================                 |
			|                                    |
			| BREAKING CHANGES                   |
			| ----------------                   |
			| This is a very big breaking change |
			|                                    |
			| - some breaking change             |
			| - some breaking fix                |
			|                                    |
			| Feature                            |
			| -------                            |
			| - some feature                     |

	Scenario: Exporting plain text release notes without version header
		Given the parsed version 1.0.0
		And the release notes
			| displayName | type | summary      |
			| Feature     | feat | some feature |
		When the user is exporting plain text without header
		Then the exported plain release notes should be
			|                |
			| Feature        |
			| -------        |
			| - some feature |

	Scenario: Exporting plain text based on extension
		Given the path test.<ext>
		When the user checks if path can be parsed
		Then the result should be true
		Examples:
			| ext |
			| txt |
			| TXT |
			| tXt |

	Scenario: Exporting non-plain text based on extension
		Given the path test.md
		When the user checks if path can be parsed
		Then the result should be false
