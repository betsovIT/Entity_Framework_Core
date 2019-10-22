using System;
using System.Data.SqlClient;

namespace Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            //The first test wont execute, relation between Bob and Gru already exists. Preventing that is part of the bonus task.

            string[] minionInformation = Console.ReadLine().Split();
            string villainName = Console.ReadLine().Split()[1];

            string minionName = minionInformation[1];
            int minionAge = int.Parse(minionInformation[2]);
            string town = minionInformation[3];

            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            connection.Open();

            using (connection)
            {
                SqlCommand searchForTown = new SqlCommand($@"SELECT COUNT(*) 
                                                           FROM Towns 
                                                           WHERE Name = '{town}'",connection);
                SqlCommand insertTown = new SqlCommand($@"INSERT INTO Towns(Name) VALUES
                                                          ('{town}')", connection);
                SqlCommand aquireTownId = new SqlCommand($@"SELECT Id
                                                            FROM Towns
                                                            WHERE Name = '{town}'", connection);
                int townExists = (int)searchForTown.ExecuteScalar();

                if (townExists == 0)
                {
                    if (insertTown.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine($"Town {town} was added to the database.");
                    }                    
                }

                int townId = (int)aquireTownId.ExecuteScalar();

                SqlCommand searchForVillain = new SqlCommand($@"SELECT COUNT(*)
                                                               FROM Villains
                                                               WHERE Name = '{villainName}'",connection);
                SqlCommand insertVillain = new SqlCommand($@"INSERT INTO Villains(Name,EvilnessFactorId) VALUES
                                                             ('{villainName}',4)", connection);
                SqlCommand aquireVillainId = new SqlCommand($@"SELECT Id
                                                               FROM Villains
                                                               WHERE Name = '{villainName}'", connection);
                int villainExists = (int)searchForVillain.ExecuteScalar();

                if (villainExists == 0)
                {
                    if(insertVillain.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine($"Villain {villainName} was added to the database.");
                    }
                }

                int villainId = (int)aquireVillainId.ExecuteScalar();
              
                SqlCommand insertMinion = new SqlCommand($@"INSERT INTO Minions(Name,Age,TownId) VALUES
                                                              ('{minionName}', {minionAge}, {townId})", connection);
                SqlCommand aquireMinionId = new SqlCommand($@"SELECT Id
                                                              FROM Minions
                                                              WHERE Name = '{minionName}'", connection);

                if (insertMinion.ExecuteNonQuery() > 0)
                {
                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                }                

                int minionId = (int)aquireMinionId.ExecuteScalar();

                SqlCommand createVillainMinionRelation = new SqlCommand($@"INSERT INTO MinionsVillains VALUES
                                                                           ({minionId},{villainId})", connection);

                createVillainMinionRelation.ExecuteNonQuery();                
            }
        }
    }
}
