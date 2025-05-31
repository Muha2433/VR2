using System.ComponentModel.DataAnnotations;

namespace VR2.DTOqMoels
{
    public class DtoShare
    {
        public int? ID { get; set; }

        //public string Type { get; set; }// car or property
        //Owner
        public string? Internal_Seller_ID { get; set; }
        public string? External_Seller_ID { get; set; }
        //

        //نسبة الملكية
        [Range(0.01, 100)]
        public double SharePercentageOfWholePropert { get; set; }

  
        public double ?SharePrice { get; set; }
        //
        public bool? IsAvailableForResale { get; set; } // اذا اردت البيع اجعلها صحيح وان لم ارد مثلا اشتريت وانتهيت ولا اريد تاالبيع اجعلها خطأ وهي مفيدة جدا واتوقع ان استفيد منها في حالة البيع الجماعي

        public bool? IsExternalOwner { get; set; }

        // Add purchase price for individual share transactions
        public double? PurchasePrice { get; set; }
    }
}


