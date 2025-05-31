namespace VR2.DTOqMoels
{
    public class DtoLocation
    {
        public double Latitude { get; set; } // خط العرض
        public double Longitude { get; set; } // خط الطول

        public string? Country { get; set; } // البلد
        public string? City { get; set; } // المدينة
        public string? Address { get; set; } // العنوان التفصيلي
        public string? ZipCode { get; set; } // الرمز البريدي
    }
}
