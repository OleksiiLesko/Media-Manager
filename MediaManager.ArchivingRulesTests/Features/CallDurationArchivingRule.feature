@CallDurationArchivingRule
Feature: CallDurationArchivingRule

The CallDurationArchivingRule feature focuses on managing archiving calls based on call duration,
covering scenarios for applying the call duration archiving rule
and refreshing call duration when configuration changes.

@ApplyCallDurationArchivingRule
Scenario Outline: Apply call duration archiving rule with correct call duration
	Given the call duration archiving rule is initialized with сomparison operator <ComparisonOperator> and call duration <CallDuration>
	And a call event with special CallId <CallId> and CallStartTime <CallStartTime>, CallEndTime <CallEndTime>
	When the call duration archiving rule is applied
	Then it should return expected result <ExpectedResult> set of recording ids

Examples:
	| ComparisonOperator | CallDuration | CallId | CallStartTime       | CallEndTime         | ExpectedResult |
	| >=                 | 2700         | 1      | 2024-03-01T10:00:00 | 2024-03-01T11:00:00 | 1,2            |
	| <                  | 2700         | 2      | 2024-03-01T09:00:00 | 2024-03-01T09:30:00 |                |

@RefreshDuration
Scenario Outline: Refresh call duration when configuration changes
	Given an initial call duration configuration
	When the configuration changes to updated call duration <UpdatedCallDuration>
	Then it should refresh call directions and log the change with message <ExpectedLogMessage>

Examples:
	| UpdatedCallDuration | ExpectedLogMessage                                     |
	| 2700                | CallDurationRuleConfig changed. Refreshed CallDuration |
	| 7777                | CallDurationRuleConfig changed. Refreshed CallDuration |

