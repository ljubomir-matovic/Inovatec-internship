using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.Admin + "," + Role.HR)]
    [ApiController]
    [Route("/api/[controller]")]
    public class ItemController : Controller
    {
        private readonly IItemUI _itemUI;

        public ItemController(IItemUI itemUI)
        {
            _itemUI = itemUI;
        }

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetItems([FromQuery] ItemFilterRequest itemFilterRequest)
        {
            return Ok(await _itemUI.GetItems(itemFilterRequest));
        }

        [Authorize]
        [HttpGet]
        [Route("page")]
        public async Task<IActionResult> GetItemsPage([FromQuery] ItemFilterRequest itemFilterRequest)
        {
            return Ok(await _itemUI.GetItemsPage(itemFilterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetItem([FromRoute] int id)
        {
            return Ok(await _itemUI.GetItemById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ItemCreateRequest newItem)
        {
            return Ok(await _itemUI.Insert(newItem));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateItem([FromBody] ItemViewModel updatedItem)
        {
            return Ok(await _itemUI.Update(updatedItem));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] long id)
        {
            return Ok(await _itemUI.Delete(id));
        }
    } 
}