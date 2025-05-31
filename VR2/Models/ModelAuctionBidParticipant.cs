using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelAuctionBidParticipant
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int BidID { get; set; }

        [ForeignKey("BidID")]
        public ModelAuctionBid? Bid { get; set; }

        [Required]
        public string BuyerID { get; set; }

        public ModelCustomer ?Buyer { get; set; }

        [Required]
        public double ContributionAmount { get; set; }

    }
}
