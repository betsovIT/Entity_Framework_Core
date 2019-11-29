using PetStore.Data;
using PetStore.Services.Implementations;
using System;

namespace PetStore
{
    public class Program
    {
        public static void Main()
        {
            using var data = new PetStoreDbContext();
            var brandService = new BrandService(data);
        }
    }
}
