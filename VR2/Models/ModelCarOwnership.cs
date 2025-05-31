using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VR2.Models
{
    public class ModelCarOwnership
    {

        [Key]
        public int ID { get; set; }

        [Required]
        public int CarID { get; set; }

        [ForeignKey("CarID")]
        public ModelCar Car { get; set; }

        [Required]
        public string CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public ModelCustomer? Customer { get; set; }

        [Required]
        public double OwnershipPercentage { get; set; } // نسبة الملكية
    }
}
