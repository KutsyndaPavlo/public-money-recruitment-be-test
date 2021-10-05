using Dapper;
using Npgsql;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.PostgreSql.Repositories
{
    public class RentalsRepository : IRentalsRepository
    {
        #region Fields

        private readonly NpgsqlConnection _connection;

        #endregion

        #region Constructor

        public RentalsRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #endregion

        #region Methods

        public async Task<RentalEntity> AddAsync(RentalEntityCreate rentalToCreate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("units", rentalToCreate.Units);
            parameters.Add("preparation_time_in_days", rentalToCreate.PreparationTimeInDays);

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO rental(units, preparation_time_in_days) ");
            queryBuilder.Append("VALUES(@units, @preparation_time_in_days) ");
            queryBuilder.Append("returning id as id, units as units, preparation_time_in_days as preparationTimeInDays;");

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var result = await _connection.QueryAsync<RentalEntity>(
                    queryBuilder.ToString(),
                    parameters,
                    commandType: CommandType.Text)
                .ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<RentalEntity> GetByIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var query = "select id as id, units as units, preparation_time_in_days as preparationTimeInDays from rental where id = @id;";
           
            var result = await _connection.QueryAsync<RentalEntity>(query, parameters, commandType: CommandType.Text)
                .ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<RentalEntity> UpdateAsync(RentalEntity rentalToUpdate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", rentalToUpdate.Id);
            parameters.Add("@units", rentalToUpdate.Units);
            parameters.Add("@preparationTimeInDays", rentalToUpdate.PreparationTimeInDays);

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("update rental set units = @units, preparation_time_in_days = @preparationTimeInDays where id = @id ");
            queryBuilder.Append("RETURNING id as id, units as units, preparation_time_in_days as preparationTimeInDays;");

            var result = await _connection.QueryAsync<RentalEntity>(
                    queryBuilder.ToString(),
                    parameters,
                    commandType: CommandType.Text)
                .ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        #endregion
    }
}
