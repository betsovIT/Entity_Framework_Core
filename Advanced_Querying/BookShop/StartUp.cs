namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
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

                Console.WriteLine(GetBooksByCategory(db, command));
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
    }
}
