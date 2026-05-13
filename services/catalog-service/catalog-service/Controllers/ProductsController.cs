using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;
using catalog_service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace catalog_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var searchResults = await _productService.SearchByNameAsync(name);
                return Ok(searchResults);
            }

            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _productService.AddAsync(request);
            if (!created.Success)
            {
                return created.Error switch
                {
                    ProductCommandError.CategoryNotFound => BadRequest("CategoryId không tồn tại."),
                    ProductCommandError.ChemicalProfileNotFound => BadRequest("ChemicalProfileId không tồn tại."),
                    ProductCommandError.DuplicateSku => Conflict("SKU đã tồn tại."),
                    _ => BadRequest("Không thể tạo sản phẩm.")
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Data!.Id }, created.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _productService.UpdateAsync(id, request);
            if (!updated.Success)
            {
                return updated.Error switch
                {
                    ProductCommandError.ProductNotFound => NotFound(),
                    ProductCommandError.CategoryNotFound => BadRequest("CategoryId không tồn tại."),
                    ProductCommandError.ChemicalProfileNotFound => BadRequest("ChemicalProfileId không tồn tại."),
                    ProductCommandError.DuplicateSku => Conflict("SKU đã tồn tại."),
                    _ => BadRequest("Không thể cập nhật sản phẩm.")
                };
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted.Success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
