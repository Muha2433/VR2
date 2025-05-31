namespace VR2.DTOqMoels
{
    public class ShareDtoMobile
    {
        public int Id { get; set; }
        public double sharePrice { get; set; }
        
        public double  Percentage { get; set; }
        public bool IsAvailableForResale { get; set; } = true;// اذا اردت البيع اجعلها صحيح وان لم ارد مثلا اشتريت وانتهيت ولا اريد تاالبيع اجعلها خطأ وهي مفيدة جدا واتوقع ان استفيد منها في حالة البيع الجماعي

    }
}
