Feature: LicenseAgreement


Scenario: Navigate to License Agreement
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "License Agreement"
	When I tap on "License Agreement" element
	Then I am on "License Agreement" page
	# to complete