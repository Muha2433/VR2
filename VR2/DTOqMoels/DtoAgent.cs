using System.ComponentModel.DataAnnotations;

namespace VR2.DTOqMoels
{
    public class DtoAgent: DtoUser
    {
        [Required]
        public string OfficeName { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public ICollection<FileTuple> lstDocumenty { get; set; }
    }
}
