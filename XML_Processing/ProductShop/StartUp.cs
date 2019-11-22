using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            //using (var db = new ProductShopContext())
            //{
            //    Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());
            //    db.Database.EnsureCreated();

            //    string usersPath = @".\Datasets\users.xml";
            //    string usersAsString = File.ReadAllText(usersPath);
            //    string productsPath = @".\Datasets\products.xml";
            //    string productsAsString = File.ReadAllText(productsPath);
            //    string categoriesPath = @".\Datasets\categories.xml";
            //    string categoriesAsString = File.ReadAllText(categoriesPath);
            //    string CPPath = @".\Datasets\categories-products.xml";
            //    string CPAsString = File.ReadAllText(CPPath);


            //    //ImportUsers(db, usersAsString);
            //    //ImportProducts(db, productsAsString);
            //    //ImportCategories(db, categoriesAsString);
            //    //ImportCategoryProducts(db, CPAsString);

            //    //Console.WriteLine(GetCategoriesByProductsCount(db));
            //}
        }

        //public static string ImportUsers(ProductShopContext context, string inputXml)
        //{
        //    XDocument users = XDocument.Parse(inputXml);
        //    var resultUsers = new List<User>();
        //    var usersParsed = users.Root.Elements();

        //    foreach (var element in usersParsed)
        //    {
        //        var user = new User()
        //        {
        //            FirstName = element.Element("firstName").Value,
        //            LastName = element.Element("lastName").Value,
        //            Age = int.Parse(element.Element("age").Value)
        //        };

        //        resultUsers.Add(user);
        //    }

        //    context.Users.AddRange(resultUsers);
        //    context.SaveChanges();

        //    return $"Successfully imported {resultUsers.Count}";
        //}

        //public static string ImportProducts(ProductShopContext context, string inputXml)
        //{
        //    XDocument productsAsXML = XDocument.Parse(inputXml);
        //    var resultProducts = new List<Product>();
        //    var parsedProducts = productsAsXML.Root.Elements();

        //    foreach (var rawProduct in parsedProducts)
        //    {
        //        var product = new Product()
        //        {
        //            Name = rawProduct.Element("name").Value,
        //            Price = decimal.Parse(rawProduct.Element("price").Value),
        //            SellerId = int.Parse(rawProduct.Element("sellerId").Value),
        //            BuyerId = rawProduct.Element("buyerId")?.Value != null ? int.Parse(rawProduct.Element("buyerId").Value) : (int?)null
        //        };

        //        resultProducts.Add(product);
        //    }

        //    context.Products.AddRange(resultProducts);
        //    context.SaveChanges();

        //    return $"Successfully imported {resultProducts.Count}";
        //}

        //public static string ImportCategories(ProductShopContext context, string inputXml)
        //{
        //    XDocument categoriesAsXML = XDocument.Parse(inputXml);
        //    var parsedCategories = categoriesAsXML.Root.Elements();
        //    var resultCategories = new List<Category>();

        //    foreach (var rawCategory in parsedCategories)
        //    {
        //        var category = new Category()
        //        {
        //            Name = rawCategory.Element("name").Value
        //        };

        //        resultCategories.Add(category);
        //    }

        //    context.Categories.AddRange(resultCategories);
        //    context.SaveChanges();

        //    return $"Successfully imported {resultCategories.Count}";
        //}

        //public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        //{
        //    XDocument categoriesProductsAsXML = XDocument.Parse(inputXml);
        //    var parsedCP = categoriesProductsAsXML.Root.Elements();
        //    var resultCP = new List<CategoryProduct>();

        //    foreach (var rawCP in parsedCP)
        //    {
        //        int categoryId = int.Parse(rawCP.Element("CategoryId").Value);
        //        int productId = int.Parse(rawCP.Element("ProductId").Value);

        //        if (context.Categories.Any(c => c.Id == categoryId) && context.Products.Any(p => p.Id == productId))
        //        {
        //            var newCP = new CategoryProduct()
        //            {
        //                CategoryId = categoryId,
        //                ProductId = productId
        //            };

        //            if (!resultCP.Any(x => x.CategoryId == newCP.CategoryId && x.ProductId == newCP.ProductId))
        //            {
        //                resultCP.Add(newCP);
        //            }
        //        }
        //    }

        //    context.CategoryProducts.AddRange(resultCP);
        //    context.SaveChanges();

        //    return $"Successfully imported {resultCP.Count}";
        //}

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserImportDTO[]), new XmlRootAttribute("Users"));

            var usersDTO = (UserImportDTO[])serializer.Deserialize(new StringReader(inputXml));
            var users = new List<User>();

            foreach (var user in usersDTO)
            {
                var resultUser = Mapper.Map<User>(user);

                users.Add(resultUser);
            }

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ProductImportDTO>), new XmlRootAttribute("Products"));

            var deserializationResult = (List<ProductImportDTO>)serializer.Deserialize(new StringReader(inputXml));
            var products = new List<Product>();

            foreach (var productResult in deserializationResult)
            {
                var product = Mapper.Map<Product>(productResult);
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CategoryImportDTO[]), new XmlRootAttribute("Categories"));

            var result = (CategoryImportDTO[])serializer.Deserialize(new StringReader(inputXml));
            var categories = new List<Category>();

            foreach (var category in result)
            {
                var categoryResult = Mapper.Map<Category>(category);
                categories.Add(categoryResult);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CategoryProductImportDTO[]), new XmlRootAttribute("CategoryProducts"));
            var deserializationResult = (CategoryProductImportDTO[])serializer.Deserialize(new StringReader(inputXml));

            var result = new List<CategoryProduct>();
            foreach (var dto in deserializationResult)
            {
                var categoryProduct = Mapper.Map<CategoryProduct>(dto);

                if (context.Products.Any(p => p.Id == categoryProduct.ProductId) && context.Categories.Any(c => c.Id == categoryProduct.CategoryId))
                {
                    result.Add(categoryProduct);
                }
            }

            context.CategoryProducts.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var serializer = new XmlSerializer(typeof(ProductsInRangeExportDTO[]), new XmlRootAttribute("Products"));

            var products = context.Products
                .Select(p => new ProductsInRangeExportDTO()
                {
                    Price = p.Price,
                    Name = p.Name,
                    Buyer = p.Buyer.FirstName + ' ' + p.Buyer.LastName
                })
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, products, namespaces);
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var result = context.Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new SoldProductsExportDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ProductExportDTO()
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToArray()
                })
                .Take(5)
                .ToArray();

            var serializer = new XmlSerializer(result.GetType(), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, result, namespaces);
            }

            return sb.ToString();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryExportDTO
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(x => x.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(x => x.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenByDescending(c => c.TotalRevenue)
                .ToArray();

            var serializer = new XmlSerializer(categories.GetType(), new XmlRootAttribute("Categories"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), categories, namespaces);


            return sb.ToString().TrimEnd();
        }
    }
}