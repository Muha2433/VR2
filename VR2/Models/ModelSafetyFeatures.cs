using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelSafetyFeatures
    {
        [Key]
        public int ID { get; set; }

        public int NumberOfAirbags { get; set; } // عدد الوسائد الهوائية
        public bool AreThereAntilockBrakesSystem { get; set; }// فرامل مانعة للانزلاق
        public bool AreThereTirePressureMonitoring { get; set; } // مراقبة ضغط الإطارات
        public bool AreThereBrakeAssist { get; set; } // نظام مساعدة الفرامل

        public int CarID { get; set; }
        [ForeignKey("ModelCarID")]
        public ModelCar? Car { get; set; }
    }

}
