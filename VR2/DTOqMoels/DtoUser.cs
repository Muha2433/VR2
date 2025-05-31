using System.ComponentModel.DataAnnotations;

namespace VR2.DTOqMoels
{
    public class DtoUser
    {
        public DtoUser()
        { 
        lstFileTuple=new HashSet<FileTuple>();
        }
        
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string? Status { get; set; }

        [Required]
        public ICollection<FileTuple> lstFileTuple { get; set; }



    }

}
