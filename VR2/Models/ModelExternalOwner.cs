using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VR2.Models
{
    public class ModelExternalOwner
    {
        public ModelExternalOwner()
        {
            lstShareAsSeller=new HashSet<ModelShare>();
        }

        [Key]
        public string ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public int ContactInfoID { get; set; }
        [ForeignKey("ContactInfoID")]
        public ModelContactInfo? ContactInfo { get; set; }



        public ICollection<ModelShare> lstShareAsSeller { get; set; }

    }

}
