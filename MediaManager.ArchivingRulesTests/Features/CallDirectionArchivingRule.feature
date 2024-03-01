@CallDirectionArchivingRule
Feature: CallDirectionArchivingRule

The CallDirectionArchivingRule feature focuses on managing archiving calls based on call direction,
covering scenarios for applying the call direction archiving rule
and refreshing call directions when configuration changes.

Background:
	Given the call direction archiving rule is initialized
	And an initial call direction configuration

@ApplyCallDirectionArchivingRule
Scenario Outline: Apply call direction archiving rule with call directions
	Given a call event with special CallId <CallId> and CallDirection <CallDirection>
	When the call direction archiving rule is applied
	Then it should return expected result <ExpectedResult> set of recording ids

Examples:
	| CallId | CallDirection | ExpectedResult |
	| 1      | Incoming      | 1,2            |
	| 2      | Outcoming     |                |

@RefreshDirections
Scenario Outline: Refresh call directions when configuration changes
	When the configuration changes to updated call direction <UpdatedCallDirection>
	Then it should refresh call directions and log the change with message <ExpectedLogMessage>

Examples:
	| UpdatedCallDirection | ExpectedLogMessage                                         |
	| Incoming             | CallDirectionRuleConfig changed. Refreshed CallDirections. |
	| Outcoming            | CallDirectionRuleConfig changed. Refreshed CallDirections. |
