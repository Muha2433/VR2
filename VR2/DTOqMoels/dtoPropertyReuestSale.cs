namespace VR2.DTOqMoels
{
   
        public class dtoPropertyReuestSale
        {
            public dtoPropertyReuestSale() { }

            public int IdOfRequest { get; set; }
            public string urlImage { get; set; }
            public string title { get; set; }
            public string type { get; set; } // villa , House , Apartment

            public int numberOfBedRooms { get; set; }
            public int numberOfBathrooms { get; set; }

            public double percantageOfWholeProperty { get; set; }
            public double PriceOfRequest { get; set; }

            public string City { get; set; } // city in Country
            public string Country { get; set; }
        
    }
}
