using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading;
using VR2.Models;
using VR2.Services;

namespace VR2.HubRealTime_Playing
{

    public class CollaborationRequest
    {
        public string GroupId { get; set; }
        public int RequestSaleID { get; set; }
        public string InitiatorId { get; set; }
        public Dictionary<string, (bool HasResponded, bool Accepted, double Percentage, double Price)> Collaborators { get; set; }
    }
    public class PurchaseHub //: Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private static Dictionary<string, CollaborationRequest> ActiveCollaborations = new();
        private static Dictionary<string, CancellationTokenSource> CollaborationTimeouts = new();
        private readonly IPurchaseService _purchaseService;
        //End Parameter
        public PurchaseHub(ApplicationDbContext dbContext, IPurchaseService purchaseService)
        {
            _dbContext = dbContext;
            _purchaseService = purchaseService;
        }
        //End Construcor

        public async Task StartCollaborativePurchase (string initiatorID , ICollection<string> collaboratorIds ,
            int requestSaleId , double initiatorPercentage , double initiatorPrice)
        {
            string groupId = Guid.NewGuid().ToString();

            var collab = new CollaborationRequest
            {
                GroupId = groupId,
                RequestSaleID = requestSaleId,
                InitiatorId = initiatorID,
                Collaborators = collaboratorIds.ToDictionary(id => id, id => (HasResponded: false, Accepted: false, Percentage: 0.0, Price: 0.0 ))
            };

            collab.Collaborators[initiatorID] = (true,true,initiatorPercentage,initiatorPrice);

            ActiveCollaborations[groupId] = collab;

            // Send SignalR invites
            foreach (var buyerId in collaboratorIds)
            {
              //  await Clients.User(buyerId).SendAsync("ReceiveInvite", groupId, requestSaleId);
            }
            // إعداد مؤقت لمدة 60 دقيقة
            var cancellationTokenSource = new CancellationTokenSource();
            CollaborationTimeouts[groupId] = cancellationTokenSource;
            _ = Task.Delay(TimeSpan.FromMinutes(60), cancellationTokenSource.Token)
                .ContinueWith(async t =>
                {
                    if (!t.IsCanceled)
                    {
                        await HandleTimeout(groupId);
                    }
                });

        }

        private async Task HandleTimeout(string groupId)
        {
            if (!ActiveCollaborations.TryGetValue(groupId, out var collaboration))
                { throw new Exception("Collaboration request not found."); }

            foreach (var key in collaboration.Collaborators.Keys.ToList())
            {
                if (!collaboration.Collaborators[key].HasResponded)
                {
                    collaboration.Collaborators[key]=(true,false,0.0,0.0);
                }
            }
        }

        private async Task CompletePurchaseOrFail(string groupId, CollaborationRequest collaboration)
        {
            if (collaboration.Collaborators.Values.All(e => e.Accepted))
            {
                // تحقق من صحة النسب والأسعار
                var totalPercentage =  collaboration.Collaborators.Values.Sum(c => c.Percentage);
                var totalPrice      = collaboration.Collaborators.Values.Sum(t => t.Price);

                var requestSale = await _dbContext.RequestForSales.Include(r=>r.lstShare).FirstOrDefaultAsync(e=>e.ID==collaboration.RequestSaleID);

                if (requestSale == null)
                {
                  //  await Clients.User(collaboration.InitiatorId).SendAsync("CollaborativePurchaseComplete", false, "Sale request not found.");
                    ActiveCollaborations.Remove(groupId);
                    return;
                }

                var originalSharePrice = requestSale.TotalAskingPrice;
                //100% originalSharePercantage

            }
        }

        public async Task SubmitCollaboratorDecision(string groupId,string buyerId, bool accepted, double percentage, double price)
        {
            if (!ActiveCollaborations.ContainsKey(groupId)) { throw new Exception("Collaboration request not found."); }
            
            var collaboration = ActiveCollaborations[groupId];

            if (!collaboration.Collaborators.ContainsKey(buyerId)) { throw new Exception("User is not part of this collaboration.");}

            collaboration.Collaborators[buyerId] = (true,accepted,percentage,price);

            // Check if all collaborators have responded
            if (collaboration.Collaborators.Values.All(a => a.HasResponded))
            {
                // Check if all collaborators has accepted 
                if (collaboration.Collaborators.Values.All(a => a.Accepted))
                {

                }
            }

        }


    }
}
