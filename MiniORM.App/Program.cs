using MiniORM.App.Data;
using MiniORM.App.Data.Entities;
using System;
using System.Linq;

namespace MiniORM.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SoftUniDbContext(@"server=DESKTOP-AGCLSI5\SQLEXPRESS;database=MiniORM;trusted_connection=true;");

            db.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = 1,
                IsEmployed = true
            });

            db.SaveChanges();
        }
    }
}
