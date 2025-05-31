namespace VR2.DTOqMoels
{
    public class PropertyDecisionDto
    {
        public PropertyDecisionDto() 
        {
            ImageUrls = new HashSet<string>();
        }
        public ICollection<string> ImageUrls { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string ?DisplayImage { get; set; } // Added for frontend convenience
    }
}
