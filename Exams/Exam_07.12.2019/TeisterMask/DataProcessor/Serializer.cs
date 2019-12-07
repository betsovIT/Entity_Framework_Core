namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProjectExportDTO[]), new XmlRootAttribute("Projects"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var projects = context.Projects.Where(p => p.Tasks.Any())
                .OrderByDescending(p => p.Tasks.Count)
                .ThenBy(p => p.Name)
                .Select(p => new ProjectExportDTO()
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate == null ? "No" : "Yes",
                    Tasks = p.Tasks
                    .OrderBy(t => t.Name)
                    .Select(t => new TaskExportDTO()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .ToArray()
                }).ToArray();

            serializer.Serialize(new StringWriter(sb), projects, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .OrderByDescending(e => e.EmployeesTasks.Count)
                .ThenBy(e => e.Username)
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                    .Where(et => et.Task.OpenDate > date)
                    .OrderByDescending(et => et.Task.DueDate)
                    .ThenBy(et => et.Task.Name)
                    .Select(et => new
                    {
                        TaskName = et.Task.Name,
                        OpenDate = et.Task.OpenDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        DueDate = et.Task.DueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        LabelType = et.Task.LabelType.ToString(),
                        ExecutionType = et.Task.ExecutionType.ToString()
                    })
                })
                .OrderByDescending(e => e.Tasks.Count())
                .ThenBy(e => e.Username)
                .Take(10);

            var result = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return result;
        }
    }
}