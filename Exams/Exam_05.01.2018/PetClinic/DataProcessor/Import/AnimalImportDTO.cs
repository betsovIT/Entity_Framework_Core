namespace PetClinic.DataProcessor.Import
{
    public class AnimalImportDTO
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Age { get; set; }

        public PassportImportDTO Passport { get; set; }
    }

    public class PassportImportDTO
    {
        public string SerialNumber { get; set; }

        public string OwnerName { get; set; }

        public string OwnerPhoneNumber { get; set; }

        public string RegistrationDate { get; set; }
    }
}
