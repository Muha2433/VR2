namespace VR2.DTOqMoels
{
    public class customerProfileDto
    {
        public customerProfileDto()
        {
            lstShareDtoMobile=new HashSet<ShareDtoMobile>();
        }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    
        public string LastName { get; set; }

        public string ImageUrl {  get; set; }

        public ICollection<ShareDtoMobile>   lstShareDtoMobile { get; set; }
    }
}
