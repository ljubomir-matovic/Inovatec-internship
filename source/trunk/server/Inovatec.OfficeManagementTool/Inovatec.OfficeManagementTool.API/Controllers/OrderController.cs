using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR)]
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderUI _orderUI;

        public OrderController(IOrderUI orderUI)
        {
            _orderUI = orderUI;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterRequest orderFilterRequest)
        {
            return Ok(await _orderUI.GetOrders(orderFilterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            return Ok(await _orderUI.GetOrderById(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateOrderFromCart()
        {
            return Ok(await _orderUI.CreateOrderFromCart());
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> ChangeState([FromBody] OrderUpdateRequest requestBody)
        {
            return Ok(await _orderUI.ChangeState(requestBody));
        }
    } 
}