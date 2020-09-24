namespace ContosoCrafts.WebSite.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.AspNetCore.Hosting;
    using Models;

    public class JsonFileProductService
    {
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; set; }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json");

        public IEnumerable<Product> GetProducts()
        {
            using (var jsonSerializer = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<Product[]>(jsonSerializer.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public void AddRating(string productId, int rating)
        {
            var products = GetProducts();
            var query = products.First(x => x.Id == productId);

            if (query.Ratings == null)
            {
                query.Ratings = new [] {rating};
            }
            else
            {
                var ratings = query.Ratings.ToList();
                ratings.Add(rating);
                query.Ratings = ratings.ToArray();
            }

            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize(new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                    products);
            }
        }
    }
}
