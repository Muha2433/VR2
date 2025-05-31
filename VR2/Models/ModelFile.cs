using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelFile
    {
        [Key] 
        public int ID { get; set; }
        public string? ImageName { get; set; }
        public string fileType { get; set; }
        //Personal Image or Identity Image // Car image // properties image and properties Document image// Agent Document File or Image 
        public int? CarID { get; set; }
        [ForeignKey("CarID")]
        public ModelCar? Car { get; set; }

        public int? PropertyID { get; set; }
        [ForeignKey("PropertyID")]
        public ModelProperties? Property { get; set; }

        public int? AgentDocumentID { get; set; }
        [ForeignKey("AgentDocumentID")]
        public ModelAgentDocument? AgentDocumen { get; set; }
         
        public string? AppUserID { get; set; }
        [ForeignKey("AppUserID")]
        public AppUser? AppUser { get; set; }


    }
}