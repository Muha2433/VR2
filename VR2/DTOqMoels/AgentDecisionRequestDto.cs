namespace VR2.DTOqMoels
{
    public class AgentDecisionRequestDto
    {
        public AgentDecisionRequestDto() 
        { 
            lstShares=new HashSet<DtoShare>();
        }
        public int PropertyID { get; set; }
        public string Decision { get; set; } // "accept" or "reject"
        public ICollection<DtoShare>? lstShares { get; set; } 
        public string ?Cause { get; set; }
    }

}
