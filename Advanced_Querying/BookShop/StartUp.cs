namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            string command = Console.ReadLine();

            using (var db = new BookShopContext())
            {
                DbInitializer.ResetDatabase(db);

                Console.WriteLine(RemoveBooks(db));
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var refinedCommand = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == refinedCommand)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var specialBooks = context.Books
                .Select(b => new { b.Title, b.EditionType, b.Copies })
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in specialBooks)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Select(b => new { b.Title, b.Price })
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Select(b => new { b.BookId, b.Title, b.ReleaseDate })
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categoriesAllowed = input.Split().Select(c => c.ToLower()).ToList();

            var books = context.Books
                .Select(b => new { b.Title, b.BookCategories })
                .Where(b => b.BookCategories.Select(bc => bc.Category.Name.ToLower()).Intersect<string>(categoriesAllowed).Any()
                      )
                .OrderBy(b => b.Title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Select(b => new { b.Title, b.EditionType, b.Price, b.ReleaseDate })
                .Where(b => b.ReleaseDate < parsedDate && b.ReleaseDate != null)
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType.ToString()} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Select(a => new { a.FirstName, a.LastName })
                .Where(a => a.FirstName.EndsWith(input) && a.FirstName != null)
                .Select(a => a.FirstName + ' ' + a.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors.OrderBy(a => a))
            {
                sb.AppendLine(author);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Select(b => b.Title)
                .Where(b => b.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Select(b => new { b.BookId, b.Title, b.Author.FirstName, b.Author.LastName})
                .Where(b => b.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books.Select(b => b.Title).Where(b => b.Length > lengthCheck).Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Copies = a.Books.Sum(s => s.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.Copies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    Revenue = c.CategoryBooks.Sum(s => s.Book.Copies * s.Book.Price)
                })
                .OrderByDescending(c => c.Revenue)
                .ThenBy(c => c.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Name} ${category.Revenue:F2}");
            }

            return sb.ToString();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var query = context.Categories
                .Select(c => new 
            {
               Name = c.Name,
               Books = c.CategoryBooks.Select(s => s.Book).OrderByDescending(s => s.ReleaseDate).ToList()
            })
                .OrderBy(c => c.Name);

            var sb = new StringBuilder();

            foreach (var entity in query)
            {
                sb.AppendLine($"--{entity.Name}");
                for (int i = 0; i < 3; i++)
                {
                    sb.AppendLine($"{entity.Books[i].Title} ({entity.Books[i].ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksToCHange = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010).ToList();

            foreach (var book in booksToCHange)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var bookIDs = context.Books.Where(b => b.Copies < 4200).Select(b => b.BookId).ToArray();
            var books = context.Books.Where(b => b.Copies < 4200).ToList();
            var mapings = context.BooksCategories.Where(b => bookIDs.Contains(b.BookId));

            context.BooksCategories.RemoveRange(mapings);
            context.Books.RemoveRange(books);

            context.SaveChanges();

            return bookIDs.Count();
        }
    }
}
