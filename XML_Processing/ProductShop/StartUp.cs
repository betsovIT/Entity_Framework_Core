﻿using AutoMapper;
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
            using (var db = new ProductShopContext())
            {
                Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());
                db.Database.EnsureCreated();

                string usersPath = @".\Datasets\users.xml";
                string usersAsString = File.ReadAllText(usersPath);
                string productsPath = @".\Datasets\products.xml";
                string productsAsString = File.ReadAllText(productsPath);
                string categoriesPath = @".\Datasets\categories.xml";
                string categoriesAsString = File.ReadAllText(categoriesPath);
                string CPPath = @".\Datasets\categories-products.xml";
                string CPAsString = File.ReadAllText(CPPath);


                //ImportUsers(db, usersAsString);
                //ImportProducts(db, productsAsString);
                //ImportCategories(db, categoriesAsString);
                //ImportCategoryProducts(db, CPAsString);

                Console.WriteLine(GetUsersWithProducts(db));
            }
        }

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
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var serializer = new XmlSerializer(categories.GetType(), new XmlRootAttribute("Categories"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), categories, namespaces);


            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UsersExportDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,                    
                    SoldPorducts = new SoldProductDTO 
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(ps => new ProductExportDTO
                        {
                            Price = ps.Price,
                            Name = ps.Name
                        })
                        .OrderByDescending(ps => ps.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            var usersFormated = new UsersFinalExportDTO
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = users
            };

            var serializer = new XmlSerializer(usersFormated.GetType(), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), usersFormated, namespaces);


            return sb.ToString().TrimEnd();
        }
    }
}