namespace VR2.DTOqMoels
{
    public class ShareOwnerID
    {
        public string OwnerId { get; set; }
        public int ShareID { get; set; }
    }
    public class DtoGroupRequestSale
    {
        public int ShareId { get; set; }
        public int PropertyID { get; set; }
        public string AskerId { get; set; }
        public List<string> OwnerIds { get; set; }
        public Dictionary<string, bool?> Approvals { get; set; }
    }               
    public class DtoIndividualRequestSale
    {
        public int ShareId { get; set; }

        public int PropertyID { get; set; }
        public string AskerId { get; set; }
    }

}