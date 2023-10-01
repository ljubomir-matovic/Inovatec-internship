using Inovatec.OfficeManagementTool.ImplementationsUI;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR + "," + Role.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : Controller
    {
        private readonly ISupplierUI _supplierUI;
        
        public SupplierController(ISupplierUI supplierUI)
        {
            _supplierUI = supplierUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierPage([FromQuery] SupplierFilterRequest filterRequest)
        {
            return Ok(await _supplierUI.GetSupplierPage(filterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOfficeById([FromRoute] int id)
        {
            return Ok(await _supplierUI.GetSupplierById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierCreateRequest supplierCreateRequest)
        {
            return Ok(await _supplierUI.Insert(supplierCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateSupplierRequest updateSupplierRequest)
        {
            return Ok(await _supplierUI.Update(updateSupplierRequest));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] long id)
        {
            return Ok(await _supplierUI.Delete(id));
        }
    }
}
