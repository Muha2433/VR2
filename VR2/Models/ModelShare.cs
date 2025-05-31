using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VR2.Models
{
    public class ModelShare
    {
        public ModelShare()
        {
           // lstShareResale = new HashSet<ModelShareResale>();
           // lstGroupResaleMembers = new HashSet<ModelGroupResaleMembers>();
        }

        [Key]
        public int ID { get; set; }
        
        public int? RequestID { get; set; }

        [ForeignKey("RequestID")]
        public ModelRequestForSale? Request { get; set; }
        

        // Owner Of Share
        public string? InternalOwnerID { get; set; }
        [ForeignKey("InternalOwnerID")]
        public ModelCustomer? InternalOwner { get; set; } // seller
        public string? ExternalOwnerID { get; set; }// External Owner is seller
        [ForeignKey("ExternalOwnerID")]
        public ModelExternalOwner? ExternalOwner { get; set; }//  قد يكون مالك الحصة غير مسجل في الموقع ويريد المشتري شراء العقار كاملا
        //  

        //
        [Required]
        public double SharePercentageOfWholePropert { get; set; }
        
        [Required]
        public double SharePrice { get; set; }
        
        public bool IsAvailableForResale { get; set; } = true;// اذا اردت البيع اجعلها صحيح وان لم ارد مثلا اشتريت وانتهيت ولا اريد تاالبيع اجعلها خطأ وهي مفيدة جدا واتوقع ان استفيد منها في حالة البيع الجماعي

        public bool IsExternalOwner { get; set; } = false; // في حالة كان المالك غير مسجل في النظام
         

        // Add purchase price for individual share transactions
        public double? PurchasePrice { get; set; }


        public int PropertyID { get; set; }

        [ForeignKey("PropertyID")]
        public ModelProperties Property { get; set; }

        // Navigation Properties
        //  public ICollection<ModelShareResale> lstShareResale { get; set; }

        //public ICollection<ModelGroupResaleMembers> lstGroupResaleMembers { get; set; }
    }
}
