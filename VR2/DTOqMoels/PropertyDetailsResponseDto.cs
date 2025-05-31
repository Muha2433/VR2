namespace VR2.DTOqMoels
{
    public class PropertyDetailsResponseDto
    {
        public DtoProperty BaseProperty { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<DocumentInfoDto> Documents { get; set; } = new List<DocumentInfoDto>();

        public string Type { get; set; }
        // Type-specific properties (optional - could also handle this on client side)
        public DtoApartment ApartmentDetails { get; set; }
        public DtoVilla VillaDetails { get; set; }
        public DtoHouse HouseDetails { get; set; }
    }
}
