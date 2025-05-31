using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelPurchase
    {
        public ModelPurchase() 
        {
            lstPurchaseCustomer = new HashSet<ModelPurchaseCustomer>();
        }

        [Key]
        public int Id { get; set; }

        public int RequestForSaleID { get; set; }
        
        [ForeignKey("RequestForSaleID")]
        public ModelRequestForSale? RequestForSale { get; set; }

        public ICollection<ModelPurchaseCustomer>? lstPurchaseCustomer { get; set; }




    }

}
