Feature: Tutorial

Scenario: Complete Tutorial
	Given I am on "HOME" page
	Then I can see "LabelMainTutorial"

	When I tap on "LabelMainTutorial" element
	#Then I am on "StartTutorialPages" page
	Then I can see label with text "COVID-19Radar"
	And I can see "ButtonStart"

	When I navigate back
	Then I am on "HOME" page
	And I can see "ImageHelp"

	When I tap on "ImageHelp" element
	#Then I am on "StartTutorialPages" page
	Then I can see label with text "COVID-19Radar"
	And I can see "ButtonStart"

	When I tap on "ButtonStart" button
	#Then I am on "Terms of Service" page		
	Then I can see "ButtonAgree"
	And I can see "ButtonNotAgree"

	#Then I am on "How it works" page
	#And I can see label with text "1"
	#When I scroll down to element "2"
	#Then I can see label with text "2"
	#When I scroll down to element "3"
	#Then I can see label with text "3"
	#When I scroll down to element "Next"
	#And I tap on "NextButton" button
	#Then I am on "Terms of Service" page
	#And I can see "NextButton"
	#When I tap on "NextButton" button
	
	