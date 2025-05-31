using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelAgentDocument
    {
        public ModelAgentDocument() 
        {
            lstFile=new HashSet<ModelFile>();
        }

        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public ICollection<ModelFile> lstFile { get; set; }

        public string AgentID { get; set; }
        [ForeignKey("AgentID")]
        public ModelAgent? Agent { get; set; }
    }

}
