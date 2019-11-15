using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using (var db = new ProductShopContext())
            {
                string usersPath = @"..\..\..\Datasets\users.json";
                string usersJson = File.ReadAllText(usersPath);
                string productsPath = @"..\..\..\Datasets\products.json";
                string productsJson = File.ReadAllText(productsPath);
                string categoriesPath = @"..\..\..\Datasets\categories.json";
                string categoriesJson = File.ReadAllText(categoriesPath);
                string categoriesProductsPath = @"..\..\..\Datasets\categories-products.json";
                string categoriesProductsJson = File.ReadAllText(categoriesProductsPath);

                //ImportUsers(db, usersJson);
                //ImportProducts(db, productsJson);
                //ImportCategories(db, categoriesJson);
                //ImportCategoryProducts(db, categoriesProductsJson);

                Console.WriteLine(GetUsersWithProducts(db));
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(a => !string.IsNullOrEmpty(a.Name)).ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Select(p => new { name = p.Name, price = p.Price, seller = p.Seller.FirstName + ' ' + p.Seller.LastName })
                .Where(p => p.price >= 500 && p.price <= 1000)
                .OrderBy(p => p.price)
                .ToList();

            string resultJson = JsonConvert.SerializeObject(products, Formatting.Indented);

            return resultJson;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var results = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold

                    .Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                })
            .OrderBy(u => u.lastName)
            .ThenBy(u => u.firstName)
            .ToList();

            string resultJson = JsonConvert.SerializeObject(results, Formatting.Indented);

            return resultJson;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var results = context.Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = string.Format("{0:0.00}", c.CategoryProducts.Average(p => p.Product.Price)),
                    totalRevenue = string.Format("{0:0.00}", c.CategoryProducts.Sum(p => p.Product.Price))
                })
                .OrderByDescending(c => c.productsCount);

            string resultJson = JsonConvert.SerializeObject(results, Formatting.Indented);

            return resultJson;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var results = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(p => p.Buyer != null),
                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                    }
                })
                .ToList();

            var finalResults = new
            {
                usersCount = results.Count(),
                users = results
            };

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string resultJson = JsonConvert.SerializeObject(finalResults, Formatting.Indented, settings);

            return resultJson;
        }
    }
}