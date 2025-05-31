using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VR2.Models
{
    public class ModelVilla : ModelProperties
    {
        [Required]
        public int NumberOfFloor { get; set; }
        public double? GardenArea { get; set; }
        public bool ISThereSwimmingPool { get; set; }
        public bool IsThereGarage { get; set; }
    }
}

//GardenArea(in square meters)
//Swimming Pool(Yes/No)
//Garage(Yes / No)
//NumberOfFloor