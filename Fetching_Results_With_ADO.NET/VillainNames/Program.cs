using System;
using System.Data.SqlClient;

namespace VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            connection.Open();

            using (connection)
            {
                SqlCommand command = new SqlCommand(@"SELECT v.[Name], COUNT(m.Id) AS Minions
                                                      FROM Villains v
                                                      INNER JOIN MinionsVillains mv ON v.Id = mv.VillainId
                                                      INNER JOIN Minions m ON mv.MinionId = m.Id
                                                      GROUP BY v.[Name]
                                                      HAVING COUNT(m.Id) >= 3
                                                      ORDER BY Minions DESC",connection);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        string villianName = (string)reader[0];
                        int minionCount = (int)reader[1];

                        Console.WriteLine($"{villianName} - {minionCount} ");
                    }
                }
            }
        }
    }
}
