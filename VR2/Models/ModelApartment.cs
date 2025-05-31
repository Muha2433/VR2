namespace VR2.Models
{
    public class ModelApartment : ModelProperties
    {
        public int FloorNumber { get; set; }

        public int? ApartmentNumber { get; set; }
        public string BuildingName { get; set; } = "";
        public bool IsThereParkingSpace { get; set; } //مكان لركن السيارة
      
    }
}
