using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VR2.Models;

namespace VR2.DTOqMoels
{
    public class DtoProperty
    {
        public DtoProperty() 
        {
            lstShare = new HashSet<DtoShare>();
            lstPropertiesImage_Document = new HashSet<FileTuple>();
        }
        public int? ID { get; set; }

        [Required]
        public string PropertyCode { get; set; }

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
        public DtoLocation Location { get; set; }

        public ICollection<FileTuple>? lstPropertiesImage_Document { get; set; }// images and document

        public ICollection<DtoShare>? lstShare { get; set; }


    }
}
