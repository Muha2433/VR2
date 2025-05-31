namespace VR2.DTOqMoels
{
    public class FileTuple
    {
        public FileTuple()
        {
            lstRealFile=new HashSet<IFormFile>();
        }
        public ICollection<IFormFile> lstRealFile { get; set; }
        public string FileType { get; set; }// personal Image/ Identity Image / Car Image // Property Image
    }
}
