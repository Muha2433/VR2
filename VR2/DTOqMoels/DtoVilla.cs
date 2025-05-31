using System.ComponentModel.DataAnnotations;

namespace VR2.DTOqMoels
{
    public class DtoVilla : DtoProperty
    {

        [Required]
        public int NumberOfFloor { get; set; }
        public double? GardenArea { get; set; }
        public bool ISThereSwimmingPool { get; set; }
        public bool IsThereGarage { get; set; }
    }

}
