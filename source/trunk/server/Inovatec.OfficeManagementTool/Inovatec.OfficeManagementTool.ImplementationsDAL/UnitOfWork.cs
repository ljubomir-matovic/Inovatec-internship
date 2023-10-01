using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private OfficeManagementTool_IS2023Context _context;

        private Dictionary<Type, object> _dictionaryDALs;
        private object _lock = new object();

        public IUserDAL UserDAL => GetOrInitDAL<UserDAL>();

        public ICategoryDAL CategoryDAL => GetOrInitDAL<CategoryDAL>();

        public IItemDAL ItemDAL => GetOrInitDAL<ItemDAL>();

        public IOrderDAL OrderDAL => GetOrInitDAL<OrderDAL>();

        public IOrderItemDAL OrderItemDAL => GetOrInitDAL<OrderItemDAL>();

        public IOrderRequestDAL OrderRequestDAL => GetOrInitDAL<OrderRequestDAL>();

        public IOrderAttachmentDAL OrderAttachmentDAL => GetOrInitDAL<OrderAttachmentDAL>();

        public ICommentDAL CommentDAL => GetOrInitDAL<CommentDAL>();

        public IEquipmentDAL EquipmentDAL => GetOrInitDAL<EquipmentDAL>();

        public ILogDAL LogDAL => GetOrInitDAL<LogDAL>();

        public IReportDAL ReportDAL => GetOrInitDAL<ReportDAL>();

        public INotificationDAL NotificationDAL => GetOrInitDAL<NotificationDAL>();

        public IOfficeDAL OfficeDAL => GetOrInitDAL<OfficeDAL>();

        public IReportScheduleDAL ReportScheduleDAL => GetOrInitDAL<ReportScheduleDAL>();

        public ISupplierDAL SupplierDAL => GetOrInitDAL<SupplierDAL>();

        public UnitOfWork(OfficeManagementTool_IS2023Context context)
        {
            _context = context;
            _dictionaryDALs = new Dictionary<Type, object>();
        }
        
        private TEntity GetOrInitDAL<TEntity>() where TEntity : BaseDAL<OfficeManagementTool_IS2023Context>, new()
        {
            lock(_lock)
            {
                Type entityType = typeof(TEntity);

                if (_dictionaryDALs.ContainsKey(entityType))
                    return (TEntity)_dictionaryDALs[entityType];

                TEntity newDAL = new TEntity();
                newDAL.SetDbContext(_context);
                _dictionaryDALs.Add(entityType, newDAL);
                return newDAL;
            }
        }

        public async Task SaveChangesAsync()
        {
            _context.ChangeTracker.DetectChanges();

            List<EntityEntry> entityEntries = _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added).ToList();
            
            foreach (EntityEntry entry in entityEntries)
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        if(entry.Property("DateCreated") != null)
                        {
                            entry.Property("DateCreated").CurrentValue = DateTime.Now; 
                        }
                        break;

                    case EntityState.Modified:
                        if (entry.Property("DateModified") != null)
                        {
                            entry.Property("DateModified").CurrentValue = DateTime.Now;
                        }
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        void IDisposable.Dispose()
        {
            _context.Dispose();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}