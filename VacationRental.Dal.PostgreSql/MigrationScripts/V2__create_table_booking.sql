CREATE TABLE booking (
  id SERIAL NOT NULL,
  rental_id int NOT NULL,
  unit_id int NOT NULL,
  booking_nights int NOT NULL,  
  booking_start date NOT NULL,
  booking_end date NOT NULL,
  preparation_start date NOT NULL,
  preparation_end date NOT NULL,
  PRIMARY KEY (id)
);
 
CREATE INDEX booking_rental_id_period_idx ON booking (rental_id,booking_start, preparation_end);