using VR2.Models;

namespace VR2.Services
{
    public interface IPurchaseService
    {
      Task CreatePurchase(ModelPurchase purChase, List<string> password, int RequestSaleID);
    }
}
