Feature: Contributors List

Scenario: Navigate to Contributors List
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "Contributors List"
	When I tap on "Contributors List" element
	Then I am on "Contributors List" page
	# to complete