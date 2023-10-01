using AutoMapper;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ServiceInitializer.AutoMappers
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserCreateRequest, User>();
            CreateMap<CategoryCreateRequest, Category>();
            CreateMap<Category, CategoryViewModel>();
            CreateMap<ItemCreateRequest, Item>();
            CreateMap<Item, ItemViewModel>();
            CreateMap<Order, OrderViewModel>();
            CreateMap<SnackCreateRequest, OrderRequest>();
            CreateMap<OrderRequest, OrderViewModel>();
            CreateMap<CommentCreateRequest, Comment>();
            CreateMap<Comment, CommentViewModel>();
            CreateMap<EquipmentCreateRequest, Equipment>();
            CreateMap<Log, LogViewModel>();
            CreateMap<OfficeCreateRequest, Office>();
            CreateMap<Office, OfficeViewModel>();
            CreateMap<EquipmentOrderCreateRequest, OrderRequest>();
            CreateMap<ReportCreateRequest, Report>();
            CreateMap<Report, ReportViewModel>();
            CreateMap<Equipment, ReportedEquipmentViewModel>();
            CreateMap<ReportScheduleCreateRequest, ReportSchedule>();
            CreateMap<ReportSchedule, ReportScheduleViewModel>();
            CreateMap<ReportSchedule, UpdateReportScheduleRequest>();
            CreateMap<SupplierCreateRequest, Supplier>();
            CreateMap<Supplier, SupplierViewModel>();
            CreateMap<UpdateSupplierRequest, Supplier>();
        }
    }
}