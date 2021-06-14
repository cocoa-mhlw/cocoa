# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

Feature: LicenseAgreement

Scenario: Navigate to License Agreement
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "License Agreement"
	When I tap on "License Agreement" element
	Then I am on "License Agreement" page
	# to complete
