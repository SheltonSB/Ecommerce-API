using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Domain;

namespace Ecommerce.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductListItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        // Sale Mappings
        CreateMap<Sale, SaleListItemDto>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.SaleItems.Count));
        CreateMap<Sale, SaleDto>();

        // SaleItem Mappings
        CreateMap<SaleItem, SaleItemDto>();

        // PaymentInfo Mappings
        CreateMap<PaymentInfo, PaymentInfoDto>();

        // Category Mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryListItemDto>();
    }
}