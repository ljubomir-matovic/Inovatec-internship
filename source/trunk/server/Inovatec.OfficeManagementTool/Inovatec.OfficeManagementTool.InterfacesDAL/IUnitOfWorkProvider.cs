using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IUnitOfWorkProvider
    {
        IUnitOfWork Begin();
    }
}
