Feature: Tutorial

Scenario: Complete Tutorial
Given I can see label with text "COVID19Radar"
	Then I can see "StartButton"
	When I tap on "StartButton" button
	Then I am on "How it works" page
	And I can see label with text "1"
	When I scroll down to element "2"
	Then I can see label with text "2"
	When I scroll down to element "3"
	Then I can see label with text "3"
	When I scroll down to element "Next"
	And I tap on "NextButton" button
	Then I am on "Terms of Service" page
	And I can see "NextButton"
	When I tap on "NextButton" button
	