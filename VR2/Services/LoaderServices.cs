using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace VR2.Services
{
    public  class LoaderServices
    {

                public static async Task<List<VmHouse>> LoadHouses(string filePath, int startRow, int endRow)
                    {
                 var houseList = new List<VmHouse>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
            };

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    Console.WriteLine("📖 Reading CSV headers...");
                    csv.Read();
                    csv.ReadHeader();

                    int currentRow = 0;

                    while (csv.Read())
                    {
                        currentRow++;

                        if (currentRow < startRow)
                            continue;

                        if (currentRow > endRow)
                            break;

                        Console.WriteLine($"➡️ Row {currentRow}");

                        try
                        {
                            string street = csv.GetField("street");
                            string city = csv.GetField("city");
                            string state = csv.GetField("state");
                            string zip = csv.GetField("zip_code");

                            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) ||
                                string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(zip))
                            {
                                Console.WriteLine($"⚠️ Skipping row {currentRow} due to missing address data.");
                                continue;
                            }

                            var house = new VmHouse
                            {
                                price = ParseDouble(csv.GetField("price")),
                                bed = ParseInt(csv.GetField("bed")),
                                bath = ParseInt(csv.GetField("bath")),
                                acre_lot = ParseDouble(csv.GetField("acre_lot")),
                                street = street,
                                city = city,
                                state = state,
                                zip_code = zip,
                                house_size = ParseDouble(csv.GetField("house_size"))
                            };

                            Console.WriteLine($"📍 Geocoding: {house.street}, {house.city}, {house.state}, {house.zip_code}");

                            (house.Latitude, house.Longitude) = await GeocodeWithOpenCage(house.street, house.city, house.state, house.zip_code);

                            if (house.Latitude == 0 && house.Longitude == 0)
                            {
                                Console.WriteLine("⚠️ No valid coordinates found.");
                            }
                            else
                            {
                                Console.WriteLine($"✅ Found Coordinates: Lat={house.Latitude}, Lon={house.Longitude}");
                            }

                            houseList.Add(house);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error at row {currentRow}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception fileEx)
            {
                Console.WriteLine($"❗ File processing error: {fileEx.Message}");
            }

            return houseList;
        }

        public static async Task<(double Latitude, double Longitude)> GeocodeWithOpenCage(string street, string city, string state, string zip)
        {
            string apiKey = "2892708e50f84fab86548e46e5ed0e43"; // Replace with your actual key
            string fullAddress = $"{street}, {city}, {state}, {zip}";
            string url = $"https://api.opencagedata.com/geocode/v1/json?q={Uri.EscapeDataString(fullAddress)}&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"🌐 API Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return (0, 0);
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(json);
                    var results = obj["results"];

                    if (results != null && results.HasValues)
                    {
                        var geometry = results[0]["geometry"];
                        double lat = geometry["lat"].Value<double>();
                        double lng = geometry["lng"].Value<double>();
                        return (lat, lng);
                    }
                    else
                    {
                        Console.WriteLine(" No geocoding results.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" API call failed: {ex.Message}");
                }
            }

            return (0, 0);
        }

        private static double ParseDouble(string input)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        private static int ParseInt(string input)
        {
            return int.TryParse(input, out var result) ? result : 0;
        }
    }

    public class VmHouse
    {
        public double price { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int bed { get; set; }
        public int bath { get; set; }
        public double acre_lot { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public double house_size { get; set; }
    }

    }

