using System.ComponentModel.DataAnnotations;

namespace VR2.Models
{
    public class ModelAgent :AppUser
    {
        public ModelAgent()
        {
            SubmissionDate = DateTime.Now;
            lstAgentDocument=new HashSet<ModelAgentDocument>();
        }

        [Required]
        public string OfficeName { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public DateTime SubmissionDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public ICollection<ModelAgentDocument> lstAgentDocument { get; set; }
    }
}
