using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelPurchaseCustomer
    {
        [Key]
        public int Id { get; set; }

        public int PurchaseId { get; set; }
        [ForeignKey("PurchaseId")]
        public ModelPurchase? Purchase { get; set; }


        public string BuyerID { get; set; }
        [ForeignKey("BuyerID")]
        public ModelCustomer Buyer { get; set; }

        public double Percentage { get; set; }
        public double Price { get; set; }

    }
}
