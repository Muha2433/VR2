using Microsoft.AspNetCore.Identity;

namespace VR2.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser() 
        {
            lstPersonalImage_IdentityImage = new HashSet<ModelFile>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? PublicKey { get; set;}
        public string? PrivateKey { get; set;}
        public bool? Approved { get; set; }
        public bool? Rejected { get; set; }
        public ICollection<ModelFile> lstPersonalImage_IdentityImage { get; set; }


    }
}
