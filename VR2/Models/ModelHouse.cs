namespace VR2.Models
{
    public class ModelHouse : ModelProperties
    {
        public bool Backyard { get; set; }// هل يوجد فناء خلفي للمنزل
        public bool ISThereSwimmingPool { get; set; }
        public bool IsThereGarage { get; set; }
        public int NumberOfFloor { get; set; }
        public double? GardenArea { get; set; } // يمكن أن تكون null إذا لم يكن هناك حديقة
    }

}
