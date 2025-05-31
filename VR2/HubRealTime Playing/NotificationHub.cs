using Microsoft.AspNetCore.SignalR;
using VR2.Services;

namespace VR2.HubRealTime_Playing
{
    public class NotificationHub : Hub
    {
        private readonly IGroupSaleTracker _tracker;

        public NotificationHub(IGroupSaleTracker tracker)
        {
            _tracker = tracker;
        }

        public Task RespondToSaleRequest(string requestId, bool accepted)
        {
            var userId = Context.UserIdentifier;
            _tracker.RegisterResponse(requestId, userId, accepted);
            return Task.CompletedTask;
        }
    }
}
