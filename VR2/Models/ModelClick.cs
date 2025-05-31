using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelClick
    {
        [Key]
        public int ID { get; set; }
        public DateTime ClickDate { get; set; }
        public TimeSpan Duration { get; set; }

        //one user with multiple click
        public string CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public ModelCustomer? Customer { get; set; }

        //one request sale
        public int RequestID { get; set; }
        [ForeignKey("RequestID")]
        public ModelRequestForSale? RequestForSale { get; set; }
    }
}
