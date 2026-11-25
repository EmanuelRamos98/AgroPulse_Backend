using AgroPulse.Abstractions.DTOs.RainRecords;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgroPulse.Backend.Controllers
{
    [Route("api/lots/{lotId:int}/rain-records")]
    [ApiController]
    public class RainRecordsController(IRainRecordService _rainService) : ControllerBase
    {
        private int GetCurrentUserId() => 1;

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
            try
            {
                var created = await _rainService.AddManualRainAsync(lotId, GetCurrentUserId(), dto);
                if (created == null) return NotFound(new { message = "Lote no encontrado." });

                return Ok(created);
            }
            catch (ArgumentException _ex)
            {
                return BadRequest(new { message = _ex.Message });
            }
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
