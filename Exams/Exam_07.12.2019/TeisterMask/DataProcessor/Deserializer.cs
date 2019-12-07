namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.IO;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using System.Linq;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProjectImportDTO[]), new XmlRootAttribute("Projects"));
            var deserializationResult = (ProjectImportDTO[])serializer.Deserialize(new StringReader(xmlString));
            var projects = new List<Project>();

            foreach (var result in deserializationResult)
            {
                var project = new Project()
                {
                    Name = result.Name,
                    OpenDate = DateTime.ParseExact(result.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DueDate = string.IsNullOrEmpty(result.DueDate)
                    ? (DateTime?)null
                    : DateTime.ParseExact(result.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),

                };

                var tasks = new List<Task>();

                foreach (var taskResult in result.Tasks)
                {
                    var task = new Task()
                    {
                        Name = taskResult.Name,
                        OpenDate = DateTime.ParseExact(taskResult.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(taskResult.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ExecutionType = (ExecutionType)taskResult.ExecutionType,
                        LabelType = (LabelType)taskResult.LabelType
                    };

                    tasks.Add(task);
                }

                if (IsValid(project))
                {
                    foreach (var task in tasks)
                    {
                        if (IsValid(task))
                        {
                            if (DateTime.Compare(project.OpenDate, task.OpenDate) < 0)
                            {
                                if (project.DueDate == null)
                                {
                                    project.Tasks.Add(task);
                                }
                                else if (DateTime.Compare(task.DueDate, (DateTime)project.DueDate) < 0 && DateTime.Compare(task.OpenDate, task.DueDate) < 0)
                                {
                                    project.Tasks.Add(task);
                                }
                                else
                                {
                                    sb.AppendLine(ErrorMessage);
                                }
                            }
                            else
                            {
                                sb.AppendLine(ErrorMessage);
                            }
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                    }

                    projects.Add(project);
                    sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializationResult = JsonConvert.DeserializeObject<EmployeeImportDTO[]>(jsonString);
            var employees = new List<Employee>();

            foreach (var result in deserializationResult)
            {
                var employee = new Employee()
                {
                    Username = result.Username,
                    Email = result.Email,
                    Phone = result.Phone,
                };

                var employeeTasks = new List<EmployeeTask>();

                if (IsValid(employee))
                {
                    foreach (var task in result.Tasks.Distinct())
                    {
                        if (context.Tasks.Any(t => t.Id == task))
                        {
                            employeeTasks.Add(new EmployeeTask() { TaskId = task });
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                    }
                    foreach (var tt in employeeTasks)
                    {
                        employee.EmployeesTasks.Add(tt);
                    }
                    employees.Add(employee);
                    sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}