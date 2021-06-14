# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

Feature: UpdateInformation

Scenario: Navigate to Update Information
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "Update Information"
	When I tap on "Update Information" element
	Then I am on "Update Information" page
	# to complete
