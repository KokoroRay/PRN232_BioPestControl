using article_service.Models;
using article_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace article_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _contactService.GetAllAsync();
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Message))
                return BadRequest("Invalid contact data");

            request.SubmittedAt = DateTime.UtcNow;
            var created = await _contactService.CreateAsync(request);
            return Ok(created);
        }

        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> Resolve(string id, [FromBody] ResolveRequest request)
        {
            var result = await _contactService.ResolveAsync(id, request.ResolutionNotes);
            if (!result) return NotFound();

            return Ok(new { Success = true });
        }
    }

    public class ResolveRequest
    {
        public string ResolutionNotes { get; set; } = string.Empty;
    }
}
