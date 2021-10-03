Feature: Calendar
	Testing calendar functionality

Scenario: Correctly added booking for rental with 1 unit and check calendar
	Given rental with 1 units and preparation time 2 is created
	And booking for "2031-01-03" for 3 nights is created
	And booking for "2031-01-08" for 1 nights is created
	And booking for "2031-01-12" for 2 nights is created
	When get calendar by rental id from "2031-01-02" and 10 nights
	Then the result should be 200 and data
    | date       | booked unit  | prepared unit    |
    | 2031-01-02 |              |                  |
	| 2031-01-03 |1             |                  |
	| 2031-01-04 |1             |                  |
	| 2031-01-05 |1             |                  |
	| 2031-01-06 |              |   1              |
	| 2031-01-07 |              |   1              |
    | 2031-01-08 |1             |                  |
	| 2031-01-09 |              |         1        |
    | 2031-01-10 |              |         1        |
	| 2031-01-11 |              |                  |

Scenario: Correctly added booking for rental with 3 units and check calendar
	Given rental with 3 units and preparation time 2 is created
	And booking for "2031-02-02" for 3 nights is created
	And booking for "2031-02-03" for 4 nights is created
	And booking for "2031-02-04" for 3 nights is created
	And booking for "2031-02-08" for 2 nights is created
	When get calendar by rental id from "2031-02-01" and 10 nights
	Then the result should be 200 and data
    | date       | booked unit  | prepared  unit   |
    | 2031-02-01 |              |                  |
	| 2031-02-02 |     1        |                  |
    | 2031-02-03 |     1        |                  |
	| 2031-02-03 |     2        |                  |
	| 2031-02-04 |     1        |                  |
	| 2031-02-04 |     2        |                  |
	| 2031-02-04 |     3        |                  |
	| 2031-02-05 |     2        |                  |
	| 2031-02-05 |     3        |                  |
	| 2031-02-05 |              |       1          |
	| 2031-02-06 |     2        |                  |
	| 2031-02-06 |     3        |                  |
	| 2031-02-06 |              |       1          |
	| 2031-02-07 |              |       2          |
	| 2031-02-07 |              |       3          |
	| 2031-02-08 |     1        |                  |
	| 2031-02-08 |              |       2          |
	| 2031-02-08 |              |       3          |
	| 2031-02-09 |     1        |                  |
	| 2031-02-10 |              |       1          |

Scenario: Correctly added booking for rental with 3 unit, update renatl by increasing preparation time and check calendar
	Given rental with 3 units and preparation time 2 is created
	And booking for "2031-02-02" for 3 nights is created
	And booking for "2031-02-03" for 4 nights is created
	And booking for "2031-02-08" for 3 nights is created
    And booking for "2031-02-10" for 3 nights is created
	And update1 rental by setting 3 units and preparation time 3
	When get calendar by rental id after updates from "2031-02-01" and 10 nights
	Then the result should be 200 and data
    | date       | booked unit  | prepared  unit   |
    | 2031-02-01 |              |                  |
	| 2031-02-02 |     1        |                  |
    | 2031-02-03 |     1        |                  |
	| 2031-02-03 |     2        |                  |
	| 2031-02-04 |     1        |                  |
	| 2031-02-04 |     2        |                  |
	| 2031-02-05 |     2        |                  |
	| 2031-02-05 |              |       1          |
	| 2031-02-06 |     2        |                  |
	| 2031-02-06 |              |       1          |
	| 2031-02-07 |              |       1          |
	| 2031-02-07 |              |       2          |
	| 2031-02-08 |     1        |                  |
	| 2031-02-08 |              |       2          |
	| 2031-02-09 |     1        |                  |
	| 2031-02-09 |              |       2          |
	| 2031-02-10 |     1        |                  |
    | 2031-02-10 |     2        |                  |