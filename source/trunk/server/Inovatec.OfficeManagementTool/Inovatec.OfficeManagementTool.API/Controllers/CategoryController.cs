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
    public class CategoryController : Controller
    {
        private readonly ICategoryUI _categoryUI;

        public CategoryController(ICategoryUI categoryUI)
        {
            _categoryUI = categoryUI;
        }

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllCategories([FromQuery] CategoryFilterRequest categoryFilterRequest)
        {
            return Ok(await _categoryUI.GetCategories(categoryFilterRequest));
        }

        [Authorize]
        [HttpGet]
        [Route("page")]
        public async Task<IActionResult> GetCategoryPage([FromQuery] CategoryFilterRequest categoryFilter)
        {
            return Ok(await _categoryUI.GetCategoryPage(categoryFilter));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            return Ok(await _categoryUI.GetCategoryById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryCreateRequest newCategory)
        {
            return Ok(await _categoryUI.Insert(newCategory));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryViewModel updatedCategory)
        {
            return Ok(await _categoryUI.Update(updatedCategory));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] long id)
        {
            return Ok(await _categoryUI.Delete(id));
        }
    } 
}