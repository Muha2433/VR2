namespace VR2.DTOqMoels
{
    public class DtoRequestForSale
    {
        public DtoRequestForSale() 
        {
        lstDtoShare=new HashSet<DtoShare>();
        }
        public DateTime? RequestDate { get; set; }
        public string SaleType { get; set; }

        public ICollection<DtoShare> lstDtoShare { get; set; }

        // Total price for group sales
        public double TotalAskingPrice { get; set; }
    }
}
