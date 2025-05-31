namespace VR2.DTOqMoels
{
    
    public class RequestSaleDto
    {
        public string BrokeredBy { get; set; }
        public string status { get; set; }
        public double price { get; set; }
        public int numberBedRooms { get; set; }
        public int numberBathRoom { get;set; }
        public string? street { get; set; }
        public double? acres { get; set; }//كم فدان 
        public string city { get; set; }
        public string ZipCode { get; set; } // الرمز البريدي
        public string? state { get; set; }
        public double houseSize { get; set; }
        public DateTime? previouslySoldDate { get; set; }
    }
}
