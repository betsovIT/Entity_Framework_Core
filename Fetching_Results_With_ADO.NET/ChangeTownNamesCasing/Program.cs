using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ChangeTownNamesCasing
{
    class Program
    {
        static void Main(string[] args)
        {
            string country = Console.ReadLine();

            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            connection.Open();

            using (connection)
            {
                SqlCommand searchForCountry = new SqlCommand($@"SELECT COUNT(*)
                                                                FROM Countries
                                                                WHERE Name = '{country}'", connection);
                SqlCommand searchForTowns = new SqlCommand($@"SELECT COUNT(*)
                                                              FROM Towns t
                                                              INNER JOIN Countries c ON t.CountryCode = c.Id
                                                              WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = '{country}')", connection);
                SqlCommand updateTowns = new SqlCommand($@"UPDATE Towns
                                                           SET Name = UPPER(Name)
                                                           WHERE Id IN (SELECT t.Id
                                                           FROM Towns t
                                                           INNER JOIN Countries c ON t.CountryCode = c.Id
                                                           WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = '{country}'))", connection);
                SqlCommand selectUpdatedTowns = new SqlCommand($@"SELECT t.Name
                                                              FROM Towns t
                                                              INNER JOIN Countries c ON t.CountryCode = c.Id
                                                              WHERE t.CountryCode = (SELECT Id FROM Countries WHERE Name = '{country}')", connection);
                int countryExists = (int)searchForCountry.ExecuteScalar();
                int townCount = (int)searchForTowns.ExecuteScalar();

                if (countryExists == 0 || townCount == 0)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    updateTowns.ExecuteNonQuery();

                    SqlDataReader reader = selectUpdatedTowns.ExecuteReader();
                    var townList = new List<string>();
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            townList.Add((string)reader["Name"]);
                        }
                    }

                    Console.WriteLine(string.Join(", ",townList));
                }
            }
        }
    }
}
