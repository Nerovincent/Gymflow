using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GymFlow_Backend_Phase4E.Constants;

namespace GymFlow_Backend_Phase4E.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        [Authorize(Roles = Roles.Instructor)]
        public async Task SendMessageToClient(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier!;
            await Clients.User(receiverId)
                .SendAsync("ReceiveMessage", senderId, message);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task AdminBroadcast(string message)
        {
            await Clients.All.SendAsync("AdminMessage", message);
        }
    }
}
