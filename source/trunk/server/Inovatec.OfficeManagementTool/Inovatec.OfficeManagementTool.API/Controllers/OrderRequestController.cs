using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.Admin + "," + Role.HR + "," + Role.OrdinaryEmployee)]
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderRequestController : Controller
    {
        private readonly IOrderRequestUI _orderRequestUI;

        public OrderRequestController(IOrderRequestUI orderUI)
        {
            _orderRequestUI = orderUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveOrders([FromQuery] OrderFilterRequest orderFilterRequest)
        {
            return Ok(await _orderRequestUI.GetActiveOrders(orderFilterRequest));
        }

        [HttpGet]
        [Route("groupByItem")]
        public async Task<IActionResult> GetActiveOrdersGroupedByItem([FromQuery] OrderFilterRequest orderFilterRequest)
        {
            return Ok(await _orderRequestUI.GetActiveOrdersGroupedByItem(orderFilterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            return Ok(await _orderRequestUI.GetOrderById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddSnackOrder(SnackCreateRequest snackCreateRequest)
        {
            return Ok(await _orderRequestUI.AddSnackOrder(snackCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateSnackOrder(SnackUpdateRequest snackUpdateRequest)
        {
            return Ok(await _orderRequestUI.UpdateSnackOrder(snackUpdateRequest));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] long id)
        {
            return Ok(await _orderRequestUI.Delete(id));
        }

        [HttpGet]
        [Route("equipment")]
        public async Task<IActionResult> GetEquipmentOrders([FromQuery] OrderFilterRequest orderFilterRequest)
        {
            return Ok(await _orderRequestUI.GetEquipmentOrders(orderFilterRequest));
        }

        [HttpPost]
        [Route("equipment")]
        public async Task<IActionResult> AddEquipmentOrder(EquipmentOrderCreateRequest equipmentOrderCreateRequest)
        {
            return Ok(await _orderRequestUI.AddEquipmentOrder(equipmentOrderCreateRequest));
        }

        [HttpPatch]
        [Route("equipment")]
        public async Task<IActionResult> UpdateEquipmentOrder([FromBody] UpdateEquipmentOrderRequest updateEquipmentOrderRequest)
        {
            return Ok(await _orderRequestUI.UpdateEquipmentOrder(updateEquipmentOrderRequest));
        }
    } 
}