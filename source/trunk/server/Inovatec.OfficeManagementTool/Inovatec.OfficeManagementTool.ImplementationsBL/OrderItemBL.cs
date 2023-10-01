using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class OrderItemBL: IOrderItemBL
    {
        public readonly IUnitOfWorkProvider _unitOfWorkProvider;

        public OrderItemBL(IUnitOfWorkProvider unitOfWorkProvider)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task<(List<OrderItemViewModel>, long)> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.OrderItemDAL.GetOrderItemsForOrder(orderId, filterRequest);
            }
        }

        public async Task<ActionResultResponse<string>> Insert(OrderItemCreateRequest entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var orderState = (await unitOfWork.OrderDAL.GetById(entity.OrderId))?.State;

                if (orderState == (int)OrderState.Done || orderState == (int)OrderState.Canceled)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("MustBePendingOrInProgress");
                    return result;
                }

                if(await unitOfWork.OrderItemDAL.OrderItemExists(entity.OrderId, entity.ItemId))
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("OrderItemAlreadyExists");
                    return result;
                }

                var orderItem = new OrderItem
                {
                    ItemId = entity.ItemId,
                    Amount = entity.Amount,
                    OrderId = entity.OrderId,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    IsDeleted = false,
                };
                await unitOfWork.OrderItemDAL.Insert(orderItem);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "OrderItemCreatedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Update(OrderItemCreateRequest entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var orderItem = await unitOfWork.OrderItemDAL.GetById(entity.Id);

                if(orderItem == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("");
                    return result;
                }

                orderItem.Amount = entity.Amount;
                await unitOfWork.OrderItemDAL.Update(orderItem);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "AmountChangedSuccess";

            return result;
        }

        public async Task<ActionResultResponse<string>> DeleteMore(List<long> ids)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                foreach (var id in ids)
                {
                    await unitOfWork.OrderItemDAL.LogicalDelete(id);
                }
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "OrderItemDeletedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                await unitOfWork.OrderItemDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "OrderItemDeletedSuccess";
            return result;
        }
    }
}