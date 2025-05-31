namespace VR2.Models
{
    public class ModelCustomer : AppUser
    {
        public ModelCustomer() 
        {
            
            lstPropertyOwnership = new HashSet<ModelPropertyOwnership>();
            lstCarOwnership = new HashSet<ModelCarOwnership>();
            
            //imp
            lstShare = new HashSet<ModelShare>();
            lstPurchaseCustomer = new HashSet<ModelPurchaseCustomer>();
            lstClick=new HashSet<ModelClick>();
            lstFavorit = new HashSet<ModelFavorit>();
            //imp



        }

        // Start Important
        public ICollection<ModelShare> lstShare { get; set; }
        public ICollection<ModelPurchaseCustomer> lstPurchaseCustomer { get; set; }
        public ICollection<ModelClick> lstClick { get; set; }
        public ICollection<ModelFavorit> lstFavorit { get; set; }

        
        //End Important

        // public ICollection<ModelShare> lstShareAsSeller { get; set; }
        public ICollection<ModelPropertyOwnership> lstPropertyOwnership { get; set; }
        public ICollection<ModelCarOwnership> lstCarOwnership { get; set; }

    }
}
