using CommunicationAPI.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CommunicationAPI.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _objTracker;

        public PresenceHub(PresenceTracker objTracker)
        {
            _objTracker = objTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _objTracker.UserConnected(Context.User.getUserName(), Context.ConnectionId);
            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.getUserName());
            }
            var currentUsers = await _objTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var isOffline = await _objTracker.UserDisconnected(Context.User.getUserName(), Context.ConnectionId);

            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.getUserName());

            //var currentUsers = await _objTracker.GetOnlineUsers();
            //await Clients.All.SendAsync("GetOnlineUsers", currentUsers);


            await base.OnDisconnectedAsync(ex);
        }
    }
}
