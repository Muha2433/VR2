using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VR2.Models
{
    public class ModelPropertyOwnership
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PropertyID { get; set; }

        [ForeignKey("PropertyID")]
        public ModelProperties? Property { get; set; }

        [Required]
        public string CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public ModelCustomer? Seller { get; set; }

        [Required]
        public double OwnershipPercentage { get; set; } // نسبة الملكية
    }
}
