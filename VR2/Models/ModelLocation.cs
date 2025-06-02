using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VR2.Models
{
    public class ModelLocation
    {
        public ModelLocation()
        {
            lstProperties = new HashSet<ModelProperties>();
        }

        [Key]
        public int Id { get; set; }

        public double Latitude { get; set; } // خط العرض
        public double Longitude { get; set; } // خط الطول

        public string? Country { get; set; } // البلد
        public string? City { get; set; } // المدينة
        public string? street { get; set; }

        public string? state { get; set; }
        public string? Address { get; set; } // العنوان التفصيلي
        public string? ZipCode { get; set; } // الرمز البريدي

        [JsonIgnore]
        public ICollection<ModelProperties> lstProperties { get; set; }

    }
}
