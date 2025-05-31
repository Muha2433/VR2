using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace VR2.HubRealTime_Playing
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
