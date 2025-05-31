using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public enum Reqtype
    {
        Property,
        Car
    }

    public class ModelRequestForSale
    {
        public ModelRequestForSale() 
        { 
        //lstAuctionBid = new HashSet<ModelAuctionBid>();
        lstShare = new HashSet<ModelShare>();
        lstClick = new HashSet<ModelClick>();
        lstFavorit = new HashSet<ModelFavorit>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        // Total price for group sales
        public double TotalAskingPrice { get; set; }

        [Required]
        public string SaleType { get; set; } // "Auction" أو "FixedPrice"
    //  public ICollection<ModelAuctionBid> lstAuctionBid { get; set; }
        public ICollection<ModelShare> lstShare { get; set; }//الحصص لكل مالك(بائع(


        public ICollection<ModelClick> lstClick { get; set; }
        public ICollection<ModelFavorit> lstFavorit { get; set; }

        //Navigation Property 
        public ModelPurchase? Purchase { get; set; }
    }
}