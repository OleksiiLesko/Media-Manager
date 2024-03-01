@MediaTypeArchivingRule
Feature: MediaTypeArchivingRule

The MediaTypeArchivingRule feature focuses on managing archiving calls based on media type,
covering scenarios for applying the media type archiving rule
and refreshing media type when configuration changes.

Background:
	Given the media type archiving rule is initialized
	And an initial media type configuration

@ApplyMediaTypeArchivingRule
Scenario Outline: Apply media type archiving rule with correct media types
	And a call event with special CallId <CallId> and MediaType <MediaType>
	When the media type archiving rule is applied
	Then it should return expected result <ExpectedResult> set of recording ids

Examples:
	| CallId | MediaType          | ExpectedResult |
	| 1      | Voice,Voice,Screen | 1,2            |
	| 2      | Screen             |                |

@RefreshMediaType
Scenario Outline: Refresh media types when configuration changes
	When the configuration changes to updated media type <UpdatedMediaType>
	Then it should refresh media types and log the change with message <ExpectedLogMessage>

Examples:
	| UpdatedMediaType | ExpectedLogMessage                               |
	| Voice            | MediaTypeRulConfig changed. Refreshed MediaTypes |
	| Screen           | MediaTypeRulConfig changed. Refreshed MediaTypes |
