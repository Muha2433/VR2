using System.ComponentModel.DataAnnotations;

namespace VR2.DTOqMoels
{
    public class DtoExternalOwner
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DtoContactInfo ContactInfo { get; set; }
    }
}
