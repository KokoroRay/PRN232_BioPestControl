using Microsoft.AspNetCore.Mvc;
using agri_expert_service.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace agri_expert_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgriCalculationsController : ControllerBase
    {
        private readonly AgriDbContext _context;

        public AgriCalculationsController(AgriDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalculationsData()
        {
            var chemicals = await _context.ChemicalProfiles
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name + " (" + c.VietnameseName + ")",
                    imageUrl = "" // ChemicalProfile does not have imageUrl currently
                })
                .ToListAsync();

            var crops = new[]
            {
                new { id = 1, name = "Lúa (Rice)" },
                new { id = 2, name = "Ngô (Corn)" },
                new { id = 3, name = "Cà phê (Coffee)" },
                new { id = 4, name = "Xoài (Mango)" },
                new { id = 5, name = "Rau cải (Cabbage)" }
            };

            var pests = new[]
            {
                new { id = 1, name = "Sâu cuốn lá (Leaf Folder)" },
                new { id = 2, name = "Rầy nâu (Brown Planthopper)" },
                new { id = 3, name = "Bệnh đạo ôn (Blast Disease)" },
                new { id = 4, name = "Bệnh khô vằn (Sheath Blight)" },
                new { id = 5, name = "Ốc bươu vàng (Golden Apple Snail)" }
            };

            return Ok(new
            {
                products = chemicals,
                crops = crops,
                pests = pests,
                dataWarning = (string)null
            });
        }

        [HttpPost("Dosage")]
        public async Task<IActionResult> CalculateDosage([FromBody] DosageRequest request)
        {
            var chemical = await _context.ChemicalProfiles.FindAsync(request.ProductId);
            if (chemical == null)
            {
                return BadRequest(new { Message = "Sản phẩm không tồn tại." });
            }

            // Mock basic calculation: Dosage rate is 0.5 ml (or g) per m2
            double baseRatePerM2 = 0.5;
            
            double totalProductNeededMl = request.AreaSize * baseRatePerM2;
            
            // Assume 1 tank (of given capacity) can cover ~10m2 per liter.
            // E.g. 16L tank -> covers 160m2.
            double tankCoverageArea = request.TankCapacity * 10;
            if (tankCoverageArea <= 0) tankCoverageArea = 200; // fallback

            double numberOfTanksDouble = request.AreaSize / tankCoverageArea;
            int numberOfTanks = (int)Math.Ceiling(numberOfTanksDouble);
            
            double amountPerTankMl = totalProductNeededMl / (numberOfTanks > 0 ? numberOfTanks : 1);

            return Ok(new
            {
                dosageResult = new
                {
                    productName = chemical.Name,
                    dosageRate = $"{baseRatePerM2} ml/m²",
                    totalProductNeeded = $"{totalProductNeededMl:F2} ml",
                    numberOfTanks = numberOfTanks,
                    amountPerTank = $"{amountPerTankMl:F2} ml"
                }
            });
        }

        [HttpPost("Mixability")]
        public async Task<IActionResult> CheckMixability([FromBody] MixabilityRequest request)
        {
            if (request.MixProductIds == null || request.MixProductIds.Count < 2)
            {
                return BadRequest(new { Message = "Vui lòng chọn ít nhất 2 sản phẩm để kiểm tra." });
            }

            var chemicals = await _context.ChemicalProfiles
                .Where(c => request.MixProductIds.Contains(c.Id))
                .ToListAsync();

            bool isSafe = true;
            var warnings = new List<string>();

            if (chemicals.Count > 3)
            {
                isSafe = false;
                warnings.Add("Không nên trộn quá 3 loại hóa chất cùng lúc.");
            }

            var hasHerbicide = chemicals.Any(c => c.ChemicalGroup != null && c.ChemicalGroup.Contains("diệt cỏ", StringComparison.OrdinalIgnoreCase));
            if (hasHerbicide && chemicals.Count > 1)
            {
                isSafe = false;
                warnings.Add("Thuốc diệt cỏ thường không được khuyến cáo trộn chung với thuốc trừ sâu hoặc thuốc trừ bệnh.");
            }

            if (chemicals.Count(c => c.ToxicityLevel == "Ia" || c.ToxicityLevel == "Ib" || c.ToxicityLevel == "II") >= 2)
            {
                warnings.Add("Thận trọng: Hỗn hợp chứa nhiều hóa chất có độc tính cao, cần trang bị bảo hộ kỹ lưỡng.");
            }

            return Ok(new
            {
                mixabilityResult = new
                {
                    isSafe = isSafe,
                    warnings = warnings
                }
            });
        }
    }

    public class DosageRequest
    {
        public int ProductId { get; set; }
        public int CropId { get; set; }
        public int PestId { get; set; }
        public double AreaSize { get; set; }
        public double TankCapacity { get; set; }
    }

    public class MixabilityRequest
    {
        public List<int> MixProductIds { get; set; }
    }
}
