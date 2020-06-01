Feature: Home

Scenario: Navigate away from and to Home
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "Contributors List"
	When I tap on "Contributors List" element
	Then I am on "Contributors List" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "HOME"
	When I tap on "HOME" element
	Then I am on "HOME" page
	
	Scenario: Exposures
	Given I am on "HOME" page
	And I can see "ButtonExposures"
	When I tap on "ButtonExposures" button
	Then I am on "Exposures" page
	# to complete

	Scenario: Notify Other
	Given I am on "HOME" page
	When I scroll down to element "ButtonNotifyOther"
	Then I can see "ButtonNotifyOther"
	When I tap on "ButtonNotifyOther" button
	Then I am on "Status Settings" page
	# to complete

	Scenario: Share App
	Given I am on "HOME" page
	When I scroll down to element "ButtonShareApp"
	Then I can see "ButtonShareApp"
	When I tap on "ButtonShareApp" button
	# to complete