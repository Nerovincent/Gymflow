using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using GymFlow_Backend_Phase4E.Services;
using GymFlow_Backend_Phase4E.Hubs;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _service;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IWebHostEnvironment _env;

        public ChatController(ChatService service, IHubContext<ChatHub> hub, IWebHostEnvironment env)
        {
            _service = service;
            _hub = hub;
            _env = env;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromForm] string senderId, [FromForm] string receiverId, [FromForm] string? message, [FromForm] IFormFile? image)
        {
            string? imageUrl = null;

            // Handle image upload
            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "chat");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/chat/{uniqueFileName}";
            }

            // Save message with optional image
            var msg = await _service.SendMessage(senderId, receiverId, message, imageUrl);

            // Send via SignalR
            await _hub.Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                senderId,
                message,
                imageUrl,
                timestamp = msg.SentAt
            });

            return Ok(msg);
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(string user1, string user2)
        {
            return Ok(await _service.GetConversation(user1, user2));
        }

        [HttpGet("unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            var count = await _service.GetUnreadCount(userId);
            return Ok(new { count });
        }

        [HttpGet("unread-by-user/{userId}")]
        public async Task<IActionResult> GetUnreadCountsByUser(string userId)
        {
            var counts = await _service.GetUnreadCountsByUser(userId);
            return Ok(counts);
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            await _service.MarkAsRead(request.UserId, request.OtherUserId);
            return Ok();
        }
    }

    public class MarkAsReadRequest
    {
        public string UserId { get; set; } = "";
        public string OtherUserId { get; set; } = "";
    }
}