using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();

            using (db)
            {
                Console.WriteLine(RemoveTown(db));
            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.MiddleName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
                .OrderBy(e => e.EmployeeId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Select(e => new
            {
                e.FirstName,
                e.Salary
            })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees.Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Department.Name,
                e.Salary
            })
                .Where(e => e.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine(e);
            }

            return sb.ToString();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(e => e.Manager)
                .Include(e => e.EmployeesProjects)
                .ThenInclude(e => e.Project)
                .Where(e => e.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.Manager.FirstName} {employee.Manager.LastName}");
                foreach (var entity in employee.EmployeesProjects)
                {
                    if (entity.Project.EndDate == null)
                    {
                        sb.AppendLine($"--{entity.Project.Name} - {entity.Project.StartDate.ToString()} - not finished");
                    }
                    else
                    {
                        sb.AppendLine($"--{entity.Project.Name} - {entity.Project.StartDate.ToString()} - {entity.Project.EndDate.ToString()}");
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var results = context.Addresses
                .Select(a => new
                {
                    Text = a.AddressText,
                    Town = a.Town.Name,
                    Count = a.Employees.Count
                })
                .OrderByDescending(a => a.Count)
                .ThenBy(a => a.Town)
                .ThenBy(a => a.Text)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var entry in results)
            {
                sb.AppendLine($"{entry.Text}, {entry.Town} - {entry.Count} employees");
            }

            return sb.ToString();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Include(e => e.EmployeesProjects)
                .ThenInclude(p => p.Project)
                .Where(e => e.EmployeeId == 147)
                .ToList();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.First().FirstName} {employee.First().LastName} - {employee.First().JobTitle}");

            foreach (var project in employee.First().EmployeesProjects.OrderBy(p => p.Project.Name))
            {
                sb.AppendLine(project.Project.Name);
            }

            return sb.ToString();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var results = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    Name = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                })
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in results)
            {
                sb.AppendLine($"{item.Name} - {item.ManagerFirstName} {item.ManagerLastName}");
                foreach (var emp in item.Employees)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }
            }

            return sb.ToString();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .Select(p => new
                {
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate
                })
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
                sb.AppendLine(proj.Description);
                sb.AppendLine(proj.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering"
                    || e.Department.Name == "Tool Design"
                    || e.Department.Name == "Marketing"
                    || e.Department.Name == "Information Services")
                .ToList();

            foreach (var employee in employees)
            {
                employee.Salary += employee.Salary * 0.12m;
            }

            context.SaveChanges();

            var sb = new StringBuilder();

            foreach (var emp in employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:F2})");
            }

            return sb.ToString();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var emps = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);

            var sb = new StringBuilder();

            foreach (var emp in emps)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:F2})");
            }

            return sb.ToString();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectEmployeesToDelete = context.EmployeesProjects.Where(ep => ep.ProjectId == 2);
            var projectsToDelete = context.Projects.Where(p => p.ProjectId == 2);

            context.RemoveRange(projectEmployeesToDelete);
            context.RemoveRange(projectsToDelete);

            context.SaveChanges();

            var projectsToPrint = context.Projects.Select(p => p.Name).Take(10).ToList();

            var sb = new StringBuilder();

            foreach (var project in projectsToPrint)
            {
                sb.AppendLine(project);
            }

            return sb.ToString();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete = context.Towns.Where(t => t.Name == "Seattle").FirstOrDefault();
            var townToDeleteId = context.Towns.Where(t => t.Name == "Seattle").Select(t => t.TownId).FirstOrDefault();
            var addressesToDelete = context.Addresses.Include(a => a.Employees).Where(a => a.TownId == townToDeleteId).ToList();
            var selectedEmployees = new List<Employee>();

            int count = addressesToDelete.Count;

            foreach (var address in addressesToDelete)
            {
                foreach (var emp in address.Employees)
                {
                    selectedEmployees.Add(emp);
                }
            }

            foreach (var emp in selectedEmployees)
            {
                emp.AddressId = null;
            }

            foreach (var addr in addressesToDelete)
            {
                addr.TownId = null;
            }

            context.Addresses.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);

            context.SaveChanges();



            return $"{count} addresses in Seattle were deleted";
        }
    }
}
