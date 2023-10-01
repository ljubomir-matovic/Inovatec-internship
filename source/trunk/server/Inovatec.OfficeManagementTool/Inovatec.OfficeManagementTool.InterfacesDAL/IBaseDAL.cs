using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IBaseDAL<TEntity> where TEntity : class
    {
        Task Insert(TEntity entity);
        Task InsertRange(TEntity entities);
        Task Delete(long id);
        Task<bool> LogicalDelete(long id);
        Task<List<TEntity>> GetAll();
        Task<TEntity?> GetById(long id);
        Task<List<TEntity>> GetBySpecificProperty(Expression<Func<TEntity, bool>> filter);
        Task Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entities);
    }
}
