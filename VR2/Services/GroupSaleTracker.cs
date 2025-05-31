using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VR2.DataBaseServices;
using VR2.DTOqMoels;
using VR2.HubRealTime_Playing;
using VR2.Models;
namespace VR2
{
 public static  class Rr1
    {
        public static readonly Dictionary<string, DtoGroupRequestSale> _requests = new();
    }
}
namespace VR2.Services
{
    public class GroupSaleTracker : IGroupSaleTracker
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IServiceProvider _services;
        private readonly crudRequestSale _crudRequestSale;
        private readonly GroupSaleBackgroundWorker _worker;

        public GroupSaleTracker(IServiceProvider serviceProvider,
             IHubContext<NotificationHub> hubContext,
             crudRequestSale crudRequestSale,
             GroupSaleBackgroundWorker worker
             )
        {
            _worker = worker;
            _services= serviceProvider;
            _crudRequestSale = crudRequestSale;
            _hubContext= hubContext;
        }

        public void AddRequest(string requestId, DtoGroupRequestSale request)
        {
          Rr1._requests[requestId] = request;
          _ = MonitorRequestAsync(requestId);
        }

        //public async Task MonitorRequestAsync(string requestId)
        //{
        //    Console.WriteLine($"[MonitorRequestAsync] Start for RequestId: {requestId} at {DateTime.Now}");

        //    await Task.Delay(TimeSpan.FromSeconds(20));

        //    if (!Rr1._requests.ContainsKey(requestId))
        //    {
        //        Console.WriteLine($"[MonitorRequestAsync] RequestId {requestId} not found");
        //        return;
        //    }

        //    Console.WriteLine($"[MonitorRequestAsync] Found request for RequestId: {requestId} at {DateTime.Now}");

        //    var req = Rr1._requests[requestId];

        //    //var result = await _crudRequestSale.AddGroupRequestForSale(req);

        //    Console.WriteLine($"[MonitorRequestAsync] AddGroupRequestForSale result: {result} at {DateTime.Now}");

        //    if (req.Approvals.Values.All(v => v == true))
        //    {
        //        Console.WriteLine($"Sale Approved for Share {req.ShareId}");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Sale Cancelled for Share {req.ShareId}");
        //    }

        //    Rr1._requests.Remove(requestId);

        //    Console.WriteLine($"[MonitorRequestAsync] End for RequestId: {requestId} at {DateTime.Now}");
        //}
        public async Task MonitorRequestAsync(string requestId)
        {
            Console.WriteLine($"[MonitorRequestAsync] Start for RequestId: {requestId} at {DateTime.Now}");

            await Task.Delay(TimeSpan.FromSeconds(20));

            if (!Rr1._requests.TryGetValue(requestId, out var req))
            {
                Console.WriteLine($"[MonitorRequestAsync] RequestId {requestId} not found");
                return;
            }

            // ✅ نفذ العملية في background worker لحماية DbContext من dispose
            await _worker.ProcessRequestAsync(req);

            if (req.Approvals.Values.All(v => v == true))
            {
                Console.WriteLine($"Sale Approved for Share {req.ShareId}");
            }
            else
            {
                Console.WriteLine($"Sale Cancelled for Share {req.ShareId}");
            }

            Rr1._requests.Remove(requestId);

            Console.WriteLine($"[MonitorRequestAsync] End for RequestId: {requestId} at {DateTime.Now}");
        }

        public void RegisterResponse(string requestId, string ownerId, bool accepted)
        {
            if (!Rr1._requests.TryGetValue(requestId, out var req)) 
                return;
            req.Approvals[ownerId] = accepted;

            if (req.Approvals.Values.All(v => v == true))
            {
               // // All accepted
               // FinalizeSale(req);
               //Rr1._requests.Remove(requestId);
            }
            else if (req.Approvals.Values.Any(v => v == false))
            {
                // Someone rejected
                CancelSale(req);
                Rr1._requests.Remove(requestId);
            }
        }

     


        //   private void FinalizeSale(DtoGroupRequestSale req)
        //   {
        //       //logic to store request sale model
        //var c=   _crudRequestSale.AddGroupRequestForSale(req);
        //    Console.WriteLine($"Sale Approved for Share {req.ShareId}");
        //   }

        private void CancelSale(DtoGroupRequestSale req)
        {
            Console.WriteLine($"Sale Cancelled for Share {req.ShareId}");
        }
      


    }
}
