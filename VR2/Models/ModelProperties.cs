using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VR2.Models
{
    public  class ModelProperties
    {
        public ModelProperties()
        {
            // lstPropertyOwnership = new HashSet<ModelPropertyOwnership>();
            lstShare=new HashSet<ModelShare>();
             lstPropertiesImage_Document = new HashSet<ModelFile>();
        }
        [Key]
       
        public int ID { get; set; }

        public  string PropertyCode { get; set; }

        public bool Accepted { get; set; }

        public bool Rejected { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public double SpaceInSquareMeter { get; set; }

        [Required]
        public DateTime YearOfBuilt { get; set; }

        [Required]
        public bool Furnished { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]

        public int LocationID { get; set; }
        [ForeignKey("LocationID")]
        public ModelLocation? Location { get; set; }

        public double? acres { get; set; }//كم فدان 
        public DateTime? previouslySoldDate { get; set; }

        public ICollection<ModelShare> lstShare { get; set; }
        // public ICollection<ModelPropertyOwnership> lstPropertyOwnership { get; set; }
        public ICollection<ModelFile> lstPropertiesImage_Document { get; set; }


    }

}

//Number of Bedrooms
//Number of Bathrooms
//Property Space (in square meters)
//Year Built
//Furnished (Yes/No)
//Price
//Location






