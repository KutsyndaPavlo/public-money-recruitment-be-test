using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IBaseRepository<TEntity, TRequestModel> where TEntity : BaseEntity
                                                             where TRequestModel : class

    {
        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> AddAsync(TRequestModel bookingToCreate);
    }
}
