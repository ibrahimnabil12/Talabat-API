using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext _context)
        {
            if (_context.Types.Count()==0)
            {
                  var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
            var categories = JsonSerializer.Deserialize<List<ProductType>>(categoryData);
            if (categories?.Count() > 0)
            {
                foreach (var category in categories)
                {
                    _context.Set<ProductType>().Add(category);
                }
                await _context.SaveChangesAsync();
            }
            }


            //---------------------------

            if (_context.Brands.Count()==0)
            {
                var brandData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
            if (brands?.Count() > 0)
            {
                foreach (var brand in brands)
                {
                    _context.Set<ProductBrand>().Add(brand);
                }
                await _context.SaveChangesAsync();
            }
            }


            //------------------------
            if (_context.Products.Count()==0)
            {
                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productData);
            if (products?.Count() > 0)
            {
                foreach (var product in products)
                {
                    _context.Set<Product>().Add(product);
                }
                await _context.SaveChangesAsync();
            }
            }
            //------------------------
            if (_context.DeliverMethods.Count()==0)
            {
                var deliveryMethodData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var deliverMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodData);

                if (deliverMethods.Count>0)
                {
                    foreach (var deliveryMethod in deliverMethods)
                    {
                        _context.Set<DeliveryMethod>().Add(deliveryMethod);
                    }
                    await _context.SaveChangesAsync();
                }
            }

        }
    }
}
