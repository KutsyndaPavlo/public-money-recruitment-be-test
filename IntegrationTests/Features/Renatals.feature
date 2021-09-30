Feature: Rentals
	Testing rentals functionality

Scenario: Add rental and get created rental
	Given rental with 25 units and preparation time 5 is created
	When get rental by id
	Then the result should be 200 and units 25 and preparation time 5

Scenario: Update rental by decreasing units and increasing preparation time and get success updated rental result
	Given rental with 4 units and preparation time 2 is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	And update1 rental by setting 3 units and preparation time 3
	When get updated rental by id
	Then the result should be 200 and units 3 and preparation time 3

Scenario: Update rental by increasing units and descreasing preparation time and get success updated rental result
	Given rental with 4 units and preparation time 2 is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	And update1 rental by setting 5 units and preparation time 1
	When get updated rental by id
	Then the result should be 200 and units 5 and preparation time 1

Scenario: Update rental by descreasing units get 500 status
	Given rental with 3 units and preparation time 2 is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-01" for 3 nights is created
	When update rental by setting 2 units and preparation time 2
	Then the rental update result should be 500

Scenario: Update rental by increasing preparation time and get 500 status
	Given rental with 3 units and preparation time 2 is created
	And booking for "2031-01-01" for 3 nights is created
	And booking for "2031-01-02" for 3 nights is created
	And booking for "2031-01-03" for 3 nights is created
	And booking for "2031-01-06" for 3 nights is created
	And booking for "2031-01-08" for 3 nights is created
	And booking for "2031-01-15" for 3 nights is created
	When update rental by setting 3 units and preparation time 3
	Then the rental update result should be 500

