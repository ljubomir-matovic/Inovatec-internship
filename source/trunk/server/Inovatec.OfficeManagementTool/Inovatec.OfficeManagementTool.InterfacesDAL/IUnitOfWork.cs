using Microsoft.EntityFrameworkCore.Storage;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserDAL UserDAL { get; }

        public IEquipmentDAL EquipmentDAL { get; }

        public ICategoryDAL CategoryDAL { get; }

        public IItemDAL ItemDAL { get; }

        public IOrderDAL OrderDAL { get; }

        public IOrderItemDAL OrderItemDAL { get; }

        public IOrderRequestDAL OrderRequestDAL { get; }

        public IOrderAttachmentDAL OrderAttachmentDAL { get; }

        public ICommentDAL CommentDAL { get; }

        public IReportScheduleDAL ReportScheduleDAL { get; }

        public IDbContextTransaction BeginTransaction();

        public ILogDAL LogDAL { get; }

        public IReportDAL ReportDAL { get; }

        public IOfficeDAL OfficeDAL { get; }

        public INotificationDAL NotificationDAL { get; }

        public ISupplierDAL SupplierDAL { get; }

        public Task SaveChangesAsync();
    }
}