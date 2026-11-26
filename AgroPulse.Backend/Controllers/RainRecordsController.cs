using AgroPulse.Abstractions.DTOs.RainRecords;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroPulse.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/lots/{lotId:int}/rain-records")]
    public class RainRecordsController(IRainRecordService _rainService) : ControllerBase
    {
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary(int lotId)
        {
            var summary = await _rainService.GetLotRainSummaryAsync(lotId, GetCurrentUserId());
            if (summary == null) return NotFound(new { message = "Lote no encontrado." });

            return Ok(summary);
        }

        [HttpPost]
        public async Task<IActionResult> AddManualRecord(int lotId, [FromBody] CreateRainRecordDto dto)
        {
                var created = await _rainService.AddManualRainAsync(lotId, GetCurrentUserId(), dto);
                if (created == null) return NotFound(new { message = "Lote no encontrado." });

                return Ok(created);
        }

        [HttpDelete("{recordId:int}")]
        public async Task<IActionResult> DeleteRecord(int lotId, int recordId) 
        { 
            var deleted = await _rainService.DeleteRainRecordAsync( recordId, GetCurrentUserId(), lotId);
            if (!deleted) return NotFound(new { message = "Registro o lote no encontrado." });

            return NoContent();
        }
    }
}
