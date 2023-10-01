using Inovatec.OfficeManagementTool.ImplementationsUI;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR)]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : Controller
    {
        public readonly IOrderItemUI _orderItemUI;

        public OrderItemController(IOrderItemUI orderItemUI) 
        {
            _orderItemUI = orderItemUI;
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<IActionResult> GetOrderItemsForOrder([FromRoute] long orderId, [FromQuery] EquipmentFilterRequest queryParams)
        {
            if (queryParams.PageNumber < 1)
            {
                ModelStateDictionary dict = new ModelStateDictionary();
                dict.AddModelError("pageNumber", "pageNumber must be whole number greater than 0");
                return BadRequest(dict);
            }

            var page = await _orderItemUI.GetOrderItemsForOrder(orderId, queryParams);

            if (page.Data.Count == 0)
            {
                return NotFound(new { message = "Page not exists" });
            }

            return Ok(page);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateOrderItem([FromBody] OrderItemCreateRequest requestBody)
        {
            var result = await _orderItemUI.Insert(requestBody);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> ChangeAmountOfOrderItem([FromBody] OrderItemCreateRequest requestBody)
        {
            var result = await _orderItemUI.Update(requestBody);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteSelectedOrderItems([FromBody] List<long> ids)
        {
            var result = await _orderItemUI.DeleteMore(ids);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, result.Errors);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOrderItem([FromRoute] long id)
        {
            var result = await _orderItemUI.Delete(id);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, result.Errors);
        }
    }
}