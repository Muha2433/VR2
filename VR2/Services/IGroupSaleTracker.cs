using VR2.DTOqMoels;

namespace VR2.Services
{
    public interface IGroupSaleTracker
    {
        void AddRequest(string requestId, DtoGroupRequestSale request);
        void RegisterResponse(string requestId, string ownerId, bool accepted);
        Task MonitorRequestAsync(string requestId);
    }
}
