namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.DataProcessor.Import;
    using PetClinic.Models;

    public class Deserializer
    {

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializationResult = JsonConvert.DeserializeObject<AnimalAidImportDTO[]>(jsonString);
            var animalAids = new List<AnimalAid>();

            foreach (var result in deserializationResult)
            {
                var animalAid = new AnimalAid()
                {
                    Name = result.Name,
                    Price = result.Price
                };

                if (IsValid(animalAid) && !animalAids.Any(a => a.Name == result.Name))
                {
                    animalAids.Add(animalAid);
                    sb.AppendLine($"Record {animalAid.Name} successfully imported.");
                }
                else
                {
                    sb.AppendLine($"Error: Invalid data.");
                }
            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializationResult = JsonConvert.DeserializeObject<AnimalImportDTO[]>(jsonString);
            var animals = new List<Animal>();

            foreach (var result in deserializationResult)
            {
                var animal = new Animal()
                {
                    Name = result.Name,
                    Type = result.Type,
                    Age = result.Age,
                    PassportSerialNumber = result.Passport.SerialNumber,
                    Passport = new Passport()
                    {
                        SerialNumber = result.Passport.SerialNumber,
                        OwnerName = result.Passport.OwnerName,
                        OwnerPhoneNumber = result.Passport.OwnerPhoneNumber,
                        RegistrationDate = DateTime.ParseExact(result.Passport.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                    }
                };

                if (IsValid(animal) && IsValid(animal.Passport) && !animals.Select(a => a.Passport).Any(p => p.SerialNumber == animal.Passport.SerialNumber))
                {
                    animals.Add(animal);
                    sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }

            context.Animals.AddRange(animals);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(VetImportDTO[]), new XmlRootAttribute("Vets"));
            var deserializationResult = (VetImportDTO[])serializer.Deserialize(new StringReader(xmlString));
            var vets = new List<Vet>();

            foreach (var result in deserializationResult)
            {
                var vet = new Vet()
                {
                    Name = result.Name,
                    Profession = result.Profession,
                    Age = result.Age,
                    PhoneNumber = result.PhoneNumber
                };

                if (IsValid(vet) && !vets.Any(v => v.PhoneNumber.Equals(vet.PhoneNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    vets.Add(vet);
                    sb.AppendLine($"Record {vet.Name} successfully imported.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ProcedureImportDTO[]), new XmlRootAttribute("Procedures"));
            var deserializationResult = (ProcedureImportDTO[])serializer.Deserialize(new StringReader(xmlString));
            var procedures = new List<Procedure>();

            foreach (var result in deserializationResult)
            {
                var vet = context.Vets
                    .FirstOrDefault(v => v.Name == result.Vet);
                var animal = context.Animals
                    .FirstOrDefault(a => a.PassportSerialNumber == result.Animal);
                var date = DateTime
                    .ParseExact(result.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var aids = new List<AnimalAid>();

                foreach (var aa in result.AnimalAids)
                {
                    aids.Add(context.AnimalAids.FirstOrDefault(a => a.Name == aa.Name));
                }

                if (vet != null && animal != null && !aids.Any(aa => aa == null) && !aids.GroupBy(a => a.Id).Where(a => a.Skip(1).Any()).Any())
                {
                    var procedure = new Procedure()
                    {
                        Animal = animal,
                        Vet = vet,
                        DateTime = date,
                        ProcedureAnimalAids = aids
                            .Select(aa => new ProcedureAnimalAid
                            {
                                AnimalAid = aa
                            })
                            .ToArray()
                    };

                    procedures.Add(procedure);
                    sb.AppendLine("Record successfully imported.");
                }
                else
                {
                    sb.AppendLine("Error: Invalid data.");
                }
            }

            context.Procedures.AddRange(procedures);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResults, true);
        }
    }
}
