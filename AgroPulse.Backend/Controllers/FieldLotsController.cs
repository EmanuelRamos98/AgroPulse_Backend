using AgroPulse.Abstractions.DTOs.Lost;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroPulse.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/lots")]
    public class FieldLotsController(IFieldLotService _lotServices, IAgroAnalyticsService _analyticsService) : ControllerBase
    {
        //Temporal
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lots = await _lotServices.GetUserLotsAsync(GetCurrentUserId());
            if (lots.Count() > 0)
                return Ok(lots);

            return NotFound(new { message = "Lotes no encontrados." });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lot = await _lotServices.GetLotByIdAsync(id, GetCurrentUserId());
            if (lot is null) return NotFound(new { message = "Lote no encontrado." });

            return Ok(lot);
        }

        [HttpGet("{id:int}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            var dashboard = await _analyticsService.GetLotDashboardDtoAsync(id, GetCurrentUserId());
            if (dashboard is null) return NotFound(new { message = "Lote no encontrado." });

            return Ok(dashboard);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveFieldLotDto dto)
        {
                var created = await _lotServices.CreateLotAsync(GetCurrentUserId(), dto);
                return CreatedAtAction(nameof(GetById), new {id=created.Id}, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFieldLotDto dto)
        {
                var updated = await _lotServices.UpdateLotAsync(id, GetCurrentUserId(), dto);
                if (!updated) return NotFound(new { message = "Lote no encontrado." });

                return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _lotServices.DeleteLotAsync(id, GetCurrentUserId());
            if (!deleted) return NotFound(new { message = "Lote no encontrado." });

            return NoContent();
        }
    }
}
