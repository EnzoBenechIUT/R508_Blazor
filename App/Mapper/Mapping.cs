using AutoMapper;
using App.DTO;
using App.Models;

namespace App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produit, ProduitDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdProduit))
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomProduit))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.NomPhoto, opt => opt.MapFrom(src => src.NomPhoto))
            .ForMember(dest => dest.UriPhoto, opt => opt.MapFrom(src => src.UriPhoto))
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.IdTypeProduit))
            .ForMember(dest => dest.MarqueId, opt => opt.MapFrom(src => src.IdMarque))
            .ForMember(dest => dest.StockReel, opt => opt.MapFrom(src => src.StockReel))
            .ForMember(dest => dest.StockMin, opt => opt.MapFrom(src => src.StockMin))
            .ForMember(dest => dest.StockMax, opt => opt.MapFrom(src => src.StockMax))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.NomTypeProduit : ""))
            .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueNavigation != null ? src.MarqueNavigation.NomMarque : ""));

            // ProduitDto -> Produit
            CreateMap<ProduitDto, Produit>()
                .ForMember(dest => dest.IdProduit, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NomProduit, opt => opt.MapFrom(src => src.Nom))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.NomPhoto, opt => opt.MapFrom(src => src.NomPhoto))
                .ForMember(dest => dest.UriPhoto, opt => opt.MapFrom(src => src.UriPhoto))
                .ForMember(dest => dest.IdTypeProduit, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.IdMarque, opt => opt.MapFrom(src => src.MarqueId))
                .ForMember(dest => dest.StockReel, opt => opt.MapFrom(src => src.StockReel))
                .ForMember(dest => dest.StockMin, opt => opt.MapFrom(src => src.StockMin))
                .ForMember(dest => dest.StockMax, opt => opt.MapFrom(src => src.StockMax));


            CreateMap<Marque, MarqueDto>()
                .ForMember(dest => dest.IdMarque, opt => opt.MapFrom(src => src.IdMarque))
                .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.NomMarque));

            CreateMap<MarqueDto, Marque>()
                .ForMember(dest => dest.IdMarque, opt => opt.MapFrom(src => src.IdMarque))
                .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.NomMarque));

            CreateMap<TypeProduit, TypeProduitDto>()
                .ForMember(dest => dest.IdTypeProduit, opt => opt.MapFrom(src => src.IdTypeProduit))
                .ForMember(dest => dest.NomTypeProduit, opt => opt.MapFrom(src => src.NomTypeProduit));

            CreateMap<TypeProduitDto, TypeProduit>()
                .ForMember(dest => dest.IdTypeProduit, opt => opt.MapFrom(src => src.IdTypeProduit))
                .ForMember(dest => dest.NomTypeProduit, opt => opt.MapFrom(src => src.NomTypeProduit));

        }
    }
}
