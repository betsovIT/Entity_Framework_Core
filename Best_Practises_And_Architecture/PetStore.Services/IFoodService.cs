using PetStore.Services.Models.Food;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Services
{
    public interface IFoodService
    {
        void BuyFromDistributor(string name, double weight, decimal price,
            double profit, DateTime expirationDate, int brandId, int categoryId);

        void BuyFromDistributor(AddingFoodServiceModel model);
    }
}
