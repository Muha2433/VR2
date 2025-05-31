using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelFeatures
    {
        [Key]
        public int ID { get; set; }
        public bool IsThereAirConditioning { get; set; }// تكييف 
        public bool IsThereBlindSpotMonitoring { get; set; }// مراقية النقاط العمياء مثل الناطق التي لا يقدر السائق على رؤيتها مثل السيارات الجانبية
        public bool IsThereCruiseControl { get; set; }//يستطيع السائق رفع قدمه عن المكابح وتبيث السرعة التي كان يسير عليها
        public bool IsThereNavigationSystem { get; set; }// نظام ملاحة
        public bool AreTherePowerWindows { get; set; }// هل يتم رفع الشباك عن طريق ور ام تدوير يدوي

        public string AudioSystem { get; set; }// نوع نظام الصوت


        public int CarID { get; set; } 
        [ForeignKey("ModelCarID")]
        public ModelCar? Car { get; set; }
    }

}
//    < label for= "audioSystem" > نظام الصوت:</ label >
//< select id = "audioSystem" name = "audioSystem" >
//    < option value = "basic" > نظام صوتي أساسي </ option >
//    < option value = "medium" > نظام صوتي متوسط </ option >
//    < option value = "high" > نظام صوتي عالي الجودة </ option >
//    < option value = "premium" > نظام صوتي متميز </ option >
//</ select >
/* شرح النظام الصوتي
نظام صوتي متميز
يُعتبر النظام الصوتي المتميز الأكثر تطورًا، حيث يتضمن را
ديو AM/FM، مشغل أقراص CD/DVD، اتصال بلوتوث، مقبس AUX وUSB، ودعم لتطبيقات الصوت مثل Apple CarPlay وAndroid Auto. بالإضافة إلى ذلك، يتميز هذا النظام بمكبرات صوت عالية الجودة (8 مكبرات أو أكثر)، مضخم صوت مدمج، ونظام صوت ساروند (Surround Sound) الذي يوفر تجربة صوتية غامرة. هذا النظام مثالي للسيارات الفاخرة جدًا التي تهدف إلى توفير أعلى مستويات الراحة والترفيه.
 */