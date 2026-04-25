using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Domain;

namespace Ecommerce.Api.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, ProductListItemDto>();
        CreateMap<SaleItem, SaleItemDto>().ReverseMap();
        CreateMap<CreateSaleItemDto, SaleItem>();

        CreateMap<Sale, SaleDto>().ReverseMap();
        CreateMap<Sale, SaleListItemDto>();
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<UpdateSaleDto, Sale>();

        CreateMap<PaymentInfo, PaymentInfoDto>().ReverseMap();
    }
}
