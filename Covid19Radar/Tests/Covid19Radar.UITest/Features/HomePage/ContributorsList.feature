Feature: Contributors List

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

Scenario: Navigate to Contributors List
	Given I am on "HOME" page
	When I tap on MasterDetail Hamburger
	Then I can see a label marked as "Contributors List"
	When I tap on "Contributors List" element
	Then I am on "Contributors List" page
	# to complete