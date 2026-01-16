using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Services;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [ApiController]
    [Route("api/trainers")]
    public class TrainerController : ControllerBase
    {
        private readonly TrainerService _service;

        public TrainerController(TrainerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string userId, string specialty, string bio)
        {
            var t = await _service.CreateTrainer(userId, specialty, bio);
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _service.GetById(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var t = await _service.GetByUserId(userId);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string specialty)
        {
            return Ok(await _service.Search(specialty));
        }
    }
}