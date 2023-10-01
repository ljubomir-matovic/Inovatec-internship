using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class BaseDAL<TContext, TEntity> : BaseDAL<TContext>, IBaseDAL<TEntity> where TContext : OfficeManagementTool_IS2023Context where TEntity : class
    {
        protected DbSet<TEntity> Table => Context.Set<TEntity>();

        public BaseDAL() { }

        public virtual async Task<List<TEntity>> GetAll()
        {
            return await Table.ToListAsync();
        }

        public virtual async Task<TEntity?> GetById(long id)
        {
            return await Table.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> GetBySpecificProperty(Expression<Func<TEntity, bool>> filter)
        {
            return await Table.Where(filter).ToListAsync();
        }

        public virtual async Task Delete(long id)
        {
            TEntity? entityToDelete = await Table.FindAsync(id);
            if (entityToDelete != null)
                Table.Remove(entityToDelete);
        }

        public virtual async Task Insert(TEntity entityToInsert)
        {
            await Table.AddAsync(entityToInsert);
        }

        public virtual async Task InsertRange(List<TEntity> entitiesToInsert)
        {
            await Table.AddRangeAsync(entitiesToInsert);
        }

        public virtual async Task Update(TEntity entityToUpdate)
        {
            Table.Update(entityToUpdate);
        }

        public virtual async Task UpdateRange(IEnumerable<TEntity> entities)
        {
            Table.UpdateRange(entities);
        }

        public async Task<bool> LogicalDelete(long id)
        {
            TEntity? entity = await GetById(id);

            if (entity == null)
            {
                return false;
            }

            entity.GetType().GetProperty("IsDeleted")?.SetValue(entity, true);
            Table.Update(entity);
            return true;
        }

        public Task InsertRange(TEntity entities)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseDAL<TContext> where TContext : OfficeManagementTool_IS2023Context
    {
        public BaseDAL() { }

        protected OfficeManagementTool_IS2023Context Context { get; private set; }

        public void SetDbContext(OfficeManagementTool_IS2023Context context)
        {
            Context = context;
        }

        public BaseDAL(OfficeManagementTool_IS2023Context context)
        {
            Context = context;
        }
    }
}
