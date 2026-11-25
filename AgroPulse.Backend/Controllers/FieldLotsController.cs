using AgroPulse.Abstractions.DTOs.Lost;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgroPulse.Backend.Controllers
{
    [Route("api/lots")]
    [ApiController]
    public class FieldLotsController(IFieldLotService _lotServices, IAgroAnalyticsService _analyticsService) : ControllerBase
    {
        //Temporal
        private int GetCurrentUserId() => 1;

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
            try 
            { 
                var created = await _lotServices.CreateLotAsync(GetCurrentUserId(), dto);
                return CreatedAtAction(nameof(GetById), new {id=created.Id}, created);
            }
            catch(ArgumentException _ex)
            {
                return BadRequest(new {message= _ex.Message});
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFieldLotDto dto)
        {
            try
            {
                var updated = await _lotServices.UpdateLotAsync(id, GetCurrentUserId(), dto);
                if (!updated) return NotFound(new { message = "Lote no encontrado." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
