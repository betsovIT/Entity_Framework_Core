namespace VaporStore.DataProcessor
{
	using System;
    using System.ComponentModel.DataAnnotations;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.ImportDTO;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            var deserializationResult = JsonConvert.DeserializeObject<GameImportDTO>(jsonString);
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			throw new NotImplementedException();
		}

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);


        }
	}
}