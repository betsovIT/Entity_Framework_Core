using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserImportDTO, User>();
            this.CreateMap<ProductImportDTO, Product>();
            this.CreateMap<CategoryImportDTO, Category>();
            this.CreateMap<CategoryProductImportDTO, CategoryProduct>();
        }
    }
}
