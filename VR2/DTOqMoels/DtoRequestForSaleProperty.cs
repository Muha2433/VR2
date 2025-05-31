namespace VR2.DTOqMoels
{
    public class DtoRequestForSaleProperty : DtoRequestForSale
    {
       
        public int PropertyID { get; set; }
        public string? Status { get; set; } // Pending, Accepted, Rejected حالة طلب البيع
    }
}
