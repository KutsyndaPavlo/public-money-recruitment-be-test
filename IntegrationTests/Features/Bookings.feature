Feature: Bookings
	Testing calendar functionality

Scenario: Add booking and successfully get created booking
	Given rental with 4 units and preparation time 1 is created
	And booking for "2031-01-01" for 3 nights is created
	When get created booking
	Then the result should be 200 and start date "2031-01-01" and nights 3 and unit 1

Scenario: Add booking using missing rental id and receive 400 status code in response
	When Add a booking for "2031-01-01" for 3 nights and rental id 999999999
	Then the result of creating a booking should be 400

Scenario: Add booking and get 409 status code in response because there is a booking started before current booking start day and finished after curent booking start day
	Given rental with 1 units and preparation time 1 is created
	And booking for "2002-01-01" for 7 nights is created
	When booking for "2002-01-05" for 6 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add booking and get 409 status code in response because there is a booking started before current booking end day and finished after curent booking end day
	Given rental with 1 units and preparation time 1 is created
	And booking for "2002-01-04" for 10 nights is created
	When booking for "2002-01-02" for 4 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add booking and get 409 status code in response because there is a booking started before current booking start day and finished before curent booking end day
	Given rental with 1 units and preparation time 1 is created
	And booking for "2002-01-01" for 3 nights is created
	When booking for "2002-01-02" for 3 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add booking 3 times and get 409 status code in response in 4-th time
	Given rental with 3 units and preparation time 1 is created
	And booking for "2002-01-01" for 5 nights is created
	And booking for "2002-01-01" for 6 nights is created
	And booking for "2002-01-01" for 7 nights is created
	When booking for "2002-01-01" for 8 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add booking and get 409 status code in response due to OverBooking of first preparation day
	Given rental with 1 units and preparation time 2 is created
	And booking for "2002-01-01" for 7 nights is created
	When booking for "2002-01-08" for 6 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add booking and get 409 status code in response result due to OverBooking of last preparation day
	Given rental with 1 units and preparation time 2 is created
	And booking for "2002-01-01" for 7 nights is created
	When booking for "2002-01-09" for 6 nights is posted
	Then the result should be 409 and message "Not available"

Scenario: Add next booking after finishing preparation days and receive 200 status code in response
	Given rental with 1 units and preparation time 2 is created
	And booking for "2002-01-01" for 7 nights is created
	And booking for "2002-01-10" for 6 nights is created
	When get created booking
	Then the result should be 200 and start date "2002-01-10" and nights 6 and unit 1





