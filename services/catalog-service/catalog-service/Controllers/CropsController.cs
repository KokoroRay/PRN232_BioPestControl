using catalog_service.DTOs;
using catalog_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace catalog_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropsController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropsController(ICropService cropService)
        {
            _cropService = cropService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CropResponse>>> GetAll()
        {
            var crops = await _cropService.GetAllAsync();
            return Ok(crops);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CropProfileResponse>> GetById(int id)
        {
            var crop = await _cropService.GetByIdAsync(id);
            if (crop == null)
            {
                return NotFound();
            }
            return Ok(crop);
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<CropProfileResponse>> GetBySlug(string slug)
        {
            var crop = await _cropService.GetBySlugAsync(slug);
            if (crop == null)
            {
                return NotFound();
            }
            return Ok(crop);
        }

        [HttpPost]
        public async Task<ActionResult<CropResponse>> Create([FromBody] CropRequest request)
        {
            var crop = await _cropService.CreateAsync(request);
            return Ok(crop);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CropResponse>> Update(int id, [FromBody] CropRequest request)
        {
            try
            {
                var crop = await _cropService.UpdateAsync(id, request);
                return Ok(crop);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _cropService.DeleteAsync(id);
            return NoContent();
        }
    }
}
