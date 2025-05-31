using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelContactInfo
    {
        public ModelContactInfo() {
        
            lstEmail=new HashSet<string>();
            lstPhoneNumber=new HashSet<string>();
        }

        [Key]
        public int ID { get; set; }

        [EmailAddress]
        public ICollection<string>? lstEmail { get; set; }

        public ICollection<string>? lstPhoneNumber { get; set; }

        //Navigation Properties
        public ModelExternalOwner ExternalOwner { get; set; } 

    }
}
