using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.PostgreSql.Repositories
{
    public class BookingsRepository : IBookingsRepository
    {
        #region Fields

        private readonly NpgsqlConnection _connection;

        #endregion

        #region Constructor

        public BookingsRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #endregion

        #region Methods

        public async Task<BookingEntity> AddAsync(BookingEntityCreate bookingToCreate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("rentalId", bookingToCreate.RentalId);
            parameters.Add("unitId", bookingToCreate.UnitId);
            parameters.Add("nights", bookingToCreate.Nights);
            parameters.Add("start", bookingToCreate.Start);
            parameters.Add("end", bookingToCreate.Start.AddDays(bookingToCreate.Nights - 1));
            parameters.Add("preparationStart", bookingToCreate.Start.AddDays(bookingToCreate.Nights));
            parameters.Add("preparationEnd", bookingToCreate.Start.AddDays(bookingToCreate.Nights).AddDays(bookingToCreate.PreparationTime - 1));

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO booking (rental_id, unit_id, booking_nights, booking_start, booking_end, preparation_start, preparation_end) ");
            queryBuilder.Append("VALUES(@rentalId, @unitId, @Nights, @start, @end, @preparationStart, @preparationEnd) ");
            queryBuilder.Append("returning id as id , rental_id as rentalId, unit_id as unitId, booking_nights as bookingNights, booking_start as bookingStart, ");
            queryBuilder.Append(" booking_end as bookingEnd, preparation_start as preparationStart, preparation_end as preparationEnd;");

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var result = await _connection.QueryAsync<BookingEntity>(queryBuilder.ToString(), parameters, commandType: CommandType.Text)
                .ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<BookingEntity>> BulkUpdateAsync(IEnumerable<BookingEntity> bookingsToUpdate)
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            foreach (var booking in bookingsToUpdate)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", booking.Id);
                parameters.Add("@preparationEnd", booking.PreparationEnd);

                var query = $"update booking set preparation_end = @preparationEnd where id = @id;";

                await _connection.QueryAsync<BookingEntity>(query, parameters, commandType: CommandType.Text)
                    .ConfigureAwait(false);
            }

            return bookingsToUpdate;
        }

        public async Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime startDate, DateTime endDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@rentalId", rentalId);
            parameters.Add("@startDate", startDate);
            parameters.Add("@endDate", endDate);
            
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"select id as id , rental_id as rentalId, unit_id as unitId, booking_nights as bookingNights, ");
            queryBuilder.Append($"booking_start as bookingStart, booking_end as bookingEnd, preparation_start as preparationStart, preparation_end as preparationEnd ");
            queryBuilder.Append($"from booking where rental_id = @rentalId and booking_start <= @endDate and preparation_end >= @startDate;");

            return await _connection.QueryAsync<BookingEntity>(queryBuilder.ToString(), parameters, commandType: CommandType.Text)
                .ConfigureAwait(false);
        }

        public async Task<BookingEntity> GetByIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"select id as id , rental_id as rentalId, unit_id as unitId, booking_nights as bookingNights, ");
            queryBuilder.Append($"booking_start as bookingStart, booking_end as bookingEnd, preparation_start as preparationStart, preparation_end as preparationEnd ");
            queryBuilder.Append($"from booking where id = @id;");

            var result = await _connection.QueryAsync<BookingEntity>(queryBuilder.ToString(), parameters, commandType: CommandType.Text)
                .ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        #endregion
    }
}
