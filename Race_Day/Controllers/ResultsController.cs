using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Race_Day.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/results
        [HttpGet]
        public async Task<IActionResult> GetAllResults()
        {
            var results = await _context.Results
                .Include(r => r.Race)
                .Include(r => r.Driver)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/results/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResultById(int id)
        {
            var result = await _context.Results
                .Include(r => r.Race)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null)
                return NotFound($"Result with Id {id} not found");

            return Ok(result);
        }

        // GET: api/results/race/5
        [HttpGet("race/{raceId}")]
        public async Task<IActionResult> GetResultsByRace(int raceId)
        {
            var results = await _context.Results
                .Where(r => r.RaceId == raceId)
                .Include(r => r.Driver)
                .OrderBy(r => r.Position)
                .ToListAsync();

            if (!results.Any())
                return NotFound($"No results found for Race Id {raceId}");

            return Ok(results);
        }

        // GET: api/results/driver/5
        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetResultsByDriver(int driverId)
        {
            var results = await _context.Results
                .Where(r => r.DriverId == driverId)
                .Include(r => r.Race)
                .OrderByDescending(r => r.Race.RaceDate)
                .ToListAsync();

            if (!results.Any())
                return NotFound($"No results found for Driver Id {driverId}");

            return Ok(results);
        }

        // POST: api/results
        [HttpPost]
        public async Task<IActionResult> AddNewResult(NewResultDTO nrDTO)
        {
            if (nrDTO == null)
                return BadRequest("Result data is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify race and driver exist
            var raceExists = await _context.Races.AnyAsync(r => r.Id == nrDTO.RaceId);
            if (!raceExists)
                return BadRequest($"Race with Id {nrDTO.RaceId} does not exist");

            var driverExists = await _context.Drivers.AnyAsync(d => d.Id == nrDTO.DriverId);
            if (!driverExists)
                return BadRequest($"Driver with Id {nrDTO.DriverId} does not exist");

            var newResult = new Result
            {
                RaceId = nrDTO.RaceId,
                DriverId = nrDTO.DriverId,
                Position = nrDTO.Position,
                FinishTime = nrDTO.FinishTime,
                Points = nrDTO.Points,
                Status = nrDTO.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Results.Add(newResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetResultById), new { id = newResult.Id }, newResult);
        }

        // PUT: api/results/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(int id, NewResultDTO nrDTO)
        {
            if (nrDTO == null)
                return BadRequest("Result data is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingResult = await _context.Results.FindAsync(id);
            if (existingResult == null)
                return NotFound($"Result with Id {id} not found");

            existingResult.Position = nrDTO.Position;
            existingResult.FinishTime = nrDTO.FinishTime;
            existingResult.Points = nrDTO.Points;
            existingResult.Status = nrDTO.Status;
            existingResult.UpdatedAt = DateTime.UtcNow;

            _context.Results.Update(existingResult);
            await _context.SaveChangesAsync();

            return Ok(existingResult);
        }

        // DELETE: api/results/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
                return NotFound($"Result with Id {id} not found");

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Result with Id {id} deleted successfully" });
        }
    }
}
