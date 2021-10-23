using Core.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context)
        {
            try
            {
                if (!context.ProductBrands.Any())
                {
                    var bradsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(bradsData);
                    foreach (var item in brands)
                    {
                       await context.ProductBrands.AddAsync(item);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.ProductTypes.Any())
                {
                    var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                    foreach (var item in types)
                    {
                        await context.ProductTypes.AddAsync(item);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.Products.Any())
                {
                    var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                    foreach (var item in products)
                    {
                        await context.Products.AddAsync(item);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in StoreContextSeed.");
            }
        }
    }
}
