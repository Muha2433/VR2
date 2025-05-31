using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelCar
    {
        public ModelCar() 
        {
            lstCarOwnership = new HashSet<ModelCarOwnership>();
            lstCarImages = new HashSet<ModelFile>();
        }

        [Key]
        public int ID { get; set; } // Primary Key
        public string  ChassisNumber { get; set; } // Unique رقم الشاسيه
        public string Make { get; set; } //العلامة التجارية مثلا تويوتا
        public string Model { get; set; }//الموديل مثلا كورولا
        public int YearOfMaking { get; set; }//سنة التصنيع
        public string Color { get; set; }// لون السيارة
        public int Mileage { get; set; } // المسافة المقطوعة
        public double Price { get; set; } // سعر السيارة
        public string FuelType { get; set; } //  نوع الوقود بنزين ام مازوت ام كهرباء
        public string EngineType { get; set; }// نوع المحرك مثلا سلندر
        public int Horsepower { get; set; }// القوة مقاسة بالحصان
        public int NumberOfDoors { get; set; }//عدد الابواب
        public int NumberOfSeats { get; set; }//عدد المقاعد
        public string DriveType { get; set; }// نوع الدفع مثلا دفع رباعي او امامي
        public string TransmissionType { get; set; }// ناقل الحركة يدوي ام اوتوماتيكي نوعه
        public bool IsNew { get; set; } // هل السيارة جديدة ام مستعملة

        public string ?Status { get; set; } // Pending, Accepted, Rejected حالة طلب البيع للسيارة نفسها

        // Navigation Properties
        public ModelFeatures Features { get; set; }
        public ModelSafetyFeatures SafetyFeatures { get; set; }

        public int RequsetSaleID { get; set; }
        [ForeignKey("RequsetSaleID")]

        public ICollection<ModelCarOwnership> lstCarOwnership { get; set; }

        public ICollection<ModelFile> lstCarImages { get; set; }
    }
}
