using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VR2.Models
{
    public class ModelAuctionBid
    {
        public ModelAuctionBid() 
        {
            lstParticipants = new HashSet<ModelAuctionBidParticipant>();
        }

        [Key]
        public int BidID { get; set; }

        [Required]
        public int RequestID { get; set; }

        [ForeignKey("RequestID")]
        public ModelRequestForSale? RequestForSale { get; set; }

        [Required]
        public double BidAmount { get; set; }

        [Required]
        public DateTime BidTime { get; set; }

        public bool IsWinningBid { get; set; } = false;

        public ICollection<ModelAuctionBidParticipant> lstParticipants { get; set; }


    }

}
