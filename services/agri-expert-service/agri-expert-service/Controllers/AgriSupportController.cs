using Microsoft.AspNetCore.Mvc;

namespace agri_expert_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgriSupportController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProvinces()
        {
            var provinces = new[]
            {
                new { Id = 1, Name = "Hà Nội" },
                new { Id = 2, Name = "Hồ Chí Minh" },
                new { Id = 3, Name = "Đà Nẵng" }
            };
            return Ok(new { provinces });
        }

        [HttpGet("Districts")]
        public IActionResult GetDistricts([FromQuery] int provinceId)
        {
            var districts = new[]
            {
                new { Id = 101, Name = "Quận 1" },
                new { Id = 102, Name = "Quận 3" },
                new { Id = 103, Name = "Quận Ba Đình" }
            };
            return Ok(districts);
        }

        [HttpPost("ByLocation")]
        public IActionResult GetWeatherByLocation([FromQuery] int provinceId, [FromQuery] int districtId)
        {
            var weather = new
            {
                Province = "Tỉnh Demo",
                District = "Huyện Demo",
                Temperature = 28.5,
                Humidity = 75,
                WindSpeed = 3.2,
                Description = "Nhiều mây, có mưa rào nhẹ",
                Advice = "Trời có thể mưa, hạn chế phun thuốc sâu vào thời điểm này để tránh bị rửa trôi."
            };
            return Ok(weather);
        }

        [HttpPost("MyAddress")]
        public IActionResult GetWeatherByMyAddress()
        {
            var weather = new
            {
                Province = "Hà Nội",
                District = "Quận Cầu Giấy",
                Temperature = 30.1,
                Humidity = 60,
                WindSpeed = 2.5,
                Description = "Trời nắng nhẹ",
                Advice = "Thời tiết thuận lợi cho việc phun thuốc bảo vệ thực vật."
            };
            return Ok(weather);
        }
    }
}
