using Inovatec.OfficeManagementTool.InterfacesDAL;
using Microsoft.Extensions.DependencyInjection;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        private IServiceProvider _serviceProvider;

        public UnitOfWorkProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Begin()
        {
            var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService<IUnitOfWork>();
        }
    }
}
