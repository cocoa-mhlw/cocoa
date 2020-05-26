Feature: UpdateInformation
	
Scenario: Navigate to Update Information
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "Update Information"
	When I tap on "Update Information" element
	Then I am on "Update Information" page
	# to complete